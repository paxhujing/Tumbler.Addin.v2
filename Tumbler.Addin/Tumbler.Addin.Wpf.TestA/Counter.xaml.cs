using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Tumbler.Addin.Wpf.TestA.Back;

namespace Tumbler.Addin.Wpf.TestA
{
    /// <summary>
    /// Counter.xaml 的交互逻辑
    /// </summary>
    public partial class Counter : UserControl
    {
        private Int32 _counter;

        private readonly TestAAddinProxy _proxy;

        public Counter()
        {
            InitializeComponent();
            _proxy = (TestAAddinProxy)Tag; 
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            CounterText.Text = _counter++.ToString();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
