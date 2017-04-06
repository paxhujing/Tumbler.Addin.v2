using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            IMessageTarget[] proxies = manager.LoadAddins("menu").ToArray();

            //String[] a = { "0FAEE6AA-72BA-4E13-8689-2B1F86A2502C", "74D56627-BD68-4C0F-B006-AB643E72DB8B","" };
            //Message message = _host.CreateMulticastMessage(a, ContentType.JSON, new Byte[10]);
            //_host.Send(message);
            //manager.Unload(proxies[0]);
            //_host.Send(message);
            //Console.ReadKey();

            //message = _host.CreateUnicastMessage(proxies[1].Id, ContentType.JSON, new Byte[5]);
            //_host.Send(message);
            //Console.ReadKey();

            Double[] a = new Double[] { 1.0, 2.0, 3.0 };
            String json = JsonConvert.SerializeObject(a);
            Message message = _host.CreateBroadcastMessage(ContentType.JSON, json);
            _host.SendMessage(message);
            Console.ReadKey();

            Console.Clear();
            Int32[] b = new Int32[] { 10, 20, 30 };
            json = JsonConvert.SerializeObject(b);
            manager.Unregister(proxies[0].Id);
            message = _host.CreateBroadcastMessage( ContentType.JSON, json);
            _host.SendMessage(message);
            Console.ReadKey();

            Console.Clear();
            manager.Register(proxies[0]);
            message = _host.CreateBroadcastMessage(ContentType.JSON, json);
            _host.SendMessage(message);
            Console.ReadKey();

        }
    }
}
