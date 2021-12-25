using Amlakbashi.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.TagHelpers
{
    //[HtmlTargetElement("paging")]
    [HtmlTargetElement("div", Attributes = "paging-info")]
    public class PagingTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory urlHelperFactory;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            this.urlHelperFactory = urlHelperFactory;
        }

        public PagingDTO PagingInfo { get; set; }
        public string Action { get; set; }
        public dynamic Parameters { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            var routeValues = new RouteValueDictionary(Parameters);
            routeValues.Add("page", 1);

            TagBuilder ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            TagBuilder li = new TagBuilder("li");
            TagBuilder a = new TagBuilder("a");
            TagBuilder span = new TagBuilder("span");
            a.Attributes["href"] = urlHelper.Action(Action, routeValues);
            a.InnerHtml.Append("ابتدا");
            li.InnerHtml.AppendHtml(a);
            ul.InnerHtml.AppendHtml(li);

            TagBuilder dotLi = new TagBuilder("li");
            dotLi.InnerHtml.AppendHtml("&nbsp;&nbsp;...&nbsp;&nbsp;");
            ul.InnerHtml.AppendHtml(dotLi);

            for (int i = PagingInfo.CurrentPage - 2; i < PagingInfo.CurrentPage + 3; i++)
            {
                if (i < 1 || i > PagingInfo.TotalPages)
                {
                    continue;
                }
                li = new TagBuilder("li");
                
                if (i == PagingInfo.CurrentPage)
                {
                    span.InnerHtml.Append(i.ToString());
                    li.AddCssClass("active");
                    li.InnerHtml.AppendHtml(span);
                }
                else
                {
                    a = new TagBuilder("a");
                    routeValues["page"] = i;
                    a.Attributes["href"] = urlHelper.Action(Action, routeValues);
                    a.InnerHtml.Append(i.ToString());
                    li.InnerHtml.AppendHtml(a);
                }
                ul.InnerHtml.AppendHtml(li);
            }

            ul.InnerHtml.AppendHtml(dotLi);
            li = new TagBuilder("li");
            a = new TagBuilder("a");

            routeValues["page"] = PagingInfo.TotalPages;
            a.Attributes["href"] = urlHelper.Action(Action, routeValues);

            a.InnerHtml.Append("انتها");
            li.InnerHtml.AppendHtml(a);
            ul.InnerHtml.AppendHtml(li);

            output.Attributes.Add("class", "pagination-container");
            output.Content.AppendHtml(ul);
        }
    }
}
