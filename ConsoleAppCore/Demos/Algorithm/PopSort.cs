using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.Algorithm
{
    internal class PopSort
    {
        private static void Sort(int[] arr)
        {
            // 此处只用冒泡 n-1 次就可以了
            int n = arr.Length - 1;

            for (int i = 0; i < n; i++)
            {
                // 执行完一次之后
                // 最大的数据就沉到了最后一个位置

                // 所以，每次只用排序集合中（前） n-j 的数据（后面的数据都是已经排序好的）
                for (int j = 1; j < n - i; j++)
                {
                    // 大的数字，沉到底部
                    if (arr[j - 1] > arr[j])
                    {
                        int tmp = arr[j - 1];
                        arr[j - 1] = arr[j];
                        arr[j] = tmp;
                    }
                }

                // 可以看到每一次的迭代
                Console.WriteLine(String.Join("-", arr));
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
