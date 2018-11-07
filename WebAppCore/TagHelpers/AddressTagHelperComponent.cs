using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCore.TagHelpers
{
    /// <summary>
    /// 继承了 TagHelperComponent 后，处理标签时，将会执行 ProcessAsync 方法
    /// 系统默认注册了 head 和 body 标签
    /// 自定义 AddressTagHelperComponentTagHelper，将会注册 address 标签
    /// 
    /// 需要添加 services.AddTransient<ITagHelperComponent, AddressTagHelperComponent>(); 
    /// 以支持标签处理
    /// 
    /// 与单独继承 TagHelper 不同
    /// 这个标签可以有条件的对标签内容进行处理
    /// </summary>
    public class AddressTagHelperComponent : TagHelperComponent
    {
        private readonly string _printableButton =
            "<button type='button' class='btn btn-info' onclick=\"window.open(" +
            "'https://binged.it/2AXRRYw')\">" +
            "<span class='glyphicon glyphicon-road' aria-hidden='true'></span>" +
            "</button>";

        public override int Order => 3;
        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.Equals(context.TagName, "address", StringComparison.OrdinalIgnoreCase)
                && output.Attributes.ContainsName("printable"))
            {
                var content = await output.GetChildContentAsync();
                output.Content.SetHtmlContent($"<div>{content.GetContent()}</div>{_printableButton}");
            }
        }
    }
}
