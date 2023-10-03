
import React from "react";
import { render, screen } from "@testing-library/react";
import MapImage from "../MapImage";

jest.mock("../../../../../../../node_modules/@react-google-maps/api", () => {
  return {
    useLoadScript: jest.fn().mockImplementation(() => {
      return {
        isLoaded: true,
        loadError: false,
      };
    }),
    GoogleMap: ({ children }) => <div>{children}</div>,
    Marker: () => <div data-testid="marker"></div>,
  };
});


describe("MapImage component", () => {
  test("renders the Google Map", async () => {
    const latitude = 37.7749;
    const longitude = -122.4194;

    render(<MapImage latitude={latitude} longitude={longitude} />);

    const mapImageElement = await screen.findAllByTestId("mapimagecomponent");
    expect(mapImageElement).toBeDefined();
  });

  it("disables map dragging", async () => {
    const latitude = 37.7749;
    const longitude = -122.4194;

    render(<MapImage latitude={latitude} longitude={longitude} />);

    const map = await screen.findByTestId("mapimagecomponent");
    const draggable = map.querySelector('[draggable="true"]');
    expect(draggable).toBeNull();
  });

  it("displays a marker at the specified location", async () => {
    const latitude = 37.7749;
    const longitude = -122.4194;

    render(<MapImage latitude={latitude} longitude={longitude} />);

    const marker = await screen.findByTestId("marker");
    expect(marker).toBeInTheDocument();
  });
});


