using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppCore.Entities;

namespace WebAppCore.Ext
{
    /// <summary>
    /// 1，从 ViewComponent 继承
    /// 2，
    /// 3，类名义以 ViewComponent 结尾（PriorityList 将是 Name）
    /// </summary>
    // [ViewComponent(Name = "PriorityList")] // 可以显式指定 Name
    public class PriorityListViewComponent : ViewComponent
    {
        // 此处可以使用 DI

        private static IList<TodoItem> items = new List<TodoItem>
        {
            new TodoItem{ IsDone=true, Priority=1,Name="001" },
            new TodoItem{ IsDone=false, Priority=2,Name="002" },
            new TodoItem{ IsDone=true, Priority=3,Name="003" },
            new TodoItem{ IsDone=true, Priority=4,Name="004" },
            new TodoItem{ IsDone=true, Priority=5,Name="005" }
        };

        /// <summary>
        /// InvokeAsync 将会从 CSHTML 中调用
        /// 此处可以传递任意的参数
        /// </summary>
        /// <param name="maxPriority"></param>
        /// <param name="isDone"></param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(int maxPriority, bool isDone)
        {
            string viewName = "Default";
            // If asking for all completed tasks, render with the "PVC" view.
            if (maxPriority > 3 && isDone == true)
            {
                viewName = "PVC";
            }
            var items = await GetItemsAsync(maxPriority, isDone);
            // 这个 View 方法是 ViewComponent 中的
            return View(viewName, items);  // 默认是 Default，可以指定不同的 ViewName
        }

        private async Task<List<TodoItem>> GetItemsAsync(int maxPriority, bool isDone)
        {
            await Task.Delay(0);
            return items.Where(x => x.IsDone == isDone && x.Priority <= maxPriority).ToList();
        }

        /*
        /// <summary>
        /// 没有异步方法时，使用同步方法
        /// </summary>
        /// <param name="maxPriority"></param>
        /// <param name="isDone"></param>
        /// <returns></returns>
        public IViewComponentResult Invoke(int maxPriority, bool isDone)
        {
            return View(items);  // 默认是 Default，可以指定不同的 ViewName
        }
        */
    }
}
