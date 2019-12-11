using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace DynamicForms
{
    public class DynamicFormsShowAttribute : ActionFilterAttribute
    {
        public string ActionType;
        public string DataFile;
        public string FormDataPath;
        public string NewFormPath;
        public string PartialViewHtmlSection;

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (NewFormPath != null) NewFormPath = NewFormPath.Replace("/", ".");
            if (FormDataPath != null) FormDataPath = FormDataPath.Replace("/", ".");
            switch (ActionType)
            {
                case "Load":
                    LoadForm(filterContext);
                    break;

                case "Save":
                    SaveForm(filterContext);
                    break;
            }

            base.OnActionExecuted(filterContext);
        }

        private void LoadForm(ActionExecutedContext filterContext)
        {
            JObject FormModel = ActionFilterHelper.LoadFormDataFromDataSrc(filterContext, DataFile, FormDataPath);
            
            if (FormModel != null)
            {
                ((TemplateDynamicFormModel)filterContext.Controller.ViewData.Model).CastFromJObject(FormModel);
            }

            OverrideView(filterContext);
        }

        private void SaveForm(ActionExecutedContext filterContext)
        {
            object FormModel = filterContext.Controller.ViewData.Model;
            ActionFilterHelper.SaveFormToDataSrc(filterContext, FormModel, DataFile, FormDataPath);
            OverrideView(filterContext);
        }

        private void OverrideView(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Filter = new MinifyHtmlStream(filterContext, @"_TemplateDynamicContent", PartialViewHtmlSection, null);
        }
    }
}
