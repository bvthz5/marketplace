using System.Net;

namespace MarketPlaceUser.Bussiness.Enums
{
    public enum ServiceStatus
    {
        Success = HttpStatusCode.OK,
        Created = HttpStatusCode.Created,
        BadRequest = HttpStatusCode.BadRequest,
        Unauthorized = HttpStatusCode.Unauthorized,
        NotFound = HttpStatusCode.NotFound,
        InternalServerError = HttpStatusCode.InternalServerError,
        AlreadyExists = HttpStatusCode.Conflict,
    }
}
