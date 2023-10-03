import React, { useCallback, useEffect, useState } from "react";
import Styles from "./OrderDetailView.module.css";
import CommonHeader from "../../../Header/HeaderComponent/Header";
import {
  downloadInvoice,
  emailInvoice,
  getOrderDetailsById,
} from "../../../../../core/Api/apiService";
import { Button, CircularProgress, Divider } from "@mui/material";
import DownloadIcon from "@mui/icons-material/Download";
import useWindowDimensions from "../../../../../hooks/WindowSizeReader/WindowDimensions";
import { useNavigate, useSearchParams } from "react-router-dom";
import Footer from "../../../Footer/Footer";
import Title from "../../../../Utils/PageTitle/Title";
import { toast } from "react-toastify";
import ScrollToTop from "../../../../Utils/ScrollToPageTop/ScrollToTop";
import CancelOrder from "./CancelOrder/CancelOrder";
import CircleLoader from "../../../../Utils/Loaders/CircleLoader/CircleLoader";
import StepperView from "./StepperView";
import MailIcon from "@mui/icons-material/Mail";

const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;

const OrderDetailView = () => {
  let navigate = useNavigate();
  const { width } = useWindowDimensions();
  const [searchParams] = useSearchParams();
  const id = searchParams.get("orderDetailsId");
  const [details, setDetails] = useState([]);
  const [donloadCall, setDownloadCall] = useState(false);
  const [emailCall, setEmailCall] = useState(false);
  const [apiCall, setApiCall] = useState(false);

  useEffect(() => {
    document.title = "Order Details";
    const getOrderDetails = () => {
      setApiCall(true);
      getOrderDetailsById(id)
        .then((response) => {
          setApiCall(false);
          setDetails(response.data.data);
        })
        .catch((err) => {
          setApiCall(false);
          if (err.response.status === 404) {
            navigate("/orders");
          }
          console.log(err);
        });
    };
    getOrderDetails();
  }, [id, navigate]);

  const handleDownload = useCallback(() => {
    setDownloadCall(true);
    setTimeout(() => {
      downloadInvoice(details?.orderDetailsId)
        .then((response) => {
          const blob = new Blob([response.data], { type: "application/pdf" });
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement("a");
          a.href = url;
          a.download = `${details.orderNumber}_${details.orderDetailsId}`;
          a.click();
          setDownloadCall(false);
        })
        .catch((err) => {
          toast.error("Error! Try again later", { toastId: 50 });
          console.log(err);
          setDownloadCall(false);
        });
    }, 500);
  }, [details]);

  const handleEmail = useCallback(() => {
    setEmailCall(true);
    emailInvoice(details?.orderDetailsId)
      .then(() => {
        setEmailCall(false);
        toast.success("Email sent successfully ");
      })
      .catch((err) => {
        console.log(err);
        setEmailCall(false);
        toast.error("Error! Try again later", { toastId: 51 });
      });
  }, [details]);

  const handleNavigation = () => {
    navigate(`/productdetail/?id=${details?.productView?.productId}`);
  };

  return (
    <>
      <div className={Styles.pagecontainer} data-testid="orderdetailpage">
        <ScrollToTop />
        <CommonHeader />
        <Title pageTitle={"Order Details"} />
        <div className={Styles.productcontaineralign}>
          <div className={Styles.productcontainer}>
            {!apiCall && (
              <>
                <div className={Styles.addressdetailcontainer}>
                  <div className={Styles.addresssection}>
                    <h3>Delivery Address</h3>
                    <div className={Styles.addressbox}>
                      <h4 className={Styles.addressline}>
                        {details?.deliveryAddressOrderView?.name}
                      </h4>
                      <div className={Styles.addressline}>
                        {details?.deliveryAddressOrderView?.address},
                      </div>
                      <div className={Styles.addressline}>
                        {details?.deliveryAddressOrderView?.streetAddress},
                        {details?.deliveryAddressOrderView?.city},
                      </div>
                      <div className={Styles.addressline}>
                        {details?.deliveryAddressOrderView?.zipCode} -
                        {details?.deliveryAddressOrderView?.state},
                      </div>
                      <div className={Styles.addressline}>
                        {details?.deliveryAddressOrderView?.phoneNumber}
                      </div>
                    </div>
                  </div>
                  <div className={Styles.actionsection}>
                    <h3>Actions</h3>
                    <div>
                      <div>
                        <div className={Styles.actions}>
                          <p>Download Invoice</p>
                          <Button
                            data-testid="downloadbtn"
                            sx={{ height: "35px" }}
                            onClick={handleDownload}
                            variant="contained"
                            disabled={details?.orderStatus === 6 || donloadCall} // Disable if orderStatus is 7
                          >
                            {donloadCall ? (
                              <CircularProgress size={18} />
                            ) : (
                              <DownloadIcon
                                sx={
                                  details?.orderStatus === 6 && {
                                    color: "black",
                                  }
                                }
                              />
                            )}
                          </Button>
                        </div>
                        <div className={Styles.actions}>
                          <p>Email Invoice</p>
                          <Button
                            data-testid="emailbtn"
                            sx={{ height: "35px" }}
                            onClick={handleEmail}
                            variant="contained"
                            disabled={details?.orderStatus === 6 || emailCall} // Disable if orderStatus is 7
                          >
                            {emailCall ? (
                              <CircularProgress size={18} />
                            ) : (
                              <MailIcon
                                sx={
                                  details?.orderStatus === 6 && {
                                    color: "black",
                                  }
                                }
                              />
                            )}
                          </Button>
                        </div>
                      </div>
                      <div className={Styles.actions}>
                        <CancelOrder orderDetails={details} />
                      </div>
                    </div>
                  </div>
                </div>

                <div className={Styles.productdetailcontainer}>
                  <div
                    className={Styles["product"]}
                    data-testid="detailsection"
                  >
                    <div className={Styles["imagealign"]}>
                      <img
                        src={
                          details?.productView?.thumbnail
                            ? `${baseImageUrl}${details?.productView?.thumbnail}`
                            : ""
                        }
                        className={Styles["image"]}
                        alt=""
                      />
                    </div>
                    <div className={Styles["productdetaildiv"]}>
                      <h3
                        data-testid="productname"
                        className={Styles["productname"]}
                        style={{ wordBreak: "break-word" }}
                        onClick={handleNavigation}
                      >
                        {details?.productView?.productName}
                      </h3>
                      <label className={Styles["productdetails"]}>
                        Category: {details?.productView?.categoryName}
                      </label>
                      <label className={Styles["productdetails"]}>
                        ₹{details?.productView?.price}
                      </label>
                    </div>
                  </div>
                  <Divider />
                  <div className={Styles["steppercontainer"]}>
                    <div
                      style={{
                        width: "100%",
                        height: "150px",
                        display: width < 445 ? "none" : "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        cursor: "pointer",
                      }}
                    >
                      <div style={{ width: "100%" }}>
                        <StepperView
                          orderDetails={details}
                          orderStatus={details?.orderStatus}
                          horizontal={true}
                        />
                      </div>
                    </div>
                    <div
                      style={{
                        display: width <= 444 ? "flex" : "none",
                        alignItems: "center",
                        justifyContent: "center",
                        cursor: "pointer",
                        flexDirection: "column",
                      }}
                    >
                      <StepperView
                        orderDetails={details}
                        orderStatus={details?.orderStatus}
                        horizontal={false}
                      />
                    </div>
                  </div>
                </div>
              </>
            )}
          </div>
        </div>
      </div>
      {apiCall && <CircleLoader />}
      <Footer />
    </>
  );
};

export default OrderDetailView;
