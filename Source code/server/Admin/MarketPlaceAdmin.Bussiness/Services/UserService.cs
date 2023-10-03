using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Views;
using MarketPlaceAdmin.Bussiness.Enums;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Util.Interfaces;
using Microsoft.Extensions.Logging;

namespace MarketPlaceAdmin.Bussiness.Services
{
    /// <summary>
    /// UserService is responsible for handling user related business logic.
    /// It implements IUserService interface.
    /// </summary>
    public class UserService : IUserService
    {

        private readonly IUnitOfWork _uow;

        private readonly IEmailService _emailService;
        private readonly IFileUtil _fileUtil;
        private readonly ILogger<UserService> _logger;

        /// <summary>
        /// Constructor to initialize UserService with required dependencies.
        /// </summary>
        /// <param name="uow">IUnitOfWork instance to handle database transactions.</param>
        /// <param name="emailService">IEmailService instance to send email notifications.</param>
        /// <param name="fileUtil">FileUtil instance to handle file related operations.</param>
        /// <param name="logger">ILogger instance to log events.</param>
        public UserService(IUnitOfWork uow, IEmailService emailService, IFileUtil fileUtil, ILogger<UserService> logger)
        {
            _uow = uow;
            _logger = logger;
            _emailService = emailService;
            _fileUtil = fileUtil;
        }

        /// <summary>
        /// This method retrieves the details of a user based on the provided user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve.</param>
        /// <returns>A ServiceResult object containing a UserDetailView object if the user is found, or an error message if the user is not found.</returns>
        public async Task<ServiceResult> GetUser(int userId)
        {
            ServiceResult result = new();

            // Query the UserRepository for a user with the specified ID
            User? user = await _uow.UserRepository.FindById(userId);

            // If the user is not found, return an error message
            if (user is null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = $"User Not Found for Id : {userId}";
                return result;
            }

            // If the user is found, return their details in a UserDetailView object
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "Success";
            result.Data = new UserDetailView(user);

            return result;
        }

        /// <summary>
        /// Changes the status of a user in the system.
        /// </summary>
        /// <param name="userId">The ID of the user whose status will be changed.</param>
        /// <param name="status">The new status code of the user. Must be one of the following: 0 (Inactive), 1 (Active), 2 (Blocked), or 3 (Deleted).</param>
        /// <returns>A ServiceResult object that includes a UserDetailView object if the operation was successful.</returns>
        public async Task<ServiceResult> ChangeStatusAsync(int userId, byte status)
        {
            // Create a new ServiceResult object to hold the results of the operation
            ServiceResult result = new();

            // Check if the provided status code is valid
            if (!Enum.IsDefined(typeof(User.UserStatus), status))
            {
                _logger.LogWarning("Invalid status code {status} provided for user {userId}", status, userId);

                // If the status code is not valid, return a BadRequest ServiceResult object with an error message
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Status";
                return result;
            }

            // Find the user with the provided ID
            User? user = await _uow.UserRepository.FindById(userId);

            // If the user is not found, return a NotFound ServiceResult object with an error message
            if (user is null)
            {
                _logger.LogWarning("User {userId} not found", userId);

                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = $"User Not Found for Id : {userId}";
                return result;
            }

            // If the user is already deleted, return a BadRequest ServiceResult object with an error message
            if (user.Status == User.UserStatus.DELETED)
            {
                _logger.LogWarning("User {userId} is already deleted", userId);

                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"Deleted User";
                return result;
            }

            // Update the user's status and set the updated date to the current date and time
            user.Status = (User.UserStatus)status;
            user.UpdatedDate = DateTime.Now;

            // If the new status is "DELETED", perform additional actions
            if (status == (byte)User.UserStatus.DELETED)
            {
                // Append the user's email with the current date and time
                user.Email = $"{user.Email}#{DateTime.Now}";

                // Delete all products added by the user
                await _uow.ProductRepostory.DeleteProductAsync(userId);

                // If the user has a profile picture, delete it
                if (user.ProfilePic != null)
                {
                    _fileUtil.DeleteUserProfilePic(user.ProfilePic);
                    user.ProfilePic = null;
                }

                _logger.LogInformation("User {userId} has been deleted at {deleteTime}. All products added by the user have been deleted, and their profile picture has been removed.", userId, DateTime.Now);
            }

            // Update the user in the database
            _uow.UserRepository.Update(user);

            // Save the changes to the database
            await _uow.SaveAsync();

            // Log the status change
            _logger.LogInformation("User {userId} status changed to {status}", user.UserId, user.Status);

            // Log the status change
            _logger.LogInformation("User {userId} status changed {status}", user.UserId, user.Status);

            // Return a Success ServiceResult object with a success message and the updated user's details in a UserDetailView object
            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "User Status Changed";
            result.Data = new UserDetailView(user);
            return result;
        }

        /// <summary>
        /// Retrieves a paginated list of users with search and filter functionality.
        /// </summary>
        /// <param name="form">An object containing the parameters for pagination, search, and filtering.</param>
        /// <returns>A ServiceResult object containing a PagerView of users that match the specified search and filter criteria.</returns>
        /// <remarks>
        /// <para>The UserListAsync method retrieves a list of users with pagination, search, and filter functionality. It takes a UserPaginationParams object as input, which contains the following parameters:</para>
        /// <list type="bullet">
        /// <item><description>PageNumber: The current page number of the user list.</description></item>
        /// <item><description>PageSize: The number of users to display per page.</description></item>
        /// <item><description>Search: A string to search for in the user's name or email.</description></item>
        /// <item><description>Status: An array of byte values that correspond to user status values. Only users with the specified status values will be returned.</description></item>
        /// <item><description>Role: An array of byte values that correspond to user role values. Only users with the specified role values will be returned.</description></item>
        /// <item><description>SortBy: A string representing the column to sort by. Must be one of the values defined in the UserRepository's ColumnMapForSortBy dictionary.</description></item>
        /// <item><description>SortByDesc: A boolean value indicating whether to sort in descending order.</description></item>
        /// </list>
        /// <para>If any of the parameters are invalid, a ServiceResult object with a BadRequest status and an error message will be returned.</para>
        /// <para>The UserListAsync method then applies the search and filter criteria to the user data retrieved from the database and creates a PagerView of UserView objects to return. Each UserView object contains a subset of user data that is relevant to the user list view.</para>
        /// </remarks>
        public async Task<ServiceResult> UserListAsync(UserPaginationParams form)
        {
            ServiceResult result = new();

            // Validate Status

            byte[]? status = form.Status?.Where(status => status.HasValue).Cast<byte>().ToArray();

            if (status != null && !status.All(status => Enum.IsDefined(typeof(User.UserStatus), status)))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"Invalid Status Value";
                return result;
            }

            // Validate Roles

            byte[]? roles = form.Role?.Where(role => role.HasValue).Cast<byte>().ToArray();

            if (roles != null && !roles.All(status => Enum.IsDefined(typeof(User.UserRole), status)))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Invalid Role Value";
                return result;
            }

            // Check if SortBy value if declared and acceptable
            if (form.SortBy != null && !_uow.UserRepository.ColumnMapForSortBy.ContainsKey(form.SortBy))
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"SortBy : Accepts [{string.Join(", ", _uow.UserRepository.ColumnMapForSortBy.Keys)}] values only";
                return result;
            }

            // Applying Filter and fetching data
            List<User> users = await _uow.UserRepository.FindAllByStatusAndNameOrEmailLikeAsync(
                    status?.Cast<User.UserStatus>().ToArray(),
                    roles?.Cast<User.UserRole>().ToArray(),
                    form.Search,
                    form.SortBy,
                    form.SortByDesc);

            Pager<UserView> pager = new(form.PageNumber, form.PageSize, users.Count);

            pager.SetResult(users.Skip((form.PageNumber - 1) * form.PageSize)
                                 .Take(form.PageSize)
                                 .Select(user => new UserView(user)));

            result.ServiceStatus = ServiceStatus.Success;
            result.Message = "User List";
            result.Data = pager;

            return result;
        }

        /// <summary>
        /// Retrieves a FileStream of the user's stored profile picture with the given file name.
        /// </summary>
        /// <param name="fileName">The file name of the profile picture to retrieve.</param>
        /// <returns>A FileStream object containing the user's profile picture data, or null if the file does not exist.</returns>
        public async Task<FileStream?> GetProfilePic(string fileName)
        {
            // Check if the profile picture file exists in the repository
            if (!await _uow.UserRepository.IsProfilePicExists(fileName))
                return null;

            // Retrieve the FileStream of the user's profile picture using the FileUtil class
            return _fileUtil.GetUserProfile(fileName);
        }

        /// <summary>
        /// Accepts or rejects seller request from user and sends an email to user's email regarding the status.
        /// </summary>
        /// <param name="userId">The id of the user whose seller request is to be accepted or rejected.</param>
        /// <param name="form">The form containing the information regarding whether to accept or reject the seller request and the reason (if rejected).</param>
        /// <returns>Service result containing the user's details view.</returns>
        public async Task<ServiceResult> SellerRequest(int userId, RequestForm form)
        {

            // Log that the method was called with the given parameters
            _logger.LogInformation("SellerRequest method called with userId {UserId} and form {@Form}", userId, form);

            ServiceResult result = new();

            // Find user with the given id
            User? user = await _uow.UserRepository.FindById(userId);

            // Check if user exists and is active
            if (user is null || user.Status != User.UserStatus.ACTIVE)
            {
                _logger.LogWarning("User not found for userId {UserId}", userId);


                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "User not found";
                return result;
            }

            // Check if the user's role is requested
            if (user.Role != User.UserRole.REQUESTED)
            {
                _logger.LogWarning("User role is not requested for userId {UserId}", userId);

                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = $"User role: {user.Role}";
                return result;
            }

            // Update user's role based on whether seller request is approved or not
            user.Role = form.Approved ? User.UserRole.SELLER : User.UserRole.USER;

            // Save the updated user to the database
            user = _uow.UserRepository.Update(user);
            await _uow.SaveAsync();

            // Log the success
            _logger.LogInformation("Seller request processed successfully for userId {UserId}", userId);

            result.ServiceStatus = ServiceStatus.Success;
            result.Message = user.Role == User.UserRole.SELLER ? "Seller request accepted" : "Seller request rejected";
            result.Data = new UserDetailView(user);

            // Send email to user regarding the request
            _emailService.SellerRequest(user.Email, form.Approved, form.Reason);

            return result;
        }

        /// <summary>
        /// Gets the number of products for each seller.
        /// </summary>
        /// <returns>A <see cref="ServiceResult"/> object that contains the product count for each seller.</returns>
        public async Task<ServiceResult> SellerProductCount()
        {
            Dictionary<int, int> productCountDict = await _uow.UserRepository.GetSellerProductCounts();

            return new ServiceResult
            {
                ServiceStatus = ServiceStatus.Success,

                Message = "Seller Product Count",

                Data = productCountDict.Select(kv => new CountView(kv.Key.ToString(), kv.Value))
            };
        }

        /// <summary>
        /// Gets the number of products for the given seller, grouped by product status.
        /// </summary>
        /// <param name="userId">The ID of the seller.</param>
        /// <returns>A <see cref="ServiceResult"/> object that contains the product count for each product status.</returns>
        public async Task<ServiceResult> SellerProductStatusCount(int userId)
        {
            ServiceResult result = new();

            User? user = await _uow.UserRepository.FindById(userId);

            if (user is null || user.Role != User.UserRole.SELLER)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Seller Not Found";
                return result;
            }

            Dictionary<Product.ProductStatus, int> countDict = await _uow.UserRepository.GetSellerProductCountGroupByProductStatus(userId);

            result.ServiceStatus = ServiceStatus.Success;

            result.Message = "Seller Product Status Count";

            result.Data = Enum.GetValues(typeof(Product.ProductStatus)).Cast<Product.ProductStatus>()
                                        .Where(status => status != Product.ProductStatus.DRAFT)
                                        .Select(status => new CountView(status.ToString(), countDict.TryGetValue(status, out int count) ? count : 0));
            return result;
        }
    }
}