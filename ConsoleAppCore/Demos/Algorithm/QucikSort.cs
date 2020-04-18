using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.Algorithm
{
    internal class QucikSort
    {
        private static void Sort(int[] arr, int low, int high)
        {
            int start = low;
            int end = high;

            // 这是一个基准值
            // 一般取第一个
            int key = arr[low];

            // 继续排序条件
            while (end > start)
            {
                // 从 后往前 比较
                while (end > start && arr[end] >= key)
                {
                    // 如果没有比关键值小的
                    // 比较下一个
                    end--;
                }

                // 直到有比关键值小的交换位置
                if (arr[end] <= key)
                {
                    int tmp = arr[end];
                    arr[end] = arr[start];
                    arr[start] = tmp;

                }

                // 然后
                // 又 从前往后 比较
                while (end > start && arr[start] <= key)
                {
                    // 如果没有比关键值大的
                    // 比较下一个
                    start++;
                }

                // 直到有比关键值大的交换位置
                if (arr[start] >= key)
                {
                    int tmp = arr[start];
                    arr[start] = arr[end];
                    arr[end] = tmp;
                }
            }

            // 此时第一次循环比较结束了
            // 关键值的位置已经确定

            // 左边的值都比关键值小
            // 右边的值都比关键值大 

            Console.WriteLine("---");

            // 但是两边的顺序还有可能不一样，进行下面的递归调用
            if (start > low)
            {
                // 基准默认第一个中间值
                Sort(arr, low, start - 1);
            }
            if (end < high)
            {
                // 基准默认第一个中间值
                Sort(arr, end + 1, high);
            }
        }

        public static void Run()
        {
            int[] arr = new[] { 12, 20, 5, 16, 15, 1, 30, 45 };
            Sort(arr, 0, arr.Length - 1);
            Console.WriteLine(String.Join("-", arr));
        }
    }
}
