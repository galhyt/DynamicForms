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

namespace TestCase.Controllers
{
    public class TestCaseController : Controller
    {
        [DynamicFormsConfig(ActionType="Load", DataFile = "Data/DataSrc.json", FormsKey = "DynamicForms", FormPath = "TestCase/ExampleForm", PartialViewHtmlSection = @"DynamicFormContainer", ModelMember = "Form")]
        public ActionResult Configuration()
        {
            TestCaseModel model = new TestCaseModel();
            return View(model);
        }

        [DynamicFormsConfig(ActionType = "Save", DataFile = "Data/DataSrc.json", FormsKey = "DynamicForms", FormPath = "TestCase/ExampleForm", PartialViewHtmlSection = @"DynamicFormContainer", ModelMember = "Form")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configuration(TestCaseModel model)
        {
            return View(model);
        }

    }
}
