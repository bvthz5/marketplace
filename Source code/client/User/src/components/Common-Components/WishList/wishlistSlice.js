import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getAllWishlist } from "../../../core/Api/apiService";

const initialState = [];

export const fetchWishlist = createAsyncThunk("fetch wishlist", async () => {
  try {
    const res = (await getAllWishlist()).data.data;
    return res;
  } catch (err) {
    console.log(err);
  }
});

const wishlistSlice = createSlice({
  name: "wishlist",
  initialState,
  reducers: {
    clearWishlist() {
      return initialState;
    },
  },
  extraReducers(builder) {
    builder.addCase(fetchWishlist.fulfilled, (state, action) => {
      console.log(action.payload);
      return action.payload;
    });
  },
});

export const selectAllWishlist = (state) => state.wishlist;

export const { clearWishlist } = wishlistSlice.actions;

export default wishlistSlice.reducer;
