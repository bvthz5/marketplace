using MarketPlaceAdmin.Bussiness.Settings;
using MarketPlaceAdmin.Bussiness.Util.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarketPlaceAdmin.Bussiness.Util
{
    /// <summary>
    /// Utility class for managing files, such as retrieving and uploading images for products, administrators, and users.
    /// </summary>
    public class FileUtil : IFileUtil
    {
        private readonly ILogger<FileUtil> _logger;
        private readonly ImageSettings _imageSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUtil"/> class with the specified image settings and logger.
        /// </summary>
        /// <param name="imageSttings">The image settings to use for managing files.</param>
        /// <param name="logger">The logger to use for logging errors.</param>
        public FileUtil(IOptions<ImageSettings> imageSttings, ILogger<FileUtil> logger)
        {
            _imageSettings = imageSttings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a <see cref="FileStream"/> for a product image with the specified filename.
        /// </summary>
        /// <param name="fileName">The filename of the product image to retrieve.</param>
        /// <returns>A <see cref="FileStream"/> for the product image, or <see langword="null"/> if the file could not be found.</returns>
        public FileStream? GetProductImages(string fileName)
        {
            try
            {
                // Construct the path to the product image file using the configured base path and product image path.
                var path = Path.Combine(_imageSettings.Path, _imageSettings.ProductImagePath, fileName);

                // Open a file stream for the specified path and return it.
                return File.OpenRead(path);
            }
            catch (Exception e)
            {
                // Log any errors that occur during file access and return null to indicate failure.
                _logger.LogError("{e} {message}", e, e.Message);
            }

            // If an error occurred, return null to indicate failure.
            return null;
        }

        /// <summary>
        /// Reads and returns the specified user's profile picture from disk as a FileStream object.
        /// </summary>
        /// <param name="fileName">The filename of the user's profile picture.</param>
        /// <returns>A FileStream object representing the user's profile picture, or <see langword="null"/> if the file could not be read.</returns>
        public FileStream? GetUserProfile(string fileName)
        {
            try
            {
                // Construct the full path to the user's profile picture file using the configured base path, user image path, and filename.
                var path = Path.Combine(_imageSettings.Path, _imageSettings.UserImagePath, fileName);

                // Open the file at the specified path and return it as a FileStream object.
                return File.OpenRead(path);
            }
            catch (Exception e)
            {
                // Log any errors that occur during file access and return null to indicate failure.
                _logger.LogError("{e} {message}", e, e.Message);
            }

            // If an error occurred, return null to indicate failure.
            return null;
        }

        /// <summary>
        /// Deletes the specified user's profile picture file from disk.
        /// </summary>
        /// <param name="fileName">The filename of the user's profile picture to delete.</param>
        /// <returns><see langword="true"/> if the file was deleted successfully, <see langword="false"/> otherwise.</returns>
        public bool DeleteUserProfilePic(string fileName)
        {
            // Construct the full path to the user's profile picture file using the configured base path, user image path, and filename.
            var path = Path.Combine(_imageSettings.Path, _imageSettings.UserImagePath, fileName);

            // Delete the file at the specified path and return true if the deletion succeeds.
            return DeleteFile(path);
        }

        /// <summary>
        /// Deletes the file at the specified path.
        /// </summary>
        /// <param name="path">The full path of the file to delete.</param>
        /// <returns><see langword="true"/> if the file was deleted successfully, <see langword="false"/> otherwise.</returns>
        private bool DeleteFile(string path)
        {
            try
            {
                // Attempt to delete the file at the specified path and return true if the deletion succeeds.
                File.Delete(path);
                return true;
            }
            catch (Exception e)
            {
                // If an exception is thrown during the deletion process, log the error and return false.
                _logger.LogError("{e} {message}", e, e.Message);
                return false;
            }
        }

        public string? UploadAgentProfilePic(int agentId, IFormFile file)
        {
            try
            {
                // Construct the path to the directory where admin profile pictures are stored.
                string path = Path.Combine(_imageSettings.Path, _imageSettings.AgentImagePath);

                // Get information about the uploaded file.
                var fileInfo = new FileInfo(file.FileName);

                // Construct a unique filename for the uploaded image using the agent's ID and a new GUID.
                string fileName = $"{agentId}_{Guid.NewGuid()}{fileInfo.Extension}";

                // Create the directory if it doesn't exist.
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // Combine the path and file name to get the full path to save the file.
                string fileNameWithPath = Path.Combine(path, fileName);

                // Open a new file stream and copy the uploaded file to it.
                using var stream = new FileStream(fileNameWithPath, FileMode.Create);

                file.CopyTo(stream);

                return fileName;
            }
            catch (Exception e)
            {
                // Log any errors that occur during file access and return null to indicate failure.
                _logger.LogError("{e} {message}", e, e.Message);
            }

            // If an error occurred, return null to indicate failure.
            return null;
        }

        public bool DeleteAgentProfilePic(string fileName)
        {
            // Construct the path to the agent's profile picture file using the configured base path and agent image path.
            var path = Path.Combine(_imageSettings.Path, _imageSettings.AgentImagePath, fileName);

            // Call the DeleteFile method to delete the specified file and return its result.
            return DeleteFile(path);
        }

        public FileStream? GetAgentProfile(string fileName)
        {
            try
            {
                // Construct the full path to the user's profile picture file using the configured base path, Agent image path, and filename.
                var path = Path.Combine(_imageSettings.Path, _imageSettings.AgentImagePath, fileName);

                // Open the file at the specified path and return it as a FileStream object.
                return File.OpenRead(path);
            }
            catch (Exception e)
            {
                // Log any errors that occur during file access and return null to indicate failure.
                _logger.LogError("{e} {message}", e, e.Message);
            }

            // If an error occurred, return null to indicate failure.
            return null;
        }

    }
}
