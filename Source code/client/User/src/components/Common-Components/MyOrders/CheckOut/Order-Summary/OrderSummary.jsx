import React, { useEffect, useState } from "react";
import OrderSummaryStyle from "./OrderSummary.module.css";
import MyImage from "./../../../../../Assets/images/Image_not_available.png";

const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;

const OrderSummary = (props) => {
  const [products, setProducts] = useState([]);

  useEffect(() => {
    setProducts(props.cart);
  }, [props.cart]);

  return (
    <div
      data-testid="ordersummary"
      className={OrderSummaryStyle["productcontainer"]}
    >
      {products?.length > 0 ? (
        products.map((product) => {
          return (
            <div
              className={OrderSummaryStyle["product"]}
              key={product.productId}
            >
              <div className={OrderSummaryStyle["imagealign"]}>
                <img
                  src={
                    product?.thumbnail
                      ? `${baseImageUrl}${product.thumbnail}`
                      : MyImage
                  }
                  className={OrderSummaryStyle["image"]}
                  alt=""
                />
              </div>
              <div className={OrderSummaryStyle["productdetaildiv"]}>
                <h3 style={{ wordBreak: "break-word" }}>
                  {product.productName}
                </h3>
                <label className={OrderSummaryStyle["productdetails"]}>
                  Category: {product.categoryName}
                </label>
                <label className={OrderSummaryStyle["productdetails"]}>
                  ₹{product.price}
                </label>
              </div>
            </div>
          );
        })
      ) : (
        <div className={OrderSummaryStyle["noorder"]}> No Products found</div>
      )}
    </div>
  );
};

export default OrderSummary;
