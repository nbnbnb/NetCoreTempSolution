using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.AspectCore
{
    [SampleInterceptor]
    public interface ISampleInterface
    {
        void Foo();
    }
}
