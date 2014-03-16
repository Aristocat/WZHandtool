using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WZHandtool
{
    public struct MapLifeInformation
    {
        public string Name { get; set; }

        public SortedDictionary<int, string> Life { get; set; }
    }
}
