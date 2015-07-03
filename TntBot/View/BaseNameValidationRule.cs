using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;
using TntBot.Properties;

namespace TntBot.View
{
    internal class BaseNameValidationRule : ValidationRule
    {
        private static readonly string[] ValidBaseName = { "ZlrWCb7i5QFTwJZYa4NtTbI/x/A=" };

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string text = value as string;

            if (string.IsNullOrEmpty(text))
                return new ValidationResult(false, Resources.BaseNameValidationRuleEmptyString);

            string hash = Encrypt(text);
            if (!ValidBaseName.Contains(hash))
                return new ValidationResult(false, string.Format(Resources.BaseNameValidationRuleHashCode, text));

            return new ValidationResult(true, null);
        }

        /// <summary>
        /// Gets the Hash from the text
        /// </summary>
        private static string Encrypt(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var sha1 = new SHA1Managed();
            return Convert.ToBase64String(sha1.ComputeHash(buffer));
        }
    }
}