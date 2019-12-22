using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace DynamicForms
{
    public interface IActionFilterAttributes
    {
        [JsonIgnore]
        string FormDataParentPath { get; set; }
        [JsonIgnore]
        string FormDataPath { get; set; }
        [JsonIgnore]
        string DataCreteria { get; set; }
    }

    [ModelBinder(typeof(TemplateDynamicFormModelBinder))]
    public class TemplateDynamicFormModel : IActionFilterAttributes
    {
        public string FormTitle { get; set; }
        public string Date { get; set; }
        public List<TemplateFormData.FieldMetaData> FieldsMetaData { get; set; }
        public List<List<List<TemplateFormData.FieldUIData>>> UITable { get; set; }
        public List<FormDataEntry> FormData { get; set; } // for serialization purpose

        public virtual void CastFromJObject(JObject baseModel) {}
        public virtual string FormDataParentPath { get; set; }
        public virtual string FormDataPath { get; set; }
        public virtual string DataCreteria { get; set; }

        public TemplateDynamicFormModel()
        {
        }

        public TemplateDynamicFormModel(TemplateFormData form)
        {
            FormTitle = form.FormTitle;
            Date = DateTime.Now.ToString();
            FieldsMetaData = form.FieldsMetaData;
            UITable = form.UITable;
        }

        public class FormDataEntry
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }
    }

    public class TemplateDynamicFormModelWrapper
    {
        public List<TableRow> FormTable;
        public List<TemplateFormData.FieldMetaData> FieldsMetaData { get; set; }
        public List<string> FieldsNamesInMetaData;
        public Dictionary<string, object> FormData { get; set; }

        public TemplateDynamicFormModelWrapper()
        {
        }

        public TemplateDynamicFormModelWrapper(TemplateDynamicFormModel Form)
        {
            FormTable = (from List<List<TemplateFormData.FieldUIData>> row in Form.UITable
                         select new TableRow(row, Form)).ToList();
            FieldsMetaData = Form.FieldsMetaData;
            FieldsNamesInMetaData = Form.FieldsMetaData.Select(x => x.FieldName).ToList();
            if (Form.FormData != null)
                FormData = Form.FormData.ToDictionary(x => x.Name, x => x.Value);
        }

        private static Dictionary<int, Dictionary<int,int>> ColomnWidthDic = new Dictionary<int, Dictionary<int,int>>
        {
           { 3, new Dictionary<int,int> {{10,30},{15, 40}, {20, 50}, {25, 55}}}
        };
        public int? GetRowColTitleWidth(TableRow row)
        {
            int maxTitleLength = 0;
            if (row.RowCells.SelectMany(x => x.Fields).ToList().Exists(x=>x["FieldTitle"] != null))
                maxTitleLength = row.RowCells.SelectMany(x => x.Fields).Where(x=> x["FieldTitle"] != null).Max(x => x["FieldTitle"].ToString().Count());

            int noOfCol = row.RowCells.Count();
            if (!ColomnWidthDic.ContainsKey(noOfCol)) return null;

            var rowDic = ColomnWidthDic[noOfCol];
            if (maxTitleLength < rowDic.Keys.Min()) return null;
            int titleTdWidth = rowDic[rowDic.Where(x => x.Key <= maxTitleLength).Max(x => x.Key)];

            return titleTdWidth;
        }

        public int GetRowColWidth(TableRow row)
        {
            return (int)Math.Floor((double)(100 / row.RowCells.Count()))-3;
        }

        public class TableRow
        {
            public List<RowCell> RowCells;

            public TableRow(List<List<TemplateFormData.FieldUIData>> row, TemplateDynamicFormModel Form)
            {
                RowCells = (from List<TemplateFormData.FieldUIData> cell in row
                            select new RowCell(cell, Form)).ToList();
            }
        }

        public class RowCell
        {
            public List<FieldData> Fields;

            public RowCell(List<TemplateFormData.FieldUIData> cell, TemplateDynamicFormModel Form)
            {
                Fields = (from TemplateFormData.FieldUIData FUIData in cell
                          select new FieldData(FUIData, Form)).ToList();
            }
        }

        public class FieldData
        {
            private TemplateFormData.FieldMetaData MetaData;
            private TemplateFormData.FieldUIData UIData;
            private object Value;

            public FieldData(TemplateFormData.FieldUIData FUIData, TemplateDynamicFormModel Form)
            {
                string FieldName = FUIData.FieldName;
                UIData = FUIData;
                MetaData = Form.FieldsMetaData.SingleOrDefault(x => x.FieldName == FieldName);
                if (Form.FormData != null) Value = Form.FormData.SingleOrDefault(x=>x.Name == FieldName).Value;
            }

            public object this[string PropertyName]
            {
                get
                {
                    switch (PropertyName)
                    {
                        case "FieldName":
                            return MetaData.FieldName;
                            break;
                        case "FieldTitle":
                            return MetaData.FieldTitle;
                            break;
                        case "FieldType":
                            return MetaData.FieldType;
                            break;
                        case "HtmlType":
                            return UIData.HtmlType;
                            break;
                        case "Value":
                            return Value;
                            break;
                    }

                    return null;
                }
            }
        }

        public int MetaDataIndexOf(string FieldName)
        {
            return FieldsNamesInMetaData.IndexOf(FieldName);
        }
    }
}
