using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.TestA
{
    public class DoubleCalculator : IAddin
    {

        public DoubleCalculator(AddinProxy proxy)
        {
            Proxy = proxy;
            MessageDispatcher = new MessageDispatcher(this);
        }

        public string Id { get; } = "0FAEE6AA-72BA-4E13-8689-2B1F86A2502C";

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
                    Double[] args = JsonConvert.DeserializeObject<Double[]>(json);
                    Double result = args.Sum();
                    response = message.CreateResponseMessage(ContentType.Text, result.ToString());
                }
                catch(Exception ex)
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
