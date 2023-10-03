import { useState, useEffect } from "react";
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom";
import { paymentConfirmation } from "../../../core/Api/apiService";

const RAZORPAY_KEY = process.env.REACT_APP_RAZORPAY_KEY_NEW
  ? process.env.REACT_APP_RAZORPAY_KEY_NEW
  : "rzp_test_BCGkqvgO5AhRt3";

const RazorPay = (props) => {
  const [order, setOrder] = useState({});
  let navigate = useNavigate();

  useEffect(() => {
    setOrder(props.orderDetails);
  }, [props.orderDetails]);

  useEffect(() => {
    const checkoutHandler = async () => {
      const options = {
        key: RAZORPAY_KEY,
        amount: getPrice(order.totalPrice), // Amount is in currency subunits. Default currency is INR. Hence, 50000 refers to 50000 paise
        currency: "INR",
        name: "CART_IN",
        description: "Test Transaction",
        image: "https://example.com/your_logo",
        order_id: order.orderNumber, //This is a sample Order ID. Pass the `id` obtained in the response of Step 1
        handler: async function (response) {
          paymentConfirmation(response)
            .then((response) => {
              localStorage.setItem("orderCreated", false);
              toast.success("Order Placed");
              navigate("/orders");
            })
            .catch((err) => {
              console.log(err);
              toast.error("error occured !");
              navigate("/cart");
            });
        },
        config: {
          display: {
            hide: [{ method: "paylater" }],
          },
        },
        notes: {
          address: "Razorpay Corporate Office",
        },
        theme: {
          color: "#3399cc",
        },
        method: {
          netbanking: true,
          card: true,
          wallet: false,
          upi: true,
        },
      };
      const razor = new window.Razorpay(options);
      razor.open();
    };
    if (order.orderNumber) {
      checkoutHandler();
    }
  }, [order,navigate]);



  const getPrice = (price) => {
    return price * 100;
  };
};

export default RazorPay;
