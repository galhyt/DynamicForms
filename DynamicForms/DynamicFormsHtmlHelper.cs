using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace DynamicForms
{
    public static class DynamicFormsHtmlHelper
    {
        public static MvcHtmlString GetFieldByType(this HtmlHelper _htmlHelper, TemplateDynamicFormModelWrapper.FieldData field, int rIndx, int cIndx, int fIndx, int mIndx)
        {
            HtmlHelper<TemplateDynamicFormModel> htmlHelper = new HtmlHelper<TemplateDynamicFormModel>(_htmlHelper.ViewContext, _htmlHelper.ViewDataContainer);
            StringBuilder html = new StringBuilder();
            switch (field["HtmlType"].ToString())
            {
                case "checkbox":
                    html.AppendFormat("<input type=\"checkbox\" id=\"{0}\" name=\"{1}\" value=\"{2}\" {3} />",
                        htmlHelper.IdFor(m => m.FormData[mIndx].Value),
                        htmlHelper.NameFor(m => m.FormData[mIndx].Value),
                        (bool?)field["Value"],
                        (field["Value"] != null && (bool)field["Value"] ? "checked=\"checked\"" : ""));
                    break;
                case "textarea":
                    html.AppendFormat("<textarea id=\"{0}\" name=\"{1}\">{2}</textarea>",
                        htmlHelper.IdFor(m => m.FormData[mIndx].Value),
                        htmlHelper.NameFor(m => m.FormData[mIndx].Value),
                        (field["Value"] != null ? field["Value"].ToString() : ""));
                    break;
                default:
                    html.AppendFormat("<input type=\"{0}\" id=\"{1}\" name=\"{2}\" value=\"{3}\" />",
                        field["HtmlType"].ToString(),
                        htmlHelper.IdFor(m => m.FormData[mIndx].Value),
                        htmlHelper.NameFor(m => m.FormData[mIndx].Value),
                        (field["Value"] != null ? field["Value"].ToString() : ""));
                    break;
            }

            html.AppendFormat("<input type=\"hidden\" id=\"{0}\" name=\"{1}\" value=\"{2}\" />",
                htmlHelper.IdFor(m => m.FormData[mIndx].Name),
                htmlHelper.NameFor(m => m.FormData[mIndx].Name),
                field["FieldName"].ToString());
            html.Append(htmlHelper.HiddenFor(m => m.FieldsMetaData[mIndx].FieldName));
            html.Append(htmlHelper.HiddenFor(m => m.FieldsMetaData[mIndx].FieldTitle));
            html.Append(htmlHelper.HiddenFor(m => m.FieldsMetaData[mIndx].FieldType));
            html.Append(htmlHelper.HiddenFor(m => m.UITable[rIndx][cIndx][fIndx].FieldName));
            html.Append(htmlHelper.HiddenFor(m => m.UITable[rIndx][cIndx][fIndx].HtmlType));

            return new MvcHtmlString(html.ToString());
        }

    }
}
