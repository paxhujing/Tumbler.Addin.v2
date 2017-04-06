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
        }

        public String Id => MessageService.AddinHostId;

        public MessageDispatcher MessageDispatcher { get; }

        public void Initialize()
        {
        }

        public bool Install()
        {
            return true;
        }

        public void OnReceive(Message message)
        {
            if (message.IsFailed)
            {
                Console.WriteLine($"[{message.Id}]Process message {message.Source} failed.\r\n\t{message.Exception}");
            }
            else
            {
                if (message.IsResponse)
                {
                    Console.WriteLine($"[{message.Id}]Response from {message.Source}.\r\n\t{message.ReadAsString()}");
                }
                else
                {
                    Console.WriteLine($"[{message.Id}]Addin request from {message.Source}.\r\n\t{message.ReadAsString()}");
                }
            }
        }

        public bool Uninstall()
        {
            throw new NotImplementedException();
        }
    }
}
