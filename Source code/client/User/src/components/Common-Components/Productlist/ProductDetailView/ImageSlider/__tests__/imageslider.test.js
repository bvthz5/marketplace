import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import ImageSlider from "../ImageSlider";

describe("ImageSlider", () => {
  const images = [
    { photo: "image1.jpg", photosId: 1 },
    { photo: "image2.jpg", photosId: 2 },
    { photo: "image3.jpg", photosId: 3 },
  ];

  it("should render slider with images", () => {
    render(<ImageSlider images={images} />);
    const slider = screen.getByRole("list");
    expect(slider).toBeInTheDocument();
    const sliderImages = screen.getAllByRole("img");
    expect(sliderImages.length).toBe(4);
  });

  it("should open fullscreen image on click", () => {
    render(<ImageSlider images={images} />);
    const firstImage = screen.getAllByRole("img")[0];
    fireEvent.click(firstImage);
    const fullscreenImage = screen.getByTestId("fullscreenimage");
    expect(fullscreenImage).toBeInTheDocument();
    fireEvent.keyDown(fullscreenImage, { keyCode: 27 });
    expect(fullscreenImage).not.toBeInTheDocument();
  });

  it("should open fullscreen image on click and close by clicking the image", async () => {
    render(<ImageSlider images={images} />);
    const firstImage = screen.getAllByRole("img")[0];
    fireEvent.click(firstImage);
    const fullscreenImage = screen.getByTestId("fullscreenimage");
    expect(fullscreenImage).toBeInTheDocument();

    const modalimage = await screen.findAllByTestId("1");
    expect(modalimage[0]).toBeInTheDocument();
    fireEvent.click(modalimage[0]);

    expect(fullscreenImage).not.toBeInTheDocument();
  });

  it("should close fullscreen image on close button click", () => {
    render(<ImageSlider images={images} />);
    const firstImage = screen.getAllByRole("img")[0];
    fireEvent.click(firstImage);
    const fullscreenImage = screen.queryByTestId("fullscreenimage");
    const closeButton = screen.getByTestId("modal-close-btn");
    fireEvent.keyDown(fullscreenImage, { keyCode: 35 });
    fireEvent.click(closeButton);
    expect(fullscreenImage).not.toBeInTheDocument();
  });
});
