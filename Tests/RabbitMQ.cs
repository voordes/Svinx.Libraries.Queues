using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Svinx.Libraries.Queues.RabbitMQ;
using System;
using System.Threading.Tasks;

namespace Svinx.Libraries.Queues.Tests
{
    [TestClass]
    public class RabbitMQ
    {
        [TestMethod]
        public void RPC()
        {
            var t1 = Client();
            var t2 = Server();
            Task.WaitAll(new Task[] { t1, t2 }, 1000);
            //System.Threading.Thread.Sleep(5000);
            Assert.IsTrue(t1.IsCompleted, "RPC did not finish within 1 second");
            //Assert.IsTrue(t2.IsCompleted, "Server did not finish within 1 seconds");
            Assert.IsTrue(t1.Exception == null, $"Client returned exception: {t1.Exception?.ToString()}");
            Assert.IsTrue(t2.Exception == null, $"Server returned exception: {t2.Exception?.ToString()}");
        }

        private async Task Client()
        {
            var options = Options.Create(new Queue()
            {
                queueUrl = "amqp://hjtqnayg:8s2sjVjplSmQ0G7gNBOWt7f-r0Df60R8@sheep.rmq.cloudamqp.com/hjtqnayg",
                queueName = "Search"
            });
            var client = new RPCClient(options);
            var result = await client.Call<dynamic, dynamic>(new { a = 1, b = 2 });
            Assert.IsTrue(result.a == 2 & result.b == 3, $"Result (a={result.a}, b={result.b}) does not match expected result (a=2, b=3)");
        }

        private async Task Server()
        {
            var options = Options.Create(new Queue()
            {
                queueUrl = "amqp://hjtqnayg:8s2sjVjplSmQ0G7gNBOWt7f-r0Df60R8@sheep.rmq.cloudamqp.com/hjtqnayg",
                queueName = "Search"
            });
            var server = new RPCServer(options);
            Func<dynamic, dynamic> f = (req => new { a = req.a + 1, b = req.b + 1 });
            await server.ListenOn<dynamic, dynamic>(f);
        }
    }
}
