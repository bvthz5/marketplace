import React, { useCallback, useState } from "react";
import { useForm as UseForm } from "react-hook-form";
import { useNavigate, useSearchParams } from "react-router-dom";
import Swal from "sweetalert2";
import style from "./verify_token.module.css";
import VisibilityIcon from "@mui/icons-material/Visibility";
import VisibilityOffIcon from "@mui/icons-material/VisibilityOff";
import { resetPassword } from "../../Api/coreApiService";
import { PASSWORD_PATTERN } from "../../../components/Utils/Data/Data";
import CircleLoader from "../../../components/Utils/Loaders/CircleLoader/CircleLoader";

const VerifyToken = () => {
  const [searchParams] = useSearchParams();
  let token = searchParams.get("token");
  let navigate = useNavigate();
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [passwordMatch, setPasswordMatch] = useState(false);
  const [loading, setLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    getValues,
  } = UseForm({ mode: "onChange" });

  const onSubmit = async (data) => {
    if (data.password !== data.confirmPassword) {
      return;
    }

    if (token) {
      verifyToken(data.password);
    }
  };
  const verifyToken = (password) => {
    setLoading(true);
    resetPassword(token, password)
      .then(() => {
        setLoading(false);

        localStorage.clear();
        Swal.fire({
          timer: 1500,
          showConfirmButton: false,
          willOpen: () => {
            Swal.showLoading();
          },
          willClose: () => {
            Swal.fire({
              icon: "success",
              title: "Password changed Succesfully!",
              showConfirmButton: false,
              timer: 1500,
            });
          },
        });
        navigate("/login");
      })
      .catch((err) => {
        setLoading(false);
        console.log(err);
        Swal.fire({
          timer: 1500,
          showConfirmButton: false,
          willOpen: () => {
            Swal.showLoading();
          },
          willClose: () => {
            Swal.fire({
              icon: "error",
              title: "Error Occured. Try Again!",
              showConfirmButton: true,
            });
          },
        });
        navigate("/login");
      });
  };
  const showNewPass = useCallback(() => {
    setShowNewPassword(!showNewPassword);
  }, [showNewPassword]);
  const showConfirmPass = useCallback(() => {
    setShowConfirmPassword(!showConfirmPassword);
  }, [showConfirmPassword]);
  return (
    <div data-testid="verifytokenpage" className={style["mainDiv"]}>
      <div className={style["cardStyle"]}>
        <form onSubmit={handleSubmit(onSubmit)}>
          <h2 className={style["formTitle"]}>Reset Password</h2>
          <div className={style["inputDiv"]}>
            <label className={style["inputLabel"]} htmlFor="password">
              New Password
            </label>
            <input
              data-testid="new-password"
              className={style["pswdinput"]}
              type={showNewPassword ? "text" : "password"}
              autoComplete="off"
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
            <i className={style["eyebutton"]}>
              {showNewPassword ? (
                <VisibilityIcon
                  onClick={showNewPass}
                  data-testid="eyebtn-newpassword-close"
                  style={{ width: "18px" }}
                />
              ) : (
                <VisibilityOffIcon
                  onClick={showNewPass}
                  data-testid="eyebtn-newpassword-open"
                  style={{ width: "18px" }}
                />
              )}
              {/* <VisibilityIcon onClick={showPass} /> */}
            </i>
            <br></br>
            {errors.password && (
              <small className={style["message"]}>
                {errors.password.message}
              </small>
            )}
          </div>
          <div className={style["inputDiv"]}>
            <label className={style["inputLabel"]} htmlFor="confirmPassword">
              Confirm Password
            </label>
            <input
              data-testid="confirm-password"
              className={style["pswdinput"]}
              type={showConfirmPassword ? "text" : "password"}
              autoComplete="off"
              {...register("confirmPassword", {
                required: "Password Required",
                onChange: (value) => {
                  let password = getValues("password");
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
            <i className={style["eyebutton"]}>
              {showConfirmPassword ? (
                <VisibilityIcon
                  data-testid="eyebtn-confirmpassword-close"
                  onClick={showConfirmPass}
                  style={{ width: "18px" }}
                />
              ) : (
                <VisibilityOffIcon
                  data-testid="eyebtn-confirmpassword-open"
                  onClick={showConfirmPass}
                  style={{ width: "18px" }}
                />
              )}
              {/* <VisibilityIcon onClick={showPass} /> */}
            </i>
            <br></br>
            {errors.confirmPassword ? (
              <small className={style["message"]}>
                {errors.confirmPassword.message}
              </small>
            ) : (
              <small className={style["message"]}>
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
              <span>Continue</span>
              <span id="loader" />
            </button>
          </div>
        </form>
      </div>
      {loading && <CircleLoader />}
    </div>
  );
};

export default VerifyToken;
