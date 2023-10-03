import React, { useState, useEffect, useCallback } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import style from './ChangePassword.module.css';
import VisibilityIcon from '@mui/icons-material/Visibility';
import VisibilityOffIcon from '@mui/icons-material/VisibilityOff';
import Swal from 'sweetalert2';
import { toast } from 'react-toastify';
import { changePasswordAdmin } from '../../core/api/apiService';
import {PASSWORD_PATTERN} from '../../utils/utils';

const ChangePassword = () => {
  let navigate = useNavigate();
  const [showCurrentPassword, setShowCurrentPassword] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [passwordMatch, setPasswordMatch] = useState(false);

  useEffect(() => {
    document.title = 'Change Password';
  }, []);
  const {
    register,
    handleSubmit,
    formState: { errors },
    getValues,
    reset,
  } = useForm({ mode: 'onChange' });
  //change password function//
  const onSubmit = async (data) => {
    if (data.newPassword !== data.confirmNewPassword) {
      return;
    }
    if (data.currentPassword === data.newPassword) {
      toast.warning('New password cannot be your current password');
      return;
    }
    changePasswordAdmin(data)
      .then((response) => {
        if (response?.data.status) {
          Swal.fire({
            icon: 'success',
            title: 'Password Changed Successfully ',
            showConfirmButton: true,
            timer: 2500,
          });
          navigate('/dashboard/home');
        }
      })
      .catch((err) => {
        const error = err.response.data.message;
        const noPassword = 'Password Not Set';
        if (error === 'Password MissMatch') {
          Swal.fire({
            icon: 'danger',
            title: ' Your current password is wrong',
            showConfirmButton: true,
            timer: 2500,
          });
        } else if (error === noPassword) {
          Swal.fire({
            icon: 'error',
            title: 'Invalid Current Password',
            text: 'If you want to set a password use Forgot Password',
          });
        }
      });
    reset();
  };

  const showCurrentPass = useCallback(() => {
    setShowCurrentPassword(!showCurrentPassword);
  });

  const showNewPass = useCallback(() => {
    setShowNewPassword(!showNewPassword);
  });
  const showConfirmPass = useCallback(() => {
    setShowConfirmPassword(!showConfirmPassword);
  });

  return (
    <>
      <div className={style['card']} data-testid="changepasswordpage">
        <div className={style['cardStyle']}>
          <form onSubmit={handleSubmit(onSubmit)}>
            <h2 className={style['formTitle']}>Change password</h2>
            <div className={style['inputDiv']}>
              <input
                type={showCurrentPassword ? 'text' : 'password'}
                autoComplete="off"
                data-testid="current-password"
                placeholder="Current Password "
                {...register('currentPassword', {
                  required: 'Current Password required ',
                  pattern: {
                    value: PASSWORD_PATTERN,
                    message:
                      'Password must contain  8 to 16 characters, at least one uppercase letter, one lowercase letter, one number and one special character',
                  },
                })}
              />
              <i className={style['eyebutton']}>
                {showCurrentPassword ? (
                  <VisibilityIcon onClick={showCurrentPass} data-testid="eyebtn-currentpassword-close" />
                ) : (
                  <VisibilityOffIcon onClick={showCurrentPass} data-testid="eyebtn-currentpassword-open" />
                )}
              </i>
              {errors.currentPassword && <small className={style['warning']}>{errors.currentPassword.message}</small>}
            </div>
            <br />
            <div className={style['inputDiv']}>
              <input
                type={showNewPassword ? 'text' : 'password'}
                autoComplete="off"
                data-testid="new-password"
                placeholder=" New Password "
                {...register('newPassword', {
                  required: 'Password required ',
                  onChange: (value) => {
                    let password = getValues('confirmNewPassword');
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
                  <VisibilityIcon onClick={showNewPass} data-testid="eyebtn-newpassword-close" />
                ) : (
                  <VisibilityOffIcon onClick={showNewPass} data-testid="eyebtn-newpassword-open" />
                )}
              </i>
              {errors.newPassword && <small className={style['warning']}>{errors.newPassword.message}</small>}
            </div>{' '}
            <br />
            <div className={style['inputDiv']}>
              <input
                type={showConfirmPassword ? 'text' : 'password'}
                autoComplete="off"
                placeholder=" Confirm Password "
                data-testid="confirm-password"
                {...register('confirmNewPassword', {
                  required: 'Password  required ',
                  onChange: (value) => {
                    let password = getValues('newPassword');
                    if (value.target.value !== password) {
                      setPasswordMatch(true);
                    } else setPasswordMatch(false);
                  },
                })}
              />
              <i className={style['eyebutton']}>
                {showConfirmPassword ? (
                  <VisibilityIcon onClick={showConfirmPass} data-testid="eyebtn-confirmpassword-close" />
                ) : (
                  <VisibilityOffIcon onClick={showConfirmPass} data-testid="eyebtn-confirmpassword-open" />
                )}
              </i>
              {errors.confirmNewPassword ? (
                <small className={style['warning']}>{errors.confirmNewPassword.message}</small>
              ) : (
                <small className={style['warning']}>{passwordMatch && 'Password Mismatch'}</small>
              )}
            </div>
            <div className={style['buttonWrapper']}>
              <button type="submit" data-testid="submit-button" className={style['submitButton']}>
                submit
              </button>
            </div>
          </form>
        </div>
      </div>
    </>
  );
};

export default ChangePassword;
