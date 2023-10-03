using System.ComponentModel.DataAnnotations;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms.Validations
{
    /// <summary>
    /// Validates that a field is not null or empty.
    /// </summary>
    public class FieldNotNullAttribute : ValidationAttribute
    {
        /// <summary>
        /// The name of the field to display.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldNotNullAttribute"/> class with the specified field name.
        /// </summary>
        /// <param name="fieldName">The name of the field to validate.</param>
        public FieldNotNullAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        /// <summary>
        /// Determines whether the value of the specified object is not null or empty.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <returns>true if the value is not null or empty; otherwise, false.</returns>
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
