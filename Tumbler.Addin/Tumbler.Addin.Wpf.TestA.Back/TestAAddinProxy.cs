using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf.TestA.Back
{
    public class TestAAddinProxy : WpfAddinProxy
    {
        public TestAAddinProxy()
            : base(new TestAAddin())
        {
        }
    }
}
