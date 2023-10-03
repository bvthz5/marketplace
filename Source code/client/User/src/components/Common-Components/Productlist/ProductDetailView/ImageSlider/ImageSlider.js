import React, { useCallback, useEffect, useState } from "react";
import "matchmedia-polyfill";
import Slider from "react-slick";
import detailcss from "../ProductListDetailView.module.css";
import CloseIcon from "@mui/icons-material/Close";
import { settings as defaultSettings } from "./settings";

const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;

const ImageSlider = ({ images }) => {
  const [imageModal, setImageModal] = useState(false);
  const [settings, setSettings] = useState(defaultSettings(images));

  const handleClose = useCallback(
    (index = 0) => {
      setSettings({
        ...settings,
        initialSlide: index,
        className: "",
        arrows: true,
      });
      setImageModal(false);
    },
    [settings]
  );
  useEffect(() => {
    const handleEsc = (event) => {
      if (event.keyCode === 27) {
        setImageModal(false);
        handleClose();
      }
    };
    window.addEventListener("keydown", handleEsc);
    return () => {
      window.removeEventListener("keydown", handleEsc);
    };
  }, [handleClose]);

  const toggleModal = (im) => {
    let myindex = images.map((item) => item.photosId).indexOf(im.photosId);
    setSettings({
      ...settings,
      initialSlide: myindex,
      className: detailcss.sliderContainer,
      arrows: false,
    });
    setImageModal(!imageModal);
  };

  return (
    <>
      <div className="detailview-imagecontainer">
        <Slider {...settings}>
          {images?.map((im) => {
            return (
              <img
                data-testid={im}
                onClick={(e) => {
                  toggleModal(im);
                }}
                id={im.photosId}
                key={im.photosId}
                alt="loading..."
                src={`${baseImageUrl}${im.photo}`}
              />
            );
          })}
        </Slider>
      </div>
      {imageModal && (
        <div className={detailcss["container"]} data-testid="fullscreenimage">
          <button
            className={detailcss["close"]}
            data-testid="modal-close-btn"
            onClick={() => handleClose()}
          >
            <CloseIcon style={{ fontSize: "38px" }} />
          </button>
          <div className={detailcss["fullscreen-imgouter-container"]}>
            <div className="detailview-modalimagecontainer">
              <Slider {...settings}>
                {images.map((im, index) => {
                  return (
                    <img
                      key={im.photosId}
                      data-testid={im.photosId}
                      onClick={(e) => {
                        handleClose(index);
                      }}
                      alt="loading..."
                      src={`${baseImageUrl}${im.photo}`}
                    />
                  );
                })}
              </Slider>
            </div>
          </div>
        </div>
      )}
    </>
  );
};
export default ImageSlider;
