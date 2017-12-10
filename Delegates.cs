using System;
using System.Collections.Generic;
using System.Text;

namespace Svinx.Libraries.RabbitMQ.Delegates
{
    public delegate void MessageReceivedEventHandler(object sender, MessageArgs e);
    public delegate void ActionProcessedEventHandler(object sender, ActionArgs e);

    public class MessageArgs : EventArgs
    {
        public string Message
        {
            get;
            set;
        }

        public MessageArgs(string message)
        {
            this.Message = message;
        }
    }

    public class ActionArgs : EventArgs
    {
        public long TimeTaken
        {
            get;
            set;
        }

        public string Result
        {
            get;
            set;
        }

        public ActionArgs(long milliseconds, string result)
        {
            this.TimeTaken = milliseconds;
            this.Result = result;
        }
    }
}
