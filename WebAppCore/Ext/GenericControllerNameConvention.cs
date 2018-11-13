using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCore.Ext
{
    /// <summary>
    /// 如果 T 为 Widget
    /// 默认情况下，Controller Name 格式为  GenericController`1[Widget] 格式
    /// 这个 Attribute 将其设置为 Widget
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GenericControllerNameConventionAttribute : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.GetGenericTypeDefinition() != typeof(GenericController<>))
            {
                return;
            }

            var entityType = controller.ControllerType.GenericTypeArguments[0];
            controller.ControllerName = entityType.Name;
        }
    }
}
