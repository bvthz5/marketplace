import { configureStore } from "@reduxjs/toolkit";
import productsReducer from "../components/Common-Components/Productlist/ProductList/ProductSlices/productSlice";
import filtersReducer from "../components/Common-Components/Productlist/ProductList/ProductSlices/filterSlice";
import userDetailReducer from "../components/Common-Components/User-Profile/userDetailSlice";
import wishlistReducer from "../components/Common-Components/WishList/wishlistSlice";

export const store = configureStore({
  reducer: {
    products: productsReducer,
    filters: filtersReducer,
    userDetails:userDetailReducer,
    wishlist:wishlistReducer,
  },
});
