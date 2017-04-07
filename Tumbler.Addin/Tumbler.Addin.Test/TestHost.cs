using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Test
{
    public class TestHost : IAddinHost
    {
        public TestHost()
        {
            MessageDispatcher = new MessageDispatcher(this);
            AddinManager = new AddinManager(this, @"addins\addins.xml");
        }

        public AddinManager AddinManager { get; }

        /// <summary>
        /// 文件日志记录器。
        /// </summary>
        public ILog Logger { get; } = LogManager.GetLogger("FileLogger");

        public String Id { get; }= "4E4928D0-4EF1-43F8-BCED-95C012D9DBF1";

        public MessageDispatcher MessageDispatcher { get; }

        public void Initialize()
        {
        }

        public bool Install()
        {
            return true;
        }

        public void OnReceive(Message message)
        {
            if (message.IsFailed)
            {
                Console.WriteLine($"[{message.Id}]Process message {message.Source} failed.\r\n\t{(Exception)message.Content}");
            }
            else
            {
                if (message.IsResponse)
                {
                    Console.WriteLine($"[{message.Id}]Response from {message.Source}.\r\n\t{message.ReadAsString()}");
                }
                else
                {
                    Console.WriteLine($"[{message.Id}]Addin request from {message.Source}.\r\n\t{message.ReadAsString()}");
                }
            }
        }

        public bool Uninstall()
        {
            throw new NotImplementedException();
        }
    }
}
