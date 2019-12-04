using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace DynamicForms
{
    public class TemplateDynamicFormModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            TemplateDynamicFormModel model = (TemplateDynamicFormModel)base.BindModel(controllerContext, bindingContext);
            TemplateDynamicFormModelWrapper modelWrapper = new TemplateDynamicFormModelWrapper(model);
            SetFieldsValuesByType(modelWrapper);
            model.FormData = modelWrapper.FormData.Select(x => new TemplateDynamicFormModel.FormDataEntry { Name = x.Key, Value = x.Value }).ToList();

            return model;
        }

        private void SetFieldsValuesByType(TemplateDynamicFormModelWrapper modelWrapper)
        {
            foreach (string FieldName in modelWrapper.FieldsNamesInMetaData)
            {
                object FieldValue = modelWrapper.FormData[FieldName];
                if (FieldValue != null)
                {
                    if (((string[])FieldValue)[0] == "")
                        modelWrapper.FormData[FieldName] = null;
                    else
                        modelWrapper.FormData[FieldName] = Convert.ChangeType(((string[])FieldValue)[0], Type.GetType(modelWrapper.FieldsMetaData.Single(x => x.FieldName == FieldName).FieldType));
                }
            }
        }
    }
}