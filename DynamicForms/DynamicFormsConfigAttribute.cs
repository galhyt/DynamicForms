using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using DFFDVal = System.Collections.Generic.Dictionary<string, string>;
using DynamicFormsFilesDictionary = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;
using Newtonsoft.Json.Linq;

namespace DynamicForms
{
    public class DynamicFormsConfigAttribute : ActionFilterAttribute
    {
        public string ActionType;
        public string DataFile;
        public string FormsKey;
        public string FormPath;
        public string PartialViewHtmlSection;
        public string ModelMember;

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            switch (ActionType)
            {
                case "Load":
                    LoadType(filterContext);
                    break;

                case "Save":
                    SaveType(filterContext);
                    break;
            }
            base.OnActionExecuted(filterContext);
        }

        private void LoadType(ActionExecutedContext filterContext)
        {
            var Forms = LoadFormsFromDataSrc(filterContext);
            if (Forms[FormPath] == null) Forms.Init(FormPath);

            ActionFilterHelper.SetModelMember(filterContext.Controller.ViewData.Model, ModelMember, (object)Forms[FormPath]);

            OverrideView(filterContext);
        }

        private void SaveType(ActionExecutedContext filterContext)
        {
            var Forms = LoadFormsFromDataSrc(filterContext);
            Forms[FormPath] = (TemplateFormData)ActionFilterHelper.GetModelMember(filterContext.Controller.ViewData.Model, ModelMember);
            SaveFormToDataSrc(filterContext, Forms);

            OverrideView(filterContext);
        }

        private FormsTemplates LoadFormsFromDataSrc(ActionExecutedContext filterContext)
        {
            string path = filterContext.HttpContext.Server.MapPath(HttpRuntime.AppDomainAppVirtualPath) + (@"\" + DataFile);
            JObject jsonObj = GetJsonObject(path);
            FormsTemplates Forms = Newtonsoft.Json.JsonConvert.DeserializeObject<FormsTemplates>(Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj.SelectToken(FormsKey)));
            if (Forms == null) Forms = new FormsTemplates();

            return Forms;
        }

        private void SaveFormToDataSrc(ActionExecutedContext filterContext, FormsTemplates Forms)
        {
            string path = filterContext.HttpContext.Server.MapPath(HttpRuntime.AppDomainAppVirtualPath) + (@"\" + DataFile);
            JObject jsonObj = GetJsonObject(path);
            JToken formsToken = jsonObj.SelectToken(FormsKey);
            formsToken.Replace(Newtonsoft.Json.JsonConvert.DeserializeObject<JToken>(Newtonsoft.Json.JsonConvert.SerializeObject(Forms)));
            string content = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj);
            ActionFilterHelper.SaveFile(path, content);
        }

        private JObject GetJsonObject(string path)
        {
            string content = ActionFilterHelper.ReadFile(path);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(content);
        }

        private void OverrideView(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Filter = new MinifyHtmlStream(filterContext, @"_TemplateDynamicFormConfiguration", PartialViewHtmlSection, ModelMember);
        }

    }

    internal class ActionFilterHelper
    {
        private static DynamicFormsFilesDictionary FilesDic = new DynamicFormsFilesDictionary
        {
            {"Js", new DFFDVal{{@"\Js\ThirdParty\jquery.min.js",@"\DynamicFormsExt\Js\jquery.min.js"},
                    {@"\Js\ThirdParty\handlebars-v2.0.0.js",@"\DynamicFormsExt\Js\handlebars-v2.0.0.js"},
                    {@"\Js\DynamicForms.js",@"\DynamicFormsExt\Js\DynamicForms.js"}}},
            {"Css", new DFFDVal{{@"\Css\ThirdParty\jquery.fancybox-1.3.4.css", @"\DynamicFormsExt\Css\jquery.fancybox-1.3.4.css"},
                    {@"\Css\dialog.css", @"\DynamicFormsExt\Css\dialog.css"},
                    {@"\Css\DynamicForms.css", @"\DynamicFormsExt\Css\DynamicForms.css"}}}
            //{"Partial", new DFFDVal{{@"\PartialViews\_TemplateDynamicFormConfiguration.cshtml", @"\DynamicFormsExt\PartialViews\_TemplateDynamicFormConfiguration.cshtml"}}}
        };

        public static void AddFilesToRunEnvironment(ActionExecutedContext filterContext)
        {
            string DynamicFormsProjectFolder = GetDynamicFormsProjectFolder(filterContext);
            if (DynamicFormsProjectFolder == null) return;
            string curDir = filterContext.HttpContext.Server.MapPath("~");

            foreach (string fileType in FilesDic.Keys)
            {
                DFFDVal typeDic = FilesDic[fileType];
                foreach (string srcPath in typeDic.Keys)
                {
                    string trgPath = typeDic[srcPath];
                    CreateDirsIfNotExist(trgPath, filterContext);

                    if (File.Exists(curDir + trgPath))
                    {
                        DateTime DynamicFormsProjectFileDate = File.GetLastWriteTime(DynamicFormsProjectFolder + srcPath);
                        DateTime TargetFileDate = File.GetLastWriteTime(curDir + trgPath);
                        if (DynamicFormsProjectFileDate <= TargetFileDate) continue;
                    }

                    File.Copy(DynamicFormsProjectFolder + srcPath, curDir + trgPath, true);
                }
            }

            string controllerName = filterContext.RouteData.Values["Controller"].ToString();
            try
            {
                File.Copy(DynamicFormsProjectFolder + @"\PartialViews\_TemplateDynamicFormConfiguration.cshtml", curDir + @"Views\" + controllerName +@"\_TemplateDynamicFormConfiguration.cshtml", true);
            }
            catch (Exception e)
            {
            }
        }

        public static object GetModelMember(object model, string ModelMember)
        {
            return model.GetType().GetProperty(ModelMember).GetValue(model, null);
        }

        public static void SetModelMember(object model, string ModelMember, object FormData)
        {
            model.GetType().GetProperty(ModelMember).SetValue(model, FormData, null);
        }

        public static string ResultOverride(string result, UrlHelper Url, ControllerContext context, string partialViewName, string HtmlFieldPrefix, string PartialViewHtmlSection, string ModelMember)
        {
            string toAdd = "";
            foreach (string fileType in FilesDic.Keys)
            {
                foreach (string trgPath in FilesDic[fileType].Values)
                {
                    switch (fileType)
                    {
                        case "Js":
                            toAdd += @"<script src=""" + Url.Content(@"~\" + trgPath) + @""" type=""text/javascript""></script>" + System.Environment.NewLine;
                            break;

                        case "Css":
                            toAdd += string.Format(@"<link href=""{0}"" rel=""stylesheet"" type=""text/css"" />", Url.Content(@"~\" + trgPath));
                            break;
                    }
                }
            }

            toAdd += @"<script type=""text/javascript"">" + System.Environment.NewLine +
                    @"    $(document).ready(TemplateDynamicFormConfiguration.init);" + System.Environment.NewLine +
                    @"</script>" + System.Environment.NewLine;

            Regex reg = new Regex(@"(?<=\<head\>)[.\W]*(?=\<\/head\>)", RegexOptions.Multiline);
            if (!reg.IsMatch(result)) reg = new Regex(@"(?<=\<body.*\>)[.\W]*(?=\<\/body\>)", RegexOptions.Multiline);
            Match match = reg.Match(result);
            if (match.Success)
                result = reg.Replace(result, match.Value + toAdd);

            object PartialMember = GetModelMember(context.Controller.ViewData.Model, ModelMember);
            string partialViewContent = RenderPartialViewToString(partialViewName, context, PartialMember, HtmlFieldPrefix);

            // Inject partial view
            reg = new Regex(string.Format(@"(?<=<(\w+)[\w\W]+class=""{0}""[\w\W]*>)[.\W]*?(?=<)", PartialViewHtmlSection.Replace("-", @"\-")));
            match = reg.Match(result);
            if (match.Success)
                result = reg.Replace(result, match.Value + partialViewContent, 1);

            return result;
        }

        public static string ReadFile(string path)
        {
            System.IO.StreamReader myFile = new System.IO.StreamReader(path);
            string content = myFile.ReadToEnd();
            myFile.Close();
            return content;
        }

        public static void SaveFile(string path, string content)
        {
            System.IO.StreamWriter myFile = new System.IO.StreamWriter(path, false);
            myFile.Write(content);
            myFile.Close();
        }

        public static void SearchLocationsAdd()
        {
            var razorEngine = ViewEngines.Engines.OfType<RazorViewEngine>().FirstOrDefault();

            razorEngine.PartialViewLocationFormats = razorEngine.PartialViewLocationFormats.Concat(new string[] { 
                "~/DynamicFormsExt/PartialViews/_TemplateDynamicFormConfiguration.cshtml"
            }).ToArray();
        }
        private static void CreateDirsIfNotExist(string path, ActionExecutedContext filterContext)
        {
            Regex reg = new Regex(@"(?<=[\\\/]*).+(?=[\\\/])", RegexOptions.Multiline);
            MatchCollection dirs = reg.Matches(path);
            if (dirs.Count == 0) return;
            string curDir = filterContext.HttpContext.Server.MapPath("~");

            string folderName = "";
            for (int i = 0; i < dirs.Count; i++)
            {
                folderName += @"\" + dirs[i];
                if (!Directory.Exists(curDir + @"\" + folderName))
                    Directory.CreateDirectory(curDir + @"\"+ folderName);
            }
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

        private static string RenderPartialViewToString(string partialViewName, ControllerContext context, object model, string HtmlFieldPrefix)
        {
            ViewDataDictionary orgViewData = context.Controller.ViewData;

            context.Controller.ViewData = new ViewDataDictionary()
            {
                TemplateInfo = new TemplateInfo()
                {
                    HtmlFieldPrefix = HtmlFieldPrefix
                },
                Model = model
            };

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, partialViewName);
                var viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);

                context.Controller.ViewData = orgViewData;

                return sw.GetStringBuilder().ToString();
            }
        }

    }

    internal sealed class MinifyHtmlStream : MemoryStream
    {
        private readonly MemoryStream BufferStream;
        private readonly Stream FilterStream;
        private ActionExecutedContext filterContext;
        private string partialViewName;
        private string PartialViewHtmlSection;
        private string ModelMemebr;

        public MinifyHtmlStream(ActionExecutedContext fc, string _partialViewName, string _PartialViewHtmlSection, string _ModelMemebr)
        {
            BufferStream = new MemoryStream();
            filterContext = fc;
            FilterStream = filterContext.HttpContext.Response.Filter;
            partialViewName = _partialViewName;
            PartialViewHtmlSection = _PartialViewHtmlSection;
            ModelMemebr = _ModelMemebr;
        }

        public override void Flush()
        {
            string result = System.Text.Encoding.UTF8.GetString(BufferStream.ToArray());
            ActionFilterHelper.AddFilesToRunEnvironment(filterContext);
            result = ActionFilterHelper.ResultOverride(result, ((Controller)filterContext.Controller).Url, filterContext.Controller.ControllerContext, partialViewName, ModelMemebr, PartialViewHtmlSection, ModelMemebr);
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
