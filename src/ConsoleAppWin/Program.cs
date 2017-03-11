using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppWin
{
    class Program
    {
        static void Main(string[] args)
        {
            // 4.6.2 实现了 1.5
            // 4.6.1 实现了 1.4

            Console.WriteLine($"LibraryCore The answer is {new MyLibraryStandard.Thing().Get(42)}.");

            // 使用 PCL 可以直接进行项目引用
            // Console.WriteLine($"LibraryPCL The answer is {new LibraryPCL.Thing().Get(42)}.");

            // 使用 PCL 转 Net Standard 可以直接进行项目引用
            // Console.WriteLine($"LibraryPCLToNetStandard The answer is {new LibraryPCLToNetStandard.Thing().Get(42)}.");
        }
    }
}
