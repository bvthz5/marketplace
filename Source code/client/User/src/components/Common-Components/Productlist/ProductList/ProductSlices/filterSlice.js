import { createSlice } from "@reduxjs/toolkit";
import { productlistParams } from "../../../../Utils/Data/Data";

const initialState = productlistParams;

const filterSlice = createSlice({
  name: "filters",
  initialState,
  reducers: {
    setFilters(state, action) {
      return { ...state, ...action.payload };
    },
    clearAllFilters() {
      return initialState;
    },
  },
});

export const selectAllFilters = (state) => state.filters;

export const { setFilters, clearAllFilters } = filterSlice.actions;

export default filterSlice.reducer;
