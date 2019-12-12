using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicForms
{
    public class FormsTemplates : Dictionary<string, object>
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
                SetTemplateFormData(path, value);
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

        private void _SetTemplateFormData(string[] keys, object dic, TemplateFormData value)
        {
            if (dic.GetType() != typeof(Dictionary<string, object>) && dic.GetType() != typeof(FormsTemplates)) return;
            if (keys.Count() > 1)
            {
                if (!((Dictionary<string, object>)dic).ContainsKey(keys[0])) return;
                ((Dictionary<string, object>)dic)[keys[0]] = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Newtonsoft.Json.JsonConvert.SerializeObject(((Dictionary<string, object>)dic)[keys[0]]));
                _SetTemplateFormData(keys.Skip(1).ToArray(), ((Dictionary<string, object>)dic)[keys[0]], value);
            }
            else
                ((Dictionary<string, object>)dic)[keys[0]] = value;
        }

        private void SetTemplateFormData(string path, TemplateFormData value)
        {
            string[] keys = GetPathElements(path);
            if (keys == null) return;

            _SetTemplateFormData(keys, this, value);
        }

        private object _GetTemplateFormData(string[] keys, Dictionary<string, object> dic)
        {
            if (!dic.ContainsKey(keys[0])) return null;
            if (keys.Count() == 1) {
                if (dic[keys[0]].GetType() == typeof(TemplateFormData)) return dic[keys[0]];
                if (dic[keys[0]].GetType() == typeof(JObject)) return ((JObject)dic[keys[0]]).ToObject<TemplateFormData>();
                return null;
            }

            Dictionary<string, object> _dic = null;
            if (dic[keys[0]].GetType() == typeof(JObject))
                _dic = ((JObject)dic[keys[0]]).ToObject<Dictionary<string, object>>();
            else
                _dic = (Dictionary<string, object>)dic[keys[0]];

            return _GetTemplateFormData(keys.Skip(1).ToArray(), _dic);
        }
        
        private TemplateFormData GetTemplateFormData(string path)
        {
            string[] keys = GetPathElements(path);
            if (keys == null) return null;

            return (TemplateFormData)_GetTemplateFormData(keys, this);
        }

        private string[] GetPathElements(string path)
        {
            string[] keys = path.Split('.');
            if (keys.Count() == 0) return null;

            return keys;
        }

        private void _CreateIfNotExist(string[] keys, Dictionary<string, object> dic)
        {
            if (!dic.ContainsKey(keys[0]))
            {
                object val = null;
                if (keys.Count() > 1) val = new Dictionary<string, object>();
                dic.Add(keys[0], val);
            }

            Dictionary<string, object> _dic = null;
            if (dic[keys[0]].GetType() == typeof(JObject))
                _dic = ((JObject)dic[keys[0]]).ToObject<Dictionary<string, object>>();
            else
                _dic = (Dictionary<string, object>)dic[keys[0]];
            if (keys.Count() > 1) _CreateIfNotExist(keys.Skip(1).ToArray(), _dic);
        }

        private void CreateIfNotExist(string path)
        {
            string[] keys = GetPathElements(path);
            if (keys == null) return;

            _CreateIfNotExist(keys, this);
        }
    }

}
