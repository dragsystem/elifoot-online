using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;
using HtmlAgilityPack;
namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {
        public static string AbsoluteAction(this UrlHelper url, string action, string controller, string id) {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;

            string absoluteAction = string.Format("{0}://{1}{2}",
                                                  requestUrl.Scheme,
                                                  requestUrl.Authority,
                                                  url.Action(action, controller, new { id = id }));

            return absoluteAction;
        }

        public static string AbsoluteAction(this UrlHelper url, string action, string controller) {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;

            string absoluteAction = string.Format("{0}://{1}{2}",
                                                  requestUrl.Scheme,
                                                  requestUrl.Authority,
                                                  url.Action(action, controller));

            return absoluteAction;
        }

        public static string TextReplaceLineBr(this HtmlHelper html, string text) {
            var str = text != null ? text.Replace("\n", "<br />") : "";
            return str;
        }

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, TEnum selectedValue) {
            IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>();

            IEnumerable<SelectListItem> items =
                from value in values
                select new SelectListItem {
                    Text = value.ToString(),
                    Value = value.ToString(),
                    Selected = (value.Equals(selectedValue))
                };

            return htmlHelper.DropDownList(name, items);

        }

       

        public static string RemoveAllMarkup(this string self) {
            return self.FixUpHtml(new string[0], new string[0]);
        }

        private static string FixUpHtml(this string self, string[] validElements, string[] validAttributes) {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(self ?? "");

            FixUpElement(document.DocumentNode, validElements, validAttributes);

            return document.DocumentNode.OuterHtml;
        }

        private static void FixUpElement(HtmlNode item, string[] validElements, string[] validAttributes) {
            if (item.NodeType == HtmlNodeType.Element) {
                if (validElements.Contains(item.Name)) {
                    for (int i = item.Attributes.Count - 1; i >= 0; i--) {
                        if (!validAttributes.Contains(item.Attributes[i].Name)) {
                            item.Attributes.RemoveAt(i);
                        }
                        else if (item.Attributes[i].Name == "style" && (item.Attributes[i].Value.Contains("background") || item.Attributes[i].Value.Contains("/*"))) {
                            item.Attributes.RemoveAt(i);
                        }
                        else if (item.Attributes[i].Name == "href" && item.Attributes[i].Value.Trim().StartsWith("javascript")) {
                            item.Attributes.RemoveAt(i);
                        }
                    }

                    if (item.Name == "a") {
                        item.Attributes.Append("rel", "nofollow");
                    }
                }
                else {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(item.InnerHtml);
                    HtmlNode newHtml = document.DocumentNode;
                    item.ParentNode.ReplaceChild(newHtml, item);
                    item = newHtml;
                }
            }

            foreach (var child in item.ChildNodes.ToList()) {
                FixUpElement(child, validElements, validAttributes);
            }
        }
    }
}
