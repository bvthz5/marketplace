import React, { useEffect, useState } from 'react';
import {
  Step,
  StepLabel,
  Stepper,
  Box,
  Typography,
  Alert,
  IconButton,
  StepConnector,
  stepConnectorClasses,
  styled,
} from '@mui/material';

import CancelIcon from '@mui/icons-material/Cancel';
import CloseIcon from '@mui/icons-material/Close';
import { getOrderStatusHistory } from '../../../../core/api/apiService';

import { activeStepper, getOrderStatuses, stepperValues } from './data';
import { convertDate } from '../../../../utils/formatDate';
import { convertTime } from '../../../../utils/utils';

const StepperView = ({ orderDetails, orderStatus, horizontal }) => {
  const [statusHistory, setStatusHistory] = useState([]);
  const [selectedStep, setSelectedStep] = useState(null);
  const [tooltipOpen, setTooltipOpen] = useState(false);
  const [tooltipValue, setTooltipValue] = useState('');
  const [canceledStepNumber, setCanceledStepNumber] = useState(0);

  const horizontalStyle = { width: '100%', cursor: 'pointer' };
  const verticalStyle = {
    display: 'block',
    marginRight: '30px',
    marginTop: '10px',
  };

  useEffect(() => {
    if (orderDetails?.orderDetailsId) {
      getStatusHistory();
    }
  }, []);

  const [open, setOpen] = React.useState(true);

  const getStatusHistory = () => {
    getOrderStatusHistory(orderDetails?.orderDetailsId)
      .then((response) => {
        setStatusHistory(response?.data?.data);
        let data = response?.data?.data;
        setCanceledStepNumber(data[data.length - 2].status);
      })
      .catch((err) => {
        console.log(err);
      });
  };
  let newStepperValues = [];
  if (orderStatus === 6) {
    const result = [];
    for (const obj of stepperValues) {
      result.push(obj);
      if (obj.id === canceledStepNumber - 2) {
        break;
      }
    }
    newStepperValues = result;
    newStepperValues.push({
      id: 10,
      value: 'Cancelled',
      status: 6,
    });
  }

  const CustomisedConnectorWhenCancel = styled(StepConnector)(({ theme }) => ({
    [`&.${stepConnectorClasses.active}`]: {
      [`& .${stepConnectorClasses.line}`]: {
        backgroundColor: 'red',
      },
    },
    [`&.${stepConnectorClasses.completed}`]: {
      [`& .${stepConnectorClasses.line}`]: {
        backgroundColor: 'green',
      },
    },
    [`& .${stepConnectorClasses.line}`]: {
      height: 3,
      border: '10px',
      backgroundColor: 'grey',
      borderRadius: 1,
    },
  }));

  const CustomisedConnector = styled(StepConnector)(({ theme }) => ({
    [`&.${stepConnectorClasses.active}`]: {
      [`& .${stepConnectorClasses.line}`]: {
        background: 'linear-gradient(90deg, blue 50%, grey 50%, grey 100%)',
      },
    },
    [`&.${stepConnectorClasses.completed}`]: {
      [`& .${stepConnectorClasses.line}`]: {
        backgroundColor: 'green',
      },
    },
    [`& .${stepConnectorClasses.line}`]: {
      height: 3,
      border: '10px',
      backgroundColor: 'grey',
      borderRadius: 1,
    },
  }));

  const CustomStepLabel = styled(StepLabel)(({ theme }) => ({
    '& .MuiStepIcon-root.Mui-completed': {
      color: 'green',
    },
  }));

  const CustomisedStepIcon = styled('div')(({ theme }) => ({
    color: theme.palette.success.main, // set the color to green when completed
    '&.MuiStepIcon-active': {
      color: theme.palette.primary.main, // set the color for the active step
    },
    '&.MuiStepIcon-completed': {
      color: theme.palette.success.main, // set the color to green when completed
    },
    '&.MuiStepIcon-cancel': {
      color: 'red', // set the color to red when canceled
    },
  }));

  const renderStepIcon = (index, canceled) => {
    if (canceled) {
      return (
        <CustomisedStepIcon className="MuiStepIcon-cancel">
          <span className="">
            <CancelIcon style={{ fontSize: '25px' }} />
          </span>
        </CustomisedStepIcon>
      );
    } else {
      return index + 1;
    }
  };

  const handleStepClick = (step) => {
    if (selectedStep === step) {
      // If the same step is clicked, close the modal
      setSelectedStep(null);
    } else {
      let currentStatus = 0;
      if (orderStatus === 6) {
        newStepperValues.map((item) => {
          if (item.id === step) {
            currentStatus = item;
          }
        });
      } else {
        currentStatus = stepperValues[step];
      }
      console.log('current status', currentStatus);
      console.log('new stepValue', newStepperValues);

      const selectedHistory = statusHistory.find((item) => item.status === currentStatus.status);
      if (selectedHistory) {
        console.log('history', selectedHistory);
        setTooltipValue(selectedHistory);
        setSelectedStep(step); // Set the selected step
      }
    }
  };

  const handleModalClose = () => {
    setSelectedStep(null);
  };
  return (
    <>
      <Box
        sx={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          position: 'relative',
          cursor: 'pointer',
        }}
      >
        <Stepper
          orientation={horizontal ? 'horizontal' : 'vertical'}
          alternativeLabel
          activeStep={activeStepper(orderStatus === 6 ? canceledStepNumber : orderStatus)}
          connector={orderStatus === 6 ? <CustomisedConnectorWhenCancel /> : <CustomisedConnector />}
          sx={horizontal ? horizontalStyle : verticalStyle}
        >
          {(orderStatus === 6 ? newStepperValues : stepperValues).map((data) => {
            return (
              <Step
                key={data.id}
                sx={{ cursor: 'pointer' }}
                className={`step-item${selectedStep === data.id ? ' active' : ''}`}
                onClick={() => handleStepClick(data.id)}
              >
                <StepLabel
                  StepIconComponent={CustomStepLabel}
                  sx={{ cursor: 'pointer' }}
                  cancelicon={CustomisedStepIcon} // Use the custom StepIcon component
                  StepIconProps={{
                    icon: renderStepIcon(data.id, data.value === 'Cancelled'), // Render custom icon for canceled step
                  }}
                >
                  {data.value}
                </StepLabel>
              </Step>
            );
          })}
        </Stepper>

        {tooltipOpen && (
          <Typography
            variant="body2"
            sx={{
              position: 'absolute',
              bottom: '100%',
              left: '50%',
              transform: 'translateX(-50%)',
              padding: '5px 10px',
              borderRadius: '5px',
            }}
          ></Typography>
        )}
      </Box>

      {selectedStep !== null && tooltipValue && (
        <Alert
          severity={tooltipValue.status === 6 ? 'error' : 'success'}
          action={
            <IconButton
              aria-label="close"
              color="inherit"
              size="small"
              onClick={() => {
                setOpen(false);
                handleModalClose();
              }}
            >
              <CloseIcon fontSize="inherit" />
            </IconButton>
          }
        >
          <div
            key={tooltipValue?.id}
            style={{ display: 'flex', flexDirection: 'column', alignItems: 'start', gap: '5px' }}
          >
            {tooltipValue?.status && (
              <>
                <div>
                  <strong>Status:</strong> {getOrderStatuses(tooltipValue?.status)}
                </div>
                <div>
                  <strong>Date:</strong> {convertDate(tooltipValue?.date)}
                </div>
                <div>
                  <strong>Time:</strong> {convertTime(tooltipValue?.date)}
                </div>
              </>
            )}
          </div>
        </Alert>
      )}
    </>
  );
};

export default StepperView;
