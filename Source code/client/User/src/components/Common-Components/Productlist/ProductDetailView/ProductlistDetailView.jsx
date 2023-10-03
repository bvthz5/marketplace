import React, { useCallback, useContext, useEffect, useState } from "react";
import { Skeleton, Typography } from "@mui/material";
import { useNavigate, useSearchParams } from "react-router-dom/dist";
import Footer from "../../Footer/Footer";
import CommonHeader from "../../Header/HeaderComponent/Header";
import Swal from "sweetalert2";
import detailcss from "./ProductListDetailView.module.css";
import { toast } from "react-toastify";
import { cartCounts } from "../../../../App";
import MapImage from "./Google-Maps/MapImage";
import { convertDate } from "../../../Utils/Utils";
import {
  addToCart,
  getAllCart,
  getProductDetails,
  getProductImages,
} from "../../../../core/Api/apiService";
import ScrollToTop from "../../../Utils/ScrollToPageTop/ScrollToTop";
import ImageSlider from "./ImageSlider/ImageSlider";

const ProductlistDetailView = () => {
  let navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");
  const [product, setProduct] = useState([]);
  const [images, setImages] = useState([]);
  const [cart, setCart] = useState([]);
  const [LoggedIn, setLoggedIn] = useState(false);
  const [title, setTitle] = useState("Details");
  const [, setCount] = useContext(cartCounts);
  const [dataApiCall, setDataApiCall] = useState(false);
  const [imageApiCall, setImageApiCall] = useState(false);

  const getCart = useCallback(() => {
    getAllCart()
      .then((response) => {
        setCart(response.data.data);
        setCount(response.data.data.length);
      })
      .catch((err) => console.log(err));
  }, [setCount]);

  useEffect(() => {
    document.title = `${title}`;

    // api call for fetching the product images
    const getImage = () => {
      getProductImages(id ? id : 0)
        .then((response) => {
          setImages(response?.data.data);
          setImageApiCall(false);
        })
        .catch((err) => {
          console.log(err);
          setImageApiCall(false);
        });
    };

    const getProducts = () => {
      setDataApiCall(true);
      setImageApiCall(true);
      getProductDetails(id ? id : 0)
        .then((response) => {
          getImage();
          setProduct(response?.data.data);
          setTitle(response?.data.data.productName);
          setDataApiCall(false);
        })
        .catch((err) => {
          setDataApiCall(false);
          console.log(err);
          if (err.response.status === 404) {
            navigate("/home");
          }
        });
    };

    if (localStorage.getItem("accessToken")) {
      setLoggedIn(true);
      getCart();
    }
    getProducts();
  }, [getCart, id, navigate, title]);

  // api call for fetching product details

  // fetching product from cart set the cart count

  // function to add a product to cart
  const addProductToCart = (productId) => {
    if (LoggedIn) {
      addToCart(productId)
        .then(() => {
          getCart();
          toast.success("Added to cart", {
            toastId: 13,
          });
        })
        .catch((err) => {
          console.log(err);
          const error = err.response.data.message;
          let limit = "Cart Max Limit (50) exceed";
          if (error === limit) {
            toast.warning(`${limit}ed`);
          }
        });
    } else {
      Swal.fire({
        title: "Oops...",
        text: "You haven't logged in yet!",
        icon: "error",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Login Now",
      }).then((result) => {
        if (result.isConfirmed) {
          navigate("/login");
        }
      });
    }
  };
  const cartClick = () => {
    navigate("/cart");
  };

  const buyNow = () => {
    if (LoggedIn) {
      localStorage.setItem("orderCreated", true);
      navigate(`/summary/?product_id=${id}`);
    } else {
      Swal.fire({
        title: "Oops...",
        text: "You haven't logged in yet!",
        icon: "error",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Login Now",
      }).then((result) => {
        if (result.isConfirmed) {
          navigate("/login");
        }
      });
    }
  };

  return (
    <div data-testid="detailviewpage">
      <ScrollToTop />
      <CommonHeader />
      <div className={detailcss["pagecontainer"]}>
        <div className={detailcss["contentwrapper"]}>
          <div className={detailcss["leftsection"]}>
            <div className={detailcss["imageouter"]}>
              <div className={detailcss["imagediv"]}>
                {imageApiCall ? (
                  <Skeleton variant="rectangular" width="100%" height="400px" />
                ) : (
                  <ImageSlider images={images} />
                )}
              </div>
            </div>

            <div className={detailcss["detailcontainer"]}>
              <div className={detailcss["productdetails"]}>
                {dataApiCall ? (
                  <>
                    <Typography variant="h1">
                      <Skeleton />
                    </Typography>
                    <Typography variant="h3">
                      <Skeleton />
                    </Typography>
                    <Typography variant="body1">
                      <Skeleton />
                    </Typography>
                  </>
                ) : (
                  <>
                    <span style={{ fontSize: "23px" }}>
                      <b style={{ wordBreak: "break-word" }}>
                        {product?.productName}
                      </b>
                    </span>
                    <br />
                    <div className={detailcss["fontstyle"]}>
                      {product?.productDescription}
                    </div>
                    <div className={detailcss["fontstyle"]}>
                      <b>Category: </b> {product?.categoryName}
                    </div>
                    <div className={detailcss["fontstyle"]}>
                      <b> Price:</b> {product?.price}
                    </div>
                    <div className={detailcss["datepadding"]}>
                      <div className={detailcss["buttons"]}>
                        {product.status !== 3 && (
                          <>
                            <div>
                              {cart.some(
                                (c) => c.productId === product.productId
                              ) ? (
                                <button
                                  data-testid="gotocartbtn"
                                  className={detailcss["btn"]}
                                  style={{
                                    backgroundColor: "#008296",
                                  }}
                                  onClick={() => {
                                    cartClick();
                                  }}
                                >
                                  Go To Cart
                                </button>
                              ) : (
                                <button
                                  data-testid="addtocartbtn"
                                  className={detailcss["btn"]}
                                  style={{
                                    backgroundColor: "#002f34",
                                  }}
                                  onClick={() => {
                                    addProductToCart(product.productId);
                                  }}
                                >
                                  Add To Cart
                                </button>
                              )}
                            </div>
                            <div>
                              <button
                                data-testid="buynowbtn"
                                className={detailcss["btn"]}
                                style={{
                                  backgroundColor: "rgb(230 112 53)",
                                  marginLeft: "20px",
                                }}
                                onClick={buyNow}
                              >
                                Buy Now
                              </button>
                            </div>
                          </>
                        )}
                      </div>

                      <div className={detailcss["dateshow"]}>
                        <p className={detailcss["date"]}>
                          {convertDate(product.createdDate)}
                        </p>
                      </div>
                    </div>
                  </>
                )}
              </div>
            </div>
          </div>

          <div className={detailcss["rightsection"]}>
            <div className={detailcss["dealerside"]}>
              <div className={detailcss["pricebox"]}>
                {dataApiCall ? (
                  <>
                    <Typography variant="h1" width="93%">
                      <Skeleton />
                    </Typography>
                  </>
                ) : (
                  <>
                    <b> ₹ {product?.price}</b>
                  </>
                )}
              </div>
              <div className={detailcss["dealerbox"]}>
                <div className={detailcss["dealerdetails"]}>
                  {dataApiCall ? (
                    <>
                      <Typography variant="h3">
                        <Skeleton />
                      </Typography>
                      <Typography variant="body1" width="80%">
                        <Skeleton />
                      </Typography>
                      <Typography variant="body1" width="80%">
                        <Skeleton />
                      </Typography>
                      <Typography variant="body1" width="80%">
                        <Skeleton />
                      </Typography>
                    </>
                  ) : (
                    <>
                      <h2
                        style={{
                          color: "#002f34",
                        }}
                      >
                        <b style={{ fontWeight: "600" }}>Dealer Details</b>
                      </h2>
                      <div className={detailcss["fontstyle"]}>
                        <b> Dealer:</b> {product?.createdUser?.firstName} &nbsp;
                        {product?.createdUser?.lastName}
                      </div>
                      <div className={detailcss["fontstyle"]}>
                        <b> Contact:</b>
                        <a href={`mailto: ${product?.createdUser?.email}`}>
                          {product?.createdUser?.email}
                        </a>
                      </div>
                      <div className={detailcss["fontstyle"]}>
                        <b>Location: </b> {product?.location?.address}
                      </div>
                    </>
                  )}
                </div>
              </div>
            </div>

            <div className={detailcss["locationBox"]}>
              <div className={detailcss["locationDetails"]}>
                {dataApiCall ? (
                  <>
                    <Typography variant="body1" width="50%">
                      <Skeleton />
                    </Typography>
                    <Typography variant="body1" width="80%">
                      <Skeleton />
                    </Typography>
                    <Skeleton variant="rectangular" height={250} />
                  </>
                ) : (
                  <>
                    <h2>
                      <b style={{ fontWeight: "600" }}>Posted In</b>
                    </h2>
                    <h4 className={detailcss["locationBoxh2"]}>
                      {product?.location?.address}
                    </h4>

                    <div>
                      <MapImage
                        latitude={product?.location?.latitude}
                        longitude={product?.location?.longitude}
                      />
                    </div>
                  </>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default ProductlistDetailView;
