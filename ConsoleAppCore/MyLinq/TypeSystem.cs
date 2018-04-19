using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    internal static class TypeSystem
    {
        /// <summary>
        /// 解析 Expression 中的 ElementType
        /// </summary>
        /// <param name="seqType"></param>
        /// <returns></returns>
        internal static Type GetElementType(Type seqType)
        {
            // 将 IQueryable<T> 解析为 IEnumerable<T>
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            // 获取第一个泛型参数 T
            return ienum.GetGenericArguments()[0];
        }

        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            // 将数组转换为 IEnumerable<>
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    // 创建泛型的 IEnumerable<>
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    // 如果 IEnumerable<> 使用 seqType 继承的，则满足要求
                    // 返回 IEnumerable<>
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            // 遍历接口类型，递归调用，查看是否有满足 IEnumerable<> 的接口
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            // 查询基类，查看是否有满足 IEnumerable<> 的接口
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }
    }
}
