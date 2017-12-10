using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Svinx.Libraries.RabbitMQ.Delegates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Svinx.Libraries.RabbitMQ
{
    public class RPCServer
    {
        private string _uri;

        private string _queue;

        private IModel _channel;

        private QueueingBasicConsumer _consumer;

        public event EventHandler Started;

        public event MessageReceivedEventHandler MessageReceived;

        public event ActionProcessedEventHandler ActionProcessed;

        public event UnhandledExceptionEventHandler Exception;

        public RPCServer(string uri, string queue)
        {
            this._uri = uri;
            this._queue = queue;
        }

        protected virtual void OnStarted(EventArgs e)
        {
            if (this.Started != null)
            {
                this.Started(this, e);
            }
        }

        protected virtual void OnMessageReceived(MessageArgs e)
        {
            if (this.MessageReceived != null)
            {
                this.MessageReceived(this, e);
            }
        }

        protected virtual void OnException(UnhandledExceptionEventArgs e)
        {
            if (this.Exception != null)
            {
                this.Exception(this, e);
            }
        }

        protected virtual void OnActionProcessed(ActionArgs e)
        {
            if (this.ActionProcessed != null)
            {
                this.ActionProcessed(this, e);
            }
        }

        public void ListenOn<TReq, TResp>(Func<TReq, TResp> callback)
        {
            while (true)
            {
                object obj = null;
                BasicDeliverEventArgs basicDeliverEventArgs = this._consumer.Queue.Dequeue();
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
                    obj = Helper.RunAndLogTime<TReq, TResp>(callback, req, out milliseconds);
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
                Uri = new Uri(this._uri)
            };
            IConnection connection = connectionFactory.CreateConnection();
            this._channel = connection.CreateModel();
            this._channel.QueueDeclare(this._queue, false, false, false, null);
            this._channel.BasicQos(0u, 1, false);
            this._consumer = new QueueingBasicConsumer(this._channel);
            this._channel.BasicConsume(this._queue, false, this._consumer);
            this.OnStarted(EventArgs.Empty);
        }
    }
}
