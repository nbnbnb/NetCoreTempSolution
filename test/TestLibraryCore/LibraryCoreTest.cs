using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestLibraryCore
{
    public class LibraryCoreTest
    {
        [Fact]
        public void ThingGetsObjectValFromNumber()
        {
            Assert.Equal(42, new LibraryCore.Thing().Get(42));
        }
    }
}
