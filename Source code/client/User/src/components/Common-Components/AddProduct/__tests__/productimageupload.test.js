import React, { useMemo, useState } from "react";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import ProductImgUpload from "../ProductImgUpload";
import { cartCounts } from "../../../../App";
import { rest, server } from "../../../../testServer";
import { store } from "../../../../redux/store";
import { Provider } from "react-redux";

window.scrollTo = jest.fn();
global.URL.createObjectURL = jest.fn();

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useSearchParams: () => [new URLSearchParams({ id: "28" })],
}));

const initialCartCount = 0;
const ProductImgUploadWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <ProductImgUpload />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

test("should render productimgupload component", () => {
  render(<ProductImgUploadWrapper />);
  const productImgUploadElement = screen.getByTestId("productimguploadpage");
  expect(productImgUploadElement).toBeDefined();
});

test("add images", async () => {
  server.use(
    rest.post("https://localhost:8080/api/Photos/28", (req, res, ctx) => {
      return res(ctx.status(200));
    })
  );
  localStorage.setItem("addImage", true);

  render(<ProductImgUploadWrapper />);
  const productImgUploadElement = screen.getByTestId("productimguploadpage");
  expect(productImgUploadElement).toBeDefined();

  const input = screen.getByTestId("image-uploader");
  fireEvent.change(input, {
    target: {
      files: [new File(["(⌐□_□)"], "chucknorris.png", { type: "image/png" })],
    },
  });
  await waitFor(() => {
    expect(input.files.length).toBe(1);
  });

  const deleteButton = await screen.findByTestId("image-delete-button");
  expect(deleteButton).toBeInTheDocument();
  fireEvent.click(deleteButton);

  fireEvent.change(input, {
    target: {
      files: [new File(["(⌐□_□)"], "apple.png", { type: "image/png" })],
    },
  });
  await waitFor(() => {
    expect(input.files.length).toBe(1);
  });

  const uploadButton = await screen.findByTestId("upload-button");
  expect(uploadButton).toBeInTheDocument();
  fireEvent.click(uploadButton);
});

test("add 13 images at a time", async () => {
  server.use(
    rest.post("https://localhost:8080/api/Photos/28", (req, res, ctx) => {
      return res(ctx.status(200));
    })
  );
  localStorage.setItem("addImage", true);
  render(<ProductImgUploadWrapper />);
  const productImgUploadElement = screen.getByTestId("productimguploadpage");
  expect(productImgUploadElement).toBeDefined();

  const input = screen.getByTestId("image-uploader");
  fireEvent.change(input, {
    target: {
      files: [
        new File(["(⌐□_□)"], "chucknorris.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "apple.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "mi.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "redmi.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "realme.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "oppo.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "oneplus.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "samsung.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "apple.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "lenovo.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "vivo.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "nokia.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "chucknorris.png", { type: "image/png" }),
      ],
    },
  });
  await waitFor(() => {
    expect(input.files.length).toBe(13);
  });

  const uploadButton = await screen.findByTestId("upload-button");
  expect(uploadButton).toBeInTheDocument();
  fireEvent.click(uploadButton);
});

test("add 10 images at a time and add 3 more images and submit to get error response", async () => {
  server.use(
    rest.post("https://localhost:8080/api/Photos/28", (req, res, ctx) => {
      return res(ctx.status(200));
    })
  );

  localStorage.setItem("addImage", true);
  render(<ProductImgUploadWrapper />);
  const productImgUploadElement = screen.getByTestId("productimguploadpage");
  expect(productImgUploadElement).toBeDefined();

  const input = screen.getByTestId("image-uploader");

  fireEvent.change(input, {
    target: {
      files: [
        new File(["(⌐□_□)"], "chucknorris.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "apple.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "mi.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "redmi.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "realme.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "oppo.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "oneplus.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "samsung.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "apple.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "lenovo.png", { type: "image/png" }),
      ],
    },
  });
  await waitFor(() => {
    expect(input.files.length).toBe(10);
  });

  fireEvent.change(input, {
    target: {
      files: [
        new File(["(⌐□_□)"], "vivo.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "nokia.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "chucknorris.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "samsung.png", { type: "image/png" }),
      ],
    },
  });
  await waitFor(() => {
    expect(input.files.length).toBe(4);
  });

  const uploadButton = await screen.findByTestId("upload-button");
  expect(uploadButton).toBeInTheDocument();
  fireEvent.click(uploadButton);
});

test("add 3 images and sumbit and get error response", async () => {
  server.use(
    rest.post("https://localhost:8080/api/Photos/28", (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
  localStorage.setItem("addImage", true);
  render(<ProductImgUploadWrapper />);
  const productImgUploadElement = screen.getByTestId("productimguploadpage");
  expect(productImgUploadElement).toBeDefined();

  const input = screen.getByTestId("image-uploader");

  // add 1 image with size greater than 2 mb
  // add 1 image with a unsupported file format
  fireEvent.change(input, {
    target: {
      files: [
        new File(["(⌐□_□)"], "chucknorris.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "apple.png", { type: "image/png" }),
        new File(["(⌐□_□)"], "mi.png", { type: "image/png" }),
        new File(
          [new Blob(["a".repeat(2097153)], { type: "image/png" })],
          "mock.png",
          { type: "image/png" }
        ),
        new File(["(⌐□_□)"], "chucknorris.pdf", { type: "image/pdf" }),
      ],
    },
  });
  await waitFor(() => {
    expect(input.files.length).toBe(5);
  });

  const uploadButton = await screen.findByTestId("upload-button");
  expect(uploadButton).toBeInTheDocument();
  fireEvent.click(uploadButton);
});
