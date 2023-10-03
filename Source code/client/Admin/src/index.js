import ReactDOM from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import { HelmetProvider } from 'react-helmet-async';
import App from './App';
import reportWebVitals from './reportWebVitals';
import axious from './core/api/axious';
import { toast } from 'react-toastify';
import { GoogleOAuthProvider } from '@react-oauth/google';

const CLIENT_ID = process.env.REACT_APP_GOOGLE_CLIENT_ID;

axious.interceptors.request.use((request) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    request.headers.Authorization = `Bearer ${token}`;
  }
  return request;
});

axious.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    console.log(error);
    const originalRequest = error.config;
    if (
      error.response.status === 401 &&
      !originalRequest._retry &&
      error.config.url !== '/api/login' &&
      error.config.url !== '/api/login/agent'
    ) {
      originalRequest._retry = true;
      const accessToken = await refreshAccessToken();
      axious.defaults.headers.common.Authorization = `Bearer ${accessToken}`;
      return axious(originalRequest);
    }
    return Promise.reject(error);
  }
);

const refreshAccessToken = async () => {
  const refreshToken = localStorage.getItem('refreshToken');
  const role = localStorage.getItem('role');

  if (role === '0') {
    try {
      const response = await axious.put('/api/login', JSON.stringify(refreshToken));
      const accessToken = response?.data?.data.accessToken?.value;
      const newRefreshToken = response?.data?.data.refreshToken?.value;
      localStorage.setItem('accessToken', accessToken);
      localStorage.setItem('refreshToken', newRefreshToken);
      return accessToken;
    } catch (err) {
      localStorage.clear();
      console.log(err.response);
      window.location.replace('/login');
      toast.error('Session Expired! Login Again', { toastId: 2 });
      return null;
    }
  } else if (role === '1') {
    try {
      const response = await axious.put('/api/login/agent', JSON.stringify(refreshToken));
      const accessToken = response?.data?.data.accessToken?.value;
      const newRefreshToken = response?.data?.data.refreshToken?.value;
      localStorage.setItem('accessToken', accessToken);
      localStorage.setItem('refreshToken', newRefreshToken);
      return accessToken;
    } catch (err) {
      localStorage.clear();
      console.log(err.response);
      window.location.replace('/agent/login');
      toast.error('Session Expired! Login Again', { toastId: 2 });
      return null;
    }
  }
};

const root = ReactDOM.createRoot(document.getElementById('root'));

root.render(
  <GoogleOAuthProvider clientId={CLIENT_ID}>
    <HelmetProvider>
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </HelmetProvider>
  </GoogleOAuthProvider>
);

reportWebVitals();
