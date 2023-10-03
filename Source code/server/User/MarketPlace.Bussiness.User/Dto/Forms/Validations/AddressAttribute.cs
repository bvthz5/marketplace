using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MarketPlaceUser.Bussiness.Dto.Forms.Validations
{
    public class AddressAttribute : ValidationAttribute
    {

        private readonly string _pattern = "^[A-Za-z0-9()_\\-,.'&+:/ \\n\\s]+$";

        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            ErrorMessage = "Validation Failed";
            return Regex.IsMatch((string)value, _pattern, RegexOptions.IgnoreCase);
        }
    }
}
