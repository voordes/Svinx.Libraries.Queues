using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Svinx.Libraries.Queues.Delegates;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Svinx.Libraries.Queues.RabbitMQ
{
    public class RPCServer: BaseRPCServer
    {
        private bool _listening;

        private string _queueUrl;

        private string _queueName;

        private IModel _channel;

        private QueueingBasicConsumer _consumer;

        public RPCServer(IOptions<Queue> options)
        {
            this._queueUrl = options.Value.queueUrl;
            this._queueName = options.Value.queueName;
            Connect();
        }

        public override async Task Listen<TReq, TResp>(Func<TReq, TResp> callback)
        {
            _listening = true;
            while (true)
            {
                object obj = null;
                BasicDeliverEventArgs basicDeliverEventArgs = await Task.Run(() => this._consumer.Queue.Dequeue());
                byte[] body = basicDeliverEventArgs.Body;
                IBasicProperties basicProperties = basicDeliverEventArgs.BasicProperties;
                IBasicProperties basicProperties2 = this._channel.CreateBasicProperties();
                basicProperties2.CorrelationId = basicProperties.CorrelationId;
                long milliseconds = 0L;
                try
                {
                    string @string = Encoding.UTF8.GetString(body);
                    this.OnMessageReceived(new MessageArgs(@string));
                    TReq req = JsonConvert.DeserializeObject<TReq>(@string);
                    obj = Diagnostics.RunAndLogTime<TReq, TResp>(callback, req, out milliseconds);
                }
                catch (Exception exception)
                {
                    this.OnException(new UnhandledExceptionEventArgs(exception, false));
                }
                finally
                {
                    string text = JsonConvert.SerializeObject(obj);
                    this.OnActionProcessed(new ActionArgs(milliseconds, text));
                    byte[] bytes = Encoding.UTF8.GetBytes(text);
                    this._channel.BasicPublish("", basicProperties.ReplyTo, basicProperties2, bytes);
                    this._channel.BasicAck(basicDeliverEventArgs.DeliveryTag, false);
                }
            }
        }

        public void Connect()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(this._queueUrl)
            };
            var connection = connectionFactory.CreateConnection();
            this._channel = connection.CreateModel();
            this._channel.QueueDeclare(this._queueName, false, false, false, null);
            this._channel.BasicQos(0u, 1, false);
            this._consumer = new QueueingBasicConsumer(this._channel);
            this._channel.BasicConsume(this._queueName, false, _consumer);
            this.OnStarted(EventArgs.Empty);
            connection.AutoClose = true;
        }

        public override void Stop()
        {
            _listening = false;
        }

        public void Disconnect()
        {
            this._channel.Close();
            this._channel.Dispose();
            this._consumer = null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.Stop();
        }
    }
}
