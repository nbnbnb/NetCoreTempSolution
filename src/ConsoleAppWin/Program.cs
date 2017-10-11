using MyLibraryStandard;
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
            Console.WriteLine($"LibraryCore The answer is {new Thing().Get(42)}.");
            Console.ReadKey(false);
        }
    }
}
