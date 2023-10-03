namespace MarketPlaceAdmin.Bussiness.Settings
{
    /// <summary>
    /// Represents the settings related to images used in the application.
    /// </summary>
    public class ImageSettings
    {
        /// <summary>
        /// The base path for all images.
        /// </summary>
        public string Path { get; set; } = null!;

        /// <summary>
        /// The path for product images.
        /// </summary>
        public string ProductImagePath { get; set; } = null!;

        /// <summary>
        /// The path for user profile images.
        /// </summary>
        public string UserImagePath { get; set; } = null!;

        /// <summary>
        /// The path for agent profile images.
        /// </summary>
        public string AgentImagePath { get; set; } = null!;
    }
}
