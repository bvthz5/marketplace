import axious from './axious';
//login//
export const login = (data) => {
  return axious.post('/api/login', JSON.stringify(data));
};
export const googleLogin = (userObject) => {
  return axious.post('/api/google-login/admin', JSON.stringify(userObject));
};

//Forgot password//
export const resetPassword = (token, password) => {
  return axious.put('/api/Admin/reset-password', JSON.stringify({ token, password }));
};
export const forgotPassword = (email) => {
  return axious.put('/api/Admin/forgot-password', JSON.stringify(email));
};


///login agent///
export const loginAgent = (data) => {
  return axious.post('/api/login/agent', JSON.stringify(data));
};
export const googleLoginAgent = (userObject) => {
  return axious.post('/api/google-login/agent', JSON.stringify(userObject));
};
//change password//
export const changePasswordAgent = (userObject) => {
  return axious.put('/api/agent/change-password', JSON.stringify(userObject));
};
//forgot password//
export const forgotPasswordAgent = (email) => {
  return axious.put('/api/agent/forgot-password', JSON.stringify(email));
};
//verify token//
export const resetPasswordAgent = (token, password) => {
  return axious.put('/api/agent/reset-password', JSON.stringify({ token, password }));
};