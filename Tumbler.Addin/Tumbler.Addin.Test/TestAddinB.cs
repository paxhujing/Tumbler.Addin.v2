﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Test
{
    public class TestAddinB : IAddin
    {
        public TestAddinB(AddinProxy proxy)
        {
            Proxy = proxy;
            MessageDispatcher = new MessageDispatcher(this);
        }

        public string Id { get; } = "74D56627-BD68-4C0F-B006-AB643E72DB8B";

        public MessageDispatcher MessageDispatcher { get; }

        public AddinProxy Proxy { get; }

        public void OnReceive(Message message)
        {
            Console.WriteLine($"TestAddinB Receive message from {message.Source}");
            Console.WriteLine($"\tNeedResponse:{message.NeedResponse}");
            Console.WriteLine($"\tIsResponse:{message.IsResponse}");
            Console.WriteLine($"\tContentType:{message.ContentType.ToString()}");
            Console.WriteLine($"\tContentLength:{message.Content.Length}");
            if (message.NeedResponse)
            {
                Message responseMessage = message.CreateResponseMessage(false, ContentType.Text, new Byte[3]);
                Send(responseMessage);
            }
        }

        public void Send(Message message)
        {
            Proxy.Send(message);
        }
    }


    public class TestAddinBProxy : AddinProxy
    {
        protected override IAddin CreateAddin(AddinProxy proxy)
        {
            return new TestAddinB(proxy);
        }
    }
}