import { useForm } from "react-hook-form";
import React, { useState, useEffect } from "react";
import { toast } from "react-toastify";
import Editprofilecss from "./EditProfile.module.css";
import { updateUserDetails } from "../../../../core/Api/apiService";
import { fetchUserDetails, userDetails } from "../userDetailSlice";
import { useDispatch, useSelector } from "react-redux";

const initialUserData = {
  firstName: "",
  lastName: "",
  address: "",
  state: "",
  district: "",
  city: "",
  phoneNumber: "",
};

const EditProfile = () => {
  const dispatch = useDispatch();
  const userData = useSelector(userDetails);
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    reset,
  } = useForm({
    mode: "onChange",
    defaultValues: { userData: initialUserData },
  });
  const [editing, setEditing] = useState(false);

  useEffect(() => {
    dispatch(fetchUserDetails());
  }, [dispatch]);

  useEffect(() => {
    const { firstName, lastName, address, state, district, city, phoneNumber } =
      { ...userData };
    setValue("userData", {
      firstName,
      lastName,
      address,
      state,
      district,
      city,
      phoneNumber,
    });
  }, [userData, dispatch, setValue]);

  // function for submiting  edited userdetails
  const onSubmit = async (data) => {
    if (data.userData?.firstName.trim() === "") {
      toast.error("Whitespaces are not allowed!", { toastId: "whitespace" });
      return;
    }
    updateUserDetails(data?.userData)
      .then(() => {
        dispatch(fetchUserDetails());
        setEditing(false);
        toast.success("Details updated");
      })
      .catch((err) => {
        console.log(err);
        setEditing(false);
      });
  };

  return (
    <div data-testid="editprofile" className={Editprofilecss["maindiv"]}>
      <h2>Edit Profile</h2>
      <form
        className={Editprofilecss["form"]}
        onSubmit={handleSubmit(onSubmit)}
      >
        {/* row 1 */}
        <div className={Editprofilecss["formrow"]}>
          <div className={Editprofilecss["formfield"]}>
            <span className={Editprofilecss["span"]}>
              First Name
              <span className={Editprofilecss["required"]}>*</span>
            </span>
            <input
              data-testid="fname-input"
              className={Editprofilecss["summaryinput"]}
              disabled={!editing}
              type="text"
              name="name"
              placeholder="First Name"
              {...register("userData.firstName", {
                required: "First Name is required",
                maxLength: {
                  value: 60,
                  message: "Maximum 60 characters",
                },
              })}
            />
            <span className={Editprofilecss["span"]}>
              {errors?.userData?.firstName ? (
                <small className={Editprofilecss["required"]}>
                  {errors.userData.firstName.message}
                </small>
              ) : (
                <br />
              )}
            </span>
          </div>

          <div className={Editprofilecss["formfield"]}>
            <span className={Editprofilecss["span"]}>Last Name</span>
            <input
              data-testid="lname-input"
              className={Editprofilecss["summaryinput"]}
              disabled={!editing}
              type="text"
              name="name"
              placeholder="Last Name"
              {...register("userData.lastName", {
                maxLength: {
                  value: 60,
                  message: "Maximum 60 characters",
                },
              })}
            />
            <span className={Editprofilecss["span"]}>
              {errors?.userData?.lastName ? (
                <small className={Editprofilecss["required"]}>
                  {errors.userData.lastName.message}
                </small>
              ) : (
                <br />
              )}
            </span>
          </div>
        </div>

        {/* row2 */}
        <div className={Editprofilecss["formrow"]}>
          <div className={Editprofilecss["formaddressfield"]}>
            <span className={Editprofilecss["span"]}>Address</span>
            <textarea
              data-testid="address-input"
              className={Editprofilecss["summarytextarea"]}
              disabled={!editing}
              type="text"
              name="address"
              placeholder="House name and street name"
              {...register("userData.address", {
                maxLength: {
                  value: 255,
                  message: "Maxlength is 255 characters ",
                },
                pattern: {
                  value: /^[a-z0-9\s]*$/i,
                  message: "Only Alphabets, Numbers and spaces are allowed",
                },
              })}
            />
            <span className={Editprofilecss["span"]}>
              {errors?.userData?.address ? (
                <small className={Editprofilecss["required"]}>
                  {errors.userData.address.message}
                </small>
              ) : (
                <br />
              )}
            </span>
          </div>
        </div>

        {/* row3 */}
        <div className={Editprofilecss["formrow"]}>
          <div className={Editprofilecss["formfield"]}>
            <span className={Editprofilecss["span"]}>City</span>
            <input
              data-testid="city-input"
              className={Editprofilecss["summaryinput"]}
              disabled={!editing}
              type="text"
              name="city"
              placeholder="City"
              {...register("userData.city", {
                maxLength: {
                  value: 50,
                  message: "Maxlength is 50 characters",
                },
                pattern: {
                  value: /^[a-z0-9\s]*$/i,
                  message: "Only Alphabets, Numbers and spaces are allowed",
                },
              })}
            />
            <span className={Editprofilecss["span"]}>
              {errors?.userData?.city ? (
                <small className={Editprofilecss["required"]}>
                  {errors.userData.city.message}
                </small>
              ) : (
                <br />
              )}
            </span>
          </div>

          <div className={Editprofilecss["formfield"]}>
            <span className={Editprofilecss["span"]}>District</span>
            <input
              data-testid="district-input"
              className={Editprofilecss["summaryinput"]}
              disabled={!editing}
              type="text"
              name="district"
              placeholder="district"
              {...register("userData.district", {
                maxLength: {
                  value: 50,
                  message: "Maxlength is 50 characters",
                },
                pattern: {
                  value: /^[a-z0-9\s]*$/i,
                  message: "Only Alphabets, Numbers and spaces are allowed",
                },
              })}
            />
            <span className={Editprofilecss["span"]}>
              {errors?.userData?.district ? (
                <small className={Editprofilecss["required"]}>
                  {errors.userData.district.message}
                </small>
              ) : (
                <br />
              )}
            </span>
          </div>
        </div>

        {/* row4 */}
        <div className={Editprofilecss["formrow"]}>
          <div className={Editprofilecss["formfield"]}>
            <span className={Editprofilecss["span"]}>State</span>
            <input
              data-testid="state-input"
              className={Editprofilecss["summaryinput"]}
              disabled={!editing}
              type="text"
              name="state"
              placeholder="State"
              {...register("userData.state", {
                maxLength: {
                  value: 50,
                  message: "Maxlength is 50 characters",
                },
                pattern: {
                  value: /^[a-z0-9\s]*$/i,
                  message: "Only Alphabets, Numbers and spaces are allowed",
                },
              })}
            />
            <span className={Editprofilecss["span"]}>
              {errors?.userData?.state ? (
                <small className={Editprofilecss["required"]}>
                  {errors.userData.state.message}
                </small>
              ) : (
                <br />
              )}
            </span>
          </div>
          <div className={Editprofilecss["formfield"]}>
            <span className={Editprofilecss["span"]}>Phone</span>
            <input
              data-testid="phone-input"
              className={Editprofilecss["summaryinput"]}
              disabled={!editing}
              type="tel"
              name="phoneNo"
              onInput={(e) => {
                e.target.value = e.target.value?.replace(/\D/g, "");
              }}
              placeholder="10 digit phone number"
              {...register("userData.phoneNumber", {
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
            <span className={Editprofilecss["span"]}>
              {errors?.userData?.phoneNumber ? (
                <small className={Editprofilecss["required"]}>
                  {errors.userData.phoneNumber.message}
                </small>
              ) : (
                <br />
              )}
            </span>
          </div>
        </div>

        {/* row6 */}
        <div className={Editprofilecss["formrow"]}></div>
        <div className={Editprofilecss["formaddressfield"]}>
          <div className={Editprofilecss["addrssubmitdiv"]}>
            {!editing && (
              <button
                data-testid="updatebtn"
                type="button"
                value="cancel"
                className={Editprofilecss["formbtnsubmit"]}
                onClick={() => {
                  setEditing(true);
                }}
              >
                Update
              </button>
            )}
            {editing && (
              <button
                data-testid="submitbtn"
                type="submit"
                value="submit"
                className={Editprofilecss["formbtnsubmit"]}
              >
                Submit
              </button>
            )}
            {editing && (
              <button
                data-testid="cancelbtn"
                type="button"
                value="cancel"
                className={Editprofilecss["formbtncancel"]}
                onClick={() => {
                  reset();
                  setEditing(false);
                  dispatch(fetchUserDetails());
                }}
              >
                Cancel
              </button>
            )}
          </div>
        </div>
      </form>
    </div>
  );
};

export default EditProfile;
