﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.TestA
{

    public class TestAddinAProxy : AddinProxy
    {
        protected override IAddin CreateAddin(AddinProxy proxy)
        {
            return new TestAddinA(proxy);
        }
    }
}