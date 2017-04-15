using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf.TestA
{
    public class TestAActivator : AddinActivatorBase
    {
        protected override bool LaunchCore()
        {
            return true;
        }

        protected override bool CloseCore()
        {
            return true;
        }


        protected override FrameworkElement CreateView(IAddin addin)
        {
            return new Counter();
        }

        protected override bool InactiveCore()
        {
            return true;
        }
    }
}
