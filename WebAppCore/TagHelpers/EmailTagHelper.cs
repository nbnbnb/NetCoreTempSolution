using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCore.TagHelpers
{
    /// <summary>
    /// 使用命名约定 EmailTagHelper = Email+TagHelper
    /// 所以，此处的标记就是 <email>
    /// 
    /// 还可以使用 HtmlTargetElement 进行显示设置
    /// </summary>
    // [HtmlTargetElement("email", TagStructure = TagStructure.WithoutEndTag)]
    public class EmailTagHelper : TagHelper
    {
        private const string EmailDomain = "contoso.com";

        // Can be passed via <email mail-to="..." />. 
        // Pascal case gets translated into lower-kebab-case.
        public string MailTo { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";  // Replaces <email> with <a> tag

            var address = MailTo + "@" + EmailDomain;
            output.Attributes.SetAttribute("href", "mailto:" + address);
            output.Content.SetContent(address);
        }

        /// <summary>
        /// 默认实现是 ProcessAsync 读取 Process 方法
        /// 所以，下面重写了 ProcessAsync 方法，里面没有读取 Process 方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";                                 // Replaces <email> with <a> tag
            var content = await output.GetChildContentAsync();
            var target = content.GetContent() + "@" + EmailDomain;
            output.Attributes.SetAttribute("href", "mailto:" + target);
            output.Content.SetContent(target);
        }
    }
}
