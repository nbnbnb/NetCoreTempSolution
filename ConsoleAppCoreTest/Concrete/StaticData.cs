using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.UnitTests.Concrete
{
    class StaticData
    {
        public static bool IsAssemblyInit { get; set; }

        public static bool IsClassInit { get; set; }
    }
}
