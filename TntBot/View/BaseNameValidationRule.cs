using System.Globalization;
using System.Windows.Controls;
using TntBot.Properties;

namespace TntBot.View
{
    internal class BaseNameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string text = value as string;

            if (string.IsNullOrEmpty(text))
                return new ValidationResult(false, Resources.BaseNameValidationRuleMessage);

            return new ValidationResult(true, null);
        }
    }
}