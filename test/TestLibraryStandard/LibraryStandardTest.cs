using System;
using Xunit;

namespace TestLibraryStandard
{
    public class LibraryStandardTest
    {
        [Fact]
        public void ThingGetsObjectValFromNumber()
        {
            Assert.Equal(42, new MyLibraryStandard.Thing().Get(42));
        }
    }
}
