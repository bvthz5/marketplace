import React, { useState, useCallback } from 'react';

import styledetail from './UserDetail.module.css';
import CloseIcon from '@mui/icons-material/Close';
import { allUserDetails } from './../../../core/api/apiService';
import { Avatar, Tooltip, Box, Modal } from '@mui/material';

const UserDetail = ({ user }) => {
  const [userDetails, SetUserDetails] = useState({});

  //get user details//
  const getUserDetails = () => {
    allUserDetails(user?.userId)
      .then((response) => {
        SetUserDetails(response?.data.data);
      })
      .catch((err) => {
        console.log(err);
      });
  };
  const baseImageUrl = process.env.REACT_APP_PROFILEIMAGE_PATH;

  const dateConvertion = (date) => {
    let currentDate = new Date(date);
    return currentDate.toDateString();
  };

  const style = {
    position: 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    bgcolor: 'background.paper',
    boxShadow: 24,
  };
  const [open, setOpen] = React.useState(false);

  const handleOpen = useCallback(() => {
    setOpen(true);
    getUserDetails();
  });
  const handleClose = useCallback(() => setOpen(false));

  return (
    <>
      <div data-testid="userdetailpage">
        <Tooltip title="User Profile" placement="top">
          <Avatar
            data-testid="avatarid"
            style={{ cursor: 'pointer' }}
            alt={`${user?.firstName} ${user?.lastName}`}
            src={user?.profilePic ? `${baseImageUrl}User/profile/${user.profilePic}` : user?.firstName}
            onClick={handleOpen}
          />
        </Tooltip>
        <Modal
          open={open}
          onClose={handleClose}
          aria-labelledby="modal-modal-title"
          aria-describedby="modal-modal-description"
        >
          <Box sx={style}>
            <div className={styledetail['card-container']}>
              <div className={styledetail['closeicon']}>
                <CloseIcon onClick={handleClose} />
              </div>
              <div className={styledetail['imgdiv']}>
                <Avatar
                  className={styledetail['imgstyle']}
                  alt={user?.firstName}
                  src={user?.profilePic ? `${baseImageUrl}User/profile/${user.profilePic}` : user?.firstName}
                />
              </div>
              <h2>
                {userDetails?.firstName} &nbsp; {userDetails?.lastName}
              </h2>
              <div className={styledetail['userContent']}>
                <div>
                  {' '}
                  <b className={styledetail['dataclass']}>Email : </b> {userDetails?.email}
                </div>
                <div>
                  <b>Mobile : </b>
                  {userDetails?.phoneNumber}
                </div>
                <div>
                  <b className={styledetail['dataclass']}>Address : </b>
                  {userDetails?.address}
                </div>
                <div>
                  <b className={styledetail['dataclass']}>City : </b>
                  {userDetails?.city}
                </div>
                <div className={styledetail['dataclass']}>
                  <b>District : </b>
                  {userDetails?.district}
                </div>
                <div>
                  <b className={styledetail['dataclass']}>State : </b>
                  {userDetails?.state}
                </div>
                <b>Joined On: </b>
                {dateConvertion(userDetails.createdDate)}
              </div>
            </div>
          </Box>
        </Modal>
      </div>
    </>
  );
};

export default UserDetail;
