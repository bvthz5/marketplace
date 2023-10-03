import React, { useCallback, useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import Swal from "sweetalert2";
import Footer from "../Footer/Footer";
import EditProductImageStyles from "./EditProductImage.module.css";
import { toast } from "react-toastify";
import CommonHeader from "../Header/HeaderComponent/Header";
import {
  addProductImage,
  deleteProductImageById,
  getProductImages,
} from "../../../core/Api/apiService";
import { validateProductImages } from "../../Utils/Utils";

const EditProductImage = () => {
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");
  let navigate = useNavigate();
  const [images, setImages] = useState("");
  const [file, setFile] = useState([]);
  const [count, setCount] = useState(0);
  const [loading, setLoading] = useState(false);

  const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;

  // fetching all images of a product for editing
  const getImages = useCallback(() => {
    getProductImages(id)
      .then((response) => {
        setImages(response?.data.data);
      })
      .catch((err) => console.log(err));
  },[id]);
  
  useEffect(() => {
    document.title = "Update image";
    getImages();
  }, [getImages]);


  // function for deleting current  product image
  const removeImage = (id) => {
    deleteProductImageById(id)
      .then(() => {
        getImages();
      })
      .catch((err) => {
        let error = err.response.data.message;
        if (error === "Photos Not Found") {
          toast.error("Error occured while deleting  images.Try again later", {
            toastId: "59",
          });
        }
      });
  };

  // function for storing user uploaded images

  function uploadImages(e) {
    let imgs = [];
    let counter = 0;
    if (e.target.files.length > 12) {
      toast.error("Maximum limit is 12 ");
      return;
    }
    if (e.target.files.length > 12 - images.length - file.length) {
      toast.error("Maximum limit is 12 ");
      return;
    }
    for (const images of e.target.files) {
      const fileSize = images.size / 1024 / 1024; // in MiB
      if (fileSize > 2) {
        toast.error("File size exceeds 2 MB");
      } else {
        let validtaionResult = validateProductImages(images);
        if (validtaionResult) {
          if (!file.some((f) => f.name === images.name)) {
            counter++;
            imgs.push(images);
          }
        }
      }
    }
    setCount(count + counter);
    setFile([...file, ...imgs]);
    console.log("file", file);
    document.getElementById("editimage").value = "";
  }

  // api call for posting product images

  function upload(e) {
    if (file.length <= 0) {
      Swal.fire("Please select an Image");
      return;
    }
    e.preventDefault();
    console.log(file);
    let data = new FormData();
    file.forEach((item) => {
      data.append("file", item);
    });
    console.log(data);
    setLoading(true);
    addProductImage(id, data)
      .then(() => {
        Swal.fire("Posted!", "Your product has been added.", "success");
        setLoading(false);
        navigate("/myproducts/?id=0");
      })
      .catch((err) => {
        setLoading(false);
        let error = err.response.data.message;
        if (error === "Product Not Found") {
          toast.error(
            "Error occured while uploading the new images.Try again later",
            { toastId: "59" }
          );
        }

        setLoading(false);
        setCount(count - 1);
        console.log(err);
      });
  }

  // function for deleting an image from the stored array
  function deleteFile(e) {
    const s = file.filter((item, index) => index !== e);
    setCount(count - 1);
    setFile(s);
  }
  return (
    <div data-testid="editimguploadpage">
      <CommonHeader />
      <div className={EditProductImageStyles.maincontainer}>
        <div className={EditProductImageStyles.containerimage}>
          <h2> Current Images</h2>
          <ul className={EditProductImageStyles.ul}>
            {images.length > 0 &&
              images.map((item, index) => {
                return (
                  <li className={EditProductImageStyles.li} key={item.photosId}>
                    <img
                      className={EditProductImageStyles.previewimg}
                      src={`${baseImageUrl}${item.photo}`}
                      alt=""
                    />
                    <div>
                      <button
                        data-testid="image-delete-btn"
                        type="button"
                        onClick={() => removeImage(item.photosId)}
                        className={EditProductImageStyles.deleteimage}
                      >
                        Delete
                      </button>
                    </div>
                  </li>
                );
              })}
          </ul>
        </div>

        <div className={EditProductImageStyles.containeruploadimage}>
          <div>
            <h2>Upload upto {12 - images.length} more photos</h2>
          </div>
          <div className={EditProductImageStyles.showlengthouter}>
            <div className={EditProductImageStyles.showlengthinner}>
              <input
                data-testid="image-uploader"
                className={EditProductImageStyles.chooseimage}
                type="file"
                multiple
                title=""
                id="editimage"
                accept="image/x-png,image/gif,image/jpeg"
                disabled={file.length === 12 - images.length}
                onChange={uploadImages}
              />
              <label className={EditProductImageStyles.labelsss}>
                {count === 0 ? "No File Chosen" : `${count} files chosen`}
              </label>
            </div>
          </div>

          <div className={EditProductImageStyles.imagecontainersmall}>
            <ul className={EditProductImageStyles.ul}>
              {file.length > 0 &&
                file.map((item, index) => {
                  return (
                    <li className={EditProductImageStyles.li} key={item.name}>
                      <img
                        className={EditProductImageStyles.previewimg}
                        src={URL.createObjectURL(item)}
                        alt=""
                      />
                      <div>
                        <button
                          data-testid="image-delete-button"
                          type="button"
                          onClick={() => deleteFile(index)}
                          className={EditProductImageStyles.deleteimage}
                        >
                          Delete
                        </button>
                      </div>
                    </li>
                  );
                })}
            </ul>
          </div>

          <div>
            <button
              data-testid="upload-button"
              className={EditProductImageStyles.upload}
              type="button"
              onClick={upload}
            >
              Upload
            </button>
          </div>
        </div>
      </div>

      <br></br>
      <br></br>
      <br></br>
      <Footer />
      {loading && (
        <div className={EditProductImageStyles.loading}>Loading…</div>
      )}
    </div>
  );
};

export default EditProductImage;
