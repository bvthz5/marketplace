import React, { useEffect, useState, useCallback } from 'react';
import { useForm as UseForm } from 'react-hook-form';
import Swal from 'sweetalert2';
import style from './forgotpassword-agent.module.css';
import Box from '@mui/material/Box';
import Modal from '@mui/material/Modal';
import { forgotPasswordAgent } from '../../core/api/coreApiService';
import useWindowDimensions from '../../utils/WindowDimensions';

const ForgotPasswordAgent = () => {
  const [loadingForgotPassword, setLoading] = useState(false);
  const [openForgotModal, setOpenForgotModal] = useState(false);

  const { width } = useWindowDimensions();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = UseForm({ mode: 'onChange' });

  useEffect(() => {
    console.log(openForgotModal);
  }, [openForgotModal]);

  const handleOpenForgotModal = () => setOpenForgotModal(true);
  const handleCloseForgotModal = useCallback(() => {
    setOpenForgotModal(false);
    reset();
  });

  //forgot password//
  const handleUpdate = async (data) => {
    setLoading(true);
    forgotPasswordAgent(data.email)
      .then((response) => {
        setLoading(false);
        if (response.data.status) {
          Swal.fire({
            icon: 'success',
            title: 'Email sent!',
            text: `An email has been sent to ${data.email} for changing  password`,
            showConfirmButton: false,
            timer: 2500,
          });
          handleCloseForgotModal();
        }
      })
      .catch((err) => {
        setLoading(false);
        let errMsg = 'Agent not found';
        if (err.response.data.message === errMsg) {
          Swal.fire({
            title: `${errMsg}`,
            text: ` ${data.email} is not an active Agent account!`,
            icon: 'error',
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'OK',
          });
        }
        handleCloseForgotModal();
      });
  };

  const stylemodal = {
    position: 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    width: width < 500 ? '95%' : 550,
    height: '230px',
    bgcolor: 'background.paper',
    borderRadius: '10px',
    border: '0px',
  };

  return (
    <>
      <div data-testid="forgotpasswordagentpage">
        <div className={style['forgot']} onClick={handleOpenForgotModal} data-testid="forgotpasswordbutton">
          Forgot Password
        </div>
        <Modal
          open={openForgotModal}
          onClose={handleCloseForgotModal}
          aria-labelledby="modal-modal-title"
          aria-describedby="modal-modal-description"
        >
          <Box sx={stylemodal}>
            <div className={style['container']} data-testid="forgotpasswordpage">
              <div className={style['login-block']}>
                <span
                  className={style['close']}
                  data-testid="close-button"
                  onClick={() => {
                    handleCloseForgotModal();
                  }}
                >
                  &times;
                </span>
                <div className={style['login-panel']}>
                  <h3 data-testid="passwordmodal">Forgot Password?</h3>
                </div>
                <div className={style['sign-in']}>
                  <form onSubmit={handleSubmit(handleUpdate)}>
                    <div className={style['mdl-textfield mdl-js-textfield mdl-textfield--floating-label full']}>
                      <input
                        className={style['form__field']}
                        type="text"
                        data-testid="email-input"
                        placeholder="Enter your email"
                        id={style['email']}
                        {...register('email', {
                          required: 'Email required ',
                          pattern: {
                            value: /^[A-Z0-9._%+-]+@[A-z0-9.-]+\.[A-Z]{2,}$/i,
                            message: 'Invalid Email address',
                          },
                        })}
                      />
                      <br></br>
                      {errors.email && <small className={style.red}>{errors.email.message}</small>}
                    </div>
                    <br />
                    <button
                      type="submit"
                      id={style['submit']}
                      data-testid="submit-button"
                      className={style['submitpass']}
                    >
                      submit
                    </button>
                  </form>
                </div>
              </div>
              {loadingForgotPassword && <div className={style['loading']}>Loading…</div>}
            </div>
          </Box>
        </Modal>
      </div>
    </>
  );
};

export default ForgotPasswordAgent;
