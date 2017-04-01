using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Test
{
    class Program
    {
        static TestHost _host = new TestHost();

        static void Main(string[] args)
        {
            AddinManager manager = new AddinManager(_host, "addins.xml");
            IEnumerable<AddinProxy> proxies = manager.LoadAddins("menu");
            manager.Unload(proxies);
            foreach (AddinProxy proxy in proxies)
            {
                Console.WriteLine(proxy.Id);
            }
            //_host.MessageService.Register(proxyA);
            //_host.MessageService.Register(proxyB);

            ////Message message = _host.CreateMessage(proxyB.Id, true, ContentType.JSON, new Byte[10]);
            ////_host.Send(message);
            ////Console.ReadKey();

            ////message = _host.CreateMessage(proxyB.Id, false, ContentType.JSON, new Byte[5]);
            ////_host.Send(message);
            ////Console.ReadKey();

            //Message message = _host.CreateMessageToAll(false, ContentType.JSON, new Byte[2]);
            //_host.Send(message);
            //Console.ReadKey();

            //_host.MessageService.Unregister(proxyB.Id);
            //message = _host.CreateMessageToAll(false, ContentType.JSON, new Byte[2]);
            //_host.Send(message);
            //Console.ReadKey();

            //Console.Clear();
            //_host.MessageService.Register(proxyB);
            //message = _host.CreateMessageToAll(false, ContentType.JSON, new Byte[2]);
            //_host.Send(message);
            Console.ReadKey();

        }
    }
}
