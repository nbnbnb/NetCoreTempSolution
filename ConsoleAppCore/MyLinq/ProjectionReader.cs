using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Collections;

namespace ConsoleAppCore.MyLinq
{
    /// <summary>
    /// 通过 DbDataReader
    /// 返回可枚举的投射对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ProjectionReader<T> : IEnumerable<T>, IEnumerable
    {
        Enumerator enumerator;
        internal ProjectionReader(DbDataReader reader, Func<ProjectionRow, T> projector)
        {
            this.enumerator = new Enumerator(reader, projector);
        }

        public IEnumerator<T> GetEnumerator()
        {
            Enumerator e = this.enumerator;
            if (e == null)
            {
                throw new InvalidOperationException("Cannot enumerate more than once");
            }
            this.enumerator = null;
            return e;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 可枚举的对象
        /// 继承了 ProjectionRow
        /// 因为 projector 委托需要这样的一个参数
        /// </summary>
        class Enumerator : ProjectionRow, IEnumerator<T>, IEnumerator, IDisposable
        {
            DbDataReader reader;
            T current;
            Func<ProjectionRow, T> projector;
            internal Enumerator(DbDataReader reader, Func<ProjectionRow, T> projector)
            {
                this.reader = reader;
                this.projector = projector;
            }

            // 这个方法是继承至 ProjectionRow
            // ColumnProjector 的 VisitMember 方法，将会在内部执行 MethodCallExpression
            // 最终执行这个方法的调用
            public override object GetValue(int index)
            {
                if (index >= 0)
                {
                    if (this.reader.IsDBNull(index))
                    {
                        return null;
                    }
                    else
                    {
                        return this.reader.GetValue(index);
                    }
                }
                throw new IndexOutOfRangeException();
            }

            public T Current
            {
                get { return this.current; }
            }

            object IEnumerator.Current
            {
                get { return this.current; }
            }

            public bool MoveNext()
            {
                if (this.reader.Read())
                {
                    // 执行委托，传递 ProjectionRow（自身）对象
                    // 此时将会执行匿名对象的赋值操作
                    // 赋值操作内部将会调用自身的 GetValue(int index) 方法
                    this.current = this.projector(this);

                    return true;
                }
                return false;
            }

            public void Reset()
            {
            }

            public void Dispose()
            {
                this.reader.Dispose();
            }
        }
    }
}
