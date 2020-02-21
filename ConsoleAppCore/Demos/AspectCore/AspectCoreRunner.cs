using AspectCore.DynamicProxy;
using AspectCore.Extensions.Autofac;
using AspectCore.Extensions.DependencyInjection;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.AspectCore
{
    /// <summary>
    /// AspectCore 基本使用
    /// https://www.cnblogs.com/liuhaoyang/p/aspectcore-getting-started.html
    /// </summary>
    public class AspectCoreRunner
    {
        public static void Run()
        {
            Basic();
            IocBuildIn();
            IoCAutofac();
        }

        /// <summary>
        /// 基本使用方式
        /// 非 IoC 场景
        /// </summary>
        static void Basic()
        {
            ProxyGeneratorBuilder proxyGeneratorBuilder = new ProxyGeneratorBuilder();

            IProxyGenerator proxyGenerator = proxyGeneratorBuilder.Build();

            ISampleInterface sampleInterface = proxyGenerator.CreateInterfaceProxy<ISampleInterface, SampleClass>();
            Console.WriteLine(sampleInterface);
            sampleInterface.Foo();
        }

        /// <summary>
        /// IoC 场景
        /// .NET Core 内建 Ioc
        /// </summary>
        static void IocBuildIn()
        {
            // 使用系统 IoC 方式
            // Install-Package AspectCore.Extensions.DependencyInjection        
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<ISampleInterface, SampleClass>();

            services.ConfigureDynamicProxy();

            IServiceProvider serviceProvider = services.BuildDynamicProxyProvider();
            ISampleInterface sampleInterface = serviceProvider.GetService<ISampleInterface>();
            Console.WriteLine(sampleInterface);
            sampleInterface.Foo();
        }

        /// <summary>
        /// IoC 场景
        /// Autofac
        /// </summary>
        static void IoCAutofac()
        {
            // Autofac IOC 方式
            // Install-Package AspectCore.Extensions.Autofac
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<SampleClass>().As<ISampleInterface>();

            builder.RegisterDynamicProxy();
            var container = builder.Build();

            ISampleInterface sampleInterface = container.Resolve<ISampleInterface>();
            Console.WriteLine(sampleInterface);
            sampleInterface.Foo();
        }
    }
}
