using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MarketPlaceUser.Bussiness.Dto.Forms.Validations
{
    public class PlaceAttribute : ValidationAttribute
    {

        private readonly string _pattern = "^[A-Za-z0-9()&,'\\- \\n\\s]+$";

        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            ErrorMessage = "Validation failed";
            return Regex.IsMatch((string)value, _pattern, RegexOptions.IgnoreCase);
        }
    }
}
