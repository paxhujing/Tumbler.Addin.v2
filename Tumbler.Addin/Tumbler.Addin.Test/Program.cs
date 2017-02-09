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
        static void Main(string[] args)
        {
            Message message = new Message(Guid.NewGuid().ToString().Replace("-", String.Empty),
                Guid.NewGuid().ToString().Replace("-", String.Empty),
                new Test { Name = "HuJing", Birthday = DateTime.Now, Age = 24 });
            MessageFormatter mf = new MessageFormatter();
            Byte[] data = mf.Serialize(message);
            message = mf.Deserialize(data);
            Console.WriteLine("Hello World!"); 
        }
    }

    class Test
    {
        public String Name { get; set; }

        public DateTime Birthday { get; set; }

        public Int32 Age { get; set; }
    }
}
