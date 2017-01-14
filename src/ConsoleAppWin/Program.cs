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
            // 当前这个应用是 4.6.1

            // 由于 4.6.1 版本只实现了 .NET Standard 1.4 
            // 所以我们引用 1.4 的 .NET Core 类库

            // 目前从 .NET Framework 中添加 .NET Core 程序集
            // 只能使用指定路径的方式，还不支持项目引用
            Console.WriteLine($"LibraryCore The answer is {new LibraryStandard.Thing().Get(42)}.");

            // 使用 PCL 可以直接进行项目引用
            // Console.WriteLine($"LibraryPCL The answer is {new LibraryPCL.Thing().Get(42)}.");

            // 使用 PCL 转 Net Standard 可以直接进行项目引用
            // Console.WriteLine($"LibraryPCLToNetStandard The answer is {new LibraryPCLToNetStandard.Thing().Get(42)}.");
        }
    }
}
