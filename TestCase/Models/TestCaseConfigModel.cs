using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicForms;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace TestCase.Models
{
    public class TestCaseConfigModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string content = ((string[])((ValueProviderCollection)bindingContext.ValueProvider).ToList()[1].GetValue("model").RawValue)[0];
            TestCaseConfigModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<TestCaseConfigModel>(content);

            Regex reg = new Regex(@"(?<=""FormPath"":"")[\w\W]+(?="")");
            Match m = reg.Match(content);
            if (m.Success) model.DataCreteria = m.Value;

            return model;
        }
    }

    
    public class TestCaseConfigModel : IActionFilterAttributes
    {
        public static Dictionary<string, List<string>> FormsHeirarchy= new Dictionary<string, List<string>>
        {
            {"Client", new List<string> {"Registration","Feedback"}},
            {"Supplier", new List<string> {"OrderDetails", "ReturnCertificate"}}
        };

        public TemplateFormData Form { get; set; }
        public string FormCategory { get; set; }
        public string FormSubCategory { get; set; }

        public string FormDataParentPath { get; set; }
        public string FormDataPath { get; set; }
        public string DataCreteria { get; set; }

        public TestCaseConfigModel()
        {
        }
    }
}