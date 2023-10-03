using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MarketPlaceUser.Bussiness.Dto.Forms.Validations
{
    public class ZipCodeAttribute : ValidationAttribute
    {
        private readonly string _pattern = "^[0-9]{6}$";

        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            ErrorMessage = "Invalid ZipCode";
            return Regex.IsMatch((string)value, _pattern);
        }
    }
}
