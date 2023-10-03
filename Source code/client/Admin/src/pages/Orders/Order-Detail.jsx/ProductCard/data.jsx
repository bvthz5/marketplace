export const getOrderStatuses = (status) => {
    if (status === 0) {
      return "Order Created";
    }
    if (status === 1) {
      return "Order Failed";
    }
    if (status === 2) {
      return "Confirmed";
    }
    if (status === 3) {
      return "Waiting For Pickup";
    }
    if (status === 4) {
      return "In Transit";
    }
    if (status === 5) {
      return "Out For Delivery";
    }
    if (status === 6) {
      return "Cancelled";
    }
    if (status === 7) {
      return "Delivered";
    }
    return "Status Not Found";
  };
  
  export const activeStepper = (orderStatus) => {
    if (orderStatus === 2) {
      return 1;
    }
    if (orderStatus === 3) {
      return 2;
    }
    if (orderStatus === 4) {
      return 3;
    }
    if (orderStatus === 5) {
      return 4;
    }
    if (orderStatus === 7) {
      return 5;
    }
    return 10;
  };
  
  export const stepperValues = [
    { id: 0, value: "Order Confirmed", status: 2 },
    { id: 1, value: "Waiting For Pickup", status: 3 },
    { id: 2, value: "In Transit", status: 4 },
    { id: 3, value: "Out For Delivery", status: 5 },
    { id: 4, value: "Delivered", status: 7 },
  ];
  
  export const sortValues = [
    { key: "Newest to Oldest", value: "Newest to Oldest" },
    { key: "Oldest to Newest", value: "Oldest to Newest" },
    { key: "Price:low to high", value: "Price:low to high" },
    { key: "Price:high to low", value: "Price:high to low" },
  ];
  export const PASSWORD_PATTERN =
    /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?#&]{8,16}$/;
  
  export const productlistParams = {
    Offset: 0,
    PageSize: 24,
    CategoryId: "",
    Search: "",
    Location: "",
    StartPrice: 0,
    EndPrice: 500000,
    SortBy: "CreatedDate",
    SortByDesc: true,
  };
  