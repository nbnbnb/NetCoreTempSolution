using System;

namespace ConsoleAppCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"LibraryCore The answer is {new MyLibraryStandard.Thing().Get(42)}.");
        }
    }
}