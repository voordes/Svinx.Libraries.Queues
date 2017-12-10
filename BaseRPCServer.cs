using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Svinx.Libraries.Queues.Delegates;
using System;
using System.Text;

namespace Svinx.Libraries.Queues
{
    public abstract class BaseRPCServer:IRPCServer
    {
        public event EventHandler Started;

        public event MessageReceivedEventHandler MessageReceived;

        public event ActionProcessedEventHandler ActionProcessed;

        public event UnhandledExceptionEventHandler Exception;

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

        public abstract void ListenOn<TReq, TResp>(Func<TReq, TResp> callback);

        public abstract void Connect();
    }
}
