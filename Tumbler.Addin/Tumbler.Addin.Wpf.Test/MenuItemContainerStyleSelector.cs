using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Tumbler.Addin.Wpf.Test
{
    public class MenuItemContainerStyleSelector : StyleSelector
    {
        public Style NormalStyle { get; set; }

        public Style ContainerStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            ItemsGroup ig = (ItemsGroup)item;
            return ig.Items is ICollectionView ? ContainerStyle : NormalStyle;
        }
    }
}
