import { render, screen } from "@testing-library/react";
import React from "react";
import GoogleButton from "../GoogleButton";
import { BrowserRouter as Router } from "react-router-dom";
import { GoogleOAuthProvider } from "@react-oauth/google";
import { Provider } from "react-redux";
import { store } from "../../../../../redux/store";

describe("google button", () => {
  test("render google button", () => {
    render(
      <Provider store={store}>
        <Router>
          <GoogleOAuthProvider clientId={"CLIENT_ID"}>
            <GoogleButton />
          </GoogleOAuthProvider>
        </Router>
      </Provider>
    );
  });

//   test("render google button and click button", async () => {
//     render(
//       <Provider store={store}>
//         <Router>
//           <GoogleOAuthProvider clientId={"CLIENT_ID"}>
//             <GoogleButton />
//           </GoogleOAuthProvider>
//         </Router>
//       </Provider>
//     );

//     const button = await screen.findByTestId("google-button");
//     expect(button).toBeInTheDocument();
//   });
});
