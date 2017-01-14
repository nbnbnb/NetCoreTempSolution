using System;
using Xunit;

namespace TestLibraryStandard
{
    public class LibraryStandardTest
    {
        [Fact]
        public void ThingGetsObjectValFromNumber()
        {
            Assert.Equal(42, new LibraryStandard.Thing().Get(42));
        }
    }
}
