import React, { useEffect, useState, useCallback } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import Swal from 'sweetalert2';
import VisibilityIcon from '@mui/icons-material/Visibility';
import VisibilityOffIcon from '@mui/icons-material/VisibilityOff';
import ChangePasswordmodalcss from '../ChangePassword/AgentChangePassword.module.css';
import { changePasswordAgent } from '../../core/api/coreApiService';
import { Avatar } from '@mui/material';
import { PASSWORD_PATTERN } from '../../utils/utils';

const AgentChangePassword = ({ changestyle }) => {
  let navigate = useNavigate();
  const [showCurrentPasswordAgent, setshowCurrentPasswordAgent] = useState(false);
  const [showNewPasswordAgwent, setshowNewPasswordAgwent] = useState(false);
  const [showConfirmPasswordAgent, setshowConfirmPasswordAgent] = useState(false);
  const [passwordMatchAgent, setpasswordMatchAgent] = useState(false);

  const [width, setWidth] = useState('100vh');
  useEffect(() => {
    document.title = 'Change Password';
    if (changestyle) {
      setWidth('70vh');
    }
  }, []);
  const {
    register,
    handleSubmit,
    formState: { errors },
    getValues,
    reset,
  } = useForm({ mode: 'onChange' });
  //change password function//
  const onSubmitAgent = async (data) => {
    if (data.newPassword !== data.confirmNewPassword) {
      return;
    }
    if (data.currentPassword === data.newPassword) {
      toast.warning('New password cannot be your current password');
      return;
    }
    changePasswordAgent(data)
      .then((response) => {
        if (response?.data.status) {
          Swal.fire({
            icon: 'success',
            title: 'Password Changed Successfully ',
            showConfirmButton: true,
            timer: 2500,
          });
          navigate('/agentdashboard/home');
          localStorage.setItem('isInactive', false);
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
  const showCurrentPassAgent = useCallback(() => {
    setshowCurrentPasswordAgent(!showCurrentPasswordAgent);
  });

  const showNewPassAgent = useCallback(() => {
    setshowNewPasswordAgwent(!showNewPasswordAgwent);
  });
  const showConfirmPassAgent = useCallback(() => {
    setshowConfirmPasswordAgent(!showConfirmPasswordAgent);
  });
  return (
    <div data-testid="agentchangepasswordpage">
      <div
        className={ChangePasswordmodalcss['card']}
        data-testid="changepasswordpage"
        style={{ minHeight: `${width}` }}
      >
        <div className={ChangePasswordmodalcss['cardStyle']}>
          {/* /hint/ */}
          {localStorage.getItem('isInactive') === 'true' && (
            <div className={ChangePasswordmodalcss['hintnote']}>
              <div>
                <Avatar alt="Remy Sharp" src="/assets/icons8-light-on-100.png" sx={{ width: 56, height: 56 }} />
              </div>
              <div>
                <div className={ChangePasswordmodalcss['notecss']}>
                  You have to change password for your first login
                </div>
              </div>
            </div>
          )}
          {/* // */}
          <form onSubmit={handleSubmit(onSubmitAgent)}>
            <h2 className={ChangePasswordmodalcss['formTitle']}>Change password</h2>
            <div className={ChangePasswordmodalcss['inputDiv']}>
              <input
                type={showCurrentPasswordAgent ? 'text' : 'password'}
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
              <i className={ChangePasswordmodalcss['eyebutton']}>
                {showCurrentPasswordAgent ? (
                  <VisibilityIcon onClick={showCurrentPassAgent} data-testid="eyebtn-currentpassword-close" />
                ) : (
                  <VisibilityOffIcon onClick={showCurrentPassAgent} data-testid="eyebtn-currentpassword-open" />
                )}
              </i>
              {errors.currentPassword && (
                <small className={ChangePasswordmodalcss['warning']}>{errors.currentPassword.message}</small>
              )}
            </div>
            <br />
            <div className={ChangePasswordmodalcss['inputDiv']}>
              <input
                type={showNewPasswordAgwent ? 'text' : 'password'}
                autoComplete="off"
                data-testid="new-password"
                placeholder=" New Password "
                {...register('newPassword', {
                  required: 'Password required ',
                  onChange: (value) => {
                    let password = getValues('confirmNewPassword');
                    if (password) {
                      if (value.target.value !== password) {
                        setpasswordMatchAgent(true);
                      } else setpasswordMatchAgent(false);
                    }
                  },
                  pattern: {
                    value: PASSWORD_PATTERN,
                    message:
                      'Password must contain  8 to 16 characters, at least one uppercase letter, one lowercase letter, one number and one special character',
                  },
                })}
              />
              <i className={ChangePasswordmodalcss['eyebutton']}>
                {showNewPasswordAgwent ? (
                  <VisibilityIcon onClick={showNewPassAgent} data-testid="eyebtn-newpassword-close" />
                ) : (
                  <VisibilityOffIcon onClick={showNewPassAgent} data-testid="eyebtn-newpassword-open" />
                )}
              </i>

              {errors.newPassword && (
                <small className={ChangePasswordmodalcss['warning']}>{errors.newPassword.message}</small>
              )}
            </div>{' '}
            <br />
            <div className={ChangePasswordmodalcss['inputDiv']}>
              <input
                type={showConfirmPasswordAgent ? 'text' : 'password'}
                autoComplete="off"
                placeholder=" Confirm Password "
                data-testid="confirm-password"
                {...register('confirmNewPassword', {
                  required: 'Password  required ',
                  onChange: (value) => {
                    let password = getValues('newPassword');
                    if (value.target.value !== password) {
                      setpasswordMatchAgent(true);
                    } else setpasswordMatchAgent(false);
                  },
                })}
              />
              <i className={ChangePasswordmodalcss['eyebutton']}>
                {showConfirmPasswordAgent ? (
                  <VisibilityIcon onClick={showConfirmPassAgent} data-testid="eyebtn-confirmpassword-close" />
                ) : (
                  <VisibilityOffIcon onClick={showConfirmPassAgent} data-testid="eyebtn-confirmpassword-open" />
                )}
              </i>
              {errors.confirmNewPassword ? (
                <small className={ChangePasswordmodalcss['warning']}>{errors.confirmNewPassword.message}</small>
              ) : (
                <small className={ChangePasswordmodalcss['warning']}>{passwordMatchAgent && 'Password Mismatch'}</small>
              )}
            </div>
            <div className={ChangePasswordmodalcss['buttonWrapper']}>
              <button
                type="submit"
                data-testid="submit-button"
                id="submitButton"
                className={ChangePasswordmodalcss['submitButton']}
              >
                submit
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default AgentChangePassword;
