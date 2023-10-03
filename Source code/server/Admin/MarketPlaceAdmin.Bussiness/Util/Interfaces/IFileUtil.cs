using MarketPlace.DataAccess.Model;
using Microsoft.AspNetCore.Http;

namespace MarketPlaceAdmin.Bussiness.Util.Interfaces
{
    public interface IFileUtil
    {
        bool DeleteUserProfilePic(string fileName);

        FileStream? GetProductImages(string fileName);

        FileStream? GetUserProfile(string fileName);

        // Agent

        string? UploadAgentProfilePic(int agentId, IFormFile file);

        bool DeleteAgentProfilePic(string fileName);

        FileStream? GetAgentProfile(string fileName);
    }
}
