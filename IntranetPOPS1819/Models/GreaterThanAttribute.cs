using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace IntranetPOPS1819.Models
{
    public class GreaterThanAttribute : ValidationAttribute, IClientValidatable
    {
        private string _startDatePropertyName;
        private DateTime StartDate { get; set; }
        private DateTime EndDate { get; set; }

        public GreaterThanAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            int result = DateTime.Compare(StartDate, EndDate);
            if (result < 0)
            {
                yield return new ModelClientValidationRule();
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            if (propertyInfo == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _startDatePropertyName));
            }
            DateTime propertyValue = (DateTime)propertyInfo.GetValue(validationContext.ObjectInstance, null);
            DateTime date = (DateTime)value;
            if (date.Month > propertyValue.Month)
            {
                return ValidationResult.Success;
            }
            else
            {
                var startDateDisplayName = propertyInfo
                    .GetCustomAttributes(typeof(DisplayAttribute), true)
                    .Cast<DisplayAttribute>()
                    .Single()
                    .Name;
                return new ValidationResult(validationContext.DisplayName + " must be later than " + startDateDisplayName + ".");
            }
        }
    }
}