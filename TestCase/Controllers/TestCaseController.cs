using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using TestCase.Models;
using DynamicForms;
using Newtonsoft.Json.Linq;

namespace TestCase.Controllers
{
    public class TestCaseController : Controller
    {
        [DynamicFormsConfig(ActionType = "Load", DataConnection = "Data/DataSrc.json", DataEntity = "Forms.FormsTemplates", PartialViewHtmlSection = @"DynamicFormContainer", ModelMember = "Form")]
        public ActionResult Configuration(string FormPath = "TestCase")
        {
            TestCaseConfigModel model = new TestCaseConfigModel();
            model.DataCreteria = FormPath;
            string[] keys = FormPath.Split('.');
            if (keys != null)
            {
                if (keys.Count() > 0) model.FormCategory = keys[0];
                if (keys.Count() > 1) model.FormSubCategory = keys[1];
            }
            return View(model);
        }

        [DynamicFormsConfig(ActionType = "Save", DataConnection = "Data/DataSrc.json", DataEntity = "Forms.FormsTemplates", PartialViewHtmlSection = @"DynamicFormContainer", ModelMember = "Form")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ConfigurationAjax([ModelBinder(typeof(TestCaseConfigModelBinder))]TestCaseConfigModel model)
        {
            //TestCaseConfigModel _model = Newtonsoft.Json.JsonConvert.DeserializeObject<TestCaseConfigModel>(model);
            ViewBag.StatusLbl = "Saved Successfully!";
            return View(model);
        }

        [DynamicFormsShow(ActionType = "Load", DataFile = "Data/DataSrc.json", PartialViewHtmlSection = @"DynamicFormContainer")]
        public ActionResult TestCaseShow()
        {
            TestCaseShowModel model = new TestCaseShowModel();
            model.id = 1;
            model.FormDataPath = "Forms.FormsData[?(@.id==1)]";
            model.DataCreteria = "Forms.FormsTemplates.TestCase.ExampleForm";
            return View(model);
        }

        [DynamicFormsShow(ActionType = "Save", DataFile = "Data/DataSrc.json", PartialViewHtmlSection = @"DynamicFormContainer")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TestCaseShow(TestCaseShowModel model)
        {
            model.FormDataParentPath = "Forms.FormsData";
            model.FormDataPath = "Forms.FormsData[?(@.id==1)]";
            model.DataCreteria = "Forms.FormsTemplates.TestCase.ExampleForm";
            ViewBag.StatusLbl = "Saved Successfully!";
            return View(model);
        }



    }
}
