import React, { useEffect, useState, useCallback } from "react";
import Addproductcss from "./Addproduct.module.css";
import Footer from "./../Footer/Footer";
import { useForm as UseForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import CommonHeader from "../Header/HeaderComponent/Header";
import { Tooltip } from "@mui/material";
import KeyboardBackspaceIcon from "@mui/icons-material/KeyboardBackspace";
import { useLoadScript } from "@react-google-maps/api";
import PlacesAutocomplete, {
  geocodeByAddress,
  getLatLng,
} from "react-places-autocomplete";
import { toast } from "react-toastify";
import {
  addProductDetails,
  getAllCategories,
} from "../../../core/Api/apiService";
import ScrollToTop from "../../Utils/ScrollToPageTop/ScrollToTop";

const mapKey = process.env.REACT_APP_MAPS_API_KEY;
const libraries = ["places"];

const componentRestrictions = { country: "in" };

function Addproduct() {
  const { isLoaded } = useLoadScript({
    googleMapsApiKey: mapKey,
    libraries: libraries,
  });
  let navigate = useNavigate();
  const [categories, setCategories] = useState({});
  const [address, setAddress] = useState("");
  const {
    register,
    handleSubmit,
    setValue,
    clearErrors,
    formState: { errors },
  } = UseForm({ mode: "onChange" });

  useEffect(() => {
    document.title = "Addproduct";

    getCategories();
  }, []);

  useEffect(() => {
    if (address === "") {
      setValue("location", "");
    }
  }, [address,setValue]);

  // fetching all active categories
  const getCategories = () => {
    getAllCategories()
      .then((response) => {
        setCategories(response?.data.data);
      })
      .catch((err) => console.log(err));
  };

  // function for adding product details
  const onSubmit = async (e) => {
    addProductDetails(e)
      .then((response) => {
        localStorage.setItem("addImage", true);
        toast.success("Saved as draft");
        navigate(`/productimage?id=${response?.data?.data.productId}`);
      })
      .catch((err) => console.log(err));
  };

  // function for getting latitude and longitude of user selected location
  const handleSelect = useCallback(async (value) => {
    const results = await geocodeByAddress(value);
    const ll = await getLatLng(results[0]);
    setAddress(value.split(",").splice(0, 2).join());
    setValue("location", {
      address: value.split(",").splice(0, 2).join(),
      latitude: ll.lat,
      longitude: ll.lng,
    });
    clearErrors("location");
  }, [clearErrors,setValue]);

  const goBack = useCallback(() => navigate(-1), [navigate]);

  return (
    <>
      <ScrollToTop />
      <div data-testid="addproductpage">
        <CommonHeader />
        <div className={Addproductcss.addproductbackground}>
          <div className={Addproductcss.boxaddproduct}>
            <div className={Addproductcss.heading}>
              <div className={Addproductcss.backicondiv}>
                <Tooltip title="Go back">
                  <KeyboardBackspaceIcon
                    className={Addproductcss.backicon}
                    onClick={goBack}
                    data-testid="gobackbutton"
                  />
                </Tooltip>
              </div>
              <h2>POST YOUR AD</h2>
            </div>
            <form
              className={Addproductcss.form}
              onSubmit={handleSubmit(onSubmit)}
            >
              <div className={Addproductcss.rows}>
                <span>
                  Category<span className={Addproductcss.required}>*</span>
                </span>
                <select
                  className={`${Addproductcss["inputs"]} ${Addproductcss["category"]}`}
                  {...register("categoryId", {
                    required: "Choose a Category",
                  })}
                >
                  <option value={""} defaultChecked hidden>
                    Select category
                  </option>
                  {categories.length > 0 ? (
                    categories.map((category) => {
                      return (
                        <option
                          className={Addproductcss.categoryoption}
                          key={category.categoryId}
                          value={category.categoryId}
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
                    <small className={Addproductcss.required}>
                      {errors.categoryId.message}
                    </small>
                  )}
                </div>
              </div>

              <div className={Addproductcss.rows}>
                <span>
                  Product Name <span className={Addproductcss.required}>*</span>
                </span>
                <input
                  data-testid="productname-input"
                  placeholder="Enter Product name with details"
                  type="text"
                  className={Addproductcss["inputs"]}
                  {...register("productName", {
                    required: "Product Name is required ",
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
                    <small className={Addproductcss.required}>
                      {errors.productName.message}
                    </small>
                  )}
                </div>
              </div>
              <div
                className={`${Addproductcss["rows"]} ${Addproductcss["txtrow"]}`}
              >
                <span>
                  Product Description
                  <span className={Addproductcss.required}>*</span>
                </span>
                <textarea
                  data-testid="description-input"
                  placeholder="Enter Detailed description"
                  className={`${Addproductcss["inputs"]} ${Addproductcss["txtarea"]}`}
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
                    <small className={Addproductcss.required}>
                      {errors.productDescription.message}
                    </small>
                  )}
                </div>
              </div>
              <div className={Addproductcss.rows}>
                <span>
                  Price<span className={Addproductcss.required}>*</span>
                </span>
                <input
                  data-testid="price-input"
                  className={Addproductcss["inputs"]}
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
                    <small className={Addproductcss.required}>
                      {" "}
                      {errors.price.message}
                    </small>
                  )}
                </div>
              </div>
              <div className={Addproductcss.rows}>
                <span>
                  Location<span className={Addproductcss.required}>*</span>
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
                            {...register("location", {
                              required: "location is required",
                            })}
                            {...getInputProps({
                              placeholder: "Search Places ...",
                              className: Addproductcss.inputs,
                            })}
                          />
                          <div className="autocomplete-dropdown-container">
                            {loading && <div>Loading...</div>}
                            {suggestions.map((suggestion, index) => {
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
                                  className={Addproductcss["input-suggestion"]}
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
                </div>
                <div>
                  {errors.location && (
                    <small className={Addproductcss.required}>
                      {errors.location.message}
                    </small>
                  )}
                </div>
              </div>

              <button
                data-testid="submitbtn"
                type="submit"
                value="submit"
                className={Addproductcss.button}
              >
                Submit and Continue
              </button>
            </form>
          </div>
        </div>
      </div>
      <Footer />
    </>
  );
}

export default Addproduct;
