import React, { useCallback, useState } from "react";
import { useForm as UseForm } from "react-hook-form";
import Swal from "sweetalert2";
import style from "./Forgotpassword.module.css";
import { forgotPassword } from "../../Api/coreApiService";
import { Box, Modal } from "@mui/material";
import CircleLoader from "../../../components/Utils/Loaders/CircleLoader/CircleLoader";
import useWindowDimensions from "../../../hooks/WindowSizeReader/WindowDimensions";

const ForgotPassword = () => {
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = UseForm({ mode: "onChange" });

  const { width } = useWindowDimensions();

  const handleOpen = () => setOpen(true);
  const handleClose = useCallback(() => {
    setOpen(false);
    reset();
  },[reset]);

  const stylemodal = {
    position: "absolute",
    top: "50%",
    left: "50%",
    transform: "translate(-50%, -50%)",
    width: width < 500 ? "95%" : 550,
    bgcolor: "background.paper",
    border: "2px solid #000",
    boxShadow: 24,
    borderRadius: "10px",
  };

  const handleUpdate = async (data) => {
    setLoading(true);
    forgotPassword(data.email)
      .then(() => {
        setLoading(false);
        Swal.fire({
          icon: "success",
          title: "Email sent!",
          text: `An email has been sent to ${data.email} for changing  password`,
          showConfirmButton: false,
          timer: 2500,
        });
      })
      .catch((err) => {
        setLoading(false);
        let errMsg = "User Not Found";
        if (err.response?.data?.message === errMsg) {
          Swal.fire({
            title: `${errMsg}`,
            text: ` ${data.email} is not an active account!`,
            icon: "error",
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "OK",
          });
        }
        console.log(err);
      });
    handleClose();
  };

  return (
    <>
      <div data-testid="forgotpasswordpage">
        <div
          className={style["forgot"]}
          onClick={handleOpen}
          data-testid="forgotpasswordbutton"
        >
          Forgot Password?
        </div>
        <Modal
          open={open}
          onClose={handleClose}
          aria-labelledby="modal-modal-title"
          aria-describedby="modal-modal-description"
        >
          <Box sx={stylemodal}>
            <div data-testid="forgotpasswordpage">
              <span
                className={style["close"]}
                data-testid="close-button"
                onClick={() => {
                  handleClose();
                }}
              >
                &times;
              </span>
              <div className={style["login-panel"]}>
                <h3 data-testid="passwordmodal">Forgot Password?</h3>
              </div>
              <div className={style["sign-in"]}>
                <form onSubmit={handleSubmit(handleUpdate)}>
                  <div className={style.emaildiv}>
                    <input
                      data-testid="email-input"
                      className={style["form__field"]}
                      type="text"
                      placeholder="Enter your email"
                      {...register("email", {
                        required: "Email required ",
                        pattern: {
                          value: /^[A-Z0-9._%+-]+@[A-z0-9.-]+\.[A-Z]{2,}$/i,
                          message: "Invalid Email address",
                        },
                      })}
                    />
                    {errors.email && (
                      <small className={style.red}>
                        {errors.email.message}
                      </small>
                    )}
                  </div>
                  <button
                    type="submit"
                    className={style["submitpass"]}
                    data-testid="submit-button"
                  >
                    Submit
                  </button>
                </form>
              </div>
            </div>
          </Box>
        </Modal>
      </div>
      {loading && <CircleLoader />}
    </>
  );
};

export default ForgotPassword;
