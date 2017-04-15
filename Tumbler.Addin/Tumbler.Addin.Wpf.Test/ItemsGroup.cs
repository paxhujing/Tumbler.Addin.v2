using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf.Test
{
    public class ItemsGroup
    {
        public ItemsGroup(String groupName)
        {
            GroupName = groupName;
        }

        public String GroupName { get; }

        public IEnumerable Items { get; set; }
    }
}
