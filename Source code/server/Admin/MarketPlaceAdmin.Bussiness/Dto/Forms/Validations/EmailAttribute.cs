using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms.Validations
{
    /// <summary>
    /// Validates that a string value is a valid email address.
    /// </summary>
    public class EmailAttribute : ValidationAttribute
    {
        // Regular expression pattern for matching valid email addresses.
        public static string Pattern { get; } = @"^[A-Z0-9._%+-]+@[A-z0-9.-]+\.[A-Z]{2,254}$";

        /// <summary>
        /// Determines whether the specified value is a valid email address.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>
        /// <c>true</c> if the value is a valid email address; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid(object? value)
        {
            if (value == null || (string)value == string.Empty)
            {
                ErrorMessage = "Email is Required";
                return false;
            }

            if (((string)value).Length > 255)
            {
                ErrorMessage = "Email Length should be less than 255";
                return false;
            }

            ErrorMessage = "Not a valid Email Address";
            return Regex.IsMatch((string)value, Pattern, RegexOptions.IgnoreCase);
        }
    }
}
