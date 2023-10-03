import React, { useCallback, useEffect, useMemo, useState } from "react";
import {
  Step,
  StepLabel,
  Stepper,
  Box,
  Alert,
  IconButton,
  StepConnector,
  stepConnectorClasses,
  styled,
} from "@mui/material";
import {
  activeStepper,
  getOrderStatuses,
  stepperValues,
} from "../../../../Utils/Data/Data";
import CancelIcon from "@mui/icons-material/Cancel";
import { getOrderStatusHistory } from "../../../../../core/Api/apiService";
import { convertDate, convertTime } from "../../../../Utils/Utils";
import CloseIcon from "@mui/icons-material/Close";

const StepperView = ({ orderDetails, orderStatus, horizontal }) => {
  const [statusHistory, setStatusHistory] = useState([]);
  const [selectedStep, setSelectedStep] = useState(null);
  const [tooltipValue, setTooltipValue] = useState("");
  const [canceledStepNumber, setCanceledStepNumber] = useState(0);

  const horizontalStyle = { width: "100%", cursor: "pointer" };
  const verticalStyle = {
    display: "block",
    marginRight: "30px",
    marginTop: "10px",
  };

  useEffect(() => {
    const getStatusHistory = () => {
      getOrderStatusHistory(orderDetails?.orderDetailsId)
        .then((response) => {
          setStatusHistory(response?.data?.data);
          let data = response?.data?.data;
          setCanceledStepNumber(data[data?.length - 2]?.status);
        })
        .catch((err) => {
          console.log(err);
        });
    };
    if (orderDetails?.orderDetailsId) {
      getStatusHistory();
    }
  }, [orderDetails?.orderDetailsId]);

  let newStepperValues = useMemo(() => {
    if (orderStatus === 6) {
      const result = [];
      for (const obj of stepperValues) {
        result.push(obj);
        if (obj.id === canceledStepNumber - 2) {
          break;
        }
      }
      result.push({
        id: 10,
        value: "Cancelled",
        status: 6,
      });
      return result;
    }
    return [];
  }, [orderStatus, canceledStepNumber]);

  const CustomisedConnectorWhenCancel = styled(StepConnector)(({ theme }) => ({
    [`&.${stepConnectorClasses.active}`]: {
      [`& .${stepConnectorClasses.line}`]: {
        backgroundColor: "red",
      },
    },
    [`&.${stepConnectorClasses.completed}`]: {
      [`& .${stepConnectorClasses.line}`]: {
        backgroundColor: "green",
      },
    },
    [`& .${stepConnectorClasses.line}`]: {
      height: 3,
      border: "10px",
      backgroundColor: "grey",
      borderRadius: 1,
    },
  }));

  const CustomisedConnector = styled(StepConnector)(({ theme }) => ({
    [`&.${stepConnectorClasses.active}`]: {
      [`& .${stepConnectorClasses.line}`]: {
        background: "linear-gradient(90deg, blue 50%, grey 50%, grey 100%)",
      },
    },
    [`&.${stepConnectorClasses.completed}`]: {
      [`& .${stepConnectorClasses.line}`]: {
        backgroundColor: "green",
      },
    },
    [`& .${stepConnectorClasses.line}`]: {
      height: 3,
      border: "10px",
      backgroundColor: "grey",
      borderRadius: 1,
    },
  }));

  const CustomStepLabel = styled(StepLabel)(({ theme }) => ({
    "& .MuiStepIcon-root.Mui-completed": {
      color: "green",
    },
  }));

  const CustomisedStepIcon = styled("div")(({ theme }) => ({
    color: theme.palette.success.main, // set the color to green when completed
    "&.MuiStepIcon-active": {
      color: theme.palette.primary.main, // set the color for the active step
    },
    "&.MuiStepIcon-completed": {
      color: theme.palette.success.main, // set the color to green when completed
    },
    "&.MuiStepIcon-cancel": {
      color: "red", // set the color to red when canceled
    },
  }));

  const renderStepIcon = (index, canceled) => {
    if (canceled) {
      return (
        <CustomisedStepIcon className="MuiStepIcon-cancel">
          <span>
            <CancelIcon style={{ fontSize: "25px" }} />
          </span>
        </CustomisedStepIcon>
      );
    } else {
      return index + 1;
    }
  };

  const handleStepClick = useCallback(
    (step) => {
      if (selectedStep === step) {
        setSelectedStep(null);
      } else {
        let currentStatus =
          orderStatus === 6
            ? newStepperValues.find((item) => item.id === step)
            : stepperValues[step];

        const selectedHistory = statusHistory.find(
          (item) => item.status === currentStatus?.status
        );

        if (selectedHistory) {
          setTooltipValue(selectedHistory);
          setSelectedStep(step); // Set the selected step
        }
      }
    },
    [newStepperValues, orderStatus, selectedStep, statusHistory]
  );

  const handleModalClose = useCallback(() => {
    setSelectedStep(null);
  }, []);

  return (
    <>
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          position: "relative",
          cursor: "pointer",
        }}
      >
        <Stepper
          orientation={horizontal ? "horizontal" : "vertical"}
          alternativeLabel
          activeStep={activeStepper(
            orderStatus === 6 ? canceledStepNumber : orderStatus
          )}
          connector={
            orderStatus === 6 ? (
              <CustomisedConnectorWhenCancel />
            ) : (
              <CustomisedConnector />
            )
          }
          sx={horizontal ? horizontalStyle : verticalStyle}
        >
          {(orderStatus === 6 ? newStepperValues : stepperValues).map(
            (data) => {
              return (
                <Step
                  key={data.id}
                  sx={{ cursor: "pointer" }}
                  className={`step-item${
                    selectedStep === data.id ? " active" : ""
                  }`}
                  onClick={() => handleStepClick(data.id)}
                >
                  <StepLabel
                    StepIconComponent={CustomStepLabel}
                    sx={{ cursor: "pointer" }}
                    cancelicon={CustomisedStepIcon} // Use the custom StepIcon component
                    StepIconProps={{
                      icon: renderStepIcon(data.id, data.value === "Cancelled"), // Render custom icon for canceled step
                    }}
                  >
                    {data.value}
                  </StepLabel>
                </Step>
              );
            }
          )}
        </Stepper>
      </Box>

      {selectedStep !== null && tooltipValue && (
        <Alert
          severity={tooltipValue.status === 6 ? "error" : "success"}
          action={
            <IconButton
              data-testid="history-view-close-button"
              aria-label="close"
              color="inherit"
              size="small"
              onClick={handleModalClose}
            >
              <CloseIcon fontSize="inherit" />
            </IconButton>
          }
        >
          <div key={tooltipValue?.id} style={{ alignItems: "center" }}>
            {tooltipValue?.status && (
              <p>
                <strong>Status:</strong>{" "}
                {getOrderStatuses(tooltipValue?.status)}
              </p>
            )}
            {tooltipValue?.date && (
              <p>
                <strong>Date:</strong> {convertDate(tooltipValue?.date)}
              </p>
            )}
            {tooltipValue?.date && (
              <p>
                <strong>Time:</strong> {convertTime(tooltipValue?.date)}
              </p>
            )}
          </div>
        </Alert>
      )}
    </>
  );
};

export default StepperView;
