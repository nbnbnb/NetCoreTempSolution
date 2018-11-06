using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCore.TagHelpers
{

    /**
     * To create a custom Tag Helper Component:
        Create a public class deriving from TagHelperComponentTagHelper.
        Apply an [HtmlTargetElement] attribute to the class. Specify the name of the target HTML element.
        Optional: Apply an [EditorBrowsable(EditorBrowsableState.Never)] attribute to the class to suppress the type's display in IntelliSense.
     * */

    [HtmlTargetElement("address")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AddressTagHelperComponentTagHelper : TagHelperComponentTagHelper
    {
        public AddressTagHelperComponentTagHelper(ITagHelperComponentManager manager, ILoggerFactory loggerFactory)
            : base(manager, loggerFactory)
        {

        }
    }
}
