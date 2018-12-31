using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.Coroutine
{
    /// <summary>
    /// 协程单元存储格式
    /// </summary>
    class UnitItem
    {
        /// <summary>
        /// 协程单元
        /// </summary>
        public ICoroutineUnit Unit { get; set; }

        /// <summary>
        /// 协程单元使用的迭代器
        /// </summary>
        public IEnumerator<Task> UnitResult { get; set; }
    }
}
