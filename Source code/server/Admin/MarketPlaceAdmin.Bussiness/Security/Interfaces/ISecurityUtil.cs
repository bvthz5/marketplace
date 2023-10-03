namespace MarketPlaceAdmin.Bussiness.Security.Interfaces
{
    /// <summary>
    /// Provides utility methods for security-related operations.
    /// </summary>
    public interface ISecurityUtil
    {
        /// <returns>The ID of the currently authenticated admin user, or 0 if there is no authenticated user.</returns>
        public int GetCurrentUserId();

        public bool IsAdmin();
    }
}
