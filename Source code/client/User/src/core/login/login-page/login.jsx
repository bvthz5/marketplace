import React, { useState, useEffect, useCallback } from "react";
import { useForm as UseForm } from "react-hook-form";
import { FaShoppingCart } from "react-icons/fa";
import Logincss from "./login.module.css";
import Swal from "sweetalert2";
import ForgotPassword from "../forgot-password/forgot-password";
import { useNavigate } from "react-router-dom";
import KeyboardBackspaceIcon from "@mui/icons-material/KeyboardBackspace";
import LoadingBar from "react-top-loading-bar";
import { toast } from "react-toastify";
import { Tooltip } from "@mui/material";
import VisibilityIcon from "@mui/icons-material/Visibility";
import VisibilityOffIcon from "@mui/icons-material/VisibilityOff";
import GoogleButton from "./GoogleButton/GoogleButton";
import { login, resendVerificationMail } from "../../Api/coreApiService";
import { PASSWORD_PATTERN } from "../../../components/Utils/Data/Data";
import { clearAllProducts } from "../../../components/Common-Components/Productlist/ProductList/ProductSlices/productSlice";
import { useDispatch } from "react-redux";

function Login() {
  let dispatch=useDispatch()
  let navigate = useNavigate();
  const [progress, setProgress] = useState(0);
  const [showPassword, setShowPassword] = useState(false);


  const registerClick = () => {
    navigate("/register");
  };

  useEffect(() => {
    document.title = "CART_IN-login";
  }, []);
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = UseForm({ mode: "onChange" });

  const showPass = useCallback(() => {
    setShowPassword(!showPassword);
  }, [showPassword]);

  // default login
  const onSubmit = async (data) => {
    startLoader();
    login(data)
      .then((response) => {
        const accessToken = response?.data.data?.accessToken?.value;
        const refreshToken = response?.data.data?.refreshToken?.value;
        const role = response?.data?.data.role;
        let email = response?.data?.data.email;
        localStorage.setItem("accessToken", accessToken);
        localStorage.setItem("refreshToken", refreshToken);
        localStorage.setItem("role", role);
        localStorage.setItem("email", email);
        stopLoader();
        if (response?.data?.data?.accessToken) {
          navigate("/home");
          dispatch(clearAllProducts());
        }
      })
      .catch((err) => {
        const error = err.response.data.message;
        handleErrorResponse(error, data.email);
        console.log(err);
        stopLoader();
      });
    reset();
  };

  const handleErrorResponse = (error, data) => {
    const email = "Invalid Credentials";
    const blockedError = "User Blocked";
    const inactiveError = "User not verified";
    const noPassword = "Password not set";
    if (error === email) {
      Swal.fire({
        icon: "error",
        title: `${email}`,
      });
    } else if (error === blockedError) {
      Swal.fire({
        icon: "error",
        title: "Blocked",
        text: "You have been blocked from this website",
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
          resendVerificationMail(data)
            .then(() => {
              toast.success("Email sent successfully");
            })
            .catch((err) => {
              toast.error("Error occured! Please try again.");
              console.log(err);
            });
        }
      });
    } else if (error === noPassword) {
      Swal.fire({
        icon: "error",
        title: "You don't have a password",
        text: "Please use forgot password for setting a password ",
      });
    }
  };

  function startLoader() {
    setProgress(50);
  }
  function stopLoader() {
    setProgress(100);
  }

  const resetLoader = useCallback(() => {
    setProgress(0);
  }, []);

  const goBack = useCallback(() => navigate(-1), [navigate]);

  return (
    <>
      <LoadingBar
        color="blue"
        progress={progress}
        onLoaderFinished={resetLoader}
      />
      <div className={Logincss.container} data-testid="loginpage">
        <div className={Logincss.formdiv}>
          <div className={Logincss.box}>
            <form onSubmit={handleSubmit(onSubmit)} className={Logincss.form} action="index.html">
              <div className={Logincss.backicondiv}>
                <Tooltip title="Go back">
                  <KeyboardBackspaceIcon
                    className={Logincss.backicon}
                    onClick={goBack}
                  />
                </Tooltip>
              </div>
              <h2 className={Logincss.carttitle}>
                CART.IN <FaShoppingCart />
              </h2>
              <span className={Logincss.cart}></span>
              <div className={Logincss.fielddiv}>
                <input
                  className={Logincss.field}
                  type="text"
                  autoComplete="off"
                  placeholder="Email"
                  data-testid="email-input"
                  {...register("email", {
                    required: "Email required ",
                    pattern: {
                      value: /^[A-Z0-9._%+-]+@[A-z0-9.-]+\.[A-Z]{2,254}$/i,
                      message: "Invalid Email address",
                    },
                  })}
                />
                {errors.email && (
                  <small className={Logincss.error}>
                    {errors.email.message}
                  </small>
                )}
              </div>

              <div className={Logincss.fielddiv}>
                <input
                  data-testid="password-input"
                  className={Logincss.field}
                  type={showPassword ? "text" : "password"}
                  placeholder="Password"
                  autoComplete="off"
                  {...register("password", {
                    required: "Password Required",
                    pattern: {
                      value: PASSWORD_PATTERN,
                      message: "Invalid Password",
                    },
                  })}
                />
                <div className={Logincss["eyebutton"]}>
                  {showPassword ? (
                    <VisibilityIcon
                      onClick={showPass}
                      data-testid="eyebtn-password-close"
                      style={{ width: "18px" }}
                    />
                  ) : (
                    <VisibilityOffIcon
                      data-testid="eyebtn-password-open"
                      onClick={showPass}
                      style={{ width: "18px" }}
                    />
                  )}
                </div>
                <div className={Logincss.errordiv}>
                  {errors.password && (
                    <small className={Logincss.error}>
                      {errors.password.message}
                    </small>
                  )}
                </div>
              </div>

              <button
                data-testid="login-button"
                className={Logincss.loginbtn}
                type="submit"
                value="submit"
              >
                LogIn
              </button>
            </form>

            <div className={Logincss.forgotpasswordlink}>
              <ForgotPassword />
            </div>
            <div className={Logincss.googlebtn} data-testid="google-login">
              <GoogleButton />
            </div>
            <span className={Logincss.signup}>
              New here?
              <span
                data-testid="register-link"
                className={Logincss.loginclick}
                style={{ cursor: "pointer", marginBottom: "2px" }}
                onClick={() => {
                  registerClick();
                }}
              >
                Create an account
              </span>
            </span>
          </div>
        </div>
      </div>
    </>
  );
}

export default Login;
