import React from "react";
import { toast } from "react-toastify";
import { store } from "../../redux/store";
import { clearUserData } from "../Common-Components/User-Profile/userDetailSlice";
import { clearAllFilters } from "../Common-Components/Productlist/ProductList/ProductSlices/filterSlice";
import { clearAllProducts } from "../Common-Components/Productlist/ProductList/ProductSlices/productSlice";
import { CardHeader, Divider, Skeleton } from "@mui/material";
import { clearWishlist } from "../Common-Components/WishList/wishlistSlice";



// This function converts a date into  Today /Yesterday/2 day ago to 7 days ago after that 12 Feb format
export function getRelativeDate(date) {
  const today = new Date();
  const inputDate = new Date(date);

  const oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
  const diffDays = Math.round(Math.abs((today - inputDate) / oneDay));

  if (diffDays === 0) {
    return "TODAY";
  } else if (diffDays === 1) {
    return "YESTERDAY";
  } else if (diffDays < 7) {
    return `${diffDays} DAYS AGO`;
  } else {
    const monthNames = [
      "Jan",
      "Feb",
      "Mar",
      "Apr",
      "May",
      "Jun",
      "Jul",
      "Aug",
      "Sep",
      "Oct",
      "Nov",
      "Dec",
    ];
    const month = monthNames[inputDate.getMonth()];
    const day = inputDate.getDate();
    return `${month.toUpperCase()} ${day}`;
  }
}

export function getRelativeDateTime(date) {
  const today = new Date();
  const inputDate = new Date(date);
  const diffTime = Math.abs(today - inputDate);
  const diffMinutes = Math.round(diffTime / (1000 * 60));
  const diffHours = Math.round(diffTime / (1000 * 60 * 60));
  const diffDays = Math.round(diffTime / (1000 * 60 * 60 * 24));

  if (diffMinutes < 1) {
    return "now";
  } else if (diffMinutes < 60) {
    return `${diffMinutes}m ago`;
  } else if (diffHours === 1) {
    return "1hr ago";
  } else if (diffHours < 24) {
    return `${diffHours}hr ago`;
  } else if (diffDays === 1) {
    return "yesterday";
  } else {
    const monthNames = [
      "Jan",
      "Feb",
      "Mar",
      "Apr",
      "May",
      "Jun",
      "Jul",
      "Aug",
      "Sep",
      "Oct",
      "Nov",
      "Dec",
    ];
    const month = monthNames[inputDate.getMonth()];
    const day = inputDate.getDate();
    return `${month.toUpperCase()} ${day}`;
  }
}

//   converts dates into 12 FEB 2023 format
export function convertDate(dates) {
  let date = new Date(dates);
  if (isNaN(date.getTime())) {
    return "";
  }
  let month = [
    "JAN",
    "FEB",
    "MAR",
    "APR",
    "MAY",
    "JUN",
    "JUL",
    "AUG",
    "SEP",
    "OCT",
    "NOV",
    "DEC",
  ];
  return (
    date.getDate() +
    " " +
    month[date.getMonth()] +
    ", " +
    date.getFullYear().toString()
  );
}

// function for image extension validation of profile picture
export const validateProfilePicture = (profilePic) => {
  let allowedExtension = ["jpeg", "jpg", "png", "webp"];
  let fileExtension = profilePic[0].name.split(".").pop().toLowerCase();
  let isValidFile = false;

  for (let index in allowedExtension) {
    if (fileExtension === allowedExtension[index]) {
      isValidFile = true;
      break;
    }
  }
  if (!isValidFile) {
    toast.error("Allowed Extensions are : *." + allowedExtension.join(", *."));
  }
  return isValidFile;
};

// function for image extension validation of product images
export const validateProductImages = (productImage) => {
  let allowedExtension = ["jpeg", "jpg", "png", "webp"];
  let fileExtension = productImage.name.split(".").pop().toLowerCase();
  let isValidFile = false;

  for (let index in allowedExtension) {
    if (fileExtension === allowedExtension[index]) {
      isValidFile = true;
      break;
    }
  }

  if (!isValidFile) {
    toast.error("Allowed Extensions are : *." + allowedExtension.join(", *."));
  }

  return isValidFile;
};

export const hasDataChanged = (userData1, userData2) => {
  for (let key in userData1) {
    if (userData1[key] !== userData2[key]) {
      return true;
    }
  }
  return false;
};

export const notificationLoader = Array.from({ length: 2 }, (_, index) => (
  <>
    <CardHeader
      key={index}
      avatar={
        <Skeleton animation="wave" variant="circular" width={40} height={40} />
      }
      title={
        <Skeleton
          animation="wave"
          height={10}
          width="80%"
          style={{ marginBottom: 6 }}
        />
      }
      subheader={<Skeleton animation="wave" height={10} width="40%" />}
    />
    <Divider />
  </>
));

export const clearReduxStore = () => {
  store.dispatch(clearUserData());
  store.dispatch(clearAllFilters());
  store.dispatch(clearAllProducts());
  store.dispatch(clearWishlist());
};


export function convertTime(time) {
  const date = new Date(time);
  const hours = date.getHours();
  const minutes = date.getMinutes();
  let period = "AM";

  let hour = hours;
  if (hour >= 12) {
    period = "PM";
    hour = hour === 12 ? 12 : hour - 12;
  } else if (hour === 0) {
    hour = 12;
  }

  return `${hour}:${formatTwoDigits(minutes)} ${period}`;
}

function formatTwoDigits(value) {
  return value.toString().padStart(2, "0");
}



