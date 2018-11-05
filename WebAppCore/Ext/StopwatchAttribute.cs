using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCore.Ext
{
    /// <summary>
    /// ActionFilterAttribute 继承了
    /// Attribute, IActionFilter, IFilterMetadata, IAsyncActionFilter, IResultFilter, IAsyncResultFilter, IOrderedFilter
    /// 可以放在 Action 或 Controller 上，也可以添加到 GlobalFilterCollection 中
    /// </summary>
    public class StopwatchAttribute : ActionFilterAttribute
    {
        private const string _stopwatchKey = "StopwatchFilter.Value";

        #region IActionFilter & IAsyncActionFilter
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items[_stopwatchKey] = Stopwatch.StartNew();
            Debug.WriteLine("002 - OnActionExecuting");
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var stopwatch = (Stopwatch)context.HttpContext.Items[_stopwatchKey];
            stopwatch.Stop();
            var log = string.Format("controller:{0},action:{1},execution time:{2}ms",
                context.Controller.GetType().Name,
                context.ActionDescriptor.DisplayName,
                stopwatch.ElapsedMilliseconds);
            Debug.WriteLine(log);
            Debug.WriteLine("003 - OnActionExecuted");
            base.OnActionExecuted(context);
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Debug.WriteLine("001 - OnActionExecutionAsync");
            return base.OnActionExecutionAsync(context, next);
        }
        #endregion


        #region IResultFilter & IAsyncResultFilter

        public override Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            Debug.WriteLine("001 - OnResultExecutionAsync");
            return base.OnResultExecutionAsync(context, next);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            Debug.WriteLine("002 - OnResultExecuting");
            base.OnResultExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            Debug.WriteLine("003 - OnResultExecuted");
            base.OnResultExecuted(context);
        }

        #endregion
    }
}
