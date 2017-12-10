using Newtonsoft.Json;
using Svinx.Libraries.Queues.Delegates;
using System;
using System.Text;

namespace Svinx.Libraries.Queues
{
    public abstract class BaseRPCClient: IRPCClient
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

        public abstract void Start(string queue);

        public abstract TResp Call<TReq, TResp>(TReq req);
    }
}
