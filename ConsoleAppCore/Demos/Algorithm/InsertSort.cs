using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.Algorithm
{
    internal class InsertSort
    {
        private static void Sort(int[] arr)
        {
            // 注意：i 从 1 开始，表示下一个插入的值
            for (int afterIndex = 1; afterIndex < arr.Length; afterIndex++)
            {
                // 被插入的位置（准备和前一个数比较）
                int beforeIndex = afterIndex - 1;

                // 插入的数（后一个）
                int afterVal = arr[afterIndex];

                // 如果 插入的数（后一个） 比 被插入的数（前一个） 小
                // 执行替换
                // 后一个数，继续与前一个进行匹配
                while (beforeIndex >= 0 && afterVal < arr[beforeIndex])
                {
                    // 后一个值被前一个值替换
                    // 后一个值存储在了 afterVal 中继续使用
                    arr[beforeIndex + 1] = arr[beforeIndex];

                    // 这个位置是关键（已排序的数据中，从后向前扫描）
                    // 让 index 向前移动
                    beforeIndex--;
                }

                // 如果没有交换发生
                // 那么 beforeIndex + 1 == afterIndex
                if (beforeIndex + 1 != afterIndex)
                {
                    // 把插入的数放入合适的位置

                    // 如果后一个值和前一个值进行了交换
                    // 那么 beforeIndex 的值会减小
                    // 所以将后一个值放在次位置

                    // 如果 while 没有执行，那么就没执行交换了（刚好插入的值就是数组中最大的）
                    arr[beforeIndex + 1] = afterVal;
                }

            }
        }

        public static void Run()
        {
            int[] arr = new[] { 12, 20, 5, 16, 15, 1, 30, 45 };
            Sort(arr);
            Console.WriteLine(String.Join("-", arr));
        }
    }
}
