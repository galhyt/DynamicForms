using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DynamicForms
{
    public class DynamicFormsConfigAttribute : ActionFilterAttribute
    {
        public string DataFile;
        public string FormPath;

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string path = filterContext.HttpContext.Server.MapPath(HttpRuntime.AppDomainAppVirtualPath) + (@"\" + DataFile);
            string content = ActionFilterHelper.ReadFile(path);
            FormsTemplates Forms = Newtonsoft.Json.JsonConvert.DeserializeObject<FormsTemplates>(content);
            if (Forms == null) Forms = new FormsTemplates();
            Forms.Init(FormPath);

            filterContext.Controller.ViewData.Model = (object)Forms[FormPath];
            OverrideView(filterContext);

            base.OnActionExecuted(filterContext);
        }

        private void OverrideView(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Filter = new MinifyHtmlStream(filterContext);
        }

    }

    internal class ActionFilterHelper
    {
        public static void AddFilesToRunEnvironment(ActionExecutedContext filterContext)
        {
            string DynamicFormsProjectFolder = GetDynamicFormsProjectFolder(filterContext);
            if (DynamicFormsProjectFolder == null) return;

            string curDir = filterContext.HttpContext.Server.MapPath("~");
            if (!Directory.Exists(curDir + @"\Js")) Directory.CreateDirectory(curDir + @"\Js");
            if (!Directory.Exists(curDir + @"\Js\ThirdParty")) Directory.CreateDirectory(curDir + @"\Js\ThirdParty");
            foreach (string path in new string[] { @"\Js\ThirdParty\jquery-1.6.2.min.js", @"\Js\ThirdParty\handlebars-v2.0.0.js", @"\Js\DynamicForms.js" })
            {
                if (!File.Exists(curDir + path))
                    File.Copy(DynamicFormsProjectFolder + path, curDir + path);
            }
        }

        public static string ResultOverride(string result, UrlHelper Url)
        {
            Regex reg = new Regex(@"(?<=\<head\>)[.\W]*(?=\<\/head\>)", RegexOptions.Multiline);
            string toAdd = @"<script src=""" + Url.Content(@"~\Js\ThirdParty\jquery-1.6.2.min.js") + @""" type=""text/javascript""></script>" + System.Environment.NewLine +
                @"<script src=""" + Url.Content(@"~\Js\ThirdParty\handlebars-v2.0.0.js") + @""" type=""text/javascript""></script>" + System.Environment.NewLine +
                @"<script type=""text/javascript"" src=""" + Url.Content(@"~\Js\DynamicForms.js") + @"""></script>" + System.Environment.NewLine +
                @"<script type=""text/javascript"">" + System.Environment.NewLine +
                @"    $(document).ready(TemplateDynamicFormConfiguration.init);" + System.Environment.NewLine +
                @"</script>" + System.Environment.NewLine;

            if (!reg.IsMatch(result)) reg = new Regex(@"(?<=\<body.*\>)[.\W]*(?=\<\/body\>)", RegexOptions.Multiline);
            Match match = reg.Match(result);
            if (match.Success)
                result = reg.Replace(result, match.Value + toAdd);

            return result;
        }

        public static string ReadFile(string path)
        {
            System.IO.StreamReader myFile = new System.IO.StreamReader(path);
            string content = myFile.ReadToEnd();
            myFile.Close();
            return content;
        }

        private static string GetDynamicFormsProjectFolder(ActionExecutedContext filterContext)
        {
            string curDir = filterContext.HttpContext.Server.MapPath("~");
            return _GetDynamicFormsProjectFolder(curDir);
        }

        private static string _GetDynamicFormsProjectFolder(string curDir)
        {
            string[] dirs = Directory.GetDirectories(curDir, "DynamicForms");
            if (dirs.Count() > 0) return dirs[0];
            DirectoryInfo info = Directory.GetParent(curDir);
            if (info == null) return null;

            return _GetDynamicFormsProjectFolder(info.FullName);
        }

        private static string RenderViewToString(string viewName, ControllerContext context, object model)
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

    }

    internal sealed class MinifyHtmlStream : MemoryStream
    {
        private readonly MemoryStream BufferStream;
        private readonly Stream FilterStream;
        private ActionExecutedContext filterContext;

        public MinifyHtmlStream(ActionExecutedContext fc)
        {
            BufferStream = new MemoryStream();
            filterContext = fc;
            FilterStream = filterContext.HttpContext.Response.Filter;
        }

        public override void Flush()
        {
            string result = System.Text.Encoding.UTF8.GetString(BufferStream.ToArray());
            ActionFilterHelper.AddFilesToRunEnvironment(filterContext);
            result = ActionFilterHelper.ResultOverride(result, ((Controller)filterContext.Controller).Url);
            byte[] newBuffer = Encoding.UTF8.GetBytes(result);
            
            FilterStream.Write(newBuffer, 0, newBuffer.Length);
        }

        public override void Write(
            byte[] buffer,
            int offset,
            int count)
        {
            BufferStream.Write(buffer, offset, count);
        }
    }
}
