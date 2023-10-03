using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;
using System.ComponentModel.DataAnnotations;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    /// <summary>
    /// Represents the model for forgot password form.
    /// </summary>
    ///   /// <remarks>
    /// The <see cref="Token"/> property must be non-null and non-empty.
    /// The <see cref="Password"/> property must be a valid password as per the validation rules specified in the <see cref="PasswordAttribute"/>.
    /// </remarks>
    public class ForgotPasswordForm
    {
        /// <summary>
        /// Gets or sets the token provided to the user via email to reset password.
        /// </summary>
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; } = null!;

        /// <summary>
        /// Gets or sets the new password entered by the user.
        /// </summary>
        [Password]
        public string Password { get; set; } = string.Empty;
    }

}
