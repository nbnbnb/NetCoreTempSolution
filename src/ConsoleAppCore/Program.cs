using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"The answer is {new LibraryStandard.Thing().Get(42)}");
        }
    }
}
