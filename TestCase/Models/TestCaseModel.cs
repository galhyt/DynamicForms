using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicForms;

namespace TestCase.Models
{
    public class TestCaseModel
    {
        public string Editor_Name { get; set; }
        public TemplateFormData Form { get; set; }
        public string StatusLbl { get; set; }


        public TestCaseModel()
        {
        }
    }
}