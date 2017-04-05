using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.TestB
{
    public class Int32CalculatorProxy : AddinProxy
    {
        protected override IAddin CreateAddin(AddinProxy proxy)
        {
            return new Int32Calculator(proxy);
        }
    }
}
