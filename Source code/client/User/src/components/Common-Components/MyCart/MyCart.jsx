import React, { useEffect, useState, useContext, useCallback } from "react";
import { Tooltip } from "@mui/material";
import MyImage from "./../../../Assets/images/Image_not_available.png";
import Footer from "../Footer/Footer";
import { useNavigate } from "react-router-dom/dist";
import MyCartcss from "./MyCart.module.css";
import CloseIcon from "@mui/icons-material/Close";
import Box from "@mui/material/Box";
import Grid from "@mui/material/Unstable_Grid2";
import { styled } from "@mui/material/styles";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Paper from "@mui/material/Paper";
import CommonHeader from "../Header/HeaderComponent/Header";
import { toast } from "react-toastify";
import { cartCounts } from "../../../App";
import { getAllCart, removeFromCart } from "../../../core/Api/apiService";
import Title from "../../Utils/PageTitle/Title";
import ScrollToTop from "../../Utils/ScrollToPageTop/ScrollToTop";

const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;

const MyCart = () => {
  let navigate = useNavigate();
  const [cart, setCart] = useState([]);
  const [total, setTotal] = useState(0);
  const [, setCount] = useContext(cartCounts);

  const getPriceSum = useCallback(() => {
    let sum = 0;
    let cartstatus = false;
    if (cart.length > 0) {
      cart.forEach((product) => {
        if (product.status === 1 && product.createdUserStatus === 1) {
          sum = sum + product.price;
          setTotal(sum);
          cartstatus = true;
        }
      });
      if (!cartstatus) {
        setTotal(0);
      }
    } else setTotal(0);
  }, [cart]);

  const getCart = useCallback(async () => {
    getAllCart()
      .then((response) => {
        setCart(response.data.data);
        getPriceSum();
        setCount(response.data.data.length);
      })
      .catch((err) => {
        console.log(err);
      });
  }, [getPriceSum, setCount]);

  useEffect(() => {
    document.title = "Cart";
    getCart();
  }, []);

  useEffect(() => {
    getPriceSum();
  }, [cart, getPriceSum]);

  // function for removing a product from cart
  const deleteCartItem = (id) => () => {
    removeFromCart(id)
      .then(() => {
        getCart();
        toast.success("Removed from cart");
      })
      .catch((err) => console.log(err));
  };

  const handleCheckout = () => {
    if (cart.length > 0 && total <= 500000) {
      localStorage.setItem("orderCreated", true);
      navigate("/summary");
    } else
      toast.warning("Cannot checkout with total amount more than 5 Lakh", {
        toastId: 5,
      });
  };

  const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === "#fff",
    ...theme.typography.body2,
    padding: theme.spacing(3),
    textAlign: "center",
    color: theme.palette.text.secondary,
  }));

  const renderTableBody = () => {
    let filteredItemsCount = 0;
    return (
      <TableBody data-testid="carttable">
        {cart.map((product) => {
          if (product.status === 1 && product.createdUserStatus === 1) {
            filteredItemsCount++;
            return (
              <TableRow key={product.productId}>
                <TableCell component="th" scope="row">
                  {filteredItemsCount}
                </TableCell>
                <TableCell>{product.productName}</TableCell>
                <TableCell>{product.price}</TableCell>
              </TableRow>
            );
          }
          return null;
        })}
        <TableRow>
          <TableCell></TableCell>
          <TableCell
            style={{
              fontSize: "18px",
            }}
          >
            Total{" "}
          </TableCell>
          <TableCell
            style={{
              fontSize: "18px",
            }}
          >
            ₹{total}
          </TableCell>
        </TableRow>
      </TableBody>
    );
  };

  return (
    <>
      <ScrollToTop />
      <div data-testid="mycartpage">
        <CommonHeader />
        <Title pageTitle={"My Cart"} />
        <div className={MyCartcss.productcontaineralign}>
          <div className={MyCartcss.productcontainer}>
            <Box sx={{ marginTop: "40px" }}>
              <Grid
                container
                spacing={4}
                style={{ width: "100%", justifyContent: "center" }}
              >
                {cart.length > 0 ? (
                  cart.map((product) => {
                    return (
                      <Grid
                        lg={5}
                        sm={10}
                        style={{ width: "300px" }}
                        key={product.productId}
                        data-testid="productcard"
                      >
                        <Item
                          className={MyCartcss["data-card"]}
                          key={product.productId}
                          style={{ height: "300px" }}
                        >
                          <div className={MyCartcss["datas"]}>
                            <div className={MyCartcss.deletebtndiv}>
                              <div className={MyCartcss.deletebtn}>
                                <Tooltip
                                  title="Remove from Cart"
                                  placement="top"
                                >
                                  <CloseIcon
                                    data-testid="deletebtn"
                                    onClick={deleteCartItem(product.productId)}
                                    style={{
                                      color: "black",
                                      cursor: "pointer",
                                    }}
                                    stroke={"black"}
                                    strokeWidth={1}
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
                              className={MyCartcss["image"]}
                              alt=""
                            />
                            <h3 className={MyCartcss["h3design"]}>
                              {product.productName}
                            </h3>
                            <h4 style={{ margin: "0", marginBottom: "10px" }}>
                              {product.brandName}{" "}
                            </h4>
                            <h4 style={{ margin: "0", marginBottom: "10px" }}>
                              {product.price}&#8377;
                            </h4>
                            <h4 style={{ margin: "0", marginBottom: "10px" }}>
                              {product.categoryName}
                            </h4>
                            <div className={MyCartcss["forButton"]}>
                              <button
                                data-testid="navigatebtn"
                                className={MyCartcss["button-13"]}
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
                              <div className={MyCartcss["card-blur"]}>
                                <div className={MyCartcss["blur-text"]}>
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
            <div className={MyCartcss["amountcontaineralign"]}>
              <div className={MyCartcss["amountcontainer"]}>
                <TableContainer component={Paper}>
                  <Table
                    sx={{ minWidth: 60 }}
                    size="20px"
                    aria-label="a dense table"
                  >
                    <TableHead>
                      <TableRow>
                        <TableCell>No</TableCell>
                        <TableCell>Item Name</TableCell>
                        <TableCell>Price</TableCell>
                      </TableRow>
                    </TableHead>
                    {renderTableBody()}
                  </Table>
                </TableContainer>
              </div>
            </div>
            <div>
              {cart.length > 0 && total > 0 && (
                <button
                  data-testid="placeorderbtn"
                  onClick={handleCheckout}
                  className={MyCartcss["buybtn"]}
                >
                  Place Order
                </button>
              )}
            </div>
            <br></br>
          </div>
        </div>
        <Footer />
      </div>
    </>
  );
};

export default MyCart;
