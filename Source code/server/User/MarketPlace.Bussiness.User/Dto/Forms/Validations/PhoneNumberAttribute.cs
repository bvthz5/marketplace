using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MarketPlaceUser.Bussiness.Dto.Forms.Validations
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        private readonly string _pattern = "^\\d{10}$";

        public bool Nullable { get; set; } = false;

        public override bool IsValid(object? value)
        {
            if ((value == null || (string)value == string.Empty) && Nullable)
            {
                return true;
            }
            else if (value is null)
            {
                ErrorMessage = "Phone Number Required";
                return false;
            }

            ErrorMessage = "Not a valid Phone Number";
            return Regex.IsMatch((string)value, _pattern);
        }
    }
}
