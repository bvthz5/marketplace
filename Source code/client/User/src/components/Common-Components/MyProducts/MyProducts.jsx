import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import Box from "@mui/material/Box";
import Paper from "@mui/material/Paper";
import Grid from "@mui/material/Unstable_Grid2";
import { styled } from "@mui/material/styles";
import CommonHeader from "../Header/HeaderComponent/Header";
import style from "./MyProducts.module.css";
import MyImage from "./../../../Assets/images/Image_not_available.png";
import CreateIcon from "@mui/icons-material/Create";
import Footer from "../Footer/Footer";
import { deleteProduct, getAllMyProducts } from "../../../core/Api/apiService";
import Title from "../../Utils/PageTitle/Title";
import ScrollToTop from "../../Utils/ScrollToPageTop/ScrollToTop";
import DeleteForeverIcon from "@mui/icons-material/DeleteForever";
import useWindowDimensions from "../../../hooks/WindowSizeReader/WindowDimensions";

const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;

const MyProducts = () => {
  const id = 0;
  const [products, setProducts] = useState({});
  const { width } = useWindowDimensions();

  let navigate = useNavigate();

  const editClick = (productId) => () => {
    navigate(`/editproductimage/?id=${productId}`);
  };
  useEffect(() => {
    document.title = "Products";

    getProducts();
  }, [id]);

  // api call fetching user added products
  const getProducts = async () => {
    getAllMyProducts(id)
      .then((response) => {
        setProducts(response?.data.data);
      })
      .catch((err) => {
        console.log(err);
        setProducts([]);
      });
  };

  // api call for deleting a product
  const handleDelete = async (productId, productName) => {
    Swal.fire({
      title: `Delete ${productName}?`,
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!",
    }).then(async (result) => {
      if (result.isConfirmed) {
        deleteProduct(productId)
          .then(() => {
            getProducts();
              Swal.fire(
                "Deleted!",
                `${productName} has been deleted.`,
                "success"
              );
          })
          .catch((err) => console.log(err));
      }
    });
  };

  const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: "#fff",
    ...theme.typography.body2,
    padding: theme.spacing(3),
    textAlign: "center",
    color: theme.palette.text.secondary,
  }));

  return (
    <>
      <ScrollToTop />
      <div data-testid="myproductspage" style={{ width: "100%" }}>
        <CommonHeader />
        <Title pageTitle={"My Ads"} />

        <div className={style.productcontaineralign}>
          <div className={style.productcontainer}>
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
                {products.length > 0 ? (
                  products.map((product) => {
                    return (
                      <Grid
                        lg={5}
                        sm={10}
                        style={{ width: "300px" }}
                        key={product.productId}
                      >
                        <Item
                          className={style["data-card"]}
                          key={product.productId}
                          style={{ height: "300px" }}
                        >
                          <div className={style["datas"]}>
                            <div className={style["imagealign"]}>
                              <img
                                src={
                                  product?.thumbnail
                                    ? `${baseImageUrl}${product.thumbnail}`
                                    : MyImage
                                }
                                className={style["image"]}
                                alt=""
                              />
                              {product.status !== 4 &&
                                product.status !== 6 &&
                                product.status !== 3 && (
                                  <CreateIcon
                                    data-testid="editimagebtn"
                                    disabled={product.status === 6}
                                    onClick={editClick(product.productId)}
                                    style={{
                                      width: "18px",
                                      color: "#002f34",
                                      height: "20px",
                                      background: "none",
                                      cursor: "pointer",
                                    }}
                                  />
                                )}
                            </div>
                            <div className={style["fordata"]}>
                              <h3 className={style["h3design"]}>
                                {product.productName}
                              </h3>
                              <h4>{product.categoryName}</h4>
                              <h4>&#8377;{product.price}</h4>
                            </div>
                            {product.status !== 4 && (
                              <div className={style["forButton"]}>
                                <button
                                  data-testid="editdetailsbtn"
                                  className={style["button-13"]}
                                  disabled={product.status === 6}
                                  onClick={() => {
                                    navigate(
                                      `/editproduct/?id=${product.productId}`
                                    );
                                  }}
                                >
                                  {width < 420 ? (
                                    <CreateIcon />
                                  ) : (
                                    "Edit Details"
                                  )}
                                </button>

                                <button
                                  className={style["button-14"]}
                                  data-testid="deletebtn"
                                  disabled={product.status === 6}
                                  onClick={() => {
                                    handleDelete(
                                      product.productId,
                                      product.productName
                                    );
                                  }}
                                >
                                  {width < 420 ? (
                                    <DeleteForeverIcon />
                                  ) : (
                                    "Delete"
                                  )}
                                </button>
                              </div>
                            )}

                            <div className={style["aligninfo"]}>
                              {product.status === 0 && (
                                <span className={style["logged-out"]}>
                                  Rejected
                                </span>
                              )}
                              {product.status === 1 && (
                                <span className={style["logged-in"]}>
                                  Approved
                                </span>
                              )}
                              {product.status === 2 && (
                                <span className={style["logged-pending"]}>
                                  Pending for approval
                                </span>
                              )}

                              {product.status === 4 && (
                                <span className={style["logged-out"]}>
                                  Deleted
                                </span>
                              )}
                              {product.status === 5 && (
                                <span className={style["logged-pending"]}>
                                  draft
                                </span>
                              )}
                              {product.status === 6 && (
                                <span className={style["logged-pending"]}>
                                  Order Processing
                                </span>
                              )}
                            </div>
                            {product.status === 3 && (
                              <div className={style["card-blur"]}>
                                <div className={style["blur-text"]}>Sold</div>
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
          </div>
        </div>
      </div>
      <Footer />
    </>
  );
};

export default MyProducts;
