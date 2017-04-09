using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf.Test
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application,IAddinHost
    {
        public App()
        {
            MessageDispatcher = new MessageDispatcher(this);
            AddinManager = new AddinManager(this, @"addins\addins.xml");
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }


        public Core.AddinManager AddinManager { get; }

        /// <summary>
        /// 文件日志记录器。
        /// </summary>
        public ILog Logger { get; } = LogManager.GetLogger("FileLogger");

        public String Id { get; } = "4E4928D0-4EF1-43F8-BCED-95C012D9DBF1";

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

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //e.Handled = true;
            //MessageBox.Show(e.Exception.Message);
        }
    }
}
