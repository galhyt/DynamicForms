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
        [DynamicFormsConfig(ActionType = "Load", DataFile = "Data/DataSrc.json", FormsTemplatesPath = "Forms/FormsTemplates", FormPath = "TestCase/ExampleForm", PartialViewHtmlSection = @"DynamicFormContainer", ModelMember = "Form")]
        public ActionResult Configuration()
        {
            TestCaseConfigModel model = new TestCaseConfigModel();
            return View(model);
        }

        [DynamicFormsConfig(ActionType = "Save", DataFile = "Data/DataSrc.json", FormsTemplatesPath = "Forms/FormsTemplates", FormPath = "TestCase/ExampleForm", PartialViewHtmlSection = @"DynamicFormContainer", ModelMember = "Form")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configuration(TestCaseConfigModel model)
        {
            model.StatusLbl = "Saved Successfully!";
            return View(model);
        }

        [DynamicFormsShow(ActionType = "Load", DataFile = "Data/DataSrc.json", FormDataPath = "Forms/FormsData[?(@.id==1)]/Data", NewFormPath = "Forms/FormsTemplates/TestCase/ExampleForm", PartialViewHtmlSection = @"DynamicFormContainer")]
        public ActionResult TestCaseShow()
        {
            TestCaseShowModel model = new TestCaseShowModel();
            return View(model);
        }

        [DynamicFormsShow(ActionType = "Save", DataFile = "Data/DataSrc.json", FormDataPath = "Forms/FormsData[?(@.id==1)]/Data", NewFormPath = "Forms/FormsTemplates/TestCase/ExampleForm", PartialViewHtmlSection = @"DynamicFormContainer")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TestCaseShow(TestCaseShowModel model)
        {
            model.StatusLbl = "Saved Successfully!";
            return View(model);
        }



    }
}
