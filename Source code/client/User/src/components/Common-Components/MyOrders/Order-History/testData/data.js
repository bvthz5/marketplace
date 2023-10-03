export const orderHistoryData = {
  data: [
    {
      orderDetailsId: 1,
      createdDate: "2023-03-08T11:34:17.832095",
      orderStatus: 1,
      productView: {
        productId: 79,
        productName:
          "LG C2 210 cm (83 Inches) Evo Gallery Edition 4K Ultra HD Smart OLED TV OLED83C2PSA (Black) (2022 Model) | With Eye Comfort Display",
        categoryId: 3,
        categoryName: "Television",
        thumbnail: "79_33d4c342-e228-4fbb-bb42-584afce54077.jpg",
        address: "Aluva, Kerala",
        price: 101,
        createdDate: "2023-02-23T17:29:36.3558987",
        status: 3,
      },

      orderId: 1,
      orderNumber: "order_LOvNgsWjszmfs4",
      email: "stejin.jacob@innovaturelabs.com",
      userId: 1,
      totalPrice: 459990,
      paymentStatus: 1,
    },
  ],
  message: null,
  serviceStatus: 200,
  status: true,
};

export const orderHistoryDataWithoutImage = {
  data: [
    {
      orderDetailsId: 1,
      createdDate: "2023-03-08T11:34:17.832095",
      orderStatus: 7,
      productView: {
        productId: 79,
        productName:
          "LG C2 210 cm (83 Inches) Evo Gallery Edition 4K Ultra HD Smart OLED TV OLED83C2PSA (Black) (2022 Model) | With Eye Comfort Display",
        categoryId: 3,
        categoryName: "Tv",
        thumbnail: null,
        address: "Aluva, Kerala",
        price: 101,
        createdDate: "2023-02-23T17:29:36.3558987",
        status: 3,
      },

      orderId: 1,
      orderNumber: "order_LOvNgsWjszmfs4",
      email: "stejin.jacob@innovaturelabs.com",
      userId: 1,
      totalPrice: 459990,
      paymentStatus: 1,
    },
  ],
  message: null,
  serviceStatus: 200,
  status: true,
};
