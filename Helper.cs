using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Svinx.Libraries.RabbitMQ
{
    public static class Helper
    {
        public static TResp RunAndLogTime<TReq, TResp>(Func<TReq, TResp> callback, TReq req, out long milliseconds)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            milliseconds = 0L;
            object obj;
            try
            {
                obj = callback(req);
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw new Exception(string.Format("RunAndLogTime: {0}", ex.Message), ex);
            }
            finally
            {
                stopwatch.Stop();
                milliseconds = stopwatch.ElapsedMilliseconds;
            }
            return (TResp)((object)obj);
        }
    }
}
