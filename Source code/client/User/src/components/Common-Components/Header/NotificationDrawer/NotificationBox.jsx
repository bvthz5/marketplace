import React, { useCallback, useEffect, useState } from "react";
import Box from "@mui/material/Box";
import Menu from "@mui/material/Menu";
import IconButton from "@mui/material/IconButton";
import Tooltip from "@mui/material/Tooltip";
import NotificationsIcon from "@mui/icons-material/Notifications";
import Notifications from "./Notifications/Notifications";
import { getNotificationCount } from "../../../../core/Api/apiService";
import { Badge } from "@mui/material";

export default function NotificationBox({
  callNotification,
  loggedIn,
  callNoLogin,
}) {
  const [callNewNotifications, setCallNewNotifications] = useState([]);
  const [count, setCount] = useState(0);

  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);

  useEffect(() => {
    getUnreadCount();
    if (callNotification) {
      setCallNewNotifications(true);
    } else {
      setCallNewNotifications(false);
    }
  }, [callNotification]);

  useEffect(() => {
    if (count > 0 && anchorEl) {
      setCallNewNotifications(true);
    }
  }, [anchorEl, count]);

  const handleClick = useCallback((event) => {
    if (loggedIn) {
      setAnchorEl(event.currentTarget);
    } else {
      callNoLogin();
    }
  },[callNoLogin, loggedIn]);
  
  const handleClose = useCallback(() => {
    setAnchorEl(null);
  }, []);

  const handleCount = useCallback(() => {
    setCount(0);
  }, []);

  const getUnreadCount = () => {
    if (!localStorage.getItem("accessToken")) return;
    getNotificationCount()
      .then((response) => {
        setCount(response.data.data);
      })
      .catch((err) => console.log(err));
  };

  return (
    <React.Fragment>
      <Box sx={{ display: "flex", alignItems: "center", textAlign: "center" }}>
        <Tooltip title="Notifications">
          <IconButton
            data-testid="notificationboxbtn"
            onClick={handleClick}
            size="small"
            sx={{ ml: 0 }}
            aria-controls={open ? "account-menu" : undefined}
            aria-haspopup="true"
            aria-expanded={open ? "true" : undefined}
          >
            <Badge
              badgeContent={count}
              sx={{
                "& .MuiBadge-badge": {
                  color: "white",
                  backgroundColor: "red",
                },
              }}
            >
              <NotificationsIcon
                style={{ fontSize: "28px", color: "#002f34" }}
              />
            </Badge>
          </IconButton>
        </Tooltip>
      </Box>
      <Menu
        data-testid="menuCloseBtn"
        anchorEl={anchorEl}
        id="account-menu"
        open={open}
        onClose={handleClose}
        PaperProps={{
          elevation: 0,
          sx: {
            overflow: "visible",
            filter: "drop-shadow(0px 2px 8px rgba(0,0,0,0.32))",
            mt: 1.5,
            "& .MuiAvatar-root": {
              width: 32,
              height: 32,
              ml: -0.5,
              mr: 1,
            },
            "&:before": {
              content: '""',
              display: "block",
              position: "absolute",
              top: 0,
              right: 14,
              width: 10,
              height: 10,
              bgcolor: "background.paper",
              transform: "translateY(-50%) rotate(45deg)",
              zIndex: 0,
            },
          },
        }}
        transformOrigin={{ horizontal: "right", vertical: "top" }}
        anchorOrigin={{ horizontal: "right", vertical: "bottom" }}
      >
        <Notifications
          callNewNotifications={callNewNotifications}
          handleCount={handleCount}
        />
      </Menu>
    </React.Fragment>
  );
}
