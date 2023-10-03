import React, { useCallback, useEffect, useState } from "react";
import { Tooltip } from "@mui/material";
import FavoriteIcon from "@mui/icons-material/Favorite";
import Swal from "sweetalert2";
import { useNavigate } from "react-router-dom";
import {
  addToWishlist,
  removeFromWishlist,
} from "../../../../../core/Api/apiService";
import { fetchWishlist } from "../../../WishList/wishlistSlice";
import { useDispatch } from "react-redux";

const WishlistIcon = ({ id, favourite }) => {
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const [message, setMessage] = useState("");

  useEffect(() => {
    if (favourite) {
      setMessage("Remove from Wishlist");
    } else {
      setMessage("Add to Wishlist");
    }
  }, [favourite]);

  const handleClickFunction = useCallback(() => {
    // function for adding a product to favourite list
    const addFavourites = (id) => {
      addToWishlist(id)
        .then(() => {
          dispatch(fetchWishlist());
        })
        .catch((err) => {
          console.log(err);
        });
    };

    // function for removing a product from wishlist
    const deleteFavourites = (id) => {
      removeFromWishlist(id)
        .then(() => {
          dispatch(fetchWishlist());
        })
        .catch((err) => {
          console.log(err);
        });
    };

    if (localStorage.getItem("accessToken")) {
      if (favourite) {
        deleteFavourites(id);
      } else {
        addFavourites(id);
      }
    } else {
      Swal.fire({
        title: "Oops...",
        text: "You haven't logged in yet!",
        icon: "error",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Login Now",
      }).then((result) => {
        if (result.isConfirmed) {
          navigate("/login");
        }
      });
    }
  }, [dispatch, favourite, id, navigate]);

  return (
    <>
      <Tooltip
        title={message}
        placement="top"
        data-testid={`wishlisticon${id}`}
      >
        <FavoriteIcon
          onClick={handleClickFunction}
          style={{ color: `${favourite ? "red" : "white"}` }}
          stroke={`${favourite ? "red" : "black"}`}
          strokeWidth={1}
        />
      </Tooltip>
    </>
  );
};

export default WishlistIcon;
