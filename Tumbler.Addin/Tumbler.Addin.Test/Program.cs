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
    class Program : IAddinHost
    {
        public Program()
        {
            MessageService = new MessageService(this);
        }

        public MessageService MessageService { get; }

        static void Main(string[] args)
        {
            AddinAProxy proxy = new AddinAProxy();

            Program p = new Program();
            p.MessageService.Register(proxy);

            Test t = new Test { Name = "hj", Birthday = DateTime.Now, Age = 12 };
            String str = JsonConvert.SerializeObject(t);
            Message message = p.CreateMessage(proxy.Id, ContentType.JSON, str);

            p.Send(message);

            Console.ReadKey();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public bool Install()
        {
            throw new NotImplementedException();
        }

        public void OnReceive(Message message)
        {
            String str = message.ReadAsString();
        }

        public void Send(Message message)
        {
            MessageService.Send(message);
        }

        public bool Uninstall()
        {
            throw new NotImplementedException();
        }
    }

    class Test
    {
        public String Name { get; set; }

        public DateTime Birthday { get; set; }

        public Int32 Age { get; set; }
    }

    class AddinA : IAddin
    {
        public AddinA(AddinProxy proxy)
        {
            Proxy = proxy;
        }

        public string Id { get; } = "0FAEE6AA-72BA-4E13-8689-2B1F86A2502C";

        public AddinProxy Proxy { get;}

        public void OnReceive(Message message)
        {
            Message responseMessage = message.CreateResponseMessage(ContentType.Text, "hujing");
            this.SendMessage(responseMessage);
        }
    }

    class AddinAProxy : AddinProxy
    {
        protected override IAddin CreateAddin(AddinProxy proxy)
        {
            return new AddinA(proxy);
        }
    }
}
