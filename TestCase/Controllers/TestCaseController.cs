﻿using System;
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
        [DynamicFormsConfig(ActionType = "Load", DataFile = "Data/DataSrc.json", FormsTemplatesPath = "Forms.FormsTemplates", PartialViewHtmlSection = @"DynamicFormContainer", ModelMember = "Form")]
        public ActionResult Configuration()
        {
            TestCaseConfigModel model = new TestCaseConfigModel();
            model.FormPath = "TestCase.ExampleForm";
            return View(model);
        }

        [DynamicFormsConfig(ActionType = "Save", DataFile = "Data/DataSrc.json", FormsTemplatesPath = "Forms.FormsTemplates", PartialViewHtmlSection = @"DynamicFormContainer", ModelMember = "Form")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configuration(TestCaseConfigModel model)
        {
            model.FormPath = "TestCase.ExampleForm";
            ViewBag.StatusLbl = "Saved Successfully!";
            return View(model);
        }

        [DynamicFormsShow(ActionType = "Load", DataFile = "Data/DataSrc.json", PartialViewHtmlSection = @"DynamicFormContainer")]
        public ActionResult TestCaseShow()
        {
            TestCaseShowModel model = new TestCaseShowModel();
            model.id = 1;
            model.FormDataPath = "Forms.FormsData[?(@.id==1)]";
            model.FormPath = "Forms.FormsTemplates.TestCase.ExampleForm";
            return View(model);
        }

        [DynamicFormsShow(ActionType = "Save", DataFile = "Data/DataSrc.json", PartialViewHtmlSection = @"DynamicFormContainer")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TestCaseShow(TestCaseShowModel model)
        {
            model.FormDataParentPath = "Forms.FormsData";
            model.FormDataPath = "Forms.FormsData[?(@.id==1)]";
            model.FormPath = "Forms.FormsTemplates.TestCase.ExampleForm";
            ViewBag.StatusLbl = "Saved Successfully!";
            return View(model);
        }



    }
}
