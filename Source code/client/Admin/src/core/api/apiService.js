import axious from './axious';

//getCategoyname for graph //
export const categoriesGraph = () => {
  return axious.get('/api/Dashboard/category-product-count');
};
//total sales count for graph//
export const salesCountGraph = ({ From, To }) => {
  return axious.get(`/api/Dashboard/order-count`, {
    params: {
      From,
      To,
    },
  });
};

//My product get product //
export const sellerProductList = ({ UserId, SortByDesc, Offset, pageSize }) => {
  return axious.get(`/api/Product/offset`, {
    params: {
      UserId,
      SortByDesc,
      Offset,
      pageSize,
    },
  });
};

//  Order detail page product list//
export const orderDetailProducts = (id) => {
  return axious.get(`/api/Order/${id}`);
};

//order list//
export const ordersList = ({ pageNumber, pageSize, search, SortBy, PaymentStatus, SortByDesc }) => {
  return axious.get(`/api/Order/page`, {
    params: {
      pageNumber,
      pageSize,
      search,
      SortBy,
      PaymentStatus,
      SortByDesc,
    },
  });
};
//category//
//list categories//
export const allCategories = () => {
  return axious.get('/api/Category');
};
//add new category//
export const addCategories = (e) => {
  return axious.post('/api/Category', JSON.stringify(e));
};
//edit categories//
export const editCategories = (id, e) => {
  return axious.put(`/api/Category/${id}`, JSON.stringify(e));
};
//delete categories//
export const deleteCategories = (id, status) => {
  return axious.put(`/api/Category/status/${id}`, status === 0 ? 1 : 0);
};

//product detail view//
//product image//
export const getProductImage = (id) => {
  return axious.get(`/api/Photos/${id}`);
};
//product details//
export const detailProducts = (id) => {
  return axious.get(`/api/Product/${id}`);
};
//approve /reject producs//
export const approverejectProduct = (productId) => {
  return axious.put(`/api/Product/status/${productId}`, JSON.stringify({ approved: true, reason: '' }));
};
//edit product name//
export const editProductName = (id, productName) => {
  return axious.put(`/api/Product/${id}`, JSON.stringify(productName));
};

//product list with search,sort,filter and pagination//
export const getAllProducts = ({
  Offset,
  PageSize,
  CategoryId,
  Search,
  Location,
  StartPrice,
  EndPrice,
  SortBy,
  Status,
  SortByDesc,
}) => {
  return axious.get(`/api/Product/offset`, {
    params: {
      Offset,
      PageSize,
      CategoryId,
      Search,
      Location,
      StartPrice,
      EndPrice,
      SortBy,
      Status,
      SortByDesc,
    },
  });
};

//Product Requuisition list//
export const allRequest = ({ Status, pageSize, pageNumber }) => {
  return axious.get(`/api/Product/page`, {
    params: {
      Status,
      pageSize,
      pageNumber,
    },
  });
};
//product reject approve reject
export const rejectProductRequest = (id, reason) => {
  return axious.put(`/api/product/status/${id}`, JSON.stringify({ approved: false, reason: reason }));
};

//Change password//
export const changePasswordAdmin = (data) => {
  return axious.put('/api/Admin/change-password', JSON.stringify(data));
};
//seller request list//
export const allSellerRequest = ({ pageNumber, Role, pageSize, Status }) => {
  return axious.get(`/api/User/page`, {
    params: {
      pageNumber,
      Role,
      pageSize,
      Status,
    },
  });
};
//approve seller requests//
export const approveSellerRequest = (userId) => {
  return axious.put(`/api/User/seller-request/${userId}`, JSON.stringify({ approved: true }));
};
//reject seller request//
export const rejectSellerRequest = (userId, reason) => {
  return axious.put(`/api/User/seller-request/${userId}`, JSON.stringify({ approved: false, reason: reason }));
};

//seller detail view//
export const getSellerDetails = (userId) => {
  return axious.get(`/api/User/${userId}`);
};
//seller list with pagination,search,sort,filter//
export const allSellers = ({ pageNumber, Role, pageSize, search, SortBy, Status, SortByDesc }) => {
  return axious.get(`/api/User/page`, {
    params: {
      pageNumber,
      Role,
      pageSize,
      search,
      SortBy,
      Status,
      SortByDesc,
    },
  });
};
//block/delete seller from seller list//
export const blockdeleteSellers = (id, status) => {
  return axious.put(`/api/User/status/${id}`, JSON.stringify(status));
};
//seller-product -status-count //
export const sellerProductCount = (id) => {
  return axious.get(`/api/user/seller-product-status-count/${id}`);
};
//Api for fetch category for filter in topbar//
export const topbarCategories = () => {
  return axious.get('/api/Category');
};

//user detail//
export const allUserDetails = (userId) => {
  return axious.get(`/api/User/${userId}`);
};

//api for list user  with pagination ,search,sort,and filter//
export const allUserList = ({ pageNumber, Roles, pageSize, search, SortBy, Status, SortByDesc }) => {
  return axious.get(`/api/User/page?Role=0&Role=1`, {
    params: {
      pageNumber,
      pageSize,
      search,
      SortBy,
      Status,
      SortByDesc,
    },
  });
};

//api for block and delete user//
export const blockdeleteUseraccess = (id, status) => {
  return axious.put(`/api/User/status/${id}`, JSON.stringify(status));
};

//api for order history//
export const getOrderStatusHistory = (orderDetailsId) => {
  return axious.get(`/api/order/history/${orderDetailsId}`);
};

//Agent//

//add agent api//
export const addAgentsApi = (e) => {
  return axious.post('/api/agent', JSON.stringify(e));
};

//get all agents//
export const allAgentsApi = ({ PageNumber, PageSize, search, SortBy, Status, sortValue, SortByDesc }) => {
  return axious.get(`/api/agent`, {
    params: {
      PageNumber,
      PageSize,
      search,
      SortBy,
      Status,
      sortValue,
      SortByDesc,
    },
  });
};

//edit Agents//
export const editAgents = (id, e) => {
  return axious.put(`/api/agent/${id}`, JSON.stringify(e));
};
//delete agents//
export const deleteAgentsApi = (id, status) => {
  return axious.put(`/api/agent/status/${id}`, JSON.stringify(status));
};

// agent order management//

export const agentGetOrders = ({ PageNumber, MyProductsOnly, PageSize, Search, SortBy, SortByDesc }) => {
  return axious.get('/api/agent-order/page', {
    params: {
      PageNumber,
      PageSize,
      Search,
      SortBy,
      MyProductsOnly,
      SortByDesc,
    },
  });
};

// agent order assign//
export const assignOrder = (id, e) => {
  return axious.put(`/api/agent-order/assign/${id}`, JSON.stringify(e));
};

// agent order unassign//
export const unAssignOrder = (id, e) => {
  return axious.put(`/api/agent-order/unassign/${id}`, JSON.stringify(e));
};
// agent Detail View
export const getAgentDetailOrder = (id) => {
  return axious.get(`/api/agent-order/${id}`);
};
// Agent Profile

export const getAgentProfile = () => {
  return axious.get('/api/agent/profile');
};

export const editAgentProfile = (data) => {
  return axious.put('/api/agent/profile', data);
};

export const updateProfilePic = (image) => {
  return axious.put(`/api/agent/profile-pic`, image, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });
};

export const changeDeliveryStatus = (id, status) => {
  return axious.put(`/api/agent-order/status/${id}`, status);
};

export const generateOtp = (orderId) => {
  return axious.put(`/api/agent-order/generate-otp/${orderId}`);
};

export const verifyOtp = (orderId, otp) => {
  return axious.put(`/api/agent-order/verify-otp/${orderId}`, `"${otp}"`);
};

// agent order count//
export const agentOrdersCount = () => {
  return axious.get('/api/agent-order/order-status-count');
};
