import React, { useCallback, useState } from "react";
import { useForm } from "react-hook-form";
import Swal from "sweetalert2";
import { useNavigate } from "react-router-dom";
import style from "./ChangePassword.module.css";
import CommonHeader from "../../Header/HeaderComponent/Header";
import VisibilityIcon from "@mui/icons-material/Visibility";
import VisibilityOffIcon from "@mui/icons-material/VisibilityOff";
import { toast } from "react-toastify";
import { changePassword } from "../../../../core/Api/apiService";
import ScrollToTop from "../../../Utils/ScrollToPageTop/ScrollToTop";
import { PASSWORD_PATTERN } from "../../../Utils/Data/Data";
const ChangePassword = () => {
  let navigate = useNavigate();
  const [showCurrentPassword, setShowCurrentPassword] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [passwordMatch, setPasswordMatch] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    getValues,
    reset,
  } = useForm({ mode: "onChange" });

  // api call for changing the password
  const onSubmit = async (data) => {
    if (data.newPassword !== data.confirmNewPassword) {
      return;
    }
    if (data.currentPassword === data.newPassword) {
      toast.warning("New password cannot be your current password");
      return;
    }
    changePassword(data)
      .then((response) => {
        Swal.fire({
          icon: "success",
          title: "Password Changed Successfully ",
          showConfirmButton: true,
          timer: 2500,
        });
        navigate("/profile");
      })
      .catch((err) => {
        console.log(err);
        const error = err.response.data.message;
        const noPassword = "Password Not Set";
        if (error === "Password MissMatch") {
          Swal.fire({
            icon: "error",
            title: " Your current password is wrong",
            showConfirmButton: true,
            timer: 2500,
          });
        } else if (error === noPassword) {
          Swal.fire({
            icon: "error",
            title: "You don't have a Password",
            text: "If you want to set a password use Forgot Password",
          });
        }
      });
    reset();
  };

  const showCurrentPass = useCallback(() => {
    setShowCurrentPassword(!showCurrentPassword);
  }, [showCurrentPassword]);

  const showNewPass = useCallback(() => {
    setShowNewPassword(!showNewPassword);
  }, [showNewPassword]);
  const showConfirmPass = useCallback(() => {
    setShowConfirmPassword(!showConfirmPassword);
  }, [showConfirmPassword]);

  return (
    <>
      <ScrollToTop />
      <CommonHeader />
      <div data-testid="changepasswordpage" className={style["mainDiv"]}>
        <div className={style["card"]}>
          <div className={style["cardStyle"]}>
            <form onSubmit={handleSubmit(onSubmit)}>
              <h2 className={style["formTitle"]}>Change Password</h2>
              <div className={style["inputDiv"]}>
                <input
                  data-testid="current-password"
                  type={showCurrentPassword ? "text" : "password"}
                  autoComplete="off"
                  placeholder="Current Password "
                  {...register("currentPassword", {
                    required: "Password required ",
                    pattern: {
                      value: PASSWORD_PATTERN,
                      message:
                        "Password must contain  8 to 16 characters, at least one uppercase letter, one lowercase letter, one number and one special character",
                    },
                  })}
                />
                <div className={style["eyebutton"]}>
                  {showCurrentPassword ? (
                    <VisibilityIcon
                      onClick={showCurrentPass}
                      data-testid="eyebtn-currentpassword-close"
                    />
                  ) : (
                    <VisibilityOffIcon
                      onClick={showCurrentPass}
                      data-testid="eyebtn-currentpassword-open"
                    />
                  )}
                </div>

                {errors.currentPassword && (
                  <small className={style["warning"]}>
                    {errors.currentPassword.message}
                  </small>
                )}
              </div>
              <br />
              <div className={style["inputDiv"]}>
                <input
                  type={showNewPassword ? "text" : "password"}
                  data-testid="new-password"
                  autoComplete="off"
                  placeholder=" New Password "
                  {...register("newPassword", {
                    required: "Password required ",
                    onChange: (value) => {
                      let password = getValues("confirmNewPassword");
                      if (password) {
                        if (value.target.value !== password) {
                          setPasswordMatch(true);
                        } else setPasswordMatch(false);
                      }
                    },
                    pattern: {
                      value: PASSWORD_PATTERN,
                      message:
                        "Password must contain  8 to 16 characters, at least one uppercase letter, one lowercase letter, one number and one special character",
                    },
                  })}
                />
                <div className={style["eyebutton"]}>
                  {showNewPassword ? (
                    <VisibilityIcon
                      onClick={showNewPass}
                      data-testid="eyebtn-newpassword-close"
                    />
                  ) : (
                    <VisibilityOffIcon
                      onClick={showNewPass}
                      data-testid="eyebtn-newpassword-open"
                    />
                  )}
                </div>

                {errors.newPassword && (
                  <small
                    data-testid="confirm-error"
                    className={style["warning"]}
                  >
                    {errors.newPassword.message}
                  </small>
                )}
              </div>{" "}
              <br />
              <div className={style["inputDiv"]}>
                <input
                  type={showConfirmPassword ? "text" : "password"}
                  data-testid="confirm-password"
                  autoComplete="off"
                  placeholder=" Confirm Password "
                  {...register("confirmNewPassword", {
                    required: "Password  required ",
                    onChange: (value) => {
                      let password = getValues("newPassword");
                      if (value.target.value !== password) {
                        setPasswordMatch(true);
                      } else setPasswordMatch(false);
                    },
                  })}
                />
                <div className={style["eyebutton"]}>
                  {showConfirmPassword ? (
                    <VisibilityIcon
                      onClick={showConfirmPass}
                      data-testid="eyebtn-confirmpassword-close"
                    />
                  ) : (
                    <VisibilityOffIcon
                      onClick={showConfirmPass}
                      data-testid="eyebtn-confirmpassword-open"
                    />
                  )}
                </div>
                {errors.confirmNewPassword ? (
                  <small className={style["warning"]}>
                    {errors.confirmNewPassword.message}
                  </small>
                ) : (
                  <small className={style["warning"]}>
                    {passwordMatch && "Password Mismatch"}
                  </small>
                )}
              </div>
              <div className={style["buttonWrapper"]}>
                <button
                  data-testid="submit-button"
                  type="submit"
                  id="submitButton"
                  className={style["submitButton"]}
                >
                  submit
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </>
  );
};

export default ChangePassword;
