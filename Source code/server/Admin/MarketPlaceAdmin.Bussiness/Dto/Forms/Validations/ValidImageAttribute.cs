using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms.Validations
{
    /// <summary>
    /// Validates whether the uploaded file(s) are valid images of supported file types and size.
    /// </summary>
    public class ValidImageAttribute : ValidationAttribute
    {
        private readonly string[] _acceptedFileTypes = { ".jpg", ".jpeg", ".png", ".webp" };
        private readonly int _maxBytes = 2 * 1024 * 1024; // 2 MB maximum file size

        /// <summary>
        /// Validates whether the uploaded file(s) are valid images of supported file types and size.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value is a valid image of supported file type and size, false otherwise.</returns>
        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            // Validate single image file
            if (value is IFormFile image)
            {
                // Check if the uploaded file type is supported
                if (!_acceptedFileTypes.Contains(Path.GetExtension(image.FileName).ToLower()))
                {
                    ErrorMessage = $"Only {string.Join(", ", _acceptedFileTypes)} are Supported, Uploaded : '{Path.GetExtension(image.FileName)}'";
                    return false;
                }

                // Check if the uploaded file size is within the allowed limit
                if (_maxBytes <= image.Length)
                {
                    ErrorMessage = $"Max File Size : {_maxBytes / 1024}KB, Uploaded File Size : {image.Length / 1024}KB ";
                    return false;
                }
                return true;
            }

            // Validate multiple image files
            else if (value is IFormFile[] images)
            {
                for (int i = 0; i < images.Length; i++)
                {
                    var file = images[i];
                    // Check if the uploaded file type is supported
                    if (!_acceptedFileTypes.Contains(Path.GetExtension(file.FileName).ToLower()))
                    {
                        ErrorMessage = $"Only {string.Join(", ", _acceptedFileTypes)} are Supported, Uploaded : '{Path.GetExtension(file.FileName)}'";
                        return false;
                    }

                    // Check if the uploaded file size is within the allowed limit
                    if (_maxBytes <= file.Length)
                    {
                        ErrorMessage = $"Max File Size : {_maxBytes / 1024}KB, Uploaded File Size : {file.Length / 1024}KB ";
                        return false;
                    }
                }
                return true;
            }

            // If the value is not a valid image file or array of image files, return false
            ErrorMessage = "Invalid Type";
            return false;
        }
    }
}
