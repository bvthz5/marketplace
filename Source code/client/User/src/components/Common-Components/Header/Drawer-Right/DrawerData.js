import React, { useCallback, useContext, useEffect, useState } from "react";
import { Box, Divider, List } from "@mui/material";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import { cartCounts } from "../../../../App";
import Lists from "./Lists";
import { clearReduxStore } from "../../../Utils/Utils";

const DrawerData = ({ toggleDrawer, setLoggedIn }) => {
  const navigate = useNavigate();
  const [, setCount] = useContext(cartCounts);

  const firstSection = [
    { id: 1, name: "Home" },
    { id: 2, name: "Cart" },
  ];
  const [secondSection, setSecondSection] = useState([
    { id: 3, name: "My Profile" },
    { id: 5, name: "Wishlist" },
    { id: 6, name: "My Orders" },
    { id: 7, name: "Logout" },
  ]);

  useEffect(() => {
    if (
      localStorage.getItem("role") === "2" &&
      !secondSection.some((a) => a.id === 4)
    ) {
      const updatedSecondSection = [
        ...secondSection.slice(0, 2),
        { id: 4, name: "My Ads" },
        ...secondSection.slice(2),
      ];
      setSecondSection(updatedSecondSection);
    }
  }, [secondSection]);

  const logout = useCallback(() => {
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
        localStorage.clear();
        clearReduxStore();
        setLoggedIn(false);
        console.log("logout");
        navigate("/");
      }
    });
  }, [navigate, setCount, setLoggedIn]);

  const handleNavigation = useCallback(
    (id) => {
      switch (id) {
        case 1:
          navigate("/home");
          break;
        case 2:
          navigate("/cart");
          break;
        case 3:
          navigate("/profile");
          break;
        case 4:
          navigate(`/myproducts/?id=0`);
          break;
        case 5:
          navigate("/wishlist");
          break;
        case 6:
          navigate("/orders");
          break;
        case 7:
          logout();
          break;
        default:
      }
    },
    [logout, navigate]
  );

  return (
    <Box
      sx={{ width: 250 }}
      role="presentation"
      onClick={toggleDrawer(false)}
      onKeyDown={toggleDrawer(false)}
      data-testid="drawercomponent"
    >
      <List>
        {firstSection.map((data) => (
          <Lists
            key={data.id}
            id={data.id}
            data={data.name}
            handleNavigation={handleNavigation}
          />
        ))}
      </List>
      <Divider />
      <List>
        {secondSection.map((data) => (
          <Lists
            key={data.id}
            id={data.id}
            data={data.name}
            handleNavigation={handleNavigation}
          />
        ))}
      </List>
    </Box>
  );
};

export default DrawerData;
