import React, { useCallback, useState } from 'react';
import { useForm as UseForm } from 'react-hook-form';
import Logincss from './login.module.css';
import ForgotPassword from '../forgot-password/forgot-password';
import { useNavigate } from 'react-router-dom';
import Swal from 'sweetalert2';
import LoadingBar from 'react-top-loading-bar';
import VisibilityIcon from '@mui/icons-material/Visibility';
import VisibilityOffIcon from '@mui/icons-material/VisibilityOff';
import { googleLogin, login } from '../../coreApiService';
import { GoogleButton } from '../../../../utils/GoogleButton';
import { PASSWORD_PATTERN } from '../../../../utils/utils';

function Login() {
  const [progress, setProgress] = useState(0);
  const [showPassword, setShowPassword] = useState(false);
  let navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = UseForm({ mode: 'onChange' });
  const showPass = useCallback(() => {
    setShowPassword(!showPassword);
  }, [showPassword]);

  // default login
  const onSubmit = async (data) => {
    startLoader();
    login(data)
      .then((response) => {
        const accessToken = response?.data.data?.accessToken?.value;
        const refreshToken = response?.data.data?.refreshToken?.value;
        const role = 0;
        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
        localStorage.setItem('role', role);
        stopLoader();
        navigate('/dashboard/home');
      })
      .catch((err) => {
        stopLoader();
        Swal.fire({
          icon: 'error',
          title: 'Oops...',
          text: 'Invalid credentials ',
        });
      });

    reset();
  };

  //  google signin
  const handleCallbackResponse = useCallback((response) => {
    startLoader();
    const userObject = response.credential;
    googleLogin(userObject)
      .then((response) => {
        const accessToken = response?.data.data?.accessToken?.value;
        const refreshToken = response?.data.data?.refreshToken?.value;
        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
        localStorage.setItem('role', 0);
        if (response?.data?.data?.accessToken) {
          navigate('/dashboard/home');
        }
      })
      .catch((err) => {
        Swal.fire({
          icon: 'error',
          title: 'Oops...',
          text: 'Invalid credentials ',
        });
      });
    stopLoader();
  }, []);

  function startLoader() {
    setProgress(50);
  }
  function stopLoader() {
    setProgress(100);
  }
  const AgentClick = () => {
    localStorage.setItem('Navigation', true);
    navigate('/agent/login');
  };
  return (
    <>
      <LoadingBar color="blue" progress={progress} onLoaderFinished={useCallback(() => setProgress(0), [])} />
      <div className={Logincss['container']} data-testid="loginpage">
        <div className={Logincss.formdiv}>
          <div className={Logincss.box}>
            <form onSubmit={handleSubmit(onSubmit)}>
              <div></div>
              <h2 className={Logincss.carttitle}>ADMIN LOGIN</h2>
              <div>
                <input
                  className={Logincss.boxinputtext}
                  type="text"
                  data-testid="email-input"
                  autoComplete="off"
                  placeholder="Email"
                  {...register('email', {
                    required: 'Email required ',
                    pattern: {
                      value: /^[A-Z0-9._%+-]+@[A-z0-9.-]+\.[A-Z]{2,254}$/i,
                      message: 'Invalid Email address',
                    },
                  })}
                />
                <div className={Logincss.error}>
                  <small>{errors.email && errors.email.message}</small>
                </div>
              </div>
              <div>
                <div style={{ height: '42px' }}>
                  <input
                    className={Logincss.boxinputpassword}
                    type={showPassword ? 'text' : 'password'}
                    placeholder="Password"
                    data-testid="password-input"
                    autoComplete="off"
                    {...register('password', {
                      required: 'Password Required',
                      pattern: {
                        value: PASSWORD_PATTERN,
                        message: 'Invalid Password',
                      },
                    })}
                  />
                  <div className={Logincss['eyebutton']}>
                    {showPassword ? (
                      <VisibilityIcon
                        onClick={showPass}
                        className={Logincss.iconwidth}
                        data-testid="eyebtn-password-close"
                      />
                    ) : (
                      <VisibilityOffIcon
                        onClick={showPass}
                        className={Logincss.iconwidth}
                        data-testid="eyebtn-password-open"
                      />
                    )}
                  </div>
                </div>
                <div className={Logincss.error}>{errors.password && <small>{errors.password.message}</small>}</div>
              </div>
              <button data-testid="login-button" className={Logincss.login} type="submit" value="submit">
                LogIn
              </button>
            </form>
            <div>
              <div className={Logincss.forgot} data-testid="forgot-password">
                <ForgotPassword />
              </div>

              <div className={Logincss.form}>
                <GoogleButton response={handleCallbackResponse} />
              </div>
              <button
                className={Logincss.cta}
                data-testid="agentclick"
                onClick={() => {
                  AgentClick();
                }}
              >
                <span className={Logincss.hoverunderlineanimation}> Agent Login </span>
                <svg
                  viewBox="0 0 46 16"
                  height="10"
                  width="30"
                  xmlns="http://www.w3.org/2000/svg"
                  id="arrow-horizontal"
                >
                  <path
                    transform="translate(20)"
                    d="M8,0,6.545,1.455l5.506,5.506H-30V9.039H12.052L6.545,14.545,8,16l8-8Z"
                    data-name="Path 10"
                    id="Path_10"
                    fill="#206dfd"
                  ></path>
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}

export default Login;
