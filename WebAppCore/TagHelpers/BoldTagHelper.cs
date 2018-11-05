using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCore.TagHelpers
{
    [HtmlTargetElement("bold")] // 元素为 <bold>，例如 <bold>aa</bold>
    [HtmlTargetElement(Attributes = "bold")] // 元素包含 bold 标记，例如 <p bold>
    // [HtmlTargetElement("bold", Attributes = "bold")] 这是一个 AND，<bold bold>aaa</bold>
    public class BoldTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("bold");
            output.PreContent.SetHtmlContent("<strong>");
            output.PostContent.SetHtmlContent("</strong>");
        }
    }
}
