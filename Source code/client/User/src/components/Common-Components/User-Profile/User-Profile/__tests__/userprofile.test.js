import React, { useMemo, useState } from "react";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import UserProfile from "../userProfile";
import { cartCounts } from "../../../../../App";
import { rest, server } from "../../../../../testServer";
import { userProfileData, userProfileDataWithImage } from "../testData/data";
import { Provider } from "react-redux";
import { store } from "../../../../../redux/store";

jest.mock("../../../../Assets/images/profile.png");

window.scrollTo = jest.fn();

server.use(
  rest.get("https://localhost:8080/api/User", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(userProfileData));
  })
);

window.scrollTo = jest.fn();
const initialCartCount = 0;

const UserProfileWrapper = ({ dispatch }) => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <UserProfile dispatch={dispatch} />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

describe("userprofile", () => {
  test("should render userprofile component get success response and upload a new profile image and get success response", async () => {
    server.use(
      rest.put("https://localhost:8080/api/User/profile", (req, res, ctx) => {
        return res(ctx.status(200));
      })
    );

    const dispatch = jest.fn();

    render(<UserProfileWrapper dispatch={dispatch} />);
    const userProfileElement = screen.getByTestId("userprofilepage");
    expect(userProfileElement).toBeDefined();

    const email = await screen.findByText(/stejin.jacob@innovaturelabs.com/i);
    const memberSince = await screen.findByText(/Member Since/i);

    expect(email).toBeInTheDocument();
    expect(memberSince).toBeInTheDocument();

    const input = screen.getByTestId("image-uploader");
    fireEvent.change(input, {
      target: {
        files: [new File(["(⌐□_□)"], "chucknorris.png", { type: "image/png" })],
      },
    });
    await waitFor(() => {
      expect(input.files.length).toBe(1);
    });

    const changePasswordButton = await screen.findByTestId(
      "changepassword-button"
    );
    expect(changePasswordButton).toBeInTheDocument();
    fireEvent.click(changePasswordButton);
  });

  test("upload a new profile image with size >2 and get filesize crossed error message", async () => {
    server.use(
      rest.get("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(userProfileDataWithImage));
      })
    );
    server.use(
      rest.put("https://localhost:8080/api/User/profile", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );

    render(<UserProfileWrapper />);
    const userProfileElement = screen.getByTestId("userprofilepage");
    expect(userProfileElement).toBeDefined();

    const input = screen.getByTestId("image-uploader");
    fireEvent.change(input, {
      target: {
        files: [
          new File(
            [new Blob(["a".repeat(2097153)], { type: "image/png" })],
            "mock.png",
            { type: "image/png" }
          ),
        ],
      },
    });
    await waitFor(() => {
      expect(input.files.length).toBe(1);
    });
  });

  test("upload a new profile image with invalid file format and get error message", async () => {
    server.use(
      rest.get("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(userProfileData));
      })
    );
    server.use(
      rest.put("https://localhost:8080/api/User/profile", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );

    render(<UserProfileWrapper />);
    const userProfileElement = screen.getByTestId("userprofilepage");
    expect(userProfileElement).toBeDefined();

    const input = screen.getByTestId("image-uploader");
    fireEvent.change(input, {
      target: {
        files: [new File(["(⌐□_□)"], "chucknorris.pdf", { type: "image/pdf" })],
      },
    });
    await waitFor(() => {
      expect(input.files.length).toBe(1);
    });
  });

  test("should render userprofile component get success response and upload a new profile image and get error response", async () => {
    server.use(
      rest.get("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(userProfileData));
      })
    );
    server.use(
      rest.put("https://localhost:8080/api/User/profile", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );

    render(<UserProfileWrapper />);
    const userProfileElement = screen.getByTestId("userprofilepage");
    expect(userProfileElement).toBeDefined();

    const input = screen.getByTestId("image-uploader");
    fireEvent.change(input, {
      target: {
        files: [new File(["(⌐□_□)"], "chucknorris.png", { type: "image/png" })],
      },
    });
    await waitFor(() => {
      expect(input.files.length).toBe(1);
    });
  });

  test("should render userprofile component get error response", () => {
    server.use(
      rest.get("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );

    render(<UserProfileWrapper />);
    const userProfileElement = screen.getByTestId("userprofilepage");
    expect(userProfileElement).toBeDefined();
  });
});
