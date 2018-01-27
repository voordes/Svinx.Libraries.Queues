using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Svinx.Libraries.Queues.Delegates;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Svinx.Libraries.Queues
{
    public abstract class BaseRPCServer:IRPCServer, IDisposable
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

        public abstract Task Listen<TReq, TResp>(Func<TReq, TResp> callback);

        public abstract void Stop();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
