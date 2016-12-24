using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"The answer is {new Thing().Get(42)}");
        }
    }
}
