using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicForms
{
    public class TemplateFormData
    {
        public string FormTitle { get; set; }
        public List<FieldMetaData> FieldsMetaData { get; set; }
        public List<List<List<FieldUIData>>> UITable { get; set; }

        public TemplateFormData()
        {
            FieldsMetaData = new List<FieldMetaData>();
            UITable = new List<List<List<FieldUIData>>>();
        }

        public class FieldMetaData
        {
            public string FieldName { get; set; }
            public string FieldTitle { get; set; }
            public string FieldType { get; set; }
        }

        public class FieldUIData
        {
            public string FieldName { get; set; }
            public string HtmlType { get; set; }
        }
    }
}
