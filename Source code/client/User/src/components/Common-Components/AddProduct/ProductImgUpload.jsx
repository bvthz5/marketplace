import React, { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import Swal from "sweetalert2";
import Footer from "../Footer/Footer";
import ProductImgUploadStyle from "./ProductImgUpload.module.css";
import { toast } from "react-toastify";
import CommonHeader from "../Header/HeaderComponent/Header";
import { addProductImage } from "../../../core/Api/apiService";
import { validateProductImages } from "../../Utils/Utils";

const ProductImgUpload = () => {
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");
  let navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [file, setFile] = useState([]);
  const [count, setCount] = useState(0);

  useEffect(() => {
    document.title = "upload image";
    let addImage = localStorage.getItem("addImage");
    console.log(addImage);
    if (!addImage || addImage === "false") {
      navigate("/myproducts?id=0");
    }
  }, [navigate]);
  
  // function for storing user uploaded images
  function uploadImages(e) {
    let imgs = [];
    let counter = 0;
    if (e.target.files.length > 12) {
      toast.error("Maximum limit is 12 ");
      return;
    }
    if (e.target.files.length > 12 - file.length) {
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
    document.getElementById("addimage").value = "";
  }

  // api call for posting product images
  async function upload(e) {
    if (file.length <= 0) {
      Swal.fire("Please select an Image");
      return;
    }
    setLoading(true);
    e.preventDefault();
    console.log(file);
    let data = new FormData();
    file.forEach((item) => {
      data.append("file", item);
    });
    addProductImage(id, data)
      .then(() => {
        setLoading(false);
        localStorage.setItem("addImage", false);
        Swal.fire("Posted!", "Your product has been added.", "success");
        navigate("/myproducts/?id=0");
      })
      .catch((err) => {
        console.log(err);
        setLoading(false);
      });
  }

  function deleteFile(e) {
    const s = file.filter((item, index) => index !== e);
    setFile(s);
    setCount(count - 1);
    console.log(s);
  }

  return (
    <>
      <div data-testid="productimguploadpage">
        <CommonHeader />
        <div className={ProductImgUploadStyle.maincontainer}>
          <div className={ProductImgUploadStyle.container}>
            <div className={ProductImgUploadStyle.showheading}>
              <div>
                <h2>Upload up to 12 photos</h2>
              </div>
              <div className={ProductImgUploadStyle.showlengthouter}>
                <div className={ProductImgUploadStyle.showlengthinner}>
                  <input
                    data-testid="image-uploader"
                    className={ProductImgUploadStyle.chooseimage}
                    type="file"
                    id="addimage"
                    multiple
                    title=""
                    accept="image/x-png,image/gif,image/jpeg"
                    disabled={file.length === 12}
                    onChange={uploadImages}
                  />

                  <label className={ProductImgUploadStyle.labelsss}>
                    {count === 0 ? "No File Chosen" : `${count} files chosen`}
                  </label>
                </div>
              </div>
            </div>

            <div className={ProductImgUploadStyle.imagecontainer}>
              <ul className={ProductImgUploadStyle.ul}>
                {file.length > 0 &&
                  file.map((item, index) => {
                    return (
                      <li className={ProductImgUploadStyle.li} key={item.name}>
                        <img
                          className={ProductImgUploadStyle.previewimg}
                          src={URL.createObjectURL(item)}
                          alt=""
                        />
                        <div>
                          <button
                            data-testid="image-delete-button"
                            type="button"
                            onClick={() => deleteFile(index)}
                            className={ProductImgUploadStyle.deleteimage}
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
                className={ProductImgUploadStyle.upload}
                type="button"
                onClick={upload}
              >
                Upload
              </button>
              <br />
            </div>
          </div>
        </div>

        <br></br>
        <br></br>
        <br></br>
        <Footer />
        {loading && (
          <div className={ProductImgUploadStyle.loading}>Loading…</div>
        )}
      </div>
    </>
  );
};

export default ProductImgUpload;
