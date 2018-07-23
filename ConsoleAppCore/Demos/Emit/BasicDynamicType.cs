using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ConsoleAppCore.Demos.Emit
{
    class BasicDynamicType
    {
        internal static Type DynamicCreateType()
        {
            // 动态创建程序集
            AssemblyName assemblyName = new AssemblyName("DynamicAssemble");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            // 动态创建模块
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            // 动态创建类型
            TypeBuilder typeBuilder = moduleBuilder.DefineType("MyClass", TypeAttributes.Public);
            // 动态创建字段
            FieldBuilder fieldBuilder = typeBuilder.DefineField("myField", typeof(String), FieldAttributes.Private);
            // 动态创建构造函数
            Type[] ctorType = new Type[] { typeof(String) };
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorType);
            // 生成构造函数指令
            ILGenerator iLGenerator = constructorBuilder.GetILGenerator();
            // 调用基类的构造函数
            // 将索引为 0 的参数加载到计算堆栈上
            // Ldarg_0 代表访问的是this对象
            iLGenerator.Emit(OpCodes.Ldarg_0);
            // 调用由传递的方法说明符指示的方法
            iLGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

            // 给字段赋值
            // 将索引为 0 的参数加载到计算堆栈上
            iLGenerator.Emit(OpCodes.Ldarg_0);
            // 将索引为 1 的参数加载到计算堆栈上
            iLGenerator.Emit(OpCodes.Ldarg_1);
            // 用新值替换在对象引用或指针的字段中存储的值
            iLGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            // 从当前方法返回，并将返回值（如果存在）从调用方的计算堆栈推送到被调用方的计算堆栈上
            iLGenerator.Emit(OpCodes.Ret);

            // 动态创建属性
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("MyProperty", PropertyAttributes.HasDefault, typeof(String), null);

            // 动态创建方法
            // MyMethod():String
            MethodBuilder myMethod = typeBuilder.DefineMethod(
                "MyMethod",
                MethodAttributes.Public | MethodAttributes.SpecialName,
                typeof(String), // 返回类型
                Type.EmptyTypes); // 参数类型
            // 生成方法指令
            ILGenerator methodIL = myMethod.GetILGenerator();
            // 加载字段，然后返回
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, fieldBuilder);
            methodIL.Emit(OpCodes.Ret);

            // 保存动态创建的程序集 (程序集将保存在程序目录下调试时就在Debug下)
            // assemblyBuilder.Save

            // 使用动态类型创建类型
            return typeBuilder.CreateType();
        }
    }
}
