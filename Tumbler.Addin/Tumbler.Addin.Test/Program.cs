using System;
using System.Xml.Linq;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Test
{
    class Program
    {
        static TestHost _host = new TestHost();

        static void Main(string[] args)
        {
            AddinConfigParser parser = new AddinConfigParser(@"E:\Tumbler.Addin.v2\Tumbler.Addin\Tumbler.Addin.Test\bin\Debug\addins\addins.xml");
            AddinLoader loader = new AddinLoader();
            var c = parser.GetAddinNodes("menu");
            AddinProxy addin = null;
            AppDomain domain = null;
            //var b = parser.GetServiceNodes();
            foreach (XElement addinNode in c)
            {
                addin = loader.LoadAddin(addinNode, out domain);
                if (addin == null) continue;
                _host.MessageService.Register(addin);
                Console.WriteLine(addin.Id);
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
