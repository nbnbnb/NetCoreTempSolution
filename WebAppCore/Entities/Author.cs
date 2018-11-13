using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppCore.Ext;

namespace WebAppCore.Entities
{
    /// <summary>
    /// ModelBinder 指定了，当 Author 类型作为 Model Binding 的目标类型时
    /// 使用 AuthorEntityBinder 用来设置绑定值
    /// </summary>
    [ModelBinder(BinderType = typeof(AuthorEntityBinder))]
    public class Author
    {
        public string Name { get; set; }

        public int Id { get; set; }
    }
}
