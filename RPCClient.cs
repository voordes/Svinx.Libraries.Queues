using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Svinx.Libraries.RabbitMQ.Delegates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Svinx.Libraries.RabbitMQ
{
    public class RPCClient: IRPCClient
    {
        private string _uri;

        private IConnection _connection;

        private IModel _channel;

        private string _replyQueueName;

        private QueueingBasicConsumer _consumer;

        public event EventHandler Started;

        public event MessageReceivedEventHandler MessageReceived;

        public event ActionProcessedEventHandler ActionProcessed;

        public event UnhandledExceptionEventHandler Exception;

        public RPCClient(string uri)
        {
            this._uri = uri;
        }

        public void OnStarted(EventArgs e)
        {
            if (this.Started != null)
            {
                this.Started(this, e);
            }
        }

        public void OnMessageReceived(MessageArgs e)
        {
            if (this.MessageReceived != null)
            {
                this.MessageReceived(this, e);
            }
        }

        public void OnException(UnhandledExceptionEventArgs e)
        {
            if (this.Exception != null)
            {
                this.Exception(this, e);
            }
        }

        public void OnActionProcessed(ActionArgs e)
        {
            if (this.ActionProcessed != null)
            {
                this.ActionProcessed(this, e);
            }
        }

        public void Start(string queue)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(this._uri)
            };
            this._connection = connectionFactory.CreateConnection();
            this._channel = this._connection.CreateModel();
            this._replyQueueName = this._channel.QueueDeclare().QueueName;
            this._consumer = new QueueingBasicConsumer(this._channel);
            this._channel.BasicConsume(this._replyQueueName, true, this._consumer);
        }

        public TResp Call<TReq, TResp>(TReq req)
        {
            string text = Guid.NewGuid().ToString();
            IBasicProperties basicProperties = this._channel.CreateBasicProperties();
            basicProperties.ReplyTo = this._replyQueueName;
            basicProperties.CorrelationId = text;
            string s = JsonConvert.SerializeObject(req);
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            this._channel.BasicPublish("", "rpc_queue", basicProperties, bytes);
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
