import React, { useCallback, useState } from 'react';
import { useForm as UseForm } from 'react-hook-form';
import { useNavigate, useSearchParams } from 'react-router-dom';
import Swal from 'sweetalert2';
import verifytokencss from '../forgot-pswd-tokenVerificationAgent/verify-TokenAgent.module.css';
import VisibilityIcon from '@mui/icons-material/Visibility';
import VisibilityOffIcon from '@mui/icons-material/VisibilityOff';
import { toast } from 'react-toastify';
import { resetPasswordAgent } from '../../core/api/coreApiService';
import { PASSWORD_PATTERN } from '../../utils/utils';

function VerifyToken() {
  const [searchParamsAgent] = useSearchParams();
  searchParamsAgent.get('token');
  let token = searchParamsAgent.get('token');

  const [showAgentNewPassword, setshowAgentNewPassword] = useState(false);
  const [showAgentConfirmPassword, setshowAgentConfirmPassword] = useState(false);
  const [agentPasswordMatch, setagentPasswordMatch] = useState(false);
  let navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
    getValues,
  } = UseForm({ mode: 'onChange' });

  let password;
  const onSubmitPassword = async (data) => {
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
    resetPasswordAgent(token, password)
      .then((response) => {
        if (response.data.status) {
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
          navigate('/agent/login');
        }
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
        navigate('/agent/login');
      });
  }
  const showAgentNewPass = useCallback(() => {
    setshowAgentNewPassword(!showAgentNewPassword);
  }, [showAgentNewPassword]);
  const showAgentConfirmPass = useCallback(() => {
    setshowAgentConfirmPassword(!showAgentConfirmPassword);
  }, [showAgentConfirmPassword]);
  return (
    <div className={verifytokencss['mainDiv']} data-testid="verifytokenagentpage">
      <div className={verifytokencss['cardStyle']}>
        <form onSubmit={handleSubmit(onSubmitPassword)}>
          <h2 className={verifytokencss['formTitle']}>Reset Password</h2>
          <div className={verifytokencss['inputDiv']}>
            <label className={verifytokencss['inputLabel']} htmlFor="password">
              New Password
            </label>
            <input
            data-testid='new-password'
              className={verifytokencss['pswdinput']}
              type={showAgentNewPassword ? 'text' : 'password'}
              autoComplete="off"
              {...register('password', {
                required: 'Password Required',
                onChange: (value) => {
                  let password = getValues('confirmPassword');
                  if (password) {
                    if (value.target.value !== password) {
                      setagentPasswordMatch(true);
                    } else setagentPasswordMatch(false);
                  }
                },
                pattern: {
                  value: PASSWORD_PATTERN,
                  message:
                    'Password must contain  8 to 16 characters, at least one uppercase letter, one lowercase letter, one number and one special character',
                },
              })}
            />
            <i className={verifytokencss['eyebutton']}>
              {showAgentNewPassword ? (
                <VisibilityIcon
                  onClick={showAgentNewPass}
                  className={verifytokencss['iconwidth']}
                  data-testid="eyebtn-newpassword-close"
                />
              ) : (
                <VisibilityOffIcon
                  onClick={showAgentNewPass}
                  className={verifytokencss['iconwidth']}
                  data-testid="eyebtn-newpassword-open"
                />
              )}
            </i>

            {errors.password && <small className={verifytokencss['message']}>{errors.password.message}</small>}
          </div>
          <div className={verifytokencss['inputDiv']}>
            <label className={verifytokencss['inputLabel']} htmlFor="confirmPassword">
              Confirm Password
            </label>
            <input
            data-testid='confirm-password'
              className={verifytokencss['pswdinput']}
              type={showAgentConfirmPassword ? 'text' : 'password'}
              autoComplete="off"
              {...register('confirmPassword', {
                required: 'Password Required',
                onChange: (value) => {
                  password = getValues('password');
                  if (password) {
                    if (value.target.value !== password) {
                      setagentPasswordMatch(true);
                    } else setagentPasswordMatch(false);
                  }
                },
                pattern: {
                  value: PASSWORD_PATTERN,
                  message:
                    'Password must contain  8 to 16 characters, at least one uppercase letter, one lowercase letter, one number and one special character',
                },
              })}
            />
            <i className={verifytokencss['eyebutton']}>
              {showAgentConfirmPassword ? (
                <VisibilityIcon
                  onClick={showAgentConfirmPass}
                  className={verifytokencss['iconwidth']}
                  data-testid="eyebtn-confirmpassword-close"
                />
              ) : (
                <VisibilityOffIcon
                  onClick={showAgentConfirmPass}
                  className={verifytokencss['iconwidth']}
                  data-testid="eyebtn-confirmpassword-open"
                />
              )}
            </i>
            {errors.confirmPassword ? (
              <small className={verifytokencss['message']}>{errors.confirmPassword.message}</small>
            ) : (
              <small className={verifytokencss['message']}>{agentPasswordMatch && 'Password Mismatch'}</small>
            )}
          </div>
          <div className={verifytokencss['buttonWrapper']}>
            <button type="submit" id="submitButton" className={verifytokencss['submitButton']} data-testid='submit-button'>
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
