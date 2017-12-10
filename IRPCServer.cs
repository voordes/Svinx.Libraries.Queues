using Svinx.Libraries.Queues.Delegates;
using System;

namespace Svinx.Libraries.Queues
{
    public interface IRPCServer
    {
        event EventHandler Started;

        event MessageReceivedEventHandler MessageReceived;

        event ActionProcessedEventHandler ActionProcessed;

        event UnhandledExceptionEventHandler Exception;

        void OnStarted(EventArgs e);

        void OnMessageReceived(MessageArgs e);

        void OnException(UnhandledExceptionEventArgs e);

        void OnActionProcessed(ActionArgs e);

        void ListenOn<TReq, TResp>(Func<TReq, TResp> callback);

        void Connect();
    }
}
