using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebAppCore.Ext
{
    /// <summary>
    /// 默认系统不支持泛型 Controller
    /// 这个 Provider 支持将泛型 Controller 添加到 ControllerFeature.Controllers 中
    /// </summary>
    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var entityType in EntityTypes.Types)
            {
                var typeName = entityType.Name + "Controller";
                if (!feature.Controllers.Any(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase)))
                {
                    // GenericController<Sprocket>
                    // GenericController<Widget>
                    var controllerType = typeof(GenericController<>).MakeGenericType(entityType.AsType()).GetTypeInfo();
                    feature.Controllers.Add(controllerType);
                }
            }
        }

        private static class EntityTypes
        {
            /// <summary>
            /// 将添加 GenericController<Sprocket> 和 GenericController<Widget> 到系统中
            /// </summary>
            public static IReadOnlyList<TypeInfo> Types => new List<TypeInfo>()
                {
                    typeof(Sprocket).GetTypeInfo(),
                    typeof(Widget).GetTypeInfo(),
                };

            public class Sprocket { }
            public class Widget { }
        }
    }
}
