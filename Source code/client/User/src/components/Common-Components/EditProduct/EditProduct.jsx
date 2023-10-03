import React, { useCallback, useEffect, useState } from "react";
import Editproductcss from "./EditProduct.module.css";
import Footer from "./../Footer/Footer";
import { useForm as UseForm } from "react-hook-form";
import { useNavigate, useSearchParams } from "react-router-dom";
import CommonHeader from "../Header/HeaderComponent/Header";
import Swal from "sweetalert2";
import { toast } from "react-toastify";
import KeyboardBackspaceIcon from "@mui/icons-material/KeyboardBackspace";
import { Tooltip } from "@mui/material";
import { useLoadScript } from "@react-google-maps/api";
import PlacesAutocomplete, {
  geocodeByAddress,
  getLatLng,
} from "react-places-autocomplete";
import {
  editProductDetails,
  getAllCategories,
  getProductById,
} from "../../../core/Api/apiService";
import ScrollToTop from "../../Utils/ScrollToPageTop/ScrollToTop";

const mapKey = process.env.REACT_APP_MAPS_API_KEY;
const libraries = ["places"];
const componentRestrictions = { country: "in" };

function EditProduct() {
  const { isLoaded } = useLoadScript({
    googleMapsApiKey: mapKey,
    libraries: libraries,
  });
  let navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");
  const [categories, setCategories] = useState({});
  const [editingProduct, setEditingProduct] = useState({});
  const [address, setAddress] = useState("");
  const {
    register,
    handleSubmit,
    clearErrors,
    formState: { errors },
    setValue,
  } = UseForm({ mode: "onChange" });

  // api call for fetching product details

  useEffect(() => {
    document.title = "Edit details";

    const getEditingProduct = async () => {
      getProductById(id)
        .then((response) => {
          setAddress(response.data.data?.location?.address);
          setEditingProduct(response.data?.data);
          setValue("location", response.data.data?.location);
          setValue("categoryId", response.data.data.categoryId);
          setValue("brandId", response.data.data.brandId);
          setValue("productName", response.data.data.productName);
          setValue("productDescription", response.data.data.productDescription);
          setValue("price", response.data.data.price);
        })
        .catch((err) => console.log(err));
    };
    // fetching  all  active categories

    const getCategories = () => {
      getAllCategories()
        .then((response) => {
          setCategories(response?.data.data);
          getEditingProduct();
        })
        .catch((err) => console.log(err));
    };
    getCategories();
  }, [id, setValue]);

  useEffect(() => {
    if (address === "") {
      setValue("location", "");
    }
  }, [address, setValue]);

  // function for checking any changes in data
  const findDiffrence = (data) => {
    let diffrence = false;
    if (data) {
      if (data.categoryId !== editingProduct.categoryId) diffrence = true;
      if (data.brandId !== editingProduct.brandId) diffrence = true;
      if (data.productName !== editingProduct.productName) diffrence = true;
      if (data.productDescription !== editingProduct.productDescription)
        diffrence = true;
      if (data.price !== editingProduct.price) diffrence = true;
      if (data.location.address !== editingProduct.location.address)
        diffrence = true;
    }
    return diffrence;
  };

  // function for adding product details  after editing

  const onSubmit = async (data) => {
    let diffrence = findDiffrence(data);
    if (diffrence) {
      editProductDetails(id, data)
        .then(() => {
          Swal.fire({
            icon: "success",
            title: "Saved changes",
          });
          navigate(`/myproducts/?id=0`);
        })
        .catch((err) => {
          console.log(err);
          let error = err.response.data.message;
          if (error === "Product Not Found") {
            toast.error(
              "Error occured while updating the details.Try again later",
              { toastId: "59" }
            );
          }
        });
    } else toast.warning("Their are no changes to update", { toastId: "53" });
  };

  // function for getting latitude and longitude of user selected location
  const handleSelect = useCallback(
    async (value) => {
      const results = await geocodeByAddress(value);
      const ll = await getLatLng(results[0]);
      setAddress(value.split(",").splice(0, 2).join());
      setValue("location", {
        address: value.split(",").splice(0, 2).join(),
        latitude: ll.lat,
        longitude: ll.lng,
      });
      clearErrors("location");
    },
    [clearErrors, setValue]
  );

  const goBack = useCallback(() => navigate(-1), [navigate]);

  return (
    <>
      <ScrollToTop />
      <div data-testid="editproductpage">
        <CommonHeader />
        <div className={Editproductcss.addproductbackground}>
          <div className={Editproductcss.boxaddproduct}>
            <div className={Editproductcss.heading}>
              <div className={Editproductcss.backicondiv}>
                <Tooltip title="Go back">
                  <KeyboardBackspaceIcon
                    className={Editproductcss.backicon}
                    onClick={goBack}
                  />
                </Tooltip>
              </div>
              <h2>POST YOUR AD</h2>
            </div>
            <form
              className={Editproductcss.form}
              onSubmit={handleSubmit(onSubmit)}
            >
              <div className={Editproductcss.rows}>
                <span>
                  Category<span className={Editproductcss.required}>*</span>
                </span>
                <select
                  className={`${Editproductcss["inputs"]} ${Editproductcss["category"]}`}
                  {...register("categoryId", {
                    required: "Choose a Category",
                  })}
                >
                  <option value={""} defaultChecked hidden disabled>
                    select category
                  </option>
                  {categories.length > 0 ? (
                    categories.map((category) => {
                      return (
                        <option
                          key={category.categoryId}
                          value={category.categoryId}
                          className={Editproductcss.categoryoption}
                        >
                          {category.categoryName}
                        </option>
                      );
                    })
                  ) : (
                    <option disabled value="">
                      categories not found
                    </option>
                  )}
                </select>
                <div>
                  {errors.categoryId && (
                    <small className={Editproductcss.required}>
                      {errors.categoryId.message}
                    </small>
                  )}
                </div>
              </div>

              <div className={Editproductcss.rows}>
                <span>
                  Product Name
                  <span className={Editproductcss.required}>*</span>
                </span>
                <input
                  data-testid="productname-input"
                  type="text"
                  name="productName"
                  className={Editproductcss["inputs"]}
                  {...register("productName", {
                    required: "Product Name required ",
                    maxLength: {
                      value: 200,
                      message: "Maximum 200 characters allowed",
                    },
                    pattern: {
                      value: /^[a-z0-9!@%&()_\-,."'+|:/\s]+$/i,
                      message: "Invalid Product Name",
                    },
                  })}
                />
                <div>
                  {errors.productName && (
                    <small className={Editproductcss.required}>
                      {errors.productName.message}
                    </small>
                  )}
                </div>
              </div>
              <div
                className={`${Editproductcss["rows"]} ${Editproductcss["txtrow"]}`}
              >
                <span>
                  Product Description
                  <span className={Editproductcss.required}>*</span>
                </span>
                <textarea
                  data-testid="description-input"
                  name="productDescription"
                  className={`${Editproductcss["inputs"]}  ${Editproductcss["txtarea"]}`}
                  {...register("productDescription", {
                    required: "Product description required ",
                    maxLength: {
                      value: 1000,
                      message: "Maximum 1000 characters allowed",
                    },
                  })}
                />
                <div>
                  {errors.productDescription && (
                    <small className={Editproductcss.required}>
                      {errors.productDescription.message}
                    </small>
                  )}
                </div>
              </div>
              <div className={Editproductcss.rows}>
                <span>
                  Price<span className={Editproductcss.required}>*</span>
                </span>
                <input
                  data-testid="price-input"
                  className={Editproductcss["inputs"]}
                  placeholder="Enter a price greater than  Rs 99"
                  type="text"
                  min="100.00"
                  step="0.01"
                  {...register("price", {
                    required: "Price of the product is required ",
                    min: {
                      value: 100,
                      message: "price should be greater than 99",
                    },
                    max: {
                      value: 500000,
                      message: "price should be less than 500000",
                    },
                    pattern: {
                      value: /^\d+(\.\d+)?$/i,
                      message: "Invalid Price format",
                    },
                  })}
                />
                <div>
                  {errors.price && (
                    <small className={Editproductcss.required}>
                      {errors.price.message}
                    </small>
                  )}
                </div>
              </div>

              <div className={Editproductcss.rows}>
                <span>
                  Location<span className={Editproductcss.required}>*</span>
                </span>
                <div>
                  {isLoaded && (
                    <PlacesAutocomplete
                      searchOptions={{ componentRestrictions }}
                      highlightFirstSuggestion
                      value={address}
                      onChange={setAddress}
                      onSelect={handleSelect}
                    >
                      {({
                        getInputProps,
                        suggestions,
                        getSuggestionItemProps,
                        loading,
                      }) => (
                        <div key={suggestions.description}>
                          <input
                            data-testid="location-input"
                            {...register("location", {
                              required: "location is required",
                            })}
                            {...getInputProps({
                              placeholder: "Search Places ...",
                              className: Editproductcss.inputs,
                            })}
                          />
                          <div className="autocomplete-dropdown-container">
                            {loading && <div>Loading...</div>}
                            {suggestions.map((suggestion) => {
                              const style = suggestion.active
                                ? {
                                    backgroundColor: "#d3d3d3",
                                    cursor: "pointer",
                                  }
                                : {
                                    backgroundColor: "#ffffff",
                                    cursor: "pointer",
                                  };
                              return (
                                <div
                                  className={Editproductcss["input-suggestion"]}
                                  key={suggestion.placeId}
                                  {...getSuggestionItemProps(suggestion, {
                                    style,
                                  })}
                                >
                                  <i className="material-icons">location_on </i>
                                  <span>
                                    {suggestion.description
                                      .split(",")
                                      .splice(0, 2)
                                      .join()}
                                  </span>
                                </div>
                              );
                            })}
                          </div>
                        </div>
                      )}
                    </PlacesAutocomplete>
                  )}
                  <div>
                    {errors.location && (
                      <small className={Editproductcss.required}>
                        {errors.location.message}
                      </small>
                    )}
                  </div>
                </div>
              </div>

              <button
                data-testid="submitbtn"
                type="submit"
                value="submit"
                className={Editproductcss.button}
              >
                Update
              </button>
            </form>
          </div>
        </div>
        <Footer />
      </div>
    </>
  );
}

export default EditProduct;
