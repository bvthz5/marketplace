import React, { useCallback, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import Button from '@mui/material/Button';
import { styled } from '@mui/material/styles';
import Dialog from '@mui/material/Dialog';
import DialogTitle from '@mui/material/DialogTitle';
import DialogContent from '@mui/material/DialogContent';
import DialogActions from '@mui/material/DialogActions';
import IconButton from '@mui/material/IconButton';
import CloseIcon from '@mui/icons-material/Close';
import Typography from '@mui/material/Typography';
import { MuiOtpInput } from 'mui-one-time-password-input';
import { Box, CircularProgress, useMediaQuery, useTheme } from '@mui/material';
import { LoadingButton } from '@mui/lab';
import { generateOtp, verifyOtp } from '../../../../core/api/apiService';
import { toast } from 'react-toastify';
import useWindowDimensions from '../../../../utils/WindowDimensions';
import Swal from 'sweetalert2';

const BootstrapDialog = styled(Dialog)(({ theme }) => ({
  '& .MuiDialogContent-root': {
    padding: theme.spacing(2),
  },
  '& .MuiDialogActions-root': {
    padding: theme.spacing(1),
  },
}));

function BootstrapDialogTitle(props) {
  const { children, onClose, ...other } = props;

  return (
    <DialogTitle sx={{ m: 0, p: 2 }} {...other}>
      {children}
      <IconButton
        aria-label="close"
        onClick={onClose}
        sx={{
          position: 'absolute',
          right: 8,
          top: 8,
          color: (theme) => theme.palette.grey[500],
        }}
      >
        <CloseIcon />
      </IconButton>
    </DialogTitle>
  );
}

BootstrapDialogTitle.propTypes = {
  children: PropTypes.node,
  onClose: PropTypes.func.isRequired,
};

export default function DeliveryOtp({ orderId, email, onCompleted }) {
  const [open, setOpen] = useState(false);
  const [otp, setOtp] = useState('');
  const [timer, setTimer] = useState(0);
  const [apiCall, setApiCall] = useState(false);
  const [verifyLoader, setVerifyLoading] = useState(false);

  const { width } = useWindowDimensions();

  const theme = useTheme();
  const fullScreen = useMediaQuery(theme.breakpoints.down('md'));

  useEffect(() => {
    const interval = setInterval(() => {
      if (timer) {
        setTimer((time) => time - 1);
      }
    }, 1000);

    return () => clearInterval(interval);
  }, [timer]);

  const handleChange = useCallback((newValue) => {
    setOtp(newValue);
  }, []);

  const sendOtpRequest = useCallback(() => {
    setApiCall(true);

    generateOtp(orderId)
      .then(() => {
        toast.success('Otp Sent Successfully');
      })
      .catch((err) => {
        console.error(err);
        if (err.response.data.message.startsWith('Wait for')) {
          setTimer(err.response.data.data);
        } else if (err.response.data.message === 'Order Status : DELIVERED') {
          onCompleted();
          Swal.fire({
            icon: 'info',
            title: 'Order Already Marked As Delivered',
          });
        }
      })
      .finally(() => {
        setApiCall(false);
      });
  }, [orderId]);

  const handleClickOpen = useCallback(() => {
    if (!timer) {
      setTimer(60);
      sendOtpRequest();
    }
    setOpen(true);
  }, [timer]);

  const handleResend = useCallback(() => {
    setOtp('');
    setTimer(60);
    sendOtpRequest();
  }, []);

  const handleClose = useCallback(() => {
    setOtp('');
    setOpen(false);
  }, []);

  const matchIsNumeric = useCallback((value) => {
    if (value === '') return false;

    return !/\D/g.test(value);
  }, []);

  const handleKeyDown = useCallback(
    (e) => {
      if (e.key === 'Backspace') setOtp(otp.substring(0, otp.length - 1));
    },
    [otp]
  );

  const handleVerify = useCallback(() => {
    setVerifyLoading(true);

    verifyOtp(orderId, otp)
      .then(() => {
        onCompleted();
        Swal.fire({
          icon: 'success',
          title: 'Order Marked As Delivered',
        });
      })
      .catch((err) => {
        console.error(err);
        if (err.response.data.message === 'Invalid Otp') {
          toast.error('Invalid Otp', { toastId: 'Invalid Otp' });
        } else if (err.response.data.message === 'Otp Timeout') {
          toast.error('Otp Timeout');
          sendOtpRequest();
          setOtp('');
        } else if (err.response.data.message === 'Order Status : DELIVERED') {
          onCompleted();
          Swal.fire({
            icon: 'info',
            title: 'Order Already Marked As Delivered',
          });
        }
      })
      .finally(() => {
        setVerifyLoading(false);
      });
  }, [orderId, otp]);

  return (
    <>
      <Button data-testid="deliver-button" variant="outlined" style={{ width: '100%' }} onClick={handleClickOpen}>
        Deliver
      </Button>
      <BootstrapDialog
        fullScreen={fullScreen}
        onClose={handleClose}
        aria-labelledby="customized-dialog-title"
        open={open}
      >
        <BootstrapDialogTitle data-testid="dialog-title" id="customized-dialog-title" onClose={handleClose}>
          Delivery Otp
        </BootstrapDialogTitle>
        <DialogContent dividers>
          {apiCall ? (
            <Box minWidth={'35vw'} display={'flex'} flexDirection={'column'} alignItems={'center'}>
              <Typography>Generating otp...</Typography>
              <CircularProgress size={30} color="success" />
            </Box>
          ) : (
            <>
              <Typography gutterBottom style={{ wordBreak: 'break-all' }}>
                Please enther the otp sent to {email}
              </Typography>
              <MuiOtpInput
                TextFieldsProps={{
                  placeholder: '-',
                }}
                length={6}
                value={otp}
                validateChar={matchIsNumeric}
                onChange={handleChange}
                onKeyDown={handleKeyDown}
                gap={width < 468 ? 0 : 1}
              />
              <Box style={{ display: 'flex', flexDirection: 'row-reverse' }}>
                <Button onClick={handleResend} disabled={!!timer}>
                  Resend
                </Button>
                {!!timer && (
                  <Typography data-testid="wait-for-msg" style={{ paddingTop: '5px' }}>
                    wait for {timer} seconds
                  </Typography>
                )}
              </Box>
            </>
          )}
        </DialogContent>

        <DialogActions>
          <LoadingButton
            style={{ marginRight: '2%' }}
            color="success"
            autoFocus
            onClick={handleVerify}
            loadingIndicator={
              <>
                Verifing...
                <CircularProgress size={20}></CircularProgress>
              </>
            }
            disabled={apiCall || otp.length !== 6}
            loading={verifyLoader}
          >
            Verify
          </LoadingButton>
        </DialogActions>
      </BootstrapDialog>
    </>
  );
}
