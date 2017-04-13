using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf.TestA.Back
{
    [AddinProxy(typeof(TestAAddinProxy))]
    public class TestAAddin : WpfAddinBase
    {
        private readonly Timer _timer = new Timer(1000);

        public TestAAddin()
            :base("0FAEE6AA-72BA-4E13-8689-2B1F86A2502C")
        {
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

    }
}
