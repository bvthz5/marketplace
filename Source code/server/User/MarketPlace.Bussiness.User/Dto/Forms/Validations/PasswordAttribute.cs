using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MarketPlaceUser.Bussiness.Dto.Forms.Validations
{
    public class PasswordAttribute : ValidationAttribute
    {
        /// <summary>
        /// Regular expression pattern for password validation.
        /// </summary>
        public static string Pattern { get; } = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&#.])[A-Za-z\\d@$!%*?#&.]{8,16}$";

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
