using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Tumbler.Addin.Core;
using Tumbler.Addin.Wpf;
using Tumbler.Addin.Wpf.TestA.Back;

namespace Tumbler.Addin.Wpf.TestA
{
    /// <summary>
    /// Counter.xaml 的交互逻辑
    /// </summary>
    public partial class Counter : UserControl
    {
        private Int32 _counter;

        private Timer _timer;

        public MessageDispatcher MessageDispatcher => null;

        public string Id => "8FFA6D14-A8D2-468D-AE4E-FD5447EC8321";

        public Counter()
        {
            InitializeComponent();
            _timer = new Timer();
            _timer.Interval = 500;
            _timer.Elapsed += _timer_Tick;
        }


        private void _timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => CounterText.Text = _counter++.ToString());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _timer.Start();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _timer.Stop();
        }

        public void OnReceive(Message message)
        {
        }
    }
}
