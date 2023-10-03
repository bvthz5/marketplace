import "./App.css";
import React, { createContext, useMemo, useState } from "react";
import { ToastContainer } from "react-toastify";
import RouterFile from "./core/Routes/RouterFile";
import "react-toastify/dist/ReactToastify.css";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import { store } from "./redux/store";
import { Provider } from "react-redux";

export const cartCounts = createContext();


const initialCartCount = 0;

function App() {
  console.log(process.env.REACT_APP_API_PATH);
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);


  return (
    <>
      <div data-testid="app">
        <Provider store={store}>
          <cartCounts.Provider value={cartCountValue}>
            <RouterFile />
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
          </cartCounts.Provider>
        </Provider>
      </div>
    </>
  );
}

export default App;
