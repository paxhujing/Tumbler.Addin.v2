using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;
using Newtonsoft.Json;

namespace Tumbler.Addin.Test
{
    class Program
    {
        static TestHost _host = new TestHost();

        static void Main(string[] args)
        {
            TestAddinAProxy proxyA = new TestAddinAProxy();
            TestAddinBProxy proxyB = new TestAddinBProxy();

            _host.MessageService.Register(proxyA);
            _host.MessageService.Register(proxyB);

            //Message message = _host.CreateMessage(proxyB.Id, true, ContentType.JSON, new Byte[10]);
            //_host.Send(message);
            //Console.ReadKey();

            //message = _host.CreateMessage(proxyB.Id, false, ContentType.JSON, new Byte[5]);
            //_host.Send(message);
            //Console.ReadKey();

            Message message = _host.CreateMessageToAll(false, ContentType.JSON, new Byte[2]);
            _host.Send(message);
            Console.ReadKey();

            _host.MessageService.Unregister(proxyB.Id);
            message = _host.CreateMessageToAll(false, ContentType.JSON, new Byte[2]);
            _host.Send(message);
            Console.ReadKey();

            Console.Clear();
            _host.MessageService.Register(proxyB);
            message = _host.CreateMessageToAll(false, ContentType.JSON, new Byte[2]);
            _host.Send(message);
            Console.ReadKey();

        }
    }
}
