using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ConsoleAppCore.Demos.EmitAOP
{
    public class DynamicProxy
    {
        /// <summary>
        /// 根据接口和实现类型，注入 AOP 
        /// </summary>
        /// <typeparam name="TInterface">接口类</typeparam>
        /// <typeparam name="TImp">实现类</typeparam>
        /// <returns></returns>
        public static TInterface CreateProxyOfRealize<TInterface, TImp>() where TImp : class, new() where TInterface : class
        {
            return Invoke<TInterface, TImp>();
        }

        /// <summary>
        /// 根据实现类，注入 AOP
        /// 动态创建这个类型的实例，在方法调用前/后，可以加入 AOP 功能
        /// </summary>
        /// <typeparam name="TProxyClass">实现类（非接口）</typeparam>
        /// <returns></returns>
        public static TProxyClass CreateProxyOfInherit<TProxyClass>() where TProxyClass : class, new()
        {
            return Invoke<TProxyClass, TProxyClass>(true);
        }

        private static TInterface Invoke<TInterface, TImp>(bool inheritMode = false) where TImp : class, new() where TInterface : class
        {
            var impType = typeof(TImp);

            string nameOfAssembly = impType.Name + "ProxyAssembly";
            string nameOfModule = impType.Name + "ProxyModule";
            string nameOfType = impType.Name + "Proxy";

            var assemblyName = new AssemblyName(nameOfAssembly);

            // .NET Core
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(nameOfModule);

            /*
             * .NET Framework
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule(nameOfModule);
            */


            TypeBuilder typeBuilder;

            // 类继承模式
            if (inheritMode)
            {
                typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, impType);
            }
            // 接口继承模式
            else
            {
                typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, null, new[] { typeof(TInterface) });
            }

            // AOP 注入
            InjectInterceptor<TImp>(typeBuilder, impType.GetCustomAttribute(typeof(InterceptorBaseAttribute))?.GetType(), inheritMode);

            var t = typeBuilder.CreateType();

            return Activator.CreateInstance(t) as TInterface;
        }

        private static void InjectInterceptor<TImp>(TypeBuilder typeBuilder, Type interceptorAttributeType, bool inheritMode = false)
        {
            var impType = typeof(TImp);
            bool hasInterceptor = false;

            // 查看类型上是否有 InterceptorBaseAttribute 标记
            // 如果有，则在动态生成的类型中，添加一个私有字段，表示这个标记对象
            FieldBuilder interceptorFieldBuilder = null;
            if (interceptorAttributeType != null)
            {
                hasInterceptor = true;
                // 私有字段
                interceptorFieldBuilder = typeBuilder.DefineField("_interceptor", interceptorAttributeType, FieldAttributes.Private);

                // 从 TypeBuilder 生成 ConstructorBuilder
                // 根据类型信息
                // 获取一个 ConstructorBuilder
                ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);

                #region ILGenerator do Emit
                // 根据 ConstructorBuilder
                // 获取对应的 ILGenerator
                ILGenerator ilOfCtor = constructorBuilder.GetILGenerator();
                // 从 Stack 上，加载 arg-0 到 evaluation stack 上
                // arg-0 表示的是 this 变量
                ilOfCtor.Emit(OpCodes.Ldarg_0);
                // 这个对象创建好后，存储到 evaluation stack 上
                ilOfCtor.Emit(OpCodes.Newobj, interceptorAttributeType.GetConstructor(new Type[0]));
                // 弹出这个对象，到字段 fieldInterceptor 上
                ilOfCtor.Emit(OpCodes.Stfld, interceptorFieldBuilder);
                // 返回这个构造方法
                ilOfCtor.Emit(OpCodes.Ret);
                #endregion
            }

            // 忽略从 Object 继承的方法
            string[] ignoreMethodName = new[] { "GetType", "ToString", "GetHashCode", "Equals" };

            // 获取原始类型上的所有方法（公共、实例）
            var methodsOfType =
                impType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => !ignoreMethodName.Contains(m.Name));

            // 对每个方法进行判断
            // 是否需要执行 AOP 逻辑
            foreach (var method in methodsOfType)
            {
                bool hasAction = false;

                // 获取方法上的参数列表
                var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

                MethodAttributes methodAttributes;

                // 根据不同的模式
                // 设置动态生成方法的 MethodAttributes
                if (inheritMode)
                {
                    methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual;
                }
                else
                {
                    methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;
                }

                // 从 TypeBuilder 生成 MethodBuilder
                MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                    method.Name,
                    methodAttributes,
                    CallingConventions.Standard,
                    method.ReturnType,
                    methodParameterTypes);

                // 方法 ILGenerator
                ILGenerator ilMethod = methodBuilder.GetILGenerator();

                // CallStack 中需要的变量
                var impObjLocalBuilder = ilMethod.DeclareLocal(impType);                // instance of imp object
                var methodNameLocalBuilder = ilMethod.DeclareLocal(typeof(string));     // instance of method name
                var parametersLocalBuilder = ilMethod.DeclareLocal(typeof(object[]));   // instance of parameters
                var resultLocalBuilder = ilMethod.DeclareLocal(typeof(object));         // instance of result

                LocalBuilder actionLocalBuilder = null;

                // 首先判断方法上的 Action 标记（具有更高优先级）
                Type actionAttributeType = method.GetCustomAttribute(typeof(ActionBaseAttribute))?.GetType();
                if (actionAttributeType == null)
                {
                    // 如果方法没有，则从实例类上获取
                    actionAttributeType = impType.GetCustomAttribute(typeof(ActionBaseAttribute))?.GetType();
                }
                // 获取到了 Action 标记
                if (actionAttributeType != null)
                {
                    hasAction = true;
                    // 实例化这个 ActionAttribute，并存储在 Call Stack 上
                    actionLocalBuilder = ilMethod.DeclareLocal(actionAttributeType);
                    ilMethod.Emit(OpCodes.Newobj, actionAttributeType.GetConstructor(new Type[0]));
                    ilMethod.Emit(OpCodes.Stloc, actionLocalBuilder);
                }


                // 每个方法中，都会实例化一个原始类
                // 在调用原始类方法前，执行对应的 AOP 操作
                // 实例化需要代理的类，并存储在 Call Stack 上
                ilMethod.Emit(OpCodes.Newobj, impType.GetConstructor(new Type[0]));
                ilMethod.Emit(OpCodes.Stloc, impObjLocalBuilder);

                // 如果有 Interceptor 或 Action 标记，则需要执行 AOP
                if (hasAction || hasInterceptor)
                {
                    // 将方法名称存储到 Evaluation Stack 上
                    ilMethod.Emit(OpCodes.Ldstr, method.Name);
                    // 然后弹出，并存储到 Call Stack 变量上
                    ilMethod.Emit(OpCodes.Stloc, methodNameLocalBuilder);
                    // 存储一个 Int 值到 Evaluation Stack 上
                    // 它表示参数数组的长度
                    ilMethod.Emit(OpCodes.Ldc_I4, methodParameterTypes.Length);
                    // 然后实例化一个数组，它需要的参数，就是 Evaluation Stack 上的第一个值
                    ilMethod.Emit(OpCodes.Newarr, typeof(object));
                    // 然后弹出，并存储到 CallSatck 变量上
                    ilMethod.Emit(OpCodes.Stloc, parametersLocalBuilder);

                    // 对数组进行赋值
                    for (var j = 0; j < methodParameterTypes.Length; j++)
                    {
                        // 从 CallSatck 压入变量到 Evaluation Stack 上
                        ilMethod.Emit(OpCodes.Ldloc, parametersLocalBuilder);

                        // 存储一个 Int 值 j 到 Evaluation Stack 上
                        ilMethod.Emit(OpCodes.Ldc_I4, j);

                        // 获取方法的第一个 j+1 个参数，存储到 Evaluation Stack 上
                        // 注意，参数的索引是从 1 开始的，因为 0 表示的是 this
                        ilMethod.Emit(OpCodes.Ldarg, j + 1);

                        // 执行装箱指令，使用上面压入的 arg
                        ilMethod.Emit(OpCodes.Box, methodParameterTypes[j]);

                        // 现在，Evaluation Stack 上有三个值
                        // 从下到上为 parameters j value
                        // 这个指令，执行 parameters[j]=value
                        // 最后，弹出这三个值
                        ilMethod.Emit(OpCodes.Stelem_Ref);
                    }
                }

                // 执行 Action-Before 注入
                if (hasAction)
                {
                    // 从 CallStack 压入变量到 Evaluation Stack 上 
                    ilMethod.Emit(OpCodes.Ldloc, actionLocalBuilder);
                    // 从 CallStack 压入变量到 Evaluation Stack 上 
                    ilMethod.Emit(OpCodes.Ldloc, methodNameLocalBuilder);
                    // 从 CallStack 压入变量到 Evaluation Stack 上 
                    ilMethod.Emit(OpCodes.Ldloc, parametersLocalBuilder);
                    // 执行方法调用
                    // Evaluation Stack 上有三个值
                    // 执行完后弹出
                    // Before 方法没有返回值，所以没有对象会压入 Evaluation Stack 上
                    ilMethod.Emit(OpCodes.Call, actionAttributeType.GetMethod("Before"));
                }

                // 执行 Interceptor 注入
                if (hasInterceptor)
                {
                    // 从 CallStack 压入变量到 Evaluation Stack 上
                    // 参数索引 0，表示 this
                    ilMethod.Emit(OpCodes.Ldarg_0);
                    // Ldfld 指令需要上面的 this 参数 
                    // 这个类型变量加获取后，压入到 Evaluation Stack 上
                    ilMethod.Emit(OpCodes.Ldfld, interceptorFieldBuilder);
                    // 从 CallStack 压入变量到 Evaluation Stack 上
                    ilMethod.Emit(OpCodes.Ldloc, impObjLocalBuilder);
                    // 从 CallStack 压入变量到 Evaluation Stack 上
                    ilMethod.Emit(OpCodes.Ldloc, methodNameLocalBuilder);
                    // 从 CallStack 压入变量到 Evaluation Stack 上
                    ilMethod.Emit(OpCodes.Ldloc, parametersLocalBuilder);
                    // 注意，此处执行的是 Callvirt
                    // 这个方法需要 4 个参数，都存在了 Evaluation Stack 上
                    // 执行完后，这 4 个参数都弹出
                    // 返回值存储在 Evaluation Stack 上（result）
                    ilMethod.Emit(OpCodes.Callvirt, interceptorAttributeType.GetMethod("Invoke"));
                }
                else
                {
                    // 没有 Interceptor 的场景
                    // 直接调用实例类的方法

                    // 将实例类，加载到 Evaluation Stack 上
                    ilMethod.Emit(OpCodes.Ldloc, impObjLocalBuilder);
                    // 将每一个方法参数，加载到 Evaluation Stack 上
                    for (var j = 0; j < methodParameterTypes.Length; j++)
                    {
                        ilMethod.Emit(OpCodes.Ldarg, j + 1);
                    }
                    // 此处执行原始实例方法的调用
                    ilMethod.Emit(OpCodes.Callvirt, impType.GetMethod(method.Name));

                    // 如果还有 Action 调用，并且返回值不为 void
                    // 则将其装箱为 object
                    // 返回值存储在 Evaluation Stack 上（result）
                    if (hasAction && method.ReturnType != typeof(void))
                    {
                        ilMethod.Emit(OpCodes.Box, method.ReturnType);
                    }
                    else
                    {
                        ilMethod.Emit(OpCodes.Ldnull);
                    }
                }

                // 执行 Action-After 注入
                if (hasAction)
                {
                    // 将 Invoke 方法知道的结果，放入 result 变量中
                    // 如是 void，则 result == null
                    ilMethod.Emit(OpCodes.Stloc, resultLocalBuilder);

                    // 加载参数
                    ilMethod.Emit(OpCodes.Ldloc, actionLocalBuilder);
                    ilMethod.Emit(OpCodes.Ldloc, methodNameLocalBuilder);
                    ilMethod.Emit(OpCodes.Ldloc, resultLocalBuilder);
                    ilMethod.Emit(OpCodes.Call, actionAttributeType.GetMethod("After"));
                }

                // 如果返回值为 void
                // 则将弹入的 null 值弹出
                if (method.ReturnType == typeof(void))
                {
                    ilMethod.Emit(OpCodes.Pop);
                }
                else if (hasAction || hasInterceptor)
                {
                    // 值类型，执行拆箱
                    if (method.ReturnType.IsValueType)
                    {
                        ilMethod.Emit(OpCodes.Unbox_Any, method.ReturnType);
                    }
                    // 引用类型，执行类型转换
                    else
                    {
                        ilMethod.Emit(OpCodes.Castclass, method.ReturnType);
                    }
                }

                // 最后一个指令
                // Ret
                ilMethod.Emit(OpCodes.Ret);
            }
        }
    }


}
