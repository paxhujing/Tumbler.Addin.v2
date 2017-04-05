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
            String[] a = { "0FAEE6AA-72BA-4E13-8689-2B1F86A2502C", "74D56627-BD68-4C0F-B006-AB643E72DB8B","" };
            Message message = _host.CreateMulticastMessage(a, ContentType.JSON, new Byte[10]);
            _host.Send(message);
            manager.Unload(proxies[0]);
            _host.Send(message);
            Console.ReadKey();

            message = _host.CreateUnicastMessage(proxies[1].Id, ContentType.JSON, new Byte[5]);
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
