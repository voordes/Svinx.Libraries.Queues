using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Svinx.Libraries.Queues.Delegates;
using System;
using System.Text;

namespace Svinx.Libraries.Queues.RabbitMQ
{
    public class RPCClient: BaseRPCClient
    {
        private string _queueUrl;

        private string _queueName;

        private IConnection _connection;

        private IModel _channel;

        private string _replyQueueName;

        private QueueingBasicConsumer _consumer;

        public RPCClient(IOptions<Queue> options)
        {
            this._queueUrl = options.Value.queueUrl;
            this._queueName = options.Value.queueName;
            Start();
        }

        private void Start()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(this._queueUrl)
            };
            this._connection = connectionFactory.CreateConnection();
            this._channel = this._connection.CreateModel();
            this._replyQueueName = this._channel.QueueDeclare().QueueName;
            this._consumer = new QueueingBasicConsumer(this._channel);
            this._channel.BasicConsume(this._replyQueueName, true, this._consumer);
        }

        public override TResp Call<TReq, TResp>(TReq req)
        {
            string text = Guid.NewGuid().ToString();
            IBasicProperties basicProperties = this._channel.CreateBasicProperties();
            basicProperties.ReplyTo = this._replyQueueName;
            basicProperties.CorrelationId = text;
            string s = JsonConvert.SerializeObject(req);
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            this._channel.BasicPublish(string.Empty, this._queueName, basicProperties, bytes);
            BasicDeliverEventArgs basicDeliverEventArgs;
            do
            {
                basicDeliverEventArgs = this._consumer.Queue.Dequeue();
            }
            while (!(basicDeliverEventArgs.BasicProperties.CorrelationId == text));
            return JsonConvert.DeserializeObject<TResp>(Encoding.UTF8.GetString(basicDeliverEventArgs.Body));
        }
    }
}
