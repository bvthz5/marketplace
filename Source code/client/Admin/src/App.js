import React from 'react';
// routes
import Router from './Router';
import './App.css';
// theme
import ThemeProvider from './theme';
// components
import ScrollToTop from './components/scroll-to-top';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { Provider } from 'react-redux';
import { store } from './redux/store';
// ----------------------------------------------------------------------

export default function App() {
  return (
    <>
    <div data-testid="app">
    <Provider store={store}>
      <ThemeProvider>
        <ScrollToTop/>
        <Router/>
      </ThemeProvider>
      </Provider>
      <ToastContainer
      
        position="top-center"
        autoClose={4000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
        theme="light"
      />
   
      </div>
    </>
  );
}
