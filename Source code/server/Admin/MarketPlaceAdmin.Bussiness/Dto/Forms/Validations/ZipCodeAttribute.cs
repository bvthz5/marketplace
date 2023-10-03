using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms.Validations
{
    public class ZipCodeAttribute : ValidationAttribute
    {
        private readonly string _pattern = "^[0-9]{6}$";

        public bool IsNullable { get; set; } = false;

        public override bool IsValid(object? value)
        {
            if (value == null)
                return IsNullable;


            ErrorMessage = "Invalid ZipCode";
            return Regex.IsMatch((string)value, _pattern);
        }
    }
}
