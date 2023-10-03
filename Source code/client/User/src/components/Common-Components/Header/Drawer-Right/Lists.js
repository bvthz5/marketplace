import React, { useCallback } from "react";
import HomeIcon from "@mui/icons-material/Home";
import SellIcon from "@mui/icons-material/Sell";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import LogoutIcon from "@mui/icons-material/Logout";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";
import FavoriteIcon from "@mui/icons-material/Favorite";
import AssignmentTurnedInIcon from "@mui/icons-material/AssignmentTurnedIn";
import {
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from "@mui/material";

const Lists = ({ id, data, handleNavigation }) => {
  const onClick = useCallback(() => handleNavigation(id), [handleNavigation, id]);

  const getIcon = (id) => {
    switch (id) {
      case 1:
        return <HomeIcon />;
      case 2:
        return <ShoppingCartIcon />;
      case 3:
        return <AccountCircleIcon />;
      case 4:
        return <SellIcon />;
      case 5:
        return <FavoriteIcon />;
      case 6:
        return <AssignmentTurnedInIcon />;
      case 7:
        return <LogoutIcon />;
      default:
    }
  };

  return (
    <ListItem key={id} data-testid="listcomponent" disablePadding>
      <ListItemButton onClick={onClick}>
        <ListItemIcon>{getIcon(id)}</ListItemIcon>
        <ListItemText primary={data} />
      </ListItemButton>
    </ListItem>
  );
};

export default Lists;
