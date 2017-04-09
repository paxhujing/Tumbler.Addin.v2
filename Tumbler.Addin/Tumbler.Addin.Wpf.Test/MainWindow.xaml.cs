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

namespace Tumbler.Addin.Wpf.Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App app = (App)Application.Current;
            AddinManager am = (AddinManager)app.AddinManager;
            var addins = am.LoadAddins("menu");
            FrameworkElement ui = null;
            foreach (var addin in addins.OfType<AddinProxy>())
            {
                ui = am.GetAddinUI(addin);
            }
            Content = ui;
        }
    }
}
