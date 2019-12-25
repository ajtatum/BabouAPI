using BabouExtensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Babou.API.Web.Helpers.TagHelpers
{
    [HtmlTargetElement("script", Attributes = "asp-add-nonce")]
    [HtmlTargetElement("style", Attributes = "asp-add-nonce")]
    [HtmlTargetElement("frame", Attributes = "asp-add-nonce")]
    [HtmlTargetElement("iframe", Attributes = "asp-add-nonce")]
    [HtmlTargetElement("img", Attributes = "asp-add-nonce")]
    [HtmlTargetElement("audio", Attributes = "asp-add-nonce")]
    [HtmlTargetElement("video", Attributes = "asp-add-nonce")]
    [HtmlTargetElement("object", Attributes = "asp-add-nonce")]
    [HtmlTargetElement("applet", Attributes = "asp-add-nonce")]
    [HtmlTargetElement("embed", Attributes = "asp-add-nonce")]
    [HtmlTargetElement("base", Attributes = "asp-add-nonce")]
    [HtmlTargetElement("link", Attributes = "asp-add-nonce")]
    public class NonceTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-add-nonce")]
        public bool AddNonce { get; set; }

        [ViewContext]
        public ViewContext? ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (AddNonce)
            {
                if (ViewContext != null)
                {
                    var nonce = ViewContext.HttpContext.Items["csp-nonce"].ToString();

                    if (!nonce.IsNullOrEmpty())
                    {
                        output.Attributes.Add(new TagHelperAttribute("nonce", nonce));
                    }
                }
            }
        }

        //public NonceTagHelper(string name) : base(name)
        //{
        //    name = "nonce";
        //}

        //public NonceTagHelper(string name, object value) : base(name, value)
        //{
        //    name = "nonce";
        //    value = ViewContext.HttpContext.Items["csp-nonce"];
        //}

        //public NonceTagHelper(string name, object value, HtmlAttributeValueStyle valueStyle) : base(name, value, valueStyle)
        //{
        //    name = "nonce";
        //    value = ViewContext.HttpContext.Items["csp-nonce"];
        //    valueStyle = HtmlAttributeValueStyle.DoubleQuotes;
        //}
    }
}
