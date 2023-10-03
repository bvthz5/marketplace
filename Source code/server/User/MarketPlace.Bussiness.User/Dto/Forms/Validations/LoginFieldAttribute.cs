using System.ComponentModel.DataAnnotations;

namespace MarketPlaceUser.Bussiness.Dto.Forms.Validations
{
    public class LoginFieldAttribute : ValidationAttribute
    {

        public string FieldName { get; set; }

        public LoginFieldAttribute(string fileldName)
        {
            FieldName = fileldName;
        }

        public override bool IsValid(object? value)
        {
            if (value == null || (string)value == string.Empty)
            {
                ErrorMessage = $"{FieldName} is Required";
                return false;
            }
            return true;
        }
    }
}
