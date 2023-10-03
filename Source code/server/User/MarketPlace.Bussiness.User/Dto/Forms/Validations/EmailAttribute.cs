using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MarketPlaceUser.Bussiness.Dto.Forms.Validations
{
    public class EmailAttribute : ValidationAttribute
    {
        /// <summary>
        /// Regular expression pattern for matching valid email addresses.
        /// </summary>
        public static string Pattern { get; } = @"^[A-Z0-9._%+-]+@[A-z0-9.-]+\.[A-Z]{2,254}$";

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
