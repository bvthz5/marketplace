import React, {
  useCallback,
  useContext,
  useEffect,
  useRef,
  useState,
} from "react";
import Button from "@mui/material/Button";
import ClickAwayListener from "@mui/material/ClickAwayListener";
import Grow from "@mui/material/Grow";
import Paper from "@mui/material/Paper";
import Popper from "@mui/material/Popper";
import MenuItem from "@mui/material/MenuItem";
import MenuList from "@mui/material/MenuList";
import Stack from "@mui/material/Stack";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import { cartCounts } from "../../../../App";
import { useSelector } from "react-redux";
import { Avatar } from "@mui/material";
import { userDetails } from "../../User-Profile/userDetailSlice";
import { clearReduxStore } from "../../../Utils/Utils";

const baseImageUrl = process.env.REACT_APP_PROFILEIMAGE_PATH;

export default function UserDropdown({ setLoggedIn }) {
  let navigate = useNavigate();

  const userData = useSelector(userDetails);
  const [open, setOpen] = useState(false);
  const anchorRef = useRef(null);
  const [, setCount] = useContext(cartCounts);
  const [seller, setSeller] = useState(false);

  useEffect(() => {
    if (localStorage.getItem("role") === "2") {
      setSeller(true);
    }
  }, []);

  const handleClick = useCallback(() => {
    navigate("/profile");
  }, [navigate]);
  const myadds = useCallback(() => {
    navigate("/myproducts/?id=0");
  }, [navigate]);
  const wishlist = useCallback(() => {
    navigate("/wishlist");
  }, [navigate]);

  const orders = useCallback(() => {
    navigate("/orders");
  }, [navigate]);

  const handleToggle = useCallback(() => {
    setOpen((prevOpen) => !prevOpen);
  }, []);

  const handleClose = useCallback((event) => {
    if (anchorRef.current && anchorRef.current.contains(event.target)) {
      return;
    }
    setOpen(false);
  }, []);

  const handleListKeyDown = useCallback((event) => {
    if (event.key === "Tab") {
      event.preventDefault();
      setOpen(false);
    } else if (event.key === "Escape") {
      setOpen(false);
    }
  }, []);

  // return focus to the button when we transitioned from !open -> open
  const prevOpen = useRef(open);
  useEffect(() => {
    if (prevOpen.current === true && open === false) {
      anchorRef.current.focus();
    }
    prevOpen.current = open;
  }, [open]);

  const logOut = useCallback(() => {
    Swal.fire({
      title: "Logout?",
      text: "You will be logged out!",
      icon: "info",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, Logout!",
    }).then((result) => {
      if (result.isConfirmed) {
        setCount(0);
        clearReduxStore();
        localStorage.clear();
        setLoggedIn(false);
        navigate("/");
      }
    });
  }, [navigate, setCount, setLoggedIn]);

  return (
    <Stack data-testid="dropdowncomponent" direction="row" spacing={2}>
      <div>
        <Button
          style={{
            color: "#002f34",
          }}
          ref={anchorRef}
          data-testid="dropdown-button"
          id="composition-button"
          aria-controls={open ? "composition-menu" : undefined}
          aria-expanded={open ? "true" : undefined}
          aria-haspopup="true"
          onClick={handleToggle}
        >
          <Avatar
            alt={userData?.firstName}
            src={
              userData?.profilePic
                ? `${baseImageUrl}User/profile/${userData.profilePic}`
                : `${userData?.firstName}`
            }
          />
          <ExpandMoreIcon />
        </Button>
        <Popper
          open={open}
          anchorEl={anchorRef.current}
          role={undefined}
          placement="bottom-start"
          transition
          disablePortal
        >
          {({ TransitionProps, placement }) => (
            <Grow
              {...TransitionProps}
              style={{
                transformOrigin:
                  placement === "bottom-start" ? "left top" : "left bottom",
              }}
            >
              <Paper>
                <ClickAwayListener onClickAway={handleClose}>
                  <MenuList
                    autoFocusItem={open}
                    id="composition-menu"
                    aria-labelledby="composition-button"
                    data-testid="dropdown-menu"
                    onKeyDown={handleListKeyDown}
                  >
                    <MenuItem data-testid="myprofile" onClick={handleClick}>
                      My Profile
                    </MenuItem>
                    {seller && (
                      <MenuItem data-testid="myads" onClick={myadds}>
                        My ads
                      </MenuItem>
                    )}
                    <MenuItem data-testid="wishlist" onClick={wishlist}>
                      Wishlist
                    </MenuItem>
                    <MenuItem data-testid="myorders" onClick={orders}>
                      My Orders
                    </MenuItem>

                    <MenuItem data-testid="logout" onClick={logOut}>
                      Logout
                    </MenuItem>
                  </MenuList>
                </ClickAwayListener>
              </Paper>
            </Grow>
          )}
        </Popper>
      </div>
    </Stack>
  );
}
