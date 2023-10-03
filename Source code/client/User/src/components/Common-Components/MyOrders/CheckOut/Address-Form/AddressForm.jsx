import { useForm } from "react-hook-form";
import React, { useEffect, useState } from "react";
import AddressFormStyle from "./AddressFormStyle.module.css";
import { toast } from "react-toastify";
import {
  addDeliveryAddress,
  editDeliveryAddress,
  getDeliveryAddressById,
} from "../../../../../core/Api/apiService";
import CircleLoader from "../../../../Utils/Loaders/CircleLoader/CircleLoader";

const AddressForm = ({ handleAddAddressDiv, editId }) => {
  const [apiCall, setApiCall] = useState(false);
  const {
    register,
    handleSubmit,
    setValue,
    reset,
    formState: { errors },
  } = useForm({ mode: "onChange" });

  useEffect(() => {
    const getAddress = async () => {
      setApiCall(true);
      getDeliveryAddressById(editId)
        .then((response) => {
          setApiCall(false);
          const data = response.data.data;
          console.log(data);
          setValue("name", data.name);
          setValue("phone", data.phoneNumber);
          setValue("address", data.address);
          setValue("city", data.city);
          setValue("state", data.state);
          setValue("streetAddress", data.streetAddress);
          setValue("zipCode", data.zipCode);
        })
        .catch((err) => {
          setApiCall(false);
          toast.error("Error Occured.Try Again");
          console.log(err);
        });
    };
    window.scrollTo({ top: document.body.scrollHeight, behavior: "smooth" });
    if (editId) {
      getAddress();
    }
  }, [editId, setValue]);

  const addAddress = async (e) => {
    let address = e;
    if (editId) {
      setApiCall(true);
      editDeliveryAddress(editId, address)
        .then(() => {
          setApiCall(false);
          toast.success("Address Updated");
          handleAddAddressDiv();
          reset();
        })
        .catch((err) => {
          setApiCall(false);
          toast.error("Error Occured.Try Again");
          console.log(err);
        });
    } else {
      setApiCall(true);
      addDeliveryAddress(address)
        .then(() => {
          setApiCall(false);
          toast.success("New address added");
          handleAddAddressDiv();
        })
        .catch((err) => {
          setApiCall(false);
          let error = err.response.data.message;
          let limitError = "Address limit exceeded";
          if (error === limitError) {
            toast.warning("You can only add upto 5 addresses", {
              toastId: 10,
            });
          } else {
            toast.error("Error Occured.Try Again", {
              toastId: 11,
            });
          }
        });
      reset();
    }
  };

  

  return (
    <>
      <div data-testid="addressform" style={{ margin: "auto" }}>
        <form
          className={AddressFormStyle["form"]}
          onSubmit={handleSubmit(addAddress)}
        >
          <div className={AddressFormStyle["addressform"]}>
            <div>
              {/* row 1 */}
              <div className={AddressFormStyle["formrow"]}>
                <div className={AddressFormStyle["formfield"]}>
                  <span className={AddressFormStyle["span"]}>
                    Name <span className={AddressFormStyle["required"]}>*</span>
                  </span>
                  <input
                    data-testid="name-input"
                    className={AddressFormStyle["summaryinput"]}
                    type="text"
                    name="name"
                    label="name"
                    placeholder="Full Name"
                    {...register("name", {
                      required: "Name is required",
                      maxLength: {
                        value: 60,
                        message: "Maximum 60 characters",
                      },
                    })}
                  />
                  <span>
                    {errors.name ? (
                      <small
                        className={AddressFormStyle["error"]}
                      >
                        {errors.name.message}
                      </small>
                    ) : (
                      <br />
                    )}
                  </span>
                </div>
                <div className={AddressFormStyle["formfield"]}>
                  <span className={AddressFormStyle["span"]}>
                    Phone{" "}
                    <span className={AddressFormStyle["required"]}>*</span>
                  </span>
                  <input
                    data-testid="phone-input"
                    className={AddressFormStyle["summaryinput"]}
                    type="tel"
                    name="phoneNo"
                    onInput={(e) => {
                      e.target.value = e.target.value?.replace(/\D/g, "");
                    }}
                    placeholder="10 digit phone number"
                    {...register("phone", {
                      required: "Phone number is required",
                      minLength: {
                        value: 10,
                        message: "Not a valid  phone number",
                      },
                      maxLength: {
                        value: 10,
                        message: "Not a valid  phone number",
                      },
                    })}
                  />
                  <span>
                    {errors.phone ? (
                      <small
                      className={AddressFormStyle["error"]}>
                        {errors.phone.message}
                      </small>
                    ) : (
                      <br />
                    )}
                  </span>
                </div>
              </div>

              {/* row2 */}
              <div className={AddressFormStyle["formrow"]}>
                <div className={AddressFormStyle["formfield"]}>
                  <span className={AddressFormStyle["span"]}>
                    Address
                    <span className={AddressFormStyle["required"]}>*</span>
                  </span>
                  <textarea
                    data-testid="address-input"
                    className={AddressFormStyle["summarytextarea"]}
                    type="text"
                    name="address"
                    placeholder="House name and street name"
                    {...register("address", {
                      required: "Address is required",
                      maxLength: {
                        value: 200,
                        message: "Maxlength is 200 characters ",
                      },
                      pattern: {
                        value: /^[a-z0-9()_\-,.'&:/\s]+$/i,
                        message: "Invalid Address Format",
                      },
                    })}
                  />
                  <span>
                    {errors.address ? (
                      <small
                       className={AddressFormStyle["error"]}>
                        {errors.address.message}
                      </small>
                    ) : (
                      <br />
                    )}
                  </span>
                </div>
              </div>

              {/* row3 */}
              <div className={AddressFormStyle["formrow"]}>
                <div className={AddressFormStyle["formfield"]}>
                  <span className={AddressFormStyle["span"]}>
                    Town / City{" "}
                    <span className={AddressFormStyle["required"]}>*</span>
                  </span>
                  <input
                    data-testid="city-input"
                    className={AddressFormStyle["summaryinput"]}
                    type="text"
                    name="city"
                    placeholder="City/Town"
                    {...register("city", {
                      required: "City is required",
                      maxLength: {
                        value: 50,
                        message: "Maxlength is 50 characters",
                      },
                      pattern: {
                        value: /^[a-z0-9()&,'\-\s]+$/i,
                        message: "Invalid City Name",
                      },
                    })}
                  />
                  <span>
                    {errors.city ? (
                      <small
                       className={AddressFormStyle["error"]}>
                        {errors.city.message}
                      </small>
                    ) : (
                      <br />
                    )}
                  </span>
                </div>
                <div className={AddressFormStyle["formfield"]}>
                  <span className={AddressFormStyle["span"]}>
                    State
                    <span className={AddressFormStyle["required"]}>*</span>
                  </span>
                  <input
                    data-testid="state-input"
                    className={AddressFormStyle["summaryinput"]}
                    type="text"
                    name="state"
                    placeholder="State"
                    {...register("state", {
                      required: "State is required",
                      maxLength: {
                        value: 50,
                        message: "Maxlength is 50 characters",
                      },
                      pattern: {
                        value: /^[a-z0-9()&,'\-\s]+$/i,
                        message: "Invalid State Name",
                      },
                    })}
                  />
                  <span>
                    {errors.state ? (
                      <small
                       className={AddressFormStyle["error"]}>
                        {errors.state.message}
                      </small>
                    ) : (
                      <br />
                    )}
                  </span>
                </div>
              </div>

              {/* row4 */}
              <div className={AddressFormStyle["formrow"]}>
                <div className={AddressFormStyle["formfield"]}>
                  <span className={AddressFormStyle["span"]}>
                    Street Address
                    <span className={AddressFormStyle["required"]}>*</span>
                  </span>
                  <input
                    data-testid="street-input"
                    className={AddressFormStyle["summaryinput"]}
                    type="text"
                    name="locality"
                    placeholder="Street Adress / Locality"
                    {...register("streetAddress", {
                      required: "Street Address is required",
                      maxLength: {
                        value: 50,
                        message: "Maxlength is 50 characters",
                      },
                      pattern: {
                        value: /^[a-z0-9()_\-,'&:/.\s]+$/i,
                        message: "Invalid Street Address Format",
                      },
                    })}
                  />
                  {errors.streetAddress ? (
                    <small  
                    className={AddressFormStyle["error"]}>
                      {errors.streetAddress.message}
                    </small>
                  ) : (
                    <br />
                  )}
                </div>

                <div className={AddressFormStyle["formfield"]}>
                  <span className={AddressFormStyle["span"]}>
                    PostCode / ZIP
                    <span className={AddressFormStyle["required"]}>*</span>
                  </span>
                  <input
                    data-testid="zipcode-input"
                    className={AddressFormStyle["summaryinput"]}
                    type="text"
                    name="zipCode"
                    onInput={(e) => {
                      e.target.value = e.target.value?.replace(/\D/g, "");
                    }}
                    placeholder="Enter 6 digit Zipcode"
                    {...register("zipCode", {
                      required: "ZipCode is required",
                      minLength: {
                        value: 6,
                        message: "Invalid Zipcode",
                      },
                      maxLength: {
                        value: 6,
                        message: "Invalid Zipcode",
                      },
                    })}
                  />
                  {errors.zipCode ? (
                    <small className={AddressFormStyle["error"]}>
                      {errors.zipCode.message}
                    </small>
                  ) : (
                    <br />
                  )}
                </div>
              </div>
              <div className={AddressFormStyle["formfield"]}>
                <div className={AddressFormStyle["addrssubmitdiv"]}>
                  <button
                    data-testid="submitbutton"
                    type="submit"
                    value="submit"
                    className={AddressFormStyle["formbtnsubmit"]}
                  >
                    Submit
                  </button>
                  <button
                    type="button"
                    value="cancel"
                    className={AddressFormStyle["formbtncancel"]}
                    onClick={handleAddAddressDiv}
                  >
                    Cancel
                  </button>
                </div>
              </div>
            </div>
          </div>
        </form>
      </div>
      {apiCall && <CircleLoader />}
    </>
  );
};

export default AddressForm;
