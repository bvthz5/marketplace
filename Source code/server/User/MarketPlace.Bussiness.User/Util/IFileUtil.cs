using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarketPlaceUser.Bussiness.Util
{
    public interface IFileUtil
    {
        string? UploadProductImage(Product product, IFormFile file);
        FileStream? GetProductImages(string fileName);
        bool DeleteProductImages(string fileName);
        string? UploadUserProfilePic(User user, IFormFile file);
        FileStream? GetUserProfile(string fileName);
        bool DeleteUserProfilePic(string fileName);
        bool DeleteFile(string path);

        bool UploadImage(string path, string fileName, IFormFile file);
    }
    public class FileUtil : IFileUtil
    {
        private readonly ILogger<IFileUtil> _logger;
        private readonly ImageSettings _imageSettings;

        public FileUtil(IOptions<ImageSettings> imageSttings, ILogger<IFileUtil> logger)
        {
            _imageSettings = imageSttings.Value;
            _logger = logger;
        }

        public string? UploadProductImage(Product product, IFormFile file)
        {
            try
            {
                string path = Path.Combine(_imageSettings.Path, _imageSettings.ProductImagePath);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var fileInfo = new FileInfo(file.FileName);

                string fileName = $"{product.ProductId}_{Guid.NewGuid()}{fileInfo.Extension}";

                if (UploadImage(path, fileName, file))
                    return fileName;
            }
            catch (Exception e)
            {
                _logger.LogError("Error : {e}", e.Message);
            }

            return null;
        }

        public FileStream? GetProductImages(string fileName)
        {
            try
            {
                var path = Path.Combine(_imageSettings.Path, _imageSettings.ProductImagePath, fileName);
                return File.OpenRead(path);
            }
            catch (Exception e)
            {
                _logger.LogError("Error : {e}", e.Message);
            }
            return null;
        }

        public bool DeleteProductImages(string fileName)
        {
            var path = Path.Combine(_imageSettings.Path, _imageSettings.ProductImagePath, fileName);

            return DeleteFile(path);
        }

        public string? UploadUserProfilePic(User user, IFormFile file)
        {
            try
            {
                string path = Path.Combine(_imageSettings.Path, _imageSettings.UserImagePath);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var fileInfo = new FileInfo(file.FileName);

                string fileName = $"{user.UserId}_{Guid.NewGuid()}{fileInfo.Extension}";

                if (UploadImage(path, fileName, file))
                    return fileName;
            }
            catch (Exception e)
            {
                _logger.LogError("Error : {e}", e.Message);
            }

            return null;
        }

        public FileStream? GetUserProfile(string fileName)
        {
            try
            {
                var path = Path.Combine(_imageSettings.Path, _imageSettings.UserImagePath, fileName);
                return File.OpenRead(path);
            }
            catch (Exception e)
            {
                _logger.LogError("Error : {e}", e.Message);
            }
            return null;
        }

        public bool DeleteUserProfilePic(string fileName)
        {
            var path = Path.Combine(_imageSettings.Path, _imageSettings.UserImagePath, fileName);

            return DeleteFile(path);
        }

        public bool DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("Error : {e}", e.Message);
                return false;
            }
        }

        public bool UploadImage(string path, string fileName, IFormFile file)
        {

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string fileNameWithPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("Error : {e}", e.Message);
                return false;
            }
        }
    }
}
