import React, { useCallback, useEffect, useState } from "react";
import { FaShoppingCart } from "react-icons/fa";
import { useForm as UseForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import Registercss from "./register.module.css";
import { MdVisibility, MdVisibilityOff } from "react-icons/md";
import KeyboardBackspaceIcon from "@mui/icons-material/KeyboardBackspace";
import Swal from "sweetalert2";
import { toast } from "react-toastify";
import { Tooltip } from "@mui/material";
import { registerUser, resendVerificationMail } from "../Api/coreApiService";
import GoogleButton from "../login/login-page/GoogleButton/GoogleButton";
import CircleLoader from "../../components/Utils/Loaders/CircleLoader/CircleLoader";
import { PASSWORD_PATTERN } from "../../components/Utils/Data/Data";

function Register() {
  const [passwordShown, setPasswordShown] = useState(false);
  const [confpasswordShown, setconfPasswordShown] = useState(false);
  const [loading, setLoading] = useState(false);
  const [passwordMatch, setPasswordMatch] = useState(false);

  let navigate = useNavigate();
  const loginClick = () => {
    navigate("/login");
  };
  const {
    register,
    handleSubmit,
    formState: { errors },
    getValues,
    reset,
    setError,
  } = UseForm({
    mode: "onChange",
    defaultValues: {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
  });

  useEffect(() => {
    document.title = "CART_IN-Register";
  }, []);

  // function user registration by email and password
  const onSubmit = (data) => {
    if (data.password !== data.confirmPassword) {
      return;
    }
    if (data.firstName.trim() === "") {
      toast.error("Whitespaces are not allowed!");
    } else {
      registerUser(data)
        .then(() => {
          setLoading(false);
          Swal.fire({
            icon: "success",
            title: "Successfully Registered",
            text: `An Email has been sent to ${data.email} for verification`,
            showConfirmButton: true,
          });
          setLoading(false);
          navigate("/login");
        })
        .catch((err) => {
          const error = err.response.data.message;
          handleErrors(error, data.email);
          console.log(err);
        });
      setLoading(false);
      reset();
    }
  };

  const handleErrors = (error, email) => {
    let errMsg = "User Already Exists";
    const inactiveError = "Inactive User";
    const blockedError = "Blocked User";
    if (error === errMsg) {
      Swal.fire({
        title: "Already Registered",
        text: "Try Logging In",
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
    } else if (error === inactiveError) {
      Swal.fire({
        icon: "error",
        title: "Account Alredy Exists.Email not verified yet!",
        text: "If you didn't recieve an email,click Resend.  ",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Resend-Email",
      }).then(async (result) => {
        if (result.isConfirmed) {
          resendVerificationMail(email)
            .then((response) => {
              toast.success("Email sent successfully");
            })
            .catch((err) => {
              toast.error("Error occured! Please try again.");
              console.log(err);
            });
        }
      });
    } else if (error === blockedError) {
      Swal.fire({
        icon: "error",
        title: "Blocked",
        text: "You have been blocked from this website",
      });
    }
  };

  const togglePassword = () => {
    setPasswordShown(!passwordShown);
  };
  const toggleconfirmPassword = () => {
    setconfPasswordShown(!confpasswordShown);
  };

  const goBack = useCallback(() => navigate(-1), [navigate]);

  return (
    <>
      <div className={Registercss.formdiv} data-testid="registerpage">
        <div className={Registercss.box}>
          <Tooltip title="Go back">
            <KeyboardBackspaceIcon
              className={Registercss.backicon}
              data-testid="back-button"
              onClick={goBack}
            />
          </Tooltip>
          <form
            onSubmit={handleSubmit(onSubmit)}
            className={Registercss.boxregister}
          >
            <h2 className={Registercss.carttitle}>
              CART.IN <FaShoppingCart />
            </h2>
            <br />
            <label>
              <span className={Registercss.cart}>
                <span className={Registercss.error}></span>
              </span>
              <input
                data-testid="fname-input"
                type="text"
                autoComplete="off"
                placeholder=" First Name "
                {...register("firstName", {
                  required: "First Name required ",
                  maxLength: {
                    value: 60,
                    message: "Maximum 60 characters allowed",
                  },
                })}
              />
              {errors.firstName && (
                <small className={Registercss.error}>
                  {errors.firstName.message}
                </small>
              )}
            </label>
            <br></br>
            <label>
              <span className={Registercss.cart}></span>
              <input
                data-testid="lname-input"
                type="text"
                autoComplete="off"
                placeholder="Last Name"
                {...register("lastName", {
                  maxLength: {
                    value: 60,
                    message: "Maximum 60 characters allowed",
                  },
                })}
              />
              {errors.lastName && (
                <span className={Registercss.error}>
                  {errors.lastName.message}
                </span>
              )}
            </label>
            <br></br>
            <label>
              <span className={Registercss.cart}>
                <span className={Registercss.error}></span>
              </span>

              <input
                data-testid="email-input"
                type="text"
                placeholder="Email"
                autoComplete="off"
                {...register("email", {
                  required: "Email required ",
                  pattern: {
                    value: /^[A-Z0-9._%+-]+@[A-z0-9.-]+\.[A-Z]{2,254}$/i,
                    message: "Invalid Email address",
                  },
                  maxLength: {
                    value: 254,
                    message: "Max length exceeded",
                  },
                })}
              />
              {errors.email && (
                <small className={Registercss.error}>
                  {errors.email.message}
                </small>
              )}
            </label>
            <br></br>
            <label>
              <span className={Registercss.cart}>
                <span className={Registercss.error}></span>
              </span>
              <div>
                <input
                  data-testid="password-input"
                  className={Registercss["pswdbox"]}
                  type={passwordShown ? "text" : "password"}
                  autoComplete="new-password"
                  placeholder=" Password"
                  {...register("password", {
                    required: "Password Required",
                    onChange: (value) => {
                      let password = getValues("confirmPassword");
                      if (password) {
                        if (value.target.value !== password) {
                          setPasswordMatch(true);
                        } else setPasswordMatch(false);
                      }
                    },
                    pattern: {
                      value: PASSWORD_PATTERN,
                      message:
                        "Must contain minimum 8 and maximum 16 characters, at least one uppercase letter, one lowercase letter, one number and one special character",
                    },
                  })}
                />
                <div className={Registercss.eyebutton}>
                  <div className={Registercss.eyebuttons}>
                    {passwordShown ? (
                      <p
                        onClick={togglePassword}
                        data-testid="eyebtn-password-close"
                        style={{ cursor: "pointer" }}
                      >
                        <MdVisibility style={{ width: "18px" }} />
                      </p>
                    ) : (
                      <p
                        onClick={togglePassword}
                        data-testid="eyebtn-password-open"
                        style={{ cursor: "pointer" }}
                      >
                        <MdVisibilityOff style={{ width: "18px" }} />
                      </p>
                    )}
                  </div>
                </div>
              </div>

              {errors.password && (
                <div style={{ width: "300px", marginLeft: "-2px" }}>
                  <div style={{ height: "20px" }} className={Registercss.error}>
                    {errors.password.message}
                  </div>
                </div>
              )}
            </label>
            <br />
            <label>
              <span className={Registercss.cart}>
                <span className={Registercss.error}></span>
              </span>
              <div>
                <input
                  data-testid="confirm-password-input"
                  className={Registercss["pswdbox"]}
                  type={confpasswordShown ? "text" : "password"}
                  autoComplete="new-password"
                  placeholder="Confirm password"
                  {...register("confirmPassword", {
                    onChange: (value) => {
                      let password = getValues("password");
                      if (value.target.value !== password) {
                        setPasswordMatch(true);
                        setError(
                          "confirmPassword",
                          "notEqual",
                          "Password Mismatch"
                        );
                      } else setPasswordMatch(false);
                    },
                    required: "Password Required",
                    pattern: {
                      value: PASSWORD_PATTERN,
                      message:
                        "Must contain minimum 8 and maximum 16 characters, at least one uppercase letter, one lowercase letter, one number and one special character",
                    },
                  })}
                />
                <div className={Registercss.eyebutton}>
                  <div className={Registercss.eyebuttons}>
                    {confpasswordShown ? (
                      <p
                        data-testid="eyebtn-confirmpassword-close"
                        onClick={toggleconfirmPassword}
                        style={{ cursor: "pointer" }}
                      >
                        <MdVisibility />
                      </p>
                    ) : (
                      <p
                        data-testid="eyebtn-confirmpassword-open"
                        onClick={toggleconfirmPassword}
                        style={{ cursor: "pointer" }}
                      >
                        <MdVisibilityOff />
                      </p>
                    )}
                  </div>
                </div>
              </div>
              {errors.confirmPassword ? (
                <div style={{ width: "300px", marginLeft: "-2px" }}>
                  <div style={{ height: "20px" }} className={Registercss.error}>
                    {errors.confirmPassword.message}
                  </div>
                </div>
              ) : (
                <small className={Registercss.error}>
                  {passwordMatch && "Password Mismatch"}
                </small>
              )}
            </label>
            <br />
            <br />
            <button
              data-testid="register-button"
              className={Registercss.register}
              type="submit"
            >
              Register
            </button>
            <br />
            <div className={Registercss.form}>
              <GoogleButton />
            </div>
            <br />
            <p className={Registercss.cart}>
              Already have an account?
              <span
                data-testid="login-route"
                className={Registercss.registerclick}
                onClick={() => {
                  loginClick();
                }}
              >
                Login
              </span>
            </p>
          </form>
        </div>
      </div>
      {loading && <CircleLoader />}
    </>
  );
}

export default Register;
