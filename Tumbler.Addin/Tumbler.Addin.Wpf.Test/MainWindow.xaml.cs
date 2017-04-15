using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tumbler.Addin.Core;

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

        public ObservableCollection<ItemsGroup> AddinInfos { get; } = new ObservableCollection<ItemsGroup>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App app = (App)Application.Current;
            WpfAddinManager am = (WpfAddinManager)app.AddinManager;

            IEnumerable<String> menuSubNames = am.GetSubNames("menu");
            ItemsGroup group = null;
            Int32 i = 0;
            foreach (String subName in menuSubNames)
            {
                group = new ItemsGroup(ConvertName(subName));
                group.Items = am.GetAddinInfos("menu", subName).ToArray();
                AddinInfos.Add(group);
                i++;
                if (i == 3) break;
            }
            ItemsGroup miscGroup = new ItemsGroup("更多");

            Collection<GroupContainerItem> subGroups = new Collection<GroupContainerItem>();
            foreach (String subName in menuSubNames.Skip(3))
            {
                foreach (AddinInfo addinInfo in am.GetAddinInfos("menu", subName))
                {
                    GroupContainerItem item = new GroupContainerItem();
                    item.Category = ConvertName(subName);
                    item.Item = addinInfo;
                    subGroups.Add(item);
                }
            }
            ICollectionView view = CollectionViewSource.GetDefaultView(subGroups);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            miscGroup.Items = view;
            AddinInfos.Add(miscGroup);
        }

        private String ConvertName(String subName)
        {
            switch (subName)
            {
                case "scene":
                    return "场景";
                case "fly":
                    return "飞行";
                case "query":
                    return "查询";
                case "petrol":
                    return "巡更";
                case "location":
                    return "定位";
                case "plan":
                    return "预案";
                case "measurement":
                    return "测量";
                default:
                    return "其它";
            }
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ((Grid)sender).Background = Brushes.Blue;
        }
        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            ((TextBlock)sender).Background = Brushes.Red;

        }

        private void ListBox_Click(object sender, RoutedEventArgs e)
        {
            ButtonBase btn = (ButtonBase)e.OriginalSource;
            if (btn == null) return;
            AddinActivatorBase activator = btn.Tag as AddinActivatorBase;
            if (activator == null)
            {
                AddinInfo info = btn.DataContext as AddinInfo;
                if (info == null) return;
                App app = (App)Application.Current;
                WpfAddinManager am = (WpfAddinManager)app.AddinManager;
                activator = am.GetAddinActivator((WpfAddinInfo)info);
                btn.Tag = activator;
                activator.Active();
                if (activator.IsActived)
                {
                    activator.Launch();
                    Addins.Children.Add(activator.View);
                }
            }
            else
            {
                if (activator.IsLaunched)
                {
                    activator.Close();
                    Addins.Children.Remove(activator.View);
                }
            }
        }
    }
}
