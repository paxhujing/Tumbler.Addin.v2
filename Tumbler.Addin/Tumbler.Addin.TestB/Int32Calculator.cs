using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.TestB
{
    public class Int32Calculator : IAddin
    {
        public Int32Calculator(AddinProxy proxy)
        {
            Proxy = proxy;
            MessageDispatcher = new MessageDispatcher(this);
        }

        public string Id { get; } = "74D56627-BD68-4C0F-B006-AB643E72DB8B";

        public MessageDispatcher MessageDispatcher { get; }

        public AddinProxy Proxy { get; }

        public void OnReceive(Message message)
        {
            Message response = null;
            if (message.ContentType == ContentType.JSON)
            {
                String json = message.ReadAsString();
                try
                {
                    Int32[] args = JsonConvert.DeserializeObject<Int32[]>(json);
                    Int32 result = args.Sum();
                    response = message.CreateResponseMessage(ContentType.Text, result.ToString());
                }
                catch (Exception ex)
                {
                    response = message.CreateErrorResponseMessage(ex);
                }
            }
            else
            {
                response = message.CreateErrorResponseMessage(new ArgumentException("Only json format allowed"));
            }
            this.Send(response);
        }

        public void Send(Message message)
        {
            Proxy.Send(message);
        }


        public void Load()
        {
        }

        public void Unload()
        {
        }
    }
}
