import { useNavigate, useSearchParams } from "react-router-dom";
import Swal from "sweetalert2";
import React from "react";
import { toast } from "react-toastify";
import {
  resendVerificationMailByToken,
  tokenVerification,
} from "../Api/coreApiService";

const VerifyEmail = () => {
  const [searchParams] = useSearchParams();
  let token = searchParams.get("token");
  let navigate = useNavigate();

  if (token) {
    const verifyToken = () => {
      tokenVerification(token)
        .then((response) => {
          Swal.fire({
            timer: 1500,
            showConfirmButton: false,
            willOpen: () => {
              Swal.showLoading();
            },
            willClose: () => {
              console.log('in close');
              Swal.fire({
                icon: "success",
                title: "Verified!",
                text: `Email  verified `,
                showConfirmButton: false,
                timer: 1500,
              });
            },
          });
          navigate("/login");
        })
        .catch((err) => {
          const error = err.response.data.message;
          const tokenError = "Token Expired";
          console.log(error, tokenError);
          handleErrors(error, tokenError);
          navigate("/");
        });
    };
    verifyToken();
  }

  const handleErrors = (error, tokenError) => {
    console.log(error, tokenError);
    Swal.fire({
      timer: 1500,
      showConfirmButton: false,
      willOpen: () => {
        Swal.showLoading();
      },
      willClose: () => {
        if (error === tokenError) {
          Swal.fire({
            icon: "error",
            title: "Link Expired!",
            text: "Do you want resend a verification email?",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Resend",
          }).then(async (result) => {
            if (result.isConfirmed) {
              handleConfirmation();
            }
          });
        } else {
          Swal.fire({
            icon: "error",
            title: "Error!",
            text: "Invalid Link",
            showConfirmButton: true,
          });
        }
      },
    });
  };

  const handleConfirmation = () => {
    resendVerificationMailByToken(token)
      .then(() => {
        toast.success("Email sent successfully");
      })
      .catch((err) => {
        toast.error("Error occured! Please try again.");
        console.log(err);
      });
  };

  return <div data-testid="verifyemailpage">{"Verifying..."}</div>;
};

export default VerifyEmail;
