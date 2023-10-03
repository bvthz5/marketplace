using MarketPlace.DataAccess.Data;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Hubs;
using MarketPlaceUser.Bussiness.Interfaces;
using MarketPlaceUser.Bussiness.Settings;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Razorpay.Api;

namespace MarketPlaceUser.Bussiness.Services
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly PresenceTracker _tracker;
        private readonly IUnitOfWork _uow;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<OrderDetailsService> _logger;
        private readonly RazorpayClient _razorpayClient;

        public OrderDetailsService(PresenceTracker tracker, IUnitOfWork uow, IServiceScopeFactory serviceScopeFactory, IOptions<RazorPaySettings> razorOptions, IHubContext<NotificationHub> hubContext, ILogger<OrderDetailsService> logger)
        {
            _tracker = tracker;
            _uow = uow;
            _serviceScopeFactory = serviceScopeFactory;
            _hubContext = hubContext;
            _logger = logger;
            _razorpayClient = new RazorpayClient(razorOptions.Value.Key, razorOptions.Value.Secret);
        }

        /// <summary>
        /// Adds a new order detail to the database.
        /// </summary>
        /// <param name="productId">The ID of the product to add to the order details.</param>
        /// <param name="orderId">The ID of the order to add the product to.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddOrderDetailsAsync(int productId, int orderId)
        {
            // Create a new OrderDetails object with the specified product ID, order ID,
            // and creation and update dates set to the current date and time.
            OrderDetails orderDetails = new()
            {
                ProductId = productId,
                OrderId = orderId,
                CreatedDate = DateTime.Now,
                Histories = new List<OrderHistory> { new OrderHistory() { Date = DateTime.Now, Status = OrderHistory.HistoryStatus.CREATED } },
                UpdatedDate = DateTime.Now,
            };

            // Add the new order details to the OrderDetails repository and save the changes to the database.
            await _uow.OrderDetailsRepository.Add(orderDetails);
            await _uow.SaveAsync();
        }


        /// <summary>
        /// Changes the status of an order detail to CONFIRMED and saves changes to the database.
        /// </summary>
        /// <param name="order">The order detail to be updated.</param>
        public async Task ChangeStatus(OrderDetails order)
        {
            // Set the status of the order detail to CONFIRMED.
            order.Histories.Add(new OrderHistory() { Date = DateTime.Now, Status = OrderHistory.HistoryStatus.CONFIRMED });

            // Update the order detail in the repository.
            _uow.OrderDetailsRepository.Update(order);

            // Save changes to the database.
            await _uow.SaveAsync();
        }


        /// <summary>
        /// Retrieves all orders associated with a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve orders for.</param>
        /// <returns>A <see cref="ServiceResult"/> object containing the list of orders, or an error message if no orders were found.</returns>
        public async Task<ServiceResult> GetAllOrders(int userId)
        {
            ServiceResult result = new();

            // Retrieve all order details for the specified user
            var orderDetailList = await _uow.OrderDetailsRepository.GetAll(userId);

            // If no orders were found, return a not found status with an error message
            if (orderDetailList.Count == 0)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "No orders found";

                return result;
            }

            // Filter the order details list to only include orders with a paid payment status,
            // and convert each order into an OrderDetailsView object for serialization
            result.Data = orderDetailList.Where(orderDetail => orderDetail.Order.PaymentStatus != Orders.PaymentsStatus.UNPPAID)
                                         .Select(order => new OrderDetailsView(order, _uow.PhotoRepository.FindThumbnailPicture(order.ProductId)?.Photo));

            return result;
        }

        public async Task<ServiceResult> GetOrderDetailsById(int userId, int orderDetailsId)
        {
            ServiceResult result = new();

            OrderDetails? orderDetail = await _uow.OrderDetailsRepository.FindByOrderDetailsIdAndUserId(orderDetailsId, userId);

            if (orderDetail == null || orderDetail.Histories.Last().Status == OrderHistory.HistoryStatus.CREATED)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "No orders found";

                return result;
            }

            result.Data = new OrderDetailsView(orderDetail, _uow.PhotoRepository.FindThumbnailPicture(orderDetail.ProductId)?.Photo);

            return result;
        }


        /// <summary>
        /// A background job that checks the status of a payment confirmation after 13 minutes and updates the order and product statuses accordingly.
        /// </summary>
        /// <param name="orderNumber">The order number to check the payment status for.</param>
        /// <param name="userId">The ID of the user who placed the order.</param>
        public async void PaymentConfirmationJob(string orderNumber, int userId)
        {
            // Wait for 13 minutes before checking the payment status
            await Task.Delay(1000 * 60 * 13);

            // Create a new scope for the dependency injection container
            using var scope = _serviceScopeFactory.CreateScope();

            // Get an instance of the DbContext for the database
            var dbcontxt = scope.ServiceProvider.GetRequiredService<MarketPlaceDbContext>();

            // Create a new UnitOfWork instance using the DbContext
            IUnitOfWork unit = new UnitOfWork(dbcontxt);

            // Fetch the order details from Razorpay using the order number
            var order = _razorpayClient.Order.Fetch(orderNumber);

            // Check the status of the order
            if (order["status"].ToString() == "created")
            {
                // If the order status is "created", update the product status to "ACTIVE"
                IEnumerable<OrderDetails> orderDetailsList = (await unit.OrderDetailsRepository.FindByBuyerIdAndStatus(userId, OrderHistory.HistoryStatus.CREATED)).Where(item => item.Order.OrderNumber == orderNumber);

                foreach (var product in orderDetailsList.Select(item => item.Product))
                {
                    product.Status = Product.ProductStatus.ACTIVE;
                    product.UpdatedDate = DateTime.Now;

                    dbcontxt.Products.Update(product);

                    await unit.SaveAsync();
                }

                Orders? tempOrder = await unit.OrderRepository.FindByOrderNumberAsync(orderNumber);

                if (tempOrder != null)
                {
                    // Update the order status to "FAILED"
                    tempOrder.OrderStatus = Orders.OrdersStatus.FAILED;

                    dbcontxt.Orders.Update(tempOrder);

                    await unit.SaveAsync();
                }
            }
            if (order["status"].ToString() == "paid")
            {
                // If the order status is "paid", update the product status to "SOLD"
                IEnumerable<OrderDetails> orderDetailsList = (await unit.OrderDetailsRepository.FindByBuyerIdAndStatus(userId, OrderHistory.HistoryStatus.CREATED)).Where(item => item.Order.OrderNumber == orderNumber);

                foreach (var product in orderDetailsList.Select(item => item.Product))
                {
                    product.Status = Product.ProductStatus.SOLD;
                    product.UpdatedDate = DateTime.Now;

                    dbcontxt.Products.Update(product);

                    await unit.SaveAsync();
                }

                Orders? tempOrder = await unit.OrderRepository.FindByOrderNumberAsync(orderNumber);

                if (tempOrder != null)
                {
                    // Update the order status to "CONFIRMED"
                    tempOrder.OrderStatus = Orders.OrdersStatus.CONFIRMED;

                    dbcontxt.Orders.Update(tempOrder);

                    await unit.SaveAsync();
                }
            }
        }

        /// <summary>
        /// Sends a notification for a given order ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to send notifications for.</param>
        public async void SendNotificationJob(int orderId)
        {
            // Log that the method has started and the order ID being processed
            _logger.LogInformation("Starting SendNotificationJob method for order ID {orderId}", orderId);

            // Create a new scope for the dependency injection container
            using var scope = _serviceScopeFactory.CreateScope();

            // Get an instance of the DbContext for the database
            var dbcontxt = scope.ServiceProvider.GetRequiredService<MarketPlaceDbContext>();

            // Create a new UnitOfWork instance using the DbContext
            IUnitOfWork unit = new UnitOfWork(dbcontxt);

            // Find all order details for the given order ID
            List<OrderDetails> orders = await unit.OrderDetailsRepository.FindByOrderId(orderId);

            if (orders != null)
            {
                // Create a list of notifications
                List<Notification> notifications = new();

                // For each order detail, create a notification for the product creator and each user who wishlisted the product
                foreach (var item in orders)
                {
                    // Create a notification for the product creator
                    notifications.Add(new Notification()
                    {
                        UserId = item.Product.CreatedUserId,
                        NotificationType = Notification.NotificationTypes.MY_PRODUCT_SOLD,
                        Status = Notification.NotificationStatus.UNREAD,
                        Data = item.Product.ProductName,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                    });

                    // Find all users who wishlisted the product and create a notification for each one
                    List<int> wisListedUsersIds = await unit.WishListRepository.FindUserIdsByProductIdAndNotUserId(item.ProductId, item.Order.UserId);

                    wisListedUsersIds.ForEach(userId =>
                    {
                        notifications.Add(new Notification()
                        {
                            UserId = userId,
                            NotificationType = Notification.NotificationTypes.WISHLIST_PRODUCT_SOLD,
                            Status = Notification.NotificationStatus.UNREAD,
                            Data = item.Product.ProductName,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        });
                    });
                }

                // Add all notifications to the database
                await unit.NotificationRepository.Add(notifications);

                // Save changes to the database
                await unit.SaveAsync();

                // Dispose of the scope
                scope.Dispose();

                // Get a list of user IDs for each notification
                List<string> userIds = notifications.Select(notification => notification.UserId.ToString()).Distinct().ToList();

                // Log the number of notifications created and the list of user IDs that they were sent to
                _logger.LogInformation("Created {count} notifications and sending to users {userIds}", notifications.Count, string.Join(",", userIds));

                // Send a notification to all online users who are subscribed to the "Notification" channel
                await _hubContext.Clients.Users((await _tracker.GetOnlineUsers()).Intersect(userIds).ToList()).SendAsync("Notification");
            }
            else
            {
                // Log that there were no orders found for the given order ID
                _logger.LogInformation("No orders found for order ID {orderId}", orderId);
            }

            // Log that the method has finished
            _logger.LogInformation("Finished SendNotificationJob method for order ID {orderId}", orderId);

        }
    }
}