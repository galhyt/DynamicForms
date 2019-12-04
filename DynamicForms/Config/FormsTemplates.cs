using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicForms
{
    public class FormsTemplates : Dictionary<string, Dictionary<string, TemplateFormData>>
    {
        public TemplateFormData this[string path]
        {
            get
            {
                return GetTemplateFormData(path);
            }
            set
            {
                CreateIfNotExist(path);
                string[] keys = GetPathElements(path);
                if (keys == null) return;
                base[keys[0]][keys[1]] = value;
            }
        }

        public void Init(string path)
        {
            if (GetTemplateFormData(path) != null) return;
            this[path] = new TemplateFormData();
            this[path].FieldsMetaData.Add(new TemplateFormData.FieldMetaData { FieldName = "Field1", FieldType = "text" });
            this[path].UITable.Add(new List<List<TemplateFormData.FieldUIData>> {
                new List<TemplateFormData.FieldUIData> { new TemplateFormData.FieldUIData{FieldName="Field1", HtmlType="text"}}
            });
        }

        private TemplateFormData GetTemplateFormData(string path)
        {
            string[] keys = GetPathElements(path);
            if (keys == null) return null;
            if (!this.ContainsKey(keys[0])) return null;
            if (!base[keys[0]].ContainsKey(keys[1])) return null;
            return base[keys[0]][keys[1]];
        }

        private string[] GetPathElements(string path)
        {
            string[] keys = path.Split('/');
            if (keys.Count() != 2) return null;

            return keys;
        }

        private void CreateIfNotExist(string path)
        {
            string[] keys = GetPathElements(path);
            if (keys == null) return;
            if (!base.ContainsKey(keys[0]))
                base.Add(keys[0], new Dictionary<string, TemplateFormData>());
            if (!base[keys[0]].ContainsKey(keys[1]))
                base[keys[0]].Add(keys[1], null);
        }
    }

}
