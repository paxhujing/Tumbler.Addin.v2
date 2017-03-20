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
            Program p = new Program();
            Test t = new Test { Name = "hj", Birthday = DateTime.Now, Age = 12 };
            String str = JsonConvert.SerializeObject(t);
            p.Send(MessageService.AddinHostId, ContentType.JSON, str);
            Console.WriteLine("Hello World!");
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
}
