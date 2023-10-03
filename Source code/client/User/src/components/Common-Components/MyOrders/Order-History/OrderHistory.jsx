import React, { useEffect, useState } from "react";
import CommonHeader from "../../Header/HeaderComponent/Header";
import OrderHistoryCss from "./OrderHistory.module.css";
import MyImage from "./../../../../Assets/images/Image_not_available.png";
import Footer from "../../Footer/Footer";
import { getAllOrders } from "../../../../core/Api/apiService";
import { convertDate } from "../../../Utils/Utils";
import { useNavigate } from "react-router-dom";
import { getOrderStatuses } from "../../../Utils/Data/Data";
import Title from "../../../Utils/PageTitle/Title";
import ScrollToTop from "../../../Utils/ScrollToPageTop/ScrollToTop";
import CircleLoader from "../../../Utils/Loaders/CircleLoader/CircleLoader";

const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;

const OrderHistory = () => {
  let navigate = useNavigate();
  const [orders, setOrders] = useState([]);
  const [apiCall, setApiCall] = useState(false);

  useEffect(() => {
    document.title = "My Orders";
    getOrders();
  }, []);

  const getOrders = async () => {
    setApiCall(true);
    getAllOrders()
      .then((response) => {
        setOrders(response.data.data);
        setApiCall(false);
      })
      .catch((err) => {
        console.log(err);
        setApiCall(false);
      });
  };

  const goToDetailPage = (orderDetailsId) => {
    navigate(`/orderdetail?orderDetailsId=${orderDetailsId}`);
  };

  return (
    <>
      <ScrollToTop />
      <div
        data-testid="orderhistorypage"
        className={OrderHistoryCss.pagecontainer}
      >
        <CommonHeader />
        <Title pageTitle={"My Orders"} />
        <div className={OrderHistoryCss.productcontaineralign}>
          <div className={OrderHistoryCss.productcontainer}>
            <div className={OrderHistoryCss.listcontontainer}>
              {orders.length > 0 ? (
                orders.map((order) => {
                  return (
                    <div
                      data-testid="gotodetailbtn"
                      key={order.orderDetailsId}
                      className={OrderHistoryCss["productcard"]}
                      onClick={() => {
                        goToDetailPage(order.orderDetailsId);
                      }}
                    >
                      <div className={OrderHistoryCss["innercontainer"]}>
                        <div className={OrderHistoryCss["col1"]}>
                          <div className={OrderHistoryCss["imagealign"]}>
                            <img
                              src={
                                order?.productView.thumbnail
                                  ? `${baseImageUrl}${order.productView.thumbnail}`
                                  : MyImage
                              }
                              className={OrderHistoryCss["image"]}
                              alt=""
                            />
                          </div>
                          <div className={OrderHistoryCss["details"]}>
                            <h4 className={OrderHistoryCss["h4"]}>
                              {order?.productView?.productName}
                            </h4>
                            <label
                              style={{ marginTop: "8px", fontSize: "17px" }}
                            >
                              ₹{order?.productView?.price}
                            </label>
                          </div>
                        </div>
                        <div className={OrderHistoryCss["col2wrappper"]}>
                          <div className={OrderHistoryCss["col2"]}>
                            {order.orderStatus !== 6 &&
                              ` Ordered on ${convertDate(order?.createdDate)}`}

                            {order.orderStatus === 6 &&
                              ` Ordered on ${convertDate(
                                order?.updatedDate
                              )}`}
                          </div>
                          <div className={OrderHistoryCss["col3"]}>
                            {order.orderStatus !== 6 && (
                              <label style={{ color: "green" }}>
                                {getOrderStatuses(order.orderStatus)}
                              </label>
                            )}

                            {order.orderStatus === 6 && (
                              <label style={{ color: "red" }}>
                                {getOrderStatuses(order.orderStatus)}
                              </label>
                            )}

                            <div className={OrderHistoryCss["arrow"]}>
                             
                                  <div
                                    className={OrderHistoryCss["arrow-top"]}
                                  ></div>
                                  <div
                                    className={OrderHistoryCss["arrow-bottom"]}
                                  ></div>
                         
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  );
                })
              ) : (
                <div className={OrderHistoryCss["noorderfound"]}>
                  No Orders Found!
                </div>
              )}
            </div>
          </div>
        </div>
        <Footer />
        {apiCall && <CircleLoader />}
      </div>
    </>
  );
};

export default OrderHistory;
