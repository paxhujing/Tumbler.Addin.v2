using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Test
{
    public class TestHost : IAddinHost
    {
        public TestHost()
        {
            MessageDispatcher = new MessageDispatcher(this);
            MessageService = new MessageService(this);
        }

        public String Id => MessageService.AddinHostId;

        public MessageDispatcher MessageDispatcher { get; }

        public MessageService MessageService { get; }

        public void Initialize()
        {
        }

        public bool Install()
        {
            return true;
        }

        public void OnReceive(Message message)
        {
            Console.WriteLine($"Host Receive message from {message.Source}");
            Console.WriteLine($"\tNeedResponse:{message.NeedResponse}");
            Console.WriteLine($"\tIsResponse:{message.IsResponse}");
            Console.WriteLine($"\tContentType:{message.ContentType.ToString()}");
            Console.WriteLine($"\tContentLength:{message.Content.Length}");
        }

        public void Send(Message message)
        {
            MessageService.Transmit(message);
        }

        public bool Uninstall()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AddinProxy> Load(String configFile)
        {
            return null;
        }
    }
}
