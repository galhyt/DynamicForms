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
        public string PartialViewHtmlSection;

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
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
            TemplateDynamicFormModel model = (TemplateDynamicFormModel)filterContext.Controller.ViewData.Model;
            JObject FormModel = ActionFilterHelper.LoadFormDataFromDataSrc(filterContext, DataFile, model.FormDataPath);
            
            if (FormModel == null)
            {
                TemplateFormData templateForm = ActionFilterHelper.LoadTemplateFormFromDataSrc(filterContext, DataFile, model.DataCreteria);
                if (templateForm != null)
                    FormModel =  Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(Newtonsoft.Json.JsonConvert.SerializeObject(new TemplateDynamicFormModel(templateForm)));
            }

            model.CastFromJObject(FormModel);
            OverrideView(filterContext);
        }

        private void SaveForm(ActionExecutedContext filterContext)
        {
            TemplateDynamicFormModel FormModel = (TemplateDynamicFormModel)filterContext.Controller.ViewData.Model;
            ActionFilterHelper.SaveFormToDataSrc(filterContext, FormModel, DataFile, FormModel.FormDataPath);
            OverrideView(filterContext);
        }

        private void OverrideView(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Filter = new MinifyHtmlStream(filterContext, @"_TemplateDynamicContent", PartialViewHtmlSection, null);
        }
    }
}
