using System;
using Xunit;

namespace TestMyLibraryStandard
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
