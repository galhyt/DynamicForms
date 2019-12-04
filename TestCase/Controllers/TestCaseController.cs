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
        [DynamicFormsConfig(DataFile = "Data/DataSrc.json", FormPath = "TestCase/ExampleForm")]
        public void Configuration()
        {
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TestCaseForm(TestCaseModel model)
        {
            return View(model);
        }

    }
}
