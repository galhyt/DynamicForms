using DynamicForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestCase.Models
{
    public class TestCaseShowModel
    {
        public string StatusLbl { get; set; }
        public string Editor_Name { get; set; }
        public TemplateDynamicFormModel FormModel { get; set; }
    }
}