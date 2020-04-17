using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Linq;

namespace ConsoleAppCore.Util
{
    internal class ProxyUtil
    {
        private const string ProxyAssemblyName = "App.DynamicGenerated";
        private static readonly ModuleBuilder _moduleBuilder;
        private static readonly ConcurrentDictionary<string, Type> _proxyTypes = new ConcurrentDictionary<string, Type>();

        static ProxyUtil()
        {
            // 定义一个动态程序集
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(ProxyAssemblyName), AssemblyBuilderAccess.Run);
            _moduleBuilder = asmBuilder.DefineDynamicModule("Default");
        }

        public static Type CreateInterfaceProxy(Type interfaceType)
        {
            var proxyTypeName = $"{ProxyAssemblyName}.{interfaceType.FullName}";
            var type = _proxyTypes.GetOrAdd(proxyTypeName, name =>
            {
                // 定义要创建的类型，并实现指定类型接口
                var typeBuilder = _moduleBuilder.DefineType(name, TypeAttributes.Public, typeof(object), new[] { interfaceType });

                // 定义一个默认的构造方法
                typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

                // 获取接口中定义的方法
                var methods = interfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

                foreach (var method in methods)
                {
                    var methodBuilder = typeBuilder.DefineMethod(
                        method.Name,
                        MethodAttributes.Public | MethodAttributes.Virtual,
                        method.CallingConvention,
                        method.ReturnType,
                        method.GetParameters().Select(m => m.ParameterType).ToArray());

                    // 获取 ILGenerator，通过 Emit 实现方法体
                    var ilGenerator = methodBuilder.GetILGenerator();
                    ilGenerator.EmitWriteLine($"method {method.Name} is invoking...");
                    ilGenerator.Emit(OpCodes.Ret);

                    // 定义方法实现
                    typeBuilder.DefineMethodOverride(methodBuilder, method);
                }

                return typeBuilder.CreateType();
            });

            return type;
        }
    }
}
