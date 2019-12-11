using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicForms;
using Newtonsoft.Json;

namespace TestCase.Models
{
    public class TestCaseConfigModel : IActionFilterAttributes
    {
        public string Editor_Name { get; set; }
        public TemplateFormData Form { get; set; }

        public string FormDataParentPath { get; set; }
        public string FormDataPath { get; set; }
        public string FormPath { get; set; }

        public TestCaseConfigModel()
        {
        }
    }
}