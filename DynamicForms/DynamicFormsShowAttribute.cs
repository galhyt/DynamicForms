using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace DynamicForms
{
    public interface IDynamicFormsShowDataSrc
    {
        object Find(string DataConnection, string DataEntity, string DataCreteria);
        bool Save(string DataConnection, string DataEntity, string DataCreteria, object value);
    }

    public class JsonShowDataSrc : IDynamicFormsShowDataSrc
    {
        public object Find(string DataConnection, string DataEntity, string DataCreteria)
        {
            JObject FormModel = ActionFilterHelper.LoadFormDataFromDataSrc(DataConnection, DataEntity);

            if (FormModel == null)
            {
                TemplateFormData templateForm = ActionFilterHelper.LoadTemplateFormFromDataSrc(DataConnection, DataCreteria);
                if (templateForm != null)
                    FormModel = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(Newtonsoft.Json.JsonConvert.SerializeObject(new TemplateDynamicFormModel(templateForm)));
            }

            return FormModel;
        }
        public bool Save(string DataConnection, string DataEntity, string DataCreteria, object value)
        {
            SaveFormToDataSrc(DataConnection, value, DataEntity);
            return true;
        }

        private void SaveFormToDataSrc(string path, object Forms, string FormsKey)
        {
            JObject jsonObj = ActionFilterHelper.GetJsonObject(path);
            string FormDataParentPath = FormsKey;
            JToken parentToken = jsonObj.SelectToken(FormDataParentPath);
            JToken lastChild = parentToken.Children().LastOrDefault();
            JToken formsToken = Newtonsoft.Json.JsonConvert.DeserializeObject<JToken>(Newtonsoft.Json.JsonConvert.SerializeObject(Forms));

            if (lastChild != null)
                lastChild.AddAfterSelf(formsToken);
            else
                parentToken.Children().ToList().Add(formsToken);

            string content = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj);
            ActionFilterHelper.SaveFile(path, content);
        }


    }
    public class DynamicFormsShowAttribute : ActionFilterAttribute
    {
        public string ActionType;
        public string LibraryPath;
        public string DataConnection;
        public string DataEntity;
        public string DataCreteria;
        public string PartialViewHtmlSection;


        private static IDynamicFormsShowDataSrc _DataSrc;
        private IDynamicFormsShowDataSrc DataSrc
        {
            get
            {
                if (_DataSrc == null)
                {
                    _DataSrc = Datafactory.GetDataShow(LibraryPath);
                }
                return _DataSrc;
            }

            set { }
        }

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
            object FormModel = DataSrc.Find(DataConnection, model.FormDataPath, model.DataCreteria);
            
            //model.CastFromJObject(FormModel);
            OverrideView(filterContext);
        }

        private void SaveForm(ActionExecutedContext filterContext)
        {
            TemplateDynamicFormModel FormModel = (TemplateDynamicFormModel)filterContext.Controller.ViewData.Model;
            DataSrc.Save(DataConnection, FormModel.FormDataParentPath, FormModel.FormDataPath, FormModel);
            OverrideView(filterContext);
        }

        private void OverrideView(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Filter = new MinifyHtmlStream(filterContext, @"_TemplateDynamicContent", PartialViewHtmlSection, null);
        }
    }
}
