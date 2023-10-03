import React, { useContext, useEffect, useState } from "react";
import MyImage from "./../../../../Assets/images/Image_not_available.png";
import Footer from "../../Footer/Footer";
import style from "./Productlist.module.css";
import Sidebar from "../Sidebar/Sidebar";
import Box from "@mui/material/Box";
import Paper from "@mui/material/Paper";
import Grid from "@mui/material/Unstable_Grid2";
import { styled } from "@mui/material/styles";
import { Slide } from "react-slideshow-image";
import "react-slideshow-image/dist/styles.css";
import banner2 from "./../../../../Assets/images/banner_one.png";
import banner3 from "./../../../../Assets/images/banner_three.png";
import banner4 from "./../../../../Assets/images/banner_two.png";
import { cartCounts } from "../../../../App";
import { getRelativeDate } from "../../../Utils/Utils";
import ScrollToTopButton from "../../../Utils/ScrollToTopButton/ScrollToTopButton";
import { getAllCart, productList } from "../../../../core/Api/apiService";
import WishlistIcon from "./WishList-Icon/WishlistIcon";
import CommonHeader from "../../Header/HeaderComponent/Header";
import { Skeleton } from "@mui/material";
import { useDispatch, useSelector } from "react-redux";
import { selectAllFilters } from "./ProductSlices/filterSlice";
import {
  getProducts,
  nextPage,
  selectAllProducts,
} from "./ProductSlices/productSlice";
import useWindowDimensions from "../../../../hooks/WindowSizeReader/WindowDimensions";
import FilterMobile from "../Filter-Mobile/FilterMobile";
import { fetchWishlist, selectAllWishlist } from "../../WishList/wishlistSlice";

function Productlist() {
  const dispatch = useDispatch();
  const filters = useSelector(selectAllFilters);
  const products = useSelector(selectAllProducts);
  const favourites = useSelector(selectAllWishlist);
  const { width } = useWindowDimensions();
  const [hasNext, sethasNext] = useState(false);
  const [loaderState, setLoaderState] = useState(true);
  const [apiCall, setApiCall] = useState(false);
  const [, setCount] = useContext(cartCounts);

  const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;
  useEffect(() => {
    document.title = "CART_IN";
    const getCart = () => {
      getAllCart()
        .then((response) => {
          setCount(response?.data?.data.length);
        })
        .catch((err) => console.log(err));
    };
    if (localStorage.getItem("accessToken")) {
      dispatch(fetchWishlist());
      getCart();
    }
  }, [dispatch, setCount]);


   // function for fetching products based on the applied filters
   const setFilter = () => {
    setLoaderState(true);
    let offset =
      products.length !== 0 ? products[products.length - 1].productId : 0;
    let pageSize = offset !== 0 ? 12 : 24;
    productList({ ...filters, Offset: offset, PageSize: pageSize })
      .then((response) => {
        let data = response?.data?.data.result;
        if (products.length === 0) {
          dispatch(getProducts(data));
          executeScroll();
        } else {
          dispatch(nextPage(data));
        }
        sethasNext(response?.data.data.hasNext);
        setLoaderState(false);
        setApiCall(false);
      })
      .catch((err) => {
        console.log(err);
        setApiCall(false);
      });
  };

  useEffect(() => {
    setFilter();
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filters]);

  useEffect(() => {
    const handleScroll = (e) => {
      const scrollHeight = e.target.documentElement.scrollHeight;
      const currentHeight =
        e.target.documentElement.scrollTop + window.innerHeight;
      if (currentHeight >= (scrollHeight * 39) / 40 && !apiCall) {
        if (hasNext) {
          setApiCall(true);
          setFilter();
        }
      }
    };
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  });

 

  const executeScroll = () => {
    window.scrollTo({ top: 5, behavior: "smooth" });
  };

  const newProductDetailTab = (productId) => {
    window.open(`/productdetail/?id=${productId}`, "_blank");
  };

  const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === "#fff",
    ...theme.typography.body2,
    padding: theme.spacing(3),
    textAlign: "center",
    color: theme.palette.text.secondary,
  }));

  const banerImages = [
    { id: 1, image: banner2 },
    { id: 2, image: banner3 },
    { id: 3, image: banner4 },
  ];

  const cards = Array.from({ length: 4 }, (_, index) => (
    <Grid lg={5} sm={20} style={{ width: "275px" }} key={index}>
      <Item
        className={style["data-card"]}
        style={{
          height: "249px",
          cursor: "pointer",
          padding: "6px",
          minWidth: "250px",
        }}
      >
        <Skeleton variant="rectangular" width={250} height={200} />
        <Skeleton height={20} width="60%" />
        <Skeleton height={20} />
      </Item>
    </Grid>
  ));

  return (
    <>
      <div data-testid="productlistpage">
        <div style={{ zIndex: "999", position: "absolute" }}>
          <CommonHeader productlist={true} />
          <ScrollToTopButton />
        </div>
        <div className={style.Title}></div>
        <Slide duration={3000} indicators={true} className={style.bannerframe}>
          {banerImages.map((banner) => {
            return (
              <div className={style["each-slide-effect"]} key={banner.id}>
                <div
                  className={style.banner}
                  style={{ backgroundImage: `url(${banner.image})` }}
                ></div>
              </div>
            );
          })}
        </Slide>
        <div>
          <div className={style.box1}>
            <div className={style.box2}>
              <div className={style.boxproductlist}>
                {/* //category// */}
                {width > 701 ? (
                  <div className={style.categorybox}>
                    <div className={style.categorybox1}>
                      <div style={{ position: "sticky", top: "85px" }}>
                        <Sidebar />
                      </div>
                    </div>
                  </div>
                ) : (
                  <FilterMobile />
                )}

                {/* //product// */}

                <div className={style.productbox}>
                  <Box sx={{ marginTop: "40px" }}>
                    <Grid
                      key={"1"}
                      container
                      spacing={2}
                      style={{
                        width: "100%",
                        margin: 0,
                        padding: "0px 13px",
                        justifyContent: `${width < 468 && "center"}`,
                      }}
                    >
                      {products.length > 0 ? (
                        products.map((product) => {
                          return (
                            <Grid
                              lg={5}
                              sm={20}
                              key={product.productId}
                              className={style["list-card"]}
                            >
                              <Item
                                className={style["data-card"]}
                                style={{
                                  height: "255px",
                                  cursor: "pointer",
                                  padding: "6px",
                                  minWidth: "250px",
                                }}
                              >
                                <div className={style.likebutton}>
                                  {favourites.some(
                                    (f) => f.productId === product.productId
                                  ) ? (
                                    <WishlistIcon
                                      id={product.productId}
                                      favourite={true}
                                    />
                                  ) : (
                                    <WishlistIcon
                                      id={product.productId}
                                      favourite={false}
                                    />
                                  )}
                                </div>
                                <div
                                  data-testid="product-card"
                                  onClick={() => {
                                    newProductDetailTab(product.productId);
                                  }}
                                >
                                  <div className={style["datas"]}>
                                    <img
                                      src={
                                        product?.thumbnail
                                          ? `${baseImageUrl}${product.thumbnail}`
                                          : MyImage
                                      }
                                      className={style["image"]}
                                      alt=""
                                    />
                                  </div>

                                  <div className={style["fontsize"]}>
                                    <h2 style={{ margin: "0", padding: "0" }}>
                                      &#8377; {product.price}
                                    </h2>
                                    <div
                                      style={{
                                        display: "flex",
                                        flexDirection: "row",
                                        justifyContent: "space-between",
                                        alignItems: "center",
                                      }}
                                    >
                                      <p className={style.productname}>
                                        {product.productName}
                                      </p>
                                      <p style={{ fontSize: "10px" }}>
                                        {getRelativeDate(product.createdDate)}
                                      </p>
                                    </div>
                                  </div>
                                </div>
                              </Item>
                            </Grid>
                          );
                        })
                      ) : (
                        <div className={style["item"]}>
                          {!loaderState && <div> No Products Found! </div>}
                        </div>
                      )}
                      {loaderState && cards}
                    </Grid>
                    <br />
                  </Box>
                </div>
              </div>
            </div>
            <br />
            <br />
          </div>
        </div>
        <Footer />
      </div>
    </>
  );
}

export default Productlist;
