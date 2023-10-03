import { createSlice } from "@reduxjs/toolkit";

const initialState = [];

const productSlice = createSlice({
  name: "products",
  initialState,
  reducers: {
    getProducts(state, action) {
      return [...action.payload];
    },
    nextPage(state, action) {
      return [...state,...action.payload];
    },
    clearAllProducts() {
      return [];
    },
  },
});

export const selectAllProducts = (state) => state.products;

export const { getProducts,nextPage ,slice,clearAllProducts} = productSlice.actions;

export default productSlice.reducer;
