using ConsoleAppCore.Demos;
using ConsoleAppCore.Util;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ConsoleAppCore.MyLinq;
using ConsoleAppCore.MyLinq.SimpleVisitor;
using ConsoleAppCore.MyLinq.LinqToTerra;
using System.Reflection.Emit;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
using Ninject.Extensions.Conventions;
using MediatR;
using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Ninject.Syntax;
using Ninject.Components;
using ConsoleAppCore.Demos.MediatR;

namespace ConsoleAppCore
{

    class Program
    {
        static void Main(String[] args)
        {
            Console.WriteLine("----- Start -----");
            //TheDemos();
            Temp();
            Console.WriteLine("-----  End  -----");
            Console.WriteLine();
            Console.WriteLine("按任意键退出");
            Console.ReadKey(true);
        }

        static void Temp()
        {
            Runner.MediatR();
        }

        static void TheDemos(String[] args)
        {
            Runner.FileWatch();
            Runner.ReadCommandLineArgs();
            Runner.ReadEFConfig();
            Runner.ShowTaskLogger().NoWarning();
            Runner.ShowAppDomainExceptions();
            Runner.LockerDiff();
            Runner.CreateDynamicType();
            Runner.MediatR();
        }
    }
}