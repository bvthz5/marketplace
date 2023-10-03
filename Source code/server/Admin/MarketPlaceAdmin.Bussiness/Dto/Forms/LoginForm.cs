using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    /// <summary>
    /// Represents a login form that contains an email and password field.
    /// </summary>
    public class LoginForm
    {
        /// <summary>
        /// The email address of the user.
        /// </summary>
        /// <value>The email address of the user.</value>
        [FieldNotNull("Email")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// The password of the user.
        /// </summary>
        /// <value>The password of the user.</value>
        [FieldNotNull("Password")]
        public string Password { get; set; } = null!;
    }
}
