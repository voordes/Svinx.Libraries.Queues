using Svinx.Libraries.RabbitMQ.Delegates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Svinx.Libraries.RabbitMQ
{
    interface IRPCClient
    {
        event EventHandler Started;

        event MessageReceivedEventHandler MessageReceived;

        event ActionProcessedEventHandler ActionProcessed;

        event UnhandledExceptionEventHandler Exception;

        void OnStarted(EventArgs e);

        void OnMessageReceived(MessageArgs e);

        void OnException(UnhandledExceptionEventArgs e);

        void OnActionProcessed(ActionArgs e);

        void Start(string queue);

        TResp Call<TReq, TResp>(TReq req);
    }
}
