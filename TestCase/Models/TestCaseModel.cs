using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicForms;

namespace TestCase.Models
{
    public class TestCaseModel : TemplateDynamicFormModel
    {
        public string Editor_Name { get; set; }

        public TestCaseModel()
        {
        }

        public TestCaseModel(TemplateFormData form)
            : base(form)
        {
        }
    }
}