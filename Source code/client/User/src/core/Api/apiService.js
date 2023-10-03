import { axiosInstance } from "./interceptor";

//------------------------------------------------------------------------------------------------------------------------------------
// produclist
export const productList = ({
  Offset,
  PageSize,
  CategoryId,
  Search,
  Location,
  StartPrice,
  EndPrice,
  SortBy,
  SortByDesc,
}) => {
  return axiosInstance.get(`/api/Product/page`, {
    params: {
      Offset,
      PageSize,
      CategoryId,
      Search,
      Location,
      StartPrice,
      EndPrice,
      SortBy,
      SortByDesc,
    },
  });
};

// get productimages
export const getProductImages = (productId) => {
  return axiosInstance.get(`/api/Photos/${productId}`);
};
// get productdetails
export const getProductDetails = (productId) => {
  return axiosInstance.get(`/api/Product/${productId}`);
};

export const getAllCategories = () => {
  return axiosInstance.get(`/api/Category`);
};
//------------------------------------------------------------------------------------------------------------------------------------

//------------------------------------------------------------------------------------------------------------------------------------
// wishlist apis
export const getAllWishlist = () => {
  return axiosInstance.get(`/api/WishList`);
};

export const addToWishlist = (productId) => {
  return axiosInstance.post(`/api/WishList`, productId, {
    headers: {
      "Content-type": "application/json",
    },
  });
};

export const removeFromWishlist = (productId) => {
  return axiosInstance.delete(`/api/WishList/${productId}`);
};
//------------------------------------------------------------------------------------------------------------------------------------

//------------------------------------------------------------------------------------------------------------------------------------

// cart apis
export const getAllCart = () => {
  return axiosInstance.get(`/api/Cart`);
};
export const addToCart = (productId) => {
  return axiosInstance.post(`/api/Cart`, productId);
};
export const removeFromCart = (productId) => {
  return axiosInstance.delete(`/api/Cart/${productId}`);
};
//------------------------------------------------------------------------------------------------------------------------------------

//------------------------------------------------------------------------------------------------------------------------------------
// userProfile apis

export const getCurrentUser = () => {
  return axiosInstance.get(`/api/User`);
};
export const updateProfilePic = (image) => {
  return axiosInstance.put(`/api/User/profile`, image, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};

export const updateUserDetails = (userData) => {
  return axiosInstance.put(`/api/User`, JSON.stringify(userData));
};

export const changePassword = (data) => {
  return axiosInstance.put(`/api/User/change-password`, JSON.stringify(data));
};
//------------------------------------------------------------------------------------------------------------------------------------

//------------------------------------------------------------------------------------------------------------------------------------
// seller apis
export const requestAsSeller = () => {
  return axiosInstance.put(`/api/User/requset-to-seller`);
};

// get all curent seller products
export const getAllMyProducts = (id) => {
  return axiosInstance.get(`/api/Product/by-user/${id}`);
};

// delete product by productId
export const deleteProduct = (productId) => {
  return axiosInstance.put(`/api/product/delete/${productId}`);
};
//------------------------------------------------------------------------------------------------------------------------------------

//------------------------------------------------------------------------------------------------------------------------------------

// product apis
export const getProductById = (productId) => {
  return axiosInstance.get(`/api/Product/${productId}`);
};

export const addProductDetails = (productDetails) => {
  return axiosInstance.post(`/api/Product`, JSON.stringify(productDetails));
};

export const editProductDetails = (productId, productDetails) => {
  return axiosInstance.put(
    `/api/Product/${productId}`,
    JSON.stringify(productDetails)
  );
};

export const getProductImagesById = (productId) => {
  return axiosInstance.get(`/api/Photos/${productId}`);
};

export const addProductImage = (productId, images) => {
  return axiosInstance.post(`/api/Photos/${productId}`, images, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};

export const deleteProductImageById = (productId) => {
  return axiosInstance.delete(`/api/Photos/by-photo/${productId}`);
};
//------------------------------------------------------------------------------------------------------------------------------------

//------------------------------------------------------------------------------------------------------------------------------------
// address Apis
export const getAllDeliveryAddresses = () => {
  return axiosInstance.get(`/api/delivery-address`);
};

export const getDeliveryAddressById = (addressId) => {
  return axiosInstance.get(`/api/delivery-address/${addressId}`);
};

export const addDeliveryAddress = (address) => {
  return axiosInstance.post(`/api/delivery-address`, address);
};

export const editDeliveryAddress = (addressId, address) => {
  return axiosInstance.put(
    `/api/delivery-address/edit?deliveryAddressId=${addressId}`,
    address
  );
};

export const setDefaultDeliveryAddress = (addressId) => {
  return axiosInstance.put(
    `/api/delivery-address?deliveryAddressId=${addressId}`
  );
};

export const deleteDeliveryAddress = (productId) => {
  return axiosInstance.put(`/api/delivery-address/delete/${productId}`);
};
//------------------------------------------------------------------------------------------------------------------------------------

//------------------------------------------------------------------------------------------------------------------------------------
// Order Apis

export const getAllOrders = () => {
  return axiosInstance.get(`/api/Order/list`);
};

export const createOrder = (orderData) => {
  return axiosInstance.post(`/api/Order`, JSON.stringify(orderData));
};

export const cancelOrderByOrderNumber = (orderNumber) => {
  return axiosInstance.put(`/api/Order/cancel?orderNumber=${orderNumber}`);
};

export const cancelOrderByOrderId = (orderDetailsId, reason) => {
  return axiosInstance.post(`/api/Order/refund/${orderDetailsId}`, {
    reason: reason,
  });
};

export const getOrderDetailsById = (orderDetailsId) => {
  return axiosInstance.get(`/api/order/${orderDetailsId}`);
};

export const getOrderStatusHistory = (orderDetailsId) => {
  return axiosInstance.get(`/api/order/history/${orderDetailsId}`);
};

export const downloadInvoice = async (orderId) => {
  return await axiosInstance({
    url: `/api/order/download-invoice/${orderId}`,
    method: "GET",
    responseType: "blob",
  });
};
export const emailInvoice = (orderId) => {
  return axiosInstance.get(`/api/order/email-invoice/${orderId}`);
};

// payment verification
export const paymentConfirmation = (response) => {
  return axiosInstance.put(`/api/Order`, {
    razorpayPaymentId: response.razorpay_payment_id,
    razorpayOrderId: response.razorpay_order_id,
    razorpaySignature: response.razorpay_signature,
  });
};

//------------------------------------------------------------------------------------------------------------------------------------

//------------------------------------------------------------------------------------------------------------------------------------
// Notification Apis

export const getAllNotifications = () => {
  return axiosInstance.get(`/api/notification`);
};

export const getNotificationCount = () => {
  return axiosInstance.get(`/api/notification/count`);
};

export const markAllAsRead = () => {
  return axiosInstance.put(`/api/notification`);
};
export const clearAllNotifications = () => {
  return axiosInstance.delete(`/api/notification`);
};
export const clearNotificationsById = (notificationId) => {
  return axiosInstance.delete(`/api/notification/${notificationId}`);
};
