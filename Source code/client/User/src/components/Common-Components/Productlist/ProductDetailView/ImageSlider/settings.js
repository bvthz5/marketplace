import React from "react";
const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;

export const settings = (images) => {
  return {
    customPaging: function (i) {
      return (
        <img
          style={{ height: "50px" }}
          src={`${baseImageUrl}${images[i].photo}`}
          alt="loading..."
        />
      );
    },
    dots: true,
    lazyLoad: false,
    infinite: true,
    slidesToShow: 1,
    slidesToScroll: 1,
    speed: 500,
    autoplaySpeed: 2500,
    initialSlide: 0,
    autoplay: false,
    focusOnSelect: true,
  };
};
