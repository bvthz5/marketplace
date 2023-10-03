using System.Net;

namespace MarketPlaceAdmin.Bussiness.Enums
{
    /// <summary>
    /// Represents the status of a service operation.
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>
        /// Indicates that the service operation was successful.
        /// Corresponds to HTTP status code 200 (OK).
        /// </summary>
        Success = HttpStatusCode.OK,
        /// <summary>
        /// Indicates that the request was malformed or invalid.
        /// Corresponds to HTTP status code 400 (Bad Request).
        /// </summary>
        BadRequest = HttpStatusCode.BadRequest,

        /// <summary>
        /// Indicates that the request was not authenticated or authorized to access the requested resource.
        /// Corresponds to HTTP status code 401 (Unauthorized).
        /// </summary>
        Unauthorized = HttpStatusCode.Unauthorized,

        /// <summary>
        /// Indicates that the requested resource was not found.
        /// Corresponds to HTTP status code 404 (Not Found).
        /// </summary>
        NotFound = HttpStatusCode.NotFound,

        /// <summary>
        /// Indicates that the service encountered an unexpected error while processing the request.
        /// Corresponds to HTTP status code 500 (Internal Server Error).
        /// </summary>
        InternalServerError = HttpStatusCode.InternalServerError,

        /// <summary>
        /// Indicates that the requested resource already exists.
        /// Corresponds to HTTP status code 409 (Conflict).
        /// </summary>
        AlreadyExists = HttpStatusCode.Conflict
    }
}
