using Newtonsoft.Json;
using Svinx.Libraries.Queues.Delegates;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Svinx.Libraries.Queues
{
    public abstract class BaseRPCClient: IRPCClient, IDisposable
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

        public abstract Task<TResp> Call<TReq, TResp>(TReq req);

        public abstract void Cancel();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BaseRPCClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

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
