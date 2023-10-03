using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    /// <summary>
    /// A class representing the form for changing a user's password.
    /// </summary>
    /// <remarks>
    /// This form is typically used for allowing users to change their passwords. The <see cref="CurrentPassword"/>
    /// property should contain the user's current password, and the <see cref="NewPassword"/> property should contain the
    /// new password to be set for the user. Both properties are decorated with the <see cref="PasswordAttribute"/>,
    /// which ensures that the password meets certain complexity requirements.
    /// </remarks>
    public class ChangePasswordForm
    {
        /// <summary>
        /// Gets or sets the user's current password.
        /// </summary>
        /// <value>The user's current password.</value>
        [Password]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the new password to be set for the user.
        /// </summary>
        /// <value>The new password to be set for the user.</value>
        [Password]
        public string NewPassword { get; set; } = string.Empty;
    }
}
