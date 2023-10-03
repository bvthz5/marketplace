import React, { useCallback } from "react";
import { GoogleLogin } from "@react-oauth/google";
import { googleLogin } from "../../../Api/coreApiService";
import Swal from "sweetalert2";
import { useNavigate } from "react-router-dom";
import { clearAllProducts } from "../../../../components/Common-Components/Productlist/ProductList/ProductSlices/productSlice";
import { useDispatch } from "react-redux";

const GoogleButton = () => {
  let dispatch = useDispatch();
  let navigate = useNavigate();

  const handleCallbackResponse = useCallback((res) => {
    const userObject = res?.credential;
    googleLogin(userObject)
      .then((response) => {
        const accessToken = response?.data.data?.accessToken?.value;
        const refreshToken = response?.data.data?.refreshToken?.value;
        const role = response?.data?.data.role;
        const email = response?.data?.data.email;

        localStorage.setItem("accessToken", accessToken);
        localStorage.setItem("refreshToken", refreshToken);
        localStorage.setItem("role", role);
        localStorage.setItem("email", email);

        if (response?.data?.data?.accessToken) {
          navigate("/home");
          dispatch(clearAllProducts());
        }
      })
      .catch((err) => {
        const error = err.response.data.message;
        const blockedError = "User BLOCKED";
        if (error === blockedError) {
          Swal.fire({
            icon: "error",
            title: "Blocked",
            text: "You have been blocked from this website",
          });
        }
        console.log(err);
      });
  },[dispatch, navigate]);
  return (
    <>
      <GoogleLogin
        data-testid="google-button"
        text="continue_with"
        onSuccess={handleCallbackResponse}
        useOneTap
      />
    </>
  );
};

export default GoogleButton;
