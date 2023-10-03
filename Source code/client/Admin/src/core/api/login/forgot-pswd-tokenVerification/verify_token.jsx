import React, { useCallback, useState } from 'react';
import { useForm as UseForm } from 'react-hook-form';
import { useNavigate, useSearchParams } from 'react-router-dom';
import Swal from 'sweetalert2';
import style from './verify_token.module.css';
import VisibilityIcon from '@mui/icons-material/Visibility';
import VisibilityOffIcon from '@mui/icons-material/VisibilityOff';
import { toast } from 'react-toastify';
import { resetPassword } from '../../coreApiService';
import {PASSWORD_PATTERN} from '../../../../utils/utils';

function VerifyToken() {
  const [searchParams] = useSearchParams();
  searchParams.get('token');
  let token = searchParams.get('token');
  let navigate = useNavigate();
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [passwordMatch, setPasswordMatch] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    getValues,
  } = UseForm({ mode: 'onChange' });

  let password;
  const onSubmit = async (data) => {
    if (data.password === data.confirmPassword) {
      password = data.password;
      if (token) {
        verifyToken();
      }
    } else {
      toast("Password Doesn't Match");
    }
  };
  //reset password//
  async function verifyToken() {
    resetPassword(token, password)
      .then((response) => {
        Swal.fire({
          timer: 1500,
          showConfirmButton: false,
          willOpen: () => {
            Swal.showLoading();
          },
          willClose: () => {
            Swal.fire({
              icon: 'success',
              title: 'Password changed Succesfully!',
              showConfirmButton: false,
              timer: 1500,
            });
          },
        });
        navigate('/login');
      })
      .catch(() => {
        Swal.fire({
          timer: 1500,
          showConfirmButton: false,
          willOpen: () => {
            Swal.showLoading();
          },
          willClose: () => {
            Swal.fire({
              icon: 'error',
              title: 'Error Occured. Try Again!',
              showConfirmButton: true,
            });
          },
        });
        navigate('/login');
      });
  }
  const showNewPass = useCallback(() => {
    setShowNewPassword(!showNewPassword);
  }, [showNewPassword]);
  const showConfirmPass = useCallback(() => {
    setShowConfirmPassword(!showConfirmPassword);
  }, [showConfirmPassword]);
  return (
    <div className={style['mainDiv']} data-testid="verifytokenpage">
      <div className={style['cardStyle']}>
        <form onSubmit={handleSubmit(onSubmit)}>
          <h2 className={style['formTitle']}>Reset Password</h2>
          <div className={style['inputDiv']}>
            <label className={style['inputLabel']} htmlFor="password">
              New Password
            </label>
            <input
            data-testid='new-password'
              className={style['pswdinput']}
              type={showNewPassword ? 'text' : 'password'}
              autoComplete="off"
              {...register('password', {
                required: 'Password Required',
                onChange: (value) => {
                  let password = getValues('confirmPassword');
                  if (password) {
                    if (value.target.value !== password) {
                      setPasswordMatch(true);
                    } else setPasswordMatch(false);
                  }
                },
                pattern: {
                  value: PASSWORD_PATTERN,
                  message:
                    'Password must contain  8 to 16 characters, at least one uppercase letter, one lowercase letter, one number and one special character',
                },
              })}
            />
            <i className={style['eyebutton']}>
              {showNewPassword ? (
                <VisibilityIcon onClick={showNewPass} className={style['iconwidth']} data-testid="eyebtn-newpassword-close" />
              ) : (
                <VisibilityOffIcon onClick={showNewPass} className={style['iconwidth']}   data-testid="eyebtn-newpassword-open"/>
              )}
            </i>

            {errors.password && <small className={style['message']}>{errors.password.message}</small>}
          </div>
          <div className={style['inputDiv']}>
            <label className={style['inputLabel']} htmlFor="confirmPassword">
              Confirm Password
            </label>
            <input
            data-testid='confirm-password'
              className={style['pswdinput']}
              type={showConfirmPassword ? 'text' : 'password'}
              autoComplete="off"
              {...register('confirmPassword', {
                required: 'Password Required',
                onChange: (value) => {
                  password = getValues('password');
                  if (password) {
                    if (value.target.value !== password) {
                      setPasswordMatch(true);
                    } else setPasswordMatch(false);
                  }
                },
                pattern: {
                  value: PASSWORD_PATTERN,
                  message:
                    'Password must contain  8 to 16 characters, at least one uppercase letter, one lowercase letter, one number and one special character',
                },
              })}
            />
            <i className={style['eyebutton']}>
              {showConfirmPassword ? (
                <VisibilityIcon onClick={showConfirmPass} className={style['iconwidth']}     data-testid="eyebtn-confirmpassword-close"/>
              ) : (
                <VisibilityOffIcon onClick={showConfirmPass} className={style['iconwidth']}   data-testid="eyebtn-confirmpassword-open" />
              )}
            </i>
            {errors.confirmPassword ? (
              <small className={style['message']}>{errors.confirmPassword.message}</small>
            ) : (
              <small className={style['message']}>{passwordMatch && 'Password Mismatch'}</small>
            )}
          </div>
          <div className={style['buttonWrapper']}>
            <button type="submit" id="submitButton" className={style['submitButton']} data-testid='submit-button'>
              <span>Continue</span>
              <span id="loader" />
            </button>
          </div>
        </form>
        <div className="google" id="glogindiv"></div>
      </div>
    </div>
  );
}

export default VerifyToken;
