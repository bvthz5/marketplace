import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { getAgentProfile } from '../../core/api/apiService';

const initialState = {};

export const fetchAgentDetails = createAsyncThunk('agent details', async () => {
  try {
    const res = (await getAgentProfile()).data.data;
    return res;
  } catch (error) {
    console.log(error);
  }
});

const agentDetailSlice = createSlice({
  name: 'agentDetails',
  initialState,
  reducers: {
    clearUserData() {
      return {};
    },
  },
  extraReducers(builder) {
    builder.addCase(fetchAgentDetails.fulfilled, (state, action) => {
      return action.payload;
    });
  },
});

export const agentDetails = (state) => state.agentDetails;

export const { clearUserData } = agentDetailSlice.actions;

export default agentDetailSlice.reducer;
