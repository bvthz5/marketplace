import React, { useState, useCallback } from 'react';
import { useForm as UseForm } from 'react-hook-form';
import Swal from 'sweetalert2';
import { forgotPassword } from '../../coreApiService';
import style from './Forgotpassword.module.css';
import Box from '@mui/material/Box';
import Modal from '@mui/material/Modal';
import useWindowDimensions from '../../../../utils/WindowDimensions';

const ForgotPassword = () => {
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);
  const handleOpen = () => setOpen(true);
  const handleClose = useCallback(() => {
    setOpen(false);
    reset();
  });

  const { width } = useWindowDimensions();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = UseForm({ mode: 'onChange' });

  //forgot password//
  const handleUpdate = async (data) => {
    setLoading(true);
    forgotPassword(data.email)
      .then((response) => {
        setLoading(false);
        Swal.fire({
          icon: 'success',
          title: 'Email sent!',
          text: `An email has been sent to ${data.email} for changing  password`,
          showConfirmButton: false,
          timer: 2500,
        });
        handleClose();
      })
      .catch((err) => {
        setLoading(false);
        let errMsg = 'Not Found';
        if (err.response.data.message === errMsg) {
          Swal.fire({
            title: `${errMsg}`,
            text: ` ${data.email} is not an active admin account!`,
            icon: 'error',
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'OK',
          });
        }
        handleClose();
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
      <div data-testid="forgotpasswordpage">
        <div className={style['forgot']} onClick={handleOpen} data-testid="forgotpasswordbutton">
          Forgot Password
        </div>
        <Modal
          open={open}
          onClose={handleClose}
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
                    handleClose();
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
                        data-testid="email-input"
                        className={style['form__field']}
                        type="text"
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
                      className={style['submitpass']}
                      data-testid="submit-button"
                    >
                      submit
                    </button>
                  </form>
                </div>
              </div>
              {loading && <div className={style['loading']}>Loading…</div>}
            </div>
          </Box>
        </Modal>
      </div>
    </>
  );
};

export default ForgotPassword;
