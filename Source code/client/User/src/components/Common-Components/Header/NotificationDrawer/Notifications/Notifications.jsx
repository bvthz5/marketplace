import React, { useEffect, useState } from "react";
import notifystyle from "./Notifications.module.css";
import profileicon from "../../../../../Assets/images/orange-notification-bell-icon-png-11638985058ine0rbglzz.png";
import nonotify from "../../../../../Assets/images/icon-notification.png";
import {
  getRelativeDateTime,
  notificationLoader,
} from "../../../../Utils/Utils";
import { Divider, Tooltip } from "@mui/material";
import {
  clearAllNotifications,
  clearNotificationsById,
  getAllNotifications,
  markAllAsRead,
} from "../../../../../core/Api/apiService";
import CloseIcon from "@mui/icons-material/Close";

const Notifications = ({ callNewNotifications, handleCount }) => {
  const [notifications, setNotifications] = useState([]);
  const [apiCall, setApiCall] = useState(true);

  useEffect(() => {
    getNotifications();
  }, [callNewNotifications]);

  useEffect(() => {
    if (notifications.some((notification) => notification.status === 0)) {
      markAllAsRead()
        .then(() => handleCount())
        .catch((err) => {
          console.log(err);
        });
    }
  }, [handleCount, notifications]);

  const getNotifications = () => {
    if (!localStorage.getItem("accessToken")) return;
    setApiCall(true);
    getAllNotifications()
      .then((response) => {
        setNotifications(response.data.data);
        setApiCall(false);
      })
      .catch((err) => {
        console.log(err);
      });
  };

  const deleteNotification = (notificationId) => {
    clearNotificationsById(notificationId)
      .then(() => {
        getNotifications();
      })
      .catch((err) => console.log(err));
  };

  const deleteAllNotification = () => {
    clearAllNotifications()
      .then(() => {
        getNotifications();
      })
      .catch((err) => console.log(err));
  };

  return (
    <div className={notifystyle["container"]} data-testid="notificationlist">
      {!apiCall && (
        <>
          {notifications.length > 0 && (
            <>
              <div className={notifystyle["heading"]}>
                <button
                  onClick={deleteAllNotification}
                  className={notifystyle["clearbtn"]}
                  data-testid="clearallbtn"
                >
                  Clear All
                </button>
              </div>
              <Divider />
            </>
          )}

          {notifications.length > 0 ? (
            notifications.map((notification) => {
              return (
                <div key={notification.notificationId}>
                  <div className={notifystyle["notifybox"]}>
                    <div className={notifystyle["imagediv"]}>
                      <img
                        className={notifystyle["image"]}
                        src={profileicon}
                        alt=""
                      />
                    </div>
                    <div className={notifystyle["notificationbox"]}>
                      <Tooltip title={notification.data}>
                        <p className={notifystyle["productname"]}>
                          {notification.data}
                        </p>
                      </Tooltip>
                      {notification.type === 2 && "in your wishlist"} was marked
                      as sold.
                      <p className={notifystyle["dateshow"]}>
                        {getRelativeDateTime(notification.createdDate)}
                      </p>
                    </div>
                    <div className={notifystyle["deletebtndiv"]}>
                      <div
                        className={notifystyle["deletebtn"]}
                        data-testid="deletebutton"
                        onClick={() => {
                          deleteNotification(notification.notificationId);
                        }}
                        style={{ cursor: "pointer" }}
                      >
                        <CloseIcon />
                      </div>
                    </div>
                  </div>
                  <Divider />
                </div>
              );
            })
          ) : (
            <div className={notifystyle["nonotificationbox"]}>
              <div className={notifystyle["nonotificationhead"]}>
                <span className={notifystyle["nonotificationboxspan"]}>
                  No notifications
                </span>
                <p className={notifystyle["ptagmsg"]}>
                  Check back here for updates!
                </p>
                <img
                  className={notifystyle["nonotifyimg"]}
                  src={nonotify}
                  alt=""
                />
              </div>
            </div>
          )}
        </>
      )}
      {apiCall && notificationLoader}
    </div>
  );
};

export default Notifications;
