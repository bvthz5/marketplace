import { configureStore } from '@reduxjs/toolkit';
import agentDetailReducer from '../agentpages/MyProfile/MyProfileSlice';

export const store = configureStore({
  reducer: {
    agentDetails: agentDetailReducer,
  },
});
