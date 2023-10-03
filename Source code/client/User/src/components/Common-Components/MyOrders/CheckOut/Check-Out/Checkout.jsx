import { useForm } from "react-hook-form";
import CheckoutCss from "./Checkout.module.css";
import React, { useState, useEffect, useCallback } from "react";
import CommonHeader from "../../../Header/HeaderComponent/Header";
import AddressForm from "../Address-Form/AddressForm";
import OrderSummary from "../Order-Summary/OrderSummary";
import { toast } from "react-toastify";
import RazorPay from "../../../Razor-Pay/RazorPay";
import Swal from "sweetalert2";
import { useNavigate, useSearchParams } from "react-router-dom";
import {
  cancelOrderByOrderNumber,
  createOrder,
  deleteDeliveryAddress,
  getAllCart,
  getAllDeliveryAddresses,
  getProductDetails,
  setDefaultDeliveryAddress,
} from "../../../../../core/Api/apiService";
import ScrollToTop from "../../../../Utils/ScrollToPageTop/ScrollToTop";
import CircleLoader from "../../../../Utils/Loaders/CircleLoader/CircleLoader";

const Checkout = () => {
  let navigate = useNavigate();
  const { clearErrors } = useForm({ mode: "onChange" });
  const [searchParams] = useSearchParams();
  const id = searchParams.get("product_id");
  const [addresses, setAddresses] = useState({});
  const [cart, setCart] = useState([]);
  const [total, setTotal] = useState(0);
  const [addState, setAddState] = useState(false);
  const [summaryState, setSummaryState] = useState(false);
  const [addressList, setAddressList] = useState(true);
  const [selectedAddress, setSelectedAddress] = useState(null);
  const [orderDetails, setOrderDetails] = useState([]);
  const [editId, setEditId] = useState(null);
  const [ids, setIds] = useState(null);
  const [email, setEmail] = useState("");
  const [apiCall, setApiCall] = useState(false);

  useEffect(() => {
    document.title = "Order Checkout";
    setEmail(localStorage.getItem("email"));
    let orderCreated = localStorage.getItem("orderCreated");
    if (!orderCreated || orderCreated === "false") {
      navigate("/home");
    }
  }, [navigate]);


  useEffect(() => {
    getAddresses();
    if (id) {
      getProduct();
    } else {
      getCart();
    }
  }, [id]);

  useEffect(() => {
    window.onbeforeunload = function () {
      console.log("reload");
      return "The page that you're looking for used information that you entered. Returning to that page might cause any action that you took to be repeated. Do you want to continue?";
    };

    return () => {
      window.onbeforeunload = null;
    };
  }, []);

  useEffect(() => {
    if (selectedAddress) {
      setSummaryState(true);
    } else {
      setSummaryState(false);
    }
  }, [selectedAddress]);



  const getAddresses = async () => {
    setApiCall(true);
    getAllDeliveryAddresses()
      .then((response) => {
        setApiCall(false);
        setAddresses(response.data.data);
      })
      .catch((err) => {
        console.log(err);
        setApiCall(false);
      });
  };

  const setDefaultAddress = async (id) => {
    setApiCall(true);
    setDefaultDeliveryAddress(id)
      .then(() => {
        setApiCall(false);
        getAddresses();
      })
      .catch((err) => {
        console.log(err);
        setApiCall(false);
      });
  };
  const getPriceSum = useCallback(() => {
    let sum = 0;
    if (cart.length > 0) {
      cart.forEach((product) => {
        if (product.status === 1 && product.createdUserStatus === 1) {
          sum = sum + product.price;
          setTotal(sum);
        }
      });
    }
  },[cart]);

  const getCart = async () => {
    setApiCall(true);
    setIds(null);
    getAllCart()
      .then((response) => {
        if (response.data.data) {
          getPriceSum();
          setApiCall(false);
          const data = response.data.data;
          let q = [];
          let productsss = [];
          data.forEach((product) => {
            if (product.status === 1 && product.createdUserStatus === 1) {
              q.push(product.productId);
              productsss.push(product);
            }
            setCart(productsss);
          });
          setIds(q);
        }
      })
      .catch((err) => {
        setApiCall(false);
        console.log(err);
      });
  };
  
  useEffect(() => {
    getPriceSum();
  }, [cart, getPriceSum]);

  const getProduct = () => {
    setApiCall(true);
    getProductDetails(id)
      .then((response) => {
        setApiCall(false);
        let responseData = response?.data?.data;
        let data = [
          {
            createdUserStatus: responseData.createdUser.status,
            productId: responseData.productId,
            productName: responseData.productName,
            categoryId: responseData.categoryId,
            categoryName: responseData.categoryName,
            thumbnail: responseData.thumbnail,
            productDescription: responseData.productDescription,
            address: responseData.address,
            price: responseData.price,
            createdDate: responseData.createdDate,
            status: responseData.status,
          },
        ];
        setCart(data);
        setIds([responseData.productId]);
      })
      .catch((err) => {
        console.log(err);
        setApiCall(false);
      });
  };



  const handleAddAddressDiv = useCallback(() => {
    clearErrors();
    setEditId(null);
    getAddresses();
    setAddState(!addState);
  }, [addState, clearErrors]);

  const handleAddressSelection = (id) => {
    setDefaultAddress(id);
    setSelectedAddress(null);
  };

  const handleDeliverHere = (address) => {
    setSelectedAddress(address);
    setAddressList(false);
  };

  const handleAddressChange = () => {
    setSelectedAddress(null);
    setAddressList(true);
  };

  const handleEditAddress = (id) => {
    setEditId(id);
    setAddState(!addState);
  };

  const handleRemoveAddress = (id) => {
    Swal.fire({
      title: "Are you sure?",
      text: "This will permanently remove this address",
      icon: "error",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Delete",
    }).then(async (result) => {
      if (result.isConfirmed) {
        setApiCall(true);
        deleteDeliveryAddress(id)
          .then(() => {
            setApiCall(false);
            getAddresses();
            toast.success("Address removed");
          })
          .catch((err) => {
            console.log(err);
            setApiCall(false);
            toast.error("Error occured ");
          });
      }
    });
  };

  const buyNow = async () => {
    const orderData = {
      deliveryAddressId: selectedAddress.deliveryAddressId,
      productIds: ids,
    };
    setApiCall(true);
    createOrder(orderData)
      .then((response) => {
        console.log("987654321");
        console.log(response.data.data);
        setApiCall(false);
        setOrderDetails(response.data.data);
      })
      .catch((err) => {
        setApiCall(false);
        toast.warning("An error Occured.");
        console.log(err.response.data);
      });
  };

  const cancelOrder = () => {
    Swal.fire({
      text: "Are you sure you want to cancel the order?",
      icon: "info",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes",
    }).then(async (result) => {
      if (result.isConfirmed) {
        handleCancellation();
      }
    });
  };

  const handleCancellation = () => {
    console.log(orderDetails);
    if (!orderDetails?.orderNumber) {
      console.log("987654321");
      handleCancelNavigation();
    } else {
      setApiCall(true);
      console.log(orderDetails?.orderNumber, "order num");
      console.log("123456789");
      cancelOrderByOrderNumber(orderDetails?.orderNumber)
        .then(() => {
          setApiCall(false);
          localStorage.setItem("orderCreated", false);
          handleCancelNavigation();
        })
        .catch((err) => {
          setApiCall(false);
          console.log(err);
          toast.error("Error occured ");
        });
    }
  };
  const handleCancelNavigation = () => {
    if (id) {
      navigate("/home");
    } else {
      navigate("/cart");
    }
  };

  return (
    <>
      <ScrollToTop />
      <div data-testid="checkoutpage">
        <CommonHeader />
        <RazorPay orderDetails={orderDetails} />
        <div className={CheckoutCss["container"]}>
          <div className={CheckoutCss["title"]}>
            <h2 className={CheckoutCss["h2"]}>Checkout</h2>
          </div>
          <div className={CheckoutCss["d-flex"]}>
            <div className={CheckoutCss["form"]}>
              <div className={CheckoutCss["addresscontainer"]}>
                <div className={CheckoutCss["addresslistheading"]}>
                  Delivery Address
                  {!addressList && (
                    <button
                      data-testid="addresschangebtn"
                      className={CheckoutCss["changebtn"]}
                      onClick={handleAddressChange}
                    >
                      Change
                    </button>
                  )}
                </div>
                {!addressList && (
                  <div className={CheckoutCss["slectedaddresscontainer"]}>
                    <div className={CheckoutCss["addresslistrow3"]}>
                      {selectedAddress.name},{selectedAddress.address},
                      {selectedAddress.streetAddress},{selectedAddress.city},
                      {selectedAddress.state},{selectedAddress.zipCode}
                    </div>
                  </div>
                )}
                {addressList && (
                  <div>
                    {addresses.length > 0 ? (
                      addresses.map((address) => {
                        return (
                          <div
                            key={address.deliveryAddressId}
                            data-testid="addressdiv"
                            className={CheckoutCss["addresslist"]}
                          >
                            <div>
                              <input
                                data-testid={`checkbox-${address.deliveryAddressId}`}
                                onClick={() => {
                                  handleAddressSelection(
                                    address.deliveryAddressId
                                  );
                                }}
                                className={CheckoutCss["radioinput"]}
                                type="radio"
                                checked={address.status === 1}
                                readOnly
                              />
                            </div>
                            <div className={CheckoutCss["addressrow"]}>
                              <div className={CheckoutCss["addresslistrow1"]}>
                                <div className={CheckoutCss["addresslistrow2"]}>
                                  <b className={CheckoutCss["b"]}>
                                    {address.name}
                                  </b>
                                  <b style={{ marginLeft: "10px" }}>
                                    , {address.phoneNumber}
                                  </b>
                                </div>
                                <div>
                                  <button
                                    data-testid={`editAddress-${address.deliveryAddressId}`}
                                    onClick={() => {
                                      handleEditAddress(
                                        address.deliveryAddressId
                                      );
                                    }}
                                    className={CheckoutCss["editbtn"]}
                                  >
                                    Edit
                                  </button>
                                  <button
                                    data-testid={`addressdeletebtn-${address.deliveryAddressId}`}
                                    onClick={() => {
                                      if (address.status === 1) {
                                        toast.error(
                                          "Default address can't be deleted",
                                          { toastId: 18 }
                                        );
                                      } else {
                                        handleRemoveAddress(
                                          address.deliveryAddressId
                                        );
                                      }
                                    }}
                                    className={CheckoutCss["editbtn"]}
                                  >
                                    Remove
                                  </button>
                                </div>
                              </div>
                              <div className={CheckoutCss["addresslistrow3"]}>
                                {address.address},{address.streetAddress},{" "}
                                {address.city},{address.state},{" "}
                                {address.zipCode}
                              </div>
                              {address.status === 1 && (
                                <div>
                                  <button
                                    data-testid="deliverHere"
                                    className={CheckoutCss["formbtnsubmit"]}
                                    onClick={() => {
                                      handleDeliverHere(address);
                                    }}
                                  >
                                    Deliver Here
                                  </button>
                                </div>
                              )}
                            </div>
                          </div>
                        );
                      })
                    ) : (
                      <div className={CheckoutCss["addresslistcontainer"]}>
                        No saved address found
                      </div>
                    )}
                  </div>
                )}
              </div>

              {!summaryState && (
                <div className={CheckoutCss["addaddresscontainer"]}>
                  {!addState && (
                    <button
                      data-testid="add-address-button"
                      className={CheckoutCss["addbtn"]}
                      onClick={handleAddAddressDiv}
                    >
                      <label style={{ fontSize: "30px", fontWeight: "normal" }}>
                        +
                      </label>{" "}
                      &nbsp; Add Address
                    </button>
                  )}
                  {/* add address form */}
                  {addState && (
                    <AddressForm
                      handleAddAddressDiv={handleAddAddressDiv}
                      editId={editId}
                    />
                  )}
                </div>
              )}

              <div className={CheckoutCss["cartproducts"]}>
                <div className={CheckoutCss["addresslistheading"]}>
                  Order Summary
                </div>
                <div>{summaryState && <OrderSummary cart={cart} />}</div>
              </div>
              {summaryState && (
                <div className={CheckoutCss["continuediv"]}>
                  <small>
                    Order confirmation email will be sent to <b>{email}</b>
                  </small>
                  <button
                    data-testid="buynow-btn"
                    onClick={() => {
                      buyNow();
                    }}
                    className={CheckoutCss["continuebtn"]}
                  >
                    Continue
                  </button>
                </div>
              )}
            </div>

            {/* price summary */}
            <div className={CheckoutCss["Yorder"]}>
              <table className={CheckoutCss["table"]}>
                <tbody>
                  <tr>
                    <th className={CheckoutCss["th"]} colSpan="2">
                      Your order
                    </th>
                  </tr>
                  <tr>
                    <td className={CheckoutCss["tdleft"]}>{`Price (${
                      cart.length
                    } ${cart.length > 1 ? "items" : "item"}  )`}</td>
                    <td className={CheckoutCss["tdright"]}>₹ {total}</td>
                  </tr>
                  <tr>
                    <td className={CheckoutCss["tdleft"]}>Shipping</td>
                    <td className={CheckoutCss["tdright"]}>Free shipping</td>
                  </tr>
                  <tr>
                    <td className={CheckoutCss["tdleft"]}>Subtotal</td>
                    <td className={CheckoutCss["tdright"]}>₹ {total}</td>
                  </tr>
                </tbody>
              </table>
              <br></br>
              <button
                onClick={cancelOrder}
                data-testid="cancel-btn"
                className={CheckoutCss["button"]}
              >
                Cancel Order
              </button>
            </div>
          </div>
        </div>
      </div>
      {apiCall && <CircleLoader />}
    </>
  );
};

export default Checkout;
