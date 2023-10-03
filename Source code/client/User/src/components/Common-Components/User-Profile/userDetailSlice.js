import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getCurrentUser } from "../../../core/Api/apiService";

const initialState = {};

export const fetchUserDetails = createAsyncThunk("user details", async () => {
  try {
    const res = (await getCurrentUser()).data.data;
    return res;
  } catch (error) {
    console.log(error);
  }
});

const userDetailSlice = createSlice({
  name: "userDetails",
  initialState,
  reducers: {
    clearUserData() {
      return {};
    },
  },
  extraReducers(builder) {
    builder.addCase(fetchUserDetails.fulfilled, (state, action) => {
      return action.payload;
    });
  },
});

export const userDetails = (state) => state.userDetails;

export const { clearUserData } = userDetailSlice.actions;

export default userDetailSlice.reducer;
