using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms.Validations
{
    /// <summary>
    /// Validates if the password meets the complexity requirements.
    /// </summary>
    public class PasswordAttribute : ValidationAttribute
    {
        // Regular expression pattern for password validation.
        public static string Pattern { get; } = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&#.])[A-Za-z\\d@$!%*?#&.]{8,16}$";

        /// <summary>
        /// Validates if the given value is a valid password.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>Returns true if the value is a valid password, false otherwise.</returns>
        public override bool IsValid(object? value)
        {
            if (value == null || (string)value == string.Empty)
            {
                ErrorMessage = "Password is Required";
                return false;
            }

            if (((string)value).Length > 16 || ((string)value).Length < 8)
            {
                ErrorMessage = "Password Length should be 8 - 16";
                return false;
            }

            ErrorMessage = "Must contain at least one uppercase letter, one lowercase letter, one number and one special character";
            return Regex.IsMatch((string)value, Pattern);
        }
    }
}
