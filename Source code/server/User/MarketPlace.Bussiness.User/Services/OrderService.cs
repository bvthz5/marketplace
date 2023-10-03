using iTextSharp.text;
using iTextSharp.text.pdf;
using MarketPlace.DataAccess.Interfaces;
using MarketPlace.DataAccess.Model;
using MarketPlaceUser.Bussiness.Dto.Forms;
using MarketPlaceUser.Bussiness.Dto.Views;
using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using MarketPlaceUser.Bussiness.Interfaces;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MarketPlaceUser.Bussiness.Services
{

    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow; // Unit of work for accessing repositories
        private readonly ILogger<OrderService> _logger; // Logger for logging errors
        private readonly IProductService _productService; // Service for managing products
        private readonly IOrderDetailsService _orderDetailsService; // Service for managing order details
        private readonly IEmailService _emailService; // Service for sending emails
        private readonly IRazorpayService _razorpayService; // Service for managing razorpay

        public OrderService(IUnitOfWork uow, ILogger<OrderService> logger, IProductService productService, IOrderDetailsService orderDetailsService, IEmailService emailService, IRazorpayService razorpayService)
        {
            _uow = uow;
            _logger = logger;
            _productService = productService;
            _orderDetailsService = orderDetailsService;
            _emailService = emailService;
            _razorpayService = razorpayService;
        }

        /// <summary>
        /// Creates an order for a user by initializing the payment with Razorpay API and saving the order details in the database.
        /// </summary>
        /// <param name="userId">The user ID for whom the order is being created.</param>
        /// <param name="deliveryAddressId">The ID of the delivery address.</param>
        /// <param name="productIds">An array of product IDs that the user wants to order.</param>
        /// <returns>A <see cref="ServiceResult"/> object with the result of the operation.</returns>
        public async Task<ServiceResult> AddOrderAsync(int userId, int deliveryAddressId, int[] productIds)
        {
            ServiceResult result = new();

            // Find the delivery address for the user
            DeliveryAddress? address = await _uow.DeliveryAddressRepository.FindByUserIdAndAddressIdAsync(userId, deliveryAddressId);
            if (address == null)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Address not found";
                return result;
            }

            string deliveryAddress = $"{address.Name}{'\b'}{address.Address}{'\b'}{address.StreetAddress}{'\b'}{address.City}{'\b'}{address.ZipCode}{'\b'}{address.State}{'\b'}{address.Phone}";
            // Activate the products
            await ActivateProducts(productIds, userId);

            // Get the list of products the user wants to order
            List<Product> productList = (await _uow.ProductRepostory.FindByProductIdsAndStatusAsync(productIds, Product.ProductStatus.ACTIVE))
                                            .Where(product => product.CreatedUser.Status == User.UserStatus.ACTIVE).ToList();
            if (productList == null || productList.Count == 0)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "No products found";
                return result;
            }

            // Calculate the total amount for the order
            double amount = productList.Sum(product => product.Price);

            // Check if the total amount exceeds the limit
            if (amount > 500000)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Amount limit of 500000 exceeded";
                return result;
            }

            // Change the status of the products to ONPROCESS
            foreach (var product in productList)
            {
                await _productService.ChangeStatusAsync(product, Product.ProductStatus.ONPROCESS);
            }
            await _uow.SaveAsync();

            // Initialize the payment with Razorpay API

            if (!_razorpayService.IsOrderCreated(amount, out string? orderId) || orderId is null)
            {
                // If the payment fails, change the status of the products to ACTIVE and return an error
                foreach (var product in productList)
                {
                    await _productService.ChangeStatusAsync(product, Product.ProductStatus.ACTIVE);
                }
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Payment Failed";
                return result;
            }

            // Create a new order object
            Orders orders = new()
            {
                UserId = userId,
                OrderNumber = orderId,
                OrderDate = DateTime.Now,
                DeliveryAddress = deliveryAddress,
                PaymentStatus = Orders.PaymentsStatus.UNPPAID,
                TotalPrice = amount,
                OrderStatus = Orders.OrdersStatus.CREATED,
                PaymentDate = DateTime.Now,
            };

            orders = await _uow.OrderRepository.Add(orders);

            await _uow.SaveAsync();

            foreach (var product in productList)
            {
                await _orderDetailsService.AddOrderDetailsAsync(product.ProductId, orders.OrdersId);
            }

            //Payment confirmation Job is called to check the order after 13 minuit and verify whether order is paid or unapaid
            _orderDetailsService.PaymentConfirmationJob(orderId, userId);
            result.Data = new InitialOrderView(orders);
            return result;
        }

        /// <summary>
        /// Activates products for a given user and product ids.
        /// </summary>
        /// <param name="productIds">The array of product ids to activate.</param>
        /// <param name="userId">The id of the user for whom the products are being activated.</param>
        public async Task ActivateProducts(int[] productIds, int userId)
        {
            List<OrderDetails> orderDetailslist = new();

            // Find all the order details for the given user and product ids
            foreach (var id in productIds)
            {
                orderDetailslist = await _uow.OrderDetailsRepository.FindByUserIdAndProductIdAndOrderStatus(userId, id);
            }

            // Activate each product
            foreach (var product in orderDetailslist)
            {
                await _productService.ChangeStatusAsync(product.Product, Product.ProductStatus.ACTIVE);
            }
        }

        /// <summary>
        /// Confirms payment and updates order status to CONFIRMED
        /// </summary>
        /// <param name="confirmPaymentForm">Confirmation form data</param>
        /// <param name="userId">Buyer user ID</param>
        /// <returns>ServiceResult object</returns>
        public async Task<ServiceResult> ConfirmPayment(ConfirmPaymentForm confirmPaymentForm, int userId)
        {
            ServiceResult result = new();

            // Get list of order details for the buyer and status CREATED
            List<OrderDetails> orderDetailsList = await _uow.OrderDetailsRepository.FindByBuyerIdAndStatus(userId, OrderHistory.HistoryStatus.CREATED);

            // Get order details for the specified Razorpay order ID
            var order = await _uow.OrderRepository.FindByOrderNumberAsync(confirmPaymentForm.RazorpayOrderId);

            // If order not found or payment failed, change product status to ACTIVE and return BadRequest response
            if (order is null || order.OrderStatus == Orders.OrdersStatus.FAILED)
            {
                foreach (var item in orderDetailsList)
                {
                    await _productService.ChangeStatusAsync(item.Product, Product.ProductStatus.ACTIVE);
                }
                result.Message = "Payment Failed";
                result.ServiceStatus = ServiceStatus.BadRequest;
                return result;
            }

            if (_razorpayService.IsPaymentConfirmed(confirmPaymentForm))
            {
                await ConfirmOrder(userId, 1, order.OrdersId, orderDetailsList, confirmPaymentForm.RazorpayPaymentId);
                result.Message = "Payment Successful";
                return result;
            }
            else
            {
                foreach (var item in orderDetailsList)
                {
                    await _productService.ChangeStatusAsync(item.Product, Product.ProductStatus.ACTIVE);
                }
                result.Message = "Payment Failed";
                result.ServiceStatus = ServiceStatus.BadRequest;
                return result;
            }

        }

        /// <summary>
        /// Confirms an order and updates the order status, payment status, and product status accordingly. 
        /// Sends emails to the product owner and buyer to notify them of the sale and purchase. Deletes the 
        /// user's cart items and saves changes to the database.
        /// </summary>
        /// <param name="userId">The ID of the user confirming the order.</param>
        /// <param name="status">The status of the order.</param>
        /// <param name="orderId">The ID of the order to confirm.</param>
        /// <param name="orderDetails">The list of order details associated with the order to confirm.</param>
        /// <returns>A task representing the asynchronous confirmation of the order.</returns>
        public async Task ConfirmOrder(int userId, byte status, int orderId, List<OrderDetails> orderDetails, string paymentId)
        {
            if (status == 1)
            {
                // Find the order by its ID
                Orders? order = await _uow.OrderRepository.FindByOrderIdAsync(orderId);

                _orderDetailsService.SendNotificationJob(orderId);

                if (order != null)
                {
                    // Update the order status and payment status
                    order.OrderNumber = paymentId;
                    order.OrderStatus = Orders.OrdersStatus.CONFIRMED;
                    order.PaymentStatus = Orders.PaymentsStatus.PAID;

                    _uow.OrderRepository.Update(order);
                }

                // Select the order details associated with confirmed orders
                orderDetails = orderDetails.Where(order => order.Order.OrderStatus == Orders.OrdersStatus.CONFIRMED).ToList();

                // For each confirmed order detail, send an email to the product owner, update the order detail status, 
                // and update the product status to sold

                foreach (var item in orderDetails)
                {
                    _emailService.ProductSold(item.Product.CreatedUser.Email, item.Product.ProductName, item.Product.CreatedUser.FirstName);

                    await _orderDetailsService.ChangeStatus(item);

                    await _productService.ChangeStatusAsync(item.Product, Product.ProductStatus.SOLD);

                    await _uow.CartRepository.DeleteByProductIdAndUserIdAsync(item.Product.ProductId, userId);
                }

                // Find the buyer by their ID and send an email notifying them of the purchase
                User? user = await _uow.UserRepository.FindById(userId);

                if (user != null)
                {
                    _emailService.ProductBought(user.Email, user.FirstName, orderDetails);
                }

                await _uow.SaveAsync();
            }
        }


        /// <summary>
        /// Retrieves all orders for a user by their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a <see cref="ServiceResult"/> with the list of orders.</returns>
        public async Task<ServiceResult> GetOrders(int userId)
        {
            ServiceResult result = new();

            // Find all orders for the specified user.
            var orderList = await _uow.OrderRepository.FindByUserIdAsync(userId);

            // If there are no orders for the user, return an error message.
            if (orderList.Count == 0)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Orders not found";
                return result;
            }

            // Convert each order in the list to an InitialOrderView and add it to the result.
            result.Data = orderList.ConvertAll(order => new InitialOrderView(order));
            return result;
        }



        /// <summary>
        /// Cancels an order with the specified order number.
        /// </summary>
        /// <param name="orderNumber">The order number to cancel.</param>
        /// <returns>A <see cref="ServiceResult"/>  indicating the success or failure of the operation.</returns>
        public async Task<ServiceResult> CancelOrder(string orderNumber)
        {
            ServiceResult result = new();

            // Find the order by the order number.
            Orders? order = await _uow.OrderRepository.FindByOrderNumberAsync(orderNumber);

            // If the order was not found or has already failed, return a NotFound status.
            if (order == null || order.OrderStatus == Orders.OrdersStatus.FAILED)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Order not found";

                return result;
            }

            // If the order has already been confirmed, return a BadRequest status.
            if (order.OrderStatus == Orders.OrdersStatus.CONFIRMED)
            {
                result.ServiceStatus = ServiceStatus.BadRequest;
                result.Message = "Confirmed order can't be cancelled";

                return result;
            }

            // Find all order details for the order.
            List<OrderDetails> orderList = await _uow.OrderDetailsRepository.FindByOrderId(order.OrdersId);

            // Set the status of all products in the order back to ACTIVE.
            foreach (var product in orderList)
            {
                await _productService.ChangeStatusAsync(product.Product, Product.ProductStatus.ACTIVE);
            }

            // Set the order status to FAILED and update the order.
            order.OrderStatus = Orders.OrdersStatus.FAILED;
            _uow.OrderRepository.Update(order);

            // Save the changes to the database.
            await _uow.SaveAsync();

            _logger.LogInformation("Order {orderId} Cancelled", order.OrdersId);

            // Set the result message to indicate success.
            result.Message = "Order cancelled successfully";

            return result;
        }

        public async Task<object> DownloadInvoice(int orderDetailsId, int userId)
        {
            OrderDetails? orderDetails = await _uow.OrderDetailsRepository.FindByOrderDetailsId(orderDetailsId);

            return orderDetails is null || orderDetails.Order.UserId != userId || orderDetails.Histories.Last().Status == OrderHistory.HistoryStatus.CREATED || orderDetails.Order.OrderStatus == Orders.OrdersStatus.CANCELLED
                ? new ServiceResult()
                {
                    ServiceStatus = ServiceStatus.NotFound,
                    Message = "Order not found"
                }
                : PdfCreator(orderDetails);
        }

        public async Task<ServiceResult> EmailInvoice(int orderDetailsId, int userId)
        {
            ServiceResult result = new();
            OrderDetails? orderDetails = await _uow.OrderDetailsRepository.FindByOrderDetailsId(orderDetailsId);

            if (orderDetails is null || orderDetails.Order.UserId != userId || orderDetails.Histories.Last().Status == OrderHistory.HistoryStatus.CREATED || orderDetails.Order.OrderStatus == Orders.OrdersStatus.CANCELLED)
            {
                return new ServiceResult()
                {
                    ServiceStatus = ServiceStatus.NotFound,
                    Message = "Order not found"
                };
            }

            MemoryStream memStream = PdfCreator(orderDetails);
            // create a new MemoryStream object and copy the data from the original one
            MemoryStream newStream = new(memStream.GetBuffer(), false);

            // reset the position of the new MemoryStream to the beginning
            newStream.Seek(0, SeekOrigin.Begin);

            MimePart pdfAttachment = new("application", "pdf")
            {
                Content = new MimeContent(newStream),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = $"invoice_{orderDetails.Order.OrderNumber}_{orderDetails.OrderDetailsId}.pdf"
            };

            string subject = $"Order Invoice : {orderDetails.Order.OrderNumber}_{orderDetails.OrderDetailsId}";

            _emailService.SendEmailInvoice(orderDetails.Order.User.Email, subject, pdfAttachment);

            result.Message = "Success";
            result.ServiceStatus = ServiceStatus.Success;
            return result;

        }

        public static MemoryStream PdfCreator(OrderDetails orderDetails)
        {
            //Create a new document
            Document document = new();

            //Create a new MemoryStream object
            MemoryStream memStream = new();

            //Create a new PDFWriter object
            _ = PdfWriter.GetInstance(document, memStream);

            //Open the document
            document.Open();

            document.NewPage();

            Paragraph heading = new("Order Invoice\n\n", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD))
            {
                Alignment = Element.TITLE
            };

            //Add a title to the document
            document.Add(heading);

            PdfPTable table = new(3);
            table.DefaultCell.BorderWidth = 0;
            table.WidthPercentage = 100;
            table.AddCell(new Phrase("Sold By", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
            table.AddCell(new Phrase("Shipping Address", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
            table.AddCell(new Phrase("Billing Address", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));

            PdfPCell cell = new(new Phrase(orderDetails.Product.CreatedUser.FirstName + " " + orderDetails.Product.CreatedUser.LastName + "\n" + orderDetails.Product.CreatedUser.Email + "\n"))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                BorderWidth = 0
            };

            table.AddCell(orderDetails.Product.CreatedUser.FirstName + " " + orderDetails.Product.CreatedUser.LastName + "\n" + orderDetails.Product.CreatedUser.Email + "\n");
            cell = new(new Phrase(orderDetails.Order.DeliveryAddress.Replace('\b', '\n')))
            {
                HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                BorderWidth = 0
            };
            table.AddCell(cell);
            table.AddCell(cell);

            document.Add(table);

            document.Add(new Phrase("\n"));

            //Create a new table object
            table = new(4)
            {
                WidthPercentage = 100
            }; //4 columns

            //Add data to the table
            table.AddCell("Product");
            table.AddCell("Qty");
            table.AddCell("Amount");
            table.AddCell("Total");


            table.AddCell(orderDetails.Product.ProductName);
            table.AddCell("1");
            table.AddCell(orderDetails.Product.Price.ToString());
            table.AddCell(orderDetails.Product.Price.ToString());


            document.Add(table);
            table = new(2)
            {
                WidthPercentage = 100
            };
            Paragraph totalString = new("Total Amount", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD));

            cell = new(new Phrase(orderDetails.Product.Price.ToString(), new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BorderWidthLeft = 0
            };

            table.AddCell(totalString);
            table.AddCell(cell);

            //Add the table to the document
            document.Add(table);

            document.Add(new Paragraph(new Phrase("*All values are in INR", new Font(Font.FontFamily.HELVETICA, 8)))
            {
                Alignment = Element.ALIGN_RIGHT,
            });

            //Close the document
            document.Close();

            return memStream;
        }

        public async Task<ServiceResult> GetOrderHistory(int orderDetailsId, int userId)
        {
            ServiceResult result = new();

            OrderDetails? orderDetails = await _uow.OrderDetailsRepository.FindByOrderDetailsIdAndUserId(orderDetailsId, userId);

            if (orderDetails is null || orderDetails.Order.OrderStatus == Orders.OrdersStatus.CREATED)
            {
                result.ServiceStatus = ServiceStatus.NotFound;
                result.Message = "Order Not Found";
                return result;
            }

            result.Data = orderDetails.Histories.Select(orderHistory => new OrderHistoryView(orderHistory));
            result.Message = "Order History";

            return result;
        }
    }
}