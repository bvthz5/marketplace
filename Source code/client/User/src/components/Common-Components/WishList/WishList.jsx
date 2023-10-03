import React, { useEffect, useState } from "react";
import wishlistcss from "../WishList/Wishlists.module.css";
import { Tooltip } from "@mui/material";
import MyImage from "./../../../Assets/images/Image_not_available.png";
import Footer from "../Footer/Footer";
import { useNavigate } from "react-router-dom/dist";
import CloseIcon from "@mui/icons-material/Close";
import CommonHeader from "../Header/HeaderComponent/Header";
import Box from "@mui/material/Box";
import Paper from "@mui/material/Paper";
import Grid from "@mui/material/Unstable_Grid2";
import { styled } from "@mui/material/styles";
import {
  removeFromWishlist,
} from "../../../core/Api/apiService";
import Title from "../../Utils/PageTitle/Title";
import ScrollToTop from "../../Utils/ScrollToPageTop/ScrollToTop";
import { useDispatch, useSelector } from "react-redux";
import { fetchWishlist, selectAllWishlist } from "./wishlistSlice";
import CircleLoader from "../../Utils/Loaders/CircleLoader/CircleLoader";
const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;

const WishList = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const favourites = useSelector(selectAllWishlist);
  const [apiCall, setApiCall] = useState(false);

  useEffect(() => {
    document.title = "Wishlist";
    dispatch(fetchWishlist());
  }, [dispatch]);

  // function for removing a product from wishlist
  const deleteFavourites = (id) => () => {
    setApiCall(true);
    removeFromWishlist(id)
      .then(() => {
        dispatch(fetchWishlist());
        setApiCall(false);
      })
      .catch((err) => {
        console.log(err);
        setApiCall(false);
      });
  };

  const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === "#fff",
    ...theme.typography.body2,
    padding: theme.spacing(3),
    textAlign: "center",
    color: theme.palette.text.secondary,
  }));
  return (
    <>
      <div data-testid="wishlistpage">
        <ScrollToTop />
        <CommonHeader />
        <Title pageTitle={"Wishlist"} />
        <div className={wishlistcss.productcontaineralign}>
          <div className={wishlistcss.productcontainer}>
            <Box sx={{ marginTop: "20px" }}>
              <Grid
                container
                spacing={4}
                style={{
                  width: "100%",
                  justifyContent: "center",
                  margin: "auto",
                }}
              >
                {favourites.length > 0 ? (
                  favourites.map((product) => {
                    return (
                      <Grid
                        lg={5}
                        sm={10}
                        style={{ width: "300px" }}
                        key={product.productId}
                      >
                        <Item
                          className={wishlistcss["data-card"]}
                          key={product.productId}
                          style={{ height: "300px" }}
                        >
                          <div className={wishlistcss["datas"]}>
                            <div className={wishlistcss.deletebtndiv}>
                              <div className={wishlistcss.deletebtn}>
                                <Tooltip
                                  title="Remove from Wishlist"
                                  placement="top"
                                >
                                  <CloseIcon
                                    onClick={deleteFavourites(
                                      product.productId
                                    )}
                                    style={{
                                      color: "black",
                                      cursor: "pointer",
                                    }}
                                    stroke={"black"}
                                    strokeWidth={1}
                                    data-testid="deletebtn"
                                  />
                                </Tooltip>
                              </div>
                            </div>
                            <img
                              src={
                                product?.thumbnail
                                  ? `${baseImageUrl}${product.thumbnail}`
                                  : MyImage
                              }
                              className={wishlistcss["image"]}
                              alt=""
                            />
                            <h3 className={wishlistcss["h3design"]}>
                              {" "}
                              {product.productName}
                            </h3>
                            <h4>{product.categoryName}</h4>
                            <h4>&#8377; {product.price}</h4>

                            <div className={wishlistcss["forButton"]}>
                              <button
                                className={wishlistcss["button-13"]}
                                data-testid="navigatebtn"
                                onClick={() => {
                                  navigate(
                                    `/productdetail/?id=${product.productId}`
                                  );
                                }}
                              >
                                More Details
                              </button>
                            </div>
                            {(product.status !== 1 ||
                              product.createdUserStatus !== 1) && (
                              <div className={wishlistcss["card-blur"]}>
                                <div className={wishlistcss["blur-text"]}>
                                  currently unavailable
                                </div>
                              </div>
                            )}
                          </div>
                        </Item>
                      </Grid>
                    );
                  })
                ) : (
                  <div>No Products Found!</div>
                )}
              </Grid>
            </Box>
            <br></br>
            <br></br>
            <br></br>
          </div>
        </div>
        <Footer />
      </div>
      {apiCall && <CircleLoader />}
    </>
  );
};

export default WishList;
