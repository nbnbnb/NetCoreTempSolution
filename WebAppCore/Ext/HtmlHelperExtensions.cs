using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace WebAppCore.Ext
{
    public static class HtmlHelperExtensions
    {
        public static ListGroupItem ListGroup(this IHtmlHelper htmlHelper)
        {
            return new ListGroupItem();
        }

        public class ListGroupItem
        {
            public IHtmlContent Info<T>(List<T> data, Func<T, string> getName)
            {
                return Show(data, getName, "list-group-item-info");
            }

            public IHtmlContent Warning<T>(List<T> data, Func<T, string> getName)
            {
                return Show(data, getName, "list-group-item-warning");
            }

            public IHtmlContent Danger<T>(List<T> data, Func<T, string> getName)
            {
                return Show(data, getName, "list-group-item-danger");
            }

            private IHtmlContent Show<T>(List<T> data, Func<T, string> getName, string style)
            {
                var ulBuilder = new TagBuilder("ul");
                ulBuilder.AddCssClass("list-group");
                foreach (T item in data)
                {
                    var liBuilder = new TagBuilder("li");
                    liBuilder.AddCssClass("list-group-item");
                    liBuilder.AddCssClass(style);
                    liBuilder.InnerHtml.Append(getName(item));
                    ulBuilder.InnerHtml.AppendHtml(liBuilder.ToString());
                }
                return new HtmlString(ulBuilder.ToString());
            }
        }
    }
}
