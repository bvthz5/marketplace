import React, { useEffect, useCallback, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import LoadingBar from 'react-top-loading-bar';
import { GoogleButton } from '../../../../../src/utils/GoogleButton';
import Swal from 'sweetalert2';
import { googleLoginAgent, loginAgent } from '../../coreApiService';
import ForgotPasswordAgent from '../../../../agentpages/ForgotPasswordAgent/forgotpassword-agent';
import Agentlogincss from './AgentLogin.module.css';
import VisibilityIcon from '@mui/icons-material/Visibility';
import VisibilityOffIcon from '@mui/icons-material/VisibilityOff';
import KeyboardBackspaceIcon from '@mui/icons-material/KeyboardBackspace';
import { PASSWORD_PATTERN } from '../../../../utils/utils';

const AgentLogin = () => {
  const [progress, setProgress] = useState(0);
  const [showPassword, setShowPassword] = useState(false);
  const [admin, setAdmin] = useState(false);

  useEffect(() => {
    if (localStorage.getItem('Navigation') === 'true') setAdmin(true);
  }, []);

  let navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm({ mode: 'onChange' });
  const showPass = useCallback(() => {
    setShowPassword(!showPassword);
  }, [showPassword]);

  // default login
  const onSubmit = async (data) => {
    startLoader();
    loginAgent(data)
      .then((response) => {

        console.log(response.data.data);
        const accessToken = response?.data.data?.accessToken?.value;
        const refreshToken = response?.data.data?.refreshToken?.value;
        const status = response?.data.data?.status;
        const role = 1;
        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
        localStorage.setItem('role', role);
        stopLoader();
        if (status === 0) {
          localStorage.setItem('isInactive', true);
          navigate('/changepassword');
        } else {
          navigate('/agentdashboard/home');
        }
      })
      .catch((err) => {
        const error = err.response.data.message;
        handleErrorResponse(error);
        console.log(err);
      });
    stopLoader();
    reset();
  };

  //  google signin
  const handleCallbackResponse = useCallback((response) => {
    startLoader();
    const userObject = response.credential;
    googleLoginAgent(userObject)
      .then((response) => {
        console.log(response.data.data);
        if (response.data.status) {
          const accessToken = response?.data.data?.accessToken?.value;
          const refreshToken = response?.data.data?.refreshToken?.value;
          const status = response?.data.data?.status;
          const role = 1;
          localStorage.setItem('accessToken', accessToken);
          localStorage.setItem('refreshToken', refreshToken);
          localStorage.setItem('role', role);
          if (status === 0) {
            localStorage.setItem('isInactive', true);
            navigate('/changepassword');
          } else {
            navigate('/agentdashboard/home');
          }
        }
      })
      .catch((err) => {
        const error = err.response.data.message;
        handleErrorResponse(error);
        console.log(err);
      });
    stopLoader();
  }, []);

  const handleErrorResponse = (error) => {
    const email = 'Invalid Credentials';
    const blockedError = 'Agent BLOCKED';
    const noPassword = 'Password not set';
    const deletedError = 'Agent not found';
    if (error === email) {
      Swal.fire({
        icon: 'error',
        title: `${email}`,
      });
    } else if (error === blockedError) {
      Swal.fire({
        icon: 'error',
        title: 'Blocked',
        text: 'You have been blocked from this website',
      });
    } else if (error === noPassword) {
      Swal.fire({
        icon: 'error',
        title: "You don't have a password",
        text: 'Please use forgot password for setting a password ',
      });
    } else if (error === deletedError) {
      Swal.fire({
        icon: 'error',
        title: 'Agent Not Found',
      });
    }
  };

  function startLoader() {
    setProgress(50);
  }
  function stopLoader() {
    setProgress(100);
  }
  const goBack = useCallback(() => {
    navigate(-1);
    localStorage.setItem('Navigation', false);
  }, []);
  return (
    <>
      <LoadingBar color="blue" progress={progress} onLoaderFinished={useCallback(() => setProgress(0), [])} />
      <div className={Agentlogincss['container']} data-testid="loginpage">
        <div className={Agentlogincss.formdiv}>
          <div className={Agentlogincss.box}>
            <div className={Agentlogincss.keyboardBackspaceIcon} data-testid="goback">
              {admin && <KeyboardBackspaceIcon onClick={goBack} />}
            </div>
            <form onSubmit={handleSubmit(onSubmit)} action="index.html">
              <h2 className={Agentlogincss.carttitle}>AGENT LOGIN</h2>
              <input
                className={Agentlogincss.boxinputtext}
                type="text"
                autoComplete="off"
                data-testid="email-input"
                placeholder="Email"
                {...register('email', {
                  required: 'Email required ',
                  pattern: {
                    value: /^[A-Z0-9._%+-]+@[A-z0-9.-]+\.[A-Z]{2,254}$/i,
                    message: 'Invalid Email address',
                  },
                })}
              />
              <div className={Agentlogincss.error}>
                <small>{errors.email && errors.email.message}</small>
              </div>
              <div>
                <input
                  className={Agentlogincss.boxinputpassword}
                  type={showPassword ? 'text' : 'password'}
                  placeholder="Password"
                  autoComplete="off"
                  data-testid="password-input"
                  {...register('password', {
                    required: 'Password Required',
                    pattern: {
                      value: PASSWORD_PATTERN,
                      message: 'Invalid Password',
                    },
                  })}
                />
                <div className={Agentlogincss['eyebutton']}>
                  {showPassword ? (
                    <VisibilityIcon
                      onClick={showPass}
                      className={Agentlogincss.iconwidth}
                      data-testid="eyebtn-password-close"
                    />
                  ) : (
                    <VisibilityOffIcon
                      onClick={showPass}
                      className={Agentlogincss.iconwidth}
                      data-testid="eyebtn-password-open"
                    />
                  )}
                </div>
              </div>
              <div className={Agentlogincss.error}>{errors.password && <small>{errors.password.message}</small>}</div>

              <button className={Agentlogincss.login} type="submit" value="submit" data-testid="login-button">
                LogIn
              </button>
              <br></br>
            </form>
            <div className={Agentlogincss.forgot} data-testid="forgot-password">
              <ForgotPasswordAgent />
            </div>
            <div className={Agentlogincss.form}>
              <GoogleButton response={handleCallbackResponse} />
            </div>
            <br></br>
          </div>
        </div>
      </div>
    </>
  );
};

export default AgentLogin;
