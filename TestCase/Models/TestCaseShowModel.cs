using DynamicForms;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestCase.Models
{
    public class TestCaseShowModel : TemplateDynamicFormModel
    {
        public string StatusLbl { get; set; }
        public string Editor_Name { get; set; }

        public TestCaseShowModel()
        {
        }

        // Required by TemplateDynamicFormModel parent class
        public TestCaseShowModel(TemplateFormData FormTemplate) : base(FormTemplate)
        {
        }

        // Required by TemplateDynamicFormModel parent class
        public override void CastFromJObject(JObject JModel)
        {
            TestCaseShowModel model = JModel.ToObject<TestCaseShowModel>();
            Editor_Name = model.Editor_Name;
            Date = model.Date;
            FieldsMetaData = model.FieldsMetaData;
            FormData = model.FormData;
            FormTitle = model.FormTitle;
            UITable = model.UITable;
        }
    }
}