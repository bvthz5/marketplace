import React, { useCallback, useState } from "react";
import styles from "./CancelOrder.module.css";
import { useForm } from "react-hook-form";
import { Box, Modal, Typography } from "@mui/material";
import { cancelOrderByOrderId } from "../../../../../../core/Api/apiService";
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom";
import CircleLoader from "../../../../../Utils/Loaders/CircleLoader/CircleLoader";
import Swal from "sweetalert2";

const modalStyle = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 400,
  height: "260px",
  bgcolor: "background.paper",
  borderRadius: "20px",
};

const CancelOrder = ({ orderDetails }) => {
  let navigate = useNavigate();
  const [open, setOpen] = useState(false);
  const [apiCall, setApiCall] = useState(false);
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm({ mode: "onChange" });
  
  const handleOpen = () => setOpen(true);
  const handleClose = useCallback(() => {
    setOpen(false);
    reset();
  }, [reset]);


  const handleCancelRequest = (e) => {
    if (e && e.reason && e.reason.trim() === "") {
      toast.error("Whitespaces are not allowed!");
      return;
    }
    setApiCall(true);
    cancelOrderByOrderId(orderDetails?.orderDetailsId, e.reason)
      .then((response) => {
        setApiCall(false);
        Swal.fire(
          "Cancel request submitted successfully.Money will be refunded within 3-4 days "
        );
        navigate("/orders");
      })
      .catch((err) => {
        setApiCall(false);
        console.log(err);
        toast.error("error occurred while processing your request");
      });
    handleClose();
  };

  return (
    <>
      <div>
        {orderDetails?.orderStatus === 6 ? (
          <button
            className={styles["button"]}
            data-testid="cancelorderbtn"
            disabled
            style={{ backgroundColor: "rgb(238, 204, 204)", color: "red" }}
          >
            Order Cancelled
          </button>
        ) : (
          <button
            data-testid="cancelorderbtn"
            className={styles["button"]}
            onClick={handleOpen}
          >
            Cancel Order
          </button>
        )}
        <Modal
          open={open}
          onClose={handleClose}
          aria-labelledby="modal-modal-title"
          aria-describedby="modal-modal-description"
        >
          <Box sx={modalStyle}>
            <div className={styles["modalhead"]} data-testid="modalbody">
              <Typography className={styles["modalheading"]}>
                Reason for Cancelling
              </Typography>
            </div>
            <div className={styles["formdiv"]}>
              <form onSubmit={handleSubmit(handleCancelRequest)}>
                <div className={styles["inputdiv"]}>
                  <input
                    data-testid="cancel-reason"
                    className={styles["modalinputfield"]}
                    type="text"
                    placeholder="reason"
                    {...register("reason", {
                      required: "reason is required",
                      maxLength: {
                        value: 255,
                        message: "Maximum 255 Characters",
                      },
                    })}
                  ></input>
                  <div style={{ minHeight: "15px" }}>
                    {errors.reason && (
                      <div className={styles["error"]}>
                        {errors.reason.message}
                      </div>
                    )}
                  </div>
                </div>

                <div
                  style={{
                    display: "grid",
                    paddingBottom: "20px",
                  }}
                >
                  <button
                    data-testid="submit-btn"
                    className={styles["modalsubmitbutton"]}
                    type="submit"
                    value="submit"
                  >
                    Submit
                  </button>{" "}
                  <button
                    data-testid="modalclosebtn"
                    onClick={() => {
                      handleClose();
                      reset();
                    }}
                    className={styles["modalcancelButton"]}
                  >
                    Cancel
                  </button>
                </div>
              </form>
            </div>
          </Box>
        </Modal>
      </div>
      {apiCall && <CircleLoader />}
    </>
  );
};

export default CancelOrder;
