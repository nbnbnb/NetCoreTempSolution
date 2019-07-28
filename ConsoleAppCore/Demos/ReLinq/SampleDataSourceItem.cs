using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.ReLinq
{
    internal class SampleDataSourceItem
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"Name:{Name} - Description:{Description}";
        }
    }
}
