using System;
using System.Collections.Generic;
using System.Linq;
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
            AddinProxy[] proxies = manager.LoadAddins("menu").ToArray();
            //manager.Unload(proxies);
            //foreach (AddinProxy proxy in proxies)
            //{
            //    Console.WriteLine(proxy.Id);
            //}

            Message message = _host.CreateMessage(proxies[0].Id, true, ContentType.JSON, new Byte[10]);
            _host.Send(message);
            manager.Unload(proxies[0]);
            _host.Send(message);
            Console.ReadKey();

            message = _host.CreateMessage(proxies[1].Id, false, ContentType.JSON, new Byte[5]);
            _host.Send(message);
            Console.ReadKey();

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
