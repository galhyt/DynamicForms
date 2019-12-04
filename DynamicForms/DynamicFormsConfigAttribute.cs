using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace DynamicForms
{
    public class DynamicFormsConfigAttribute : ActionFilterAttribute
    {
        public string DataFile;
        public string FormPath;
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string path = filterContext.HttpContext.Server.MapPath(HttpRuntime.AppDomainAppVirtualPath) + (@"\" + DataFile);
            string content = ReadFile(path);
            FormsTemplates Forms = Newtonsoft.Json.JsonConvert.DeserializeObject<FormsTemplates>(content);
            if (Forms == null) Forms = new FormsTemplates();
            Forms.Init(FormPath);

            AddFilesToRunEnvironment();

            string result = RenderViewToString(filterContext.RouteData.Values["Action"].ToString(), filterContext.Controller.ControllerContext, (object)Forms[FormPath]);
            result = ResultOverride(result);

            filterContext.RequestContext.HttpContext.Response.Write(result);
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        private void AddFilesToRunEnvironment()
        {
            string curDir = filterContext.HttpContext.Server.MapPath("~");
            if (!Directory.Exists(curDir + @"\Js")) Directory.CreateDirectory(curDir + @"\Js");
            if (!Directory.Exists(curDir + @"\Js\ThirdParty")) Directory.CreateDirectory(curDir + @"\Js\ThirdParty");
            foreach (string path in new string { @"\Js\ThirdParty\jquery-1.6.2.min.js", @"\Js\ThirdParty\handlebars-v2.0.0.js", @"\Js\DynamicForms.js" })
            {
                if (!File.Exists(curDir + path))
                    File.Copy(filterContext.HttpContext.Server.MapPath(@"~\..\DynamicForms" + path), curDir + path);
            }
        }

        private string ResultOverride(string result)
        {
            Regex reg = new Regex(@"(?<=\<head\>)[.\W]*(?=\<\/head\>)", RegexOptions.Multiline);
            string toAdd = @"<script src=""~\Js\ThirdParty\jquery-1.6.2.min.js"" type=""text/javascript""></script>" + System.Environment.NewLine +
                @"<script src=""~\Js\ThirdParty\handlebars-v2.0.0.js"" type=""text/javascript""></script>" + System.Environment.NewLine +
                @"<script type=""text/javascript"" src=""~\Js\DynamicForms.js""></script>" + System.Environment.NewLine +
                @"<script type=""text/javascript"">" + System.Environment.NewLine +
                @"    $(document).ready(TemplateDynamicFormConfiguration.init);" + System.Environment.NewLine +
                @"</script>" + System.Environment.NewLine;

            if (!reg.IsMatch(result)) reg = new Regex(@"(?<=\<body.*\>)[.\W]*(?=\<\/body\>)", RegexOptions.Multiline);
            Match match = reg.Match(result);
            if (match.Success)
                result = reg.Replace(result, match.Value + toAdd);

            return result;
        }

        private string RenderViewToString(string viewName, ControllerContext context, object model)
        {
            context.Controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(context, viewName, null);
                var viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        private static string ReadFile(string path)
        {
            System.IO.StreamReader myFile = new System.IO.StreamReader(path);
            string content = myFile.ReadToEnd();
            myFile.Close();
            return content;
        }
    }
}
