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
        public static Dictionary<string, List<string>> FormsHeirarchy= new Dictionary<string, List<string>>
        {
            {"Client", new List<string> {"Registration","Feedback"}},
            {"Supplier", new List<string> {"OrderDetails", "ReturnCertificate"}}
        };

        public string Editor_Name { get; set; }
        public TemplateFormData Form { get; set; }
        [JsonIgnore]
        public string FormCategory { get; set; }
        [JsonIgnore]
        public string FormSubCategory { get; set; }

        public string FormDataParentPath { get; set; }
        public string FormDataPath { get; set; }
        public string FormPath { get; set; }

        public TestCaseConfigModel()
        {
        }
    }
}