using Svinx.Libraries.Queues.Delegates;
using System;

namespace Svinx.Libraries.Queues
{
    public interface IRPCClient
    {
        event EventHandler Started;

        event MessageReceivedEventHandler MessageReceived;

        event ActionProcessedEventHandler ActionProcessed;

        event UnhandledExceptionEventHandler Exception;

        void OnStarted(EventArgs e);

        void OnMessageReceived(MessageArgs e);

        void OnException(UnhandledExceptionEventArgs e);

        void OnActionProcessed(ActionArgs e);

        TResp Call<TReq, TResp>(TReq req);
    }
}
