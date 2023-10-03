import React, { useEffect, useState } from "react";
import style from "./userProfile.module.css";
import CommonHeader from "../../Header/HeaderComponent/Header";
import { Avatar, Tooltip } from "@mui/material";
import { format } from "date-fns";
import { FaRegEdit } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import Footer from "../../Footer/Footer";
import EditProfile from "../Edit-Profile/EditProfile";
import { toast } from "react-toastify";
import { updateProfilePic } from "../../../../core/Api/apiService";
import { validateProfilePicture } from "../../../Utils/Utils";
import Title from "../../../Utils/PageTitle/Title";
import ScrollToTop from "../../../Utils/ScrollToPageTop/ScrollToTop";
import { useDispatch, useSelector } from "react-redux";
import { fetchUserDetails, userDetails } from "../userDetailSlice";
const UserProfile = () => {
  let navigate = useNavigate();
  const dispatch = useDispatch();
  const userData = useSelector(userDetails);

  const [newImage, setNewImage] = useState(null);
  const baseImageUrl = process.env.REACT_APP_PROFILEIMAGE_PATH;

  useEffect(() => {
    document.title = "Profile";
    dispatch(fetchUserDetails());
  }, [dispatch]);

  useEffect(() => {
    //   uploading new profile picture
    const handleUpload = async () => {
      // image validation size limit 2 MB
      const fileSize = newImage[0].size / 1024 / 1024; // in MiB
      if (fileSize > 2) {
        setNewImage(null);
        toast.warning("File size exceeds 2 MB", {
          position: "top-center",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
          progress: undefined,
          theme: "light",
        });
      } else {
        console.log(newImage[0]);
        // call for image extension validation
        let validtaionResult = validateProfilePicture(newImage);
        if (validtaionResult) {
          let data = new FormData();
          data.append("file", newImage[0]);
          updateProfilePic(data)
            .then(() => {
              dispatch(fetchUserDetails());
              toast.success("Profile Picture Updated", {
                position: "top-center",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
                theme: "light",
              });
            })
            .catch((err) => console.log(err));
        }
      }
    };
    if (newImage) {
      handleUpload();
    }
  }, [newImage,dispatch]);

  return (
    <div data-testid="userprofilepage">
      <ScrollToTop />
      <CommonHeader />
      <Title pageTitle={"My Profile"} />
      <div className={style.productcontaineralign}>
        <div className={style.productcontainer}>
          <div className={style.profilesection}>
            <div className={style["profiletop"]}>
              <Avatar
                className={style.profilepic}
                src={
                  userData?.profilePic
                    ? `${baseImageUrl}User/profile/${userData?.profilePic}`
                    : `${userData?.firstName}`
                }
                alt={userData?.firstName}
              />

              <Tooltip title="change profile picture">
                <label>
                  <input
                    data-testid="image-uploader"
                    id="file"
                    className={style["input"]}
                    label="change profile picture"
                    onChange={(e) => {
                      setNewImage(e.target.files);
                    }}
                    type="file"
                  />
                  <FaRegEdit style={{ cursor: "pointer" }} />
                </label>
              </Tooltip>
            </div>
            <div className={style["profilebottom"]}>
              <div className={style["emailid"]}>{userData?.email}</div>
              <div className={style["buttondiv"]}>
                <button
                  data-testid="changepassword-button"
                  onClick={() => {
                    navigate("/changepassword");
                  }}
                  className={style["button"]}
                >
                  Change password
                </button>
              </div>
              <div className={style["datecontainer"]}>
                <div>{"Member Since "}</div>
                <div>
                  {userData?.createdDate &&
                    format(new Date(userData?.createdDate), "dd/MM/yyyy")}
                </div>
              </div>
            </div>
          </div>

          <div className={style.detailsection}>
            <EditProfile />
          </div>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default UserProfile;
