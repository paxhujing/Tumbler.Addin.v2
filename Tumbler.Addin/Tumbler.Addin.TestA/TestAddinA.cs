using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.TestA
{
    public class TestAddinA : IAddin
    {

        public TestAddinA(AddinProxy proxy)
        {
            Proxy = proxy;
            MessageDispatcher = new MessageDispatcher(this);
        }

        public string Id { get; } = "0FAEE6AA-72BA-4E13-8689-2B1F86A2502C";

        public MessageDispatcher MessageDispatcher { get; }

        public AddinProxy Proxy { get; }

        public void OnReceive(Message message)
        {
            Console.WriteLine($"TestAddinA Receive message from {message.Source}");
            Console.WriteLine($"\tNeedResponse:{message.NeedResponse}");
            Console.WriteLine($"\tIsResponse:{message.IsResponse}");
            Console.WriteLine($"\tContentType:{message.ContentType.ToString()}");
            Console.WriteLine($"\tContentLength:{message.Content.Length}");
        }

        public void Send(Message message)
        {
            Proxy.Send(message);
        }

        public void Unload()
        {
        }
    }
}
