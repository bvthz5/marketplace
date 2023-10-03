import React, { useCallback, useState } from 'react';
import sellerdetailcss from './SellerDetail.module.css';
import { getSellerDetails } from '../../../core/api/apiService';
import CloseIcon from '@mui/icons-material/Close';
import { Tooltip, Box, Modal, Avatar } from '@mui/material';

const SellerDetail = ({ seller }) => {
  const [sellerDetails, SetSellerDetails] = useState({});

  //seller details//
  const getDetails = async () => {
    getSellerDetails(seller?.userId)
      .then((response) => {
        SetSellerDetails(response?.data.data);
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
    // border: '5px solid #9e9eff',
    boxShadow: 24,
  };
  const [open, setOpen] = React.useState(false);
  const handleOpen = useCallback(() => {
    setOpen(true);
    getDetails();
  });
  const handleClose = useCallback(() => setOpen(false));

  return (
    <>
      <div data-testid="sellerdetailpage">
        <Tooltip title="Seller Profile" placement="top">
          <Avatar
            data-testid="avatarid"
            alt={`${seller?.firstName} ${seller?.lastName}`}
            src={seller?.profilePic ? `${baseImageUrl}User/profile/${seller.profilePic}` : seller?.firstName}
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
            <div className={sellerdetailcss['card-container']}>
              <div className={sellerdetailcss['closeicon']}>
                <CloseIcon onClick={handleClose} />
              </div>
              <div className={sellerdetailcss['imgdiv']}>
                <Avatar
                  className={sellerdetailcss['imgstyle']}
                  alt={`${seller?.firstName} ${seller?.lastName}`}
                  src={seller?.profilePic ? `${baseImageUrl}User/profile/${seller.profilePic}` : seller?.firstName}
                />
              </div>
              <h2>
                {sellerDetails?.firstName} &nbsp; {sellerDetails?.lastName}
              </h2>
              <div className={sellerdetailcss['userContent']}>
                <div>
                  {' '}
                  <b className={sellerdetailcss['dataclass']}>Email : </b> {sellerDetails?.email}
                </div>
                <div>
                  <b>Mobile : </b>
                  {sellerDetails?.phoneNumber}
                </div>
                <div>
                  <b className={sellerdetailcss['dataclass']}>Address : </b>
                  {sellerDetails?.address}
                </div>
                <div>
                  <b className={sellerdetailcss['dataclass']}>City : </b>
                  {sellerDetails?.city}
                </div>
                <div className={sellerdetailcss['dataclass']}>
                  <b>District : </b>
                  {sellerDetails?.district}
                </div>
                <div>
                  <b className={sellerdetailcss['dataclass']}>State : </b>
                  {sellerDetails?.state}
                </div>
                <b>Joined On: </b>
                {dateConvertion(sellerDetails.createdDate)}
              </div>
            </div>
          </Box>
        </Modal>
      </div>
    </>
  );
};

export default SellerDetail;
