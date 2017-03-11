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
            // 4.6.1 实现了 1.4
            // 4.6.2 实现了 1.5

            //Console.WriteLine($"LibraryCore The answer is {new MyLibraryStandard.Thing().Get(42)}.");


            Console.WriteLine("Hello world");
            Console.WriteLine($"LibraryCore The answer is {new MyLibraryStandard.Thing().Get(42)}.");
        }
    }
}
