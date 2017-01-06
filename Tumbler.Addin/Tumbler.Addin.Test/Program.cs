using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                var other = AppDomain.CreateDomain($"Test{i}", AppDomain.CurrentDomain.Evidence, new AppDomainSetup
                {
                    LoaderOptimization = LoaderOptimization.MultiDomainHost
                });
            }
        }
    }

}
