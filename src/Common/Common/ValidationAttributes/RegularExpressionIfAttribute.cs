using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Common.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RegularExpressionIfAttribute : ValidationAttribute, IClientValidatable
    {
        public string RegEx { get; private set; }
        public string OtherProperty { get; private set; }
        public Comparison Comparison { get; private set; }
        public Object Value { get; private set; }

        public RegularExpressionIfAttribute(string regex, string otherProperty, Comparison comparison, object value)
        {
            if (string.IsNullOrEmpty(regex))
            {
                throw new ArgumentNullException("regex");
            }
            if (string.IsNullOrEmpty(otherProperty))
            {
                throw new ArgumentNullException("otherProperty");
            }
            RegEx = regex;
            OtherProperty = otherProperty;
            Comparison = comparison;
            Value = value;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, OtherProperty);
        }

        public bool Validate(object actualPropertyValue)
        {
            switch (Comparison)
            {
                case Comparison.IsNotEqualTo:
                    return actualPropertyValue == null ? Value != null : !actualPropertyValue.Equals(Value);
                default:
                    return actualPropertyValue == null ? Value == null : actualPropertyValue.Equals(Value);
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Regex.IsMatch(value.ToString(), RegEx))
            {
                try
                {
                    var property = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty);
                    var propertyValue = property.GetValue(validationContext.ObjectInstance, null);

                    if (Validate(propertyValue))
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                }
                catch (Exception e)
                {
                    if (OtherProperty.Contains('_'))
                    {
                        var prop = OtherProperty.Split('_');

                        var property = validationContext.ObjectInstance.GetType().GetProperty(prop.Last());
                        var propertyValue = property.GetValue(validationContext.ObjectInstance, null);

                        if (!Validate(propertyValue))
                        {
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]
                       {
                           new ModelClientValidationRegularExpressionIfRule(
                               FormatErrorMessage(metadata.GetDisplayName()), RegEx, OtherProperty, Comparison, Value, metadata, context)
                       };
        }
    }
    public class ModelClientValidationRegularExpressionIfRule : ModelClientValidationRule
    {
        public ModelClientValidationRegularExpressionIfRule(string errorMessage, string regex, string otherProperty, Comparison comparison, object value, ModelMetadata metadata, ControllerContext context)
        {
            ErrorMessage = errorMessage;
            ValidationType = "regularexpressionif";
            ValidationParameters.Add("regex", regex);
            ValidationParameters.Add("other", BuildDependentPropertyId(metadata, context as ViewContext, otherProperty));
            ValidationParameters.Add("comp", comparison.ToString().ToLower());
            ValidationParameters.Add("value", value.ToString().ToLower());
        }

        private string BuildDependentPropertyId(ModelMetadata metadata, ViewContext viewContext, string otherProperty)
        {
            return QualifyFieldId(metadata, otherProperty, viewContext);
        }

        protected string QualifyFieldId(ModelMetadata metadata, string fieldId, ViewContext viewContext)
        {
            // build the ID of the property
            string depProp = viewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(fieldId);
            // unfortunately this will have the name of the current field appended to the beginning,
            // because the TemplateInfo's context has had this fieldname appended to it. Instead, we
            // want to get the context as though it was one level higher (i.e. outside the current property,
            // which is the containing object (our Person), and hence the same level as the dependent property.
            var thisField = metadata.PropertyName + "_";
            if (depProp.StartsWith(thisField))
                // strip it off again
                depProp = depProp.Substring(thisField.Length);
            else if (depProp.Contains(thisField))
            {
                depProp = depProp.Remove(depProp.IndexOf(thisField), thisField.Length);
            }
            return depProp;
        }
    }
}
