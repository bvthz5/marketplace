import { Box, MobileStepper, Modal, Tooltip, Typography } from '@mui/material';
import React, { useCallback, useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom/dist';
import notfound from '../../../assets/Screenshot-2018-12-16-at-21.06.29.png';
import Swal from 'sweetalert2';
import CloseIcon from '@mui/icons-material/Close';
import style from './AdminProductDetailView.module.css';
import { useForm as UseForm } from 'react-hook-form';
import EditIcon from '@mui/icons-material/Edit';
import KeyboardArrowLeftIcon from '@mui/icons-material/KeyboardArrowLeft';
import KeyboardArrowRightIcon from '@mui/icons-material/KeyboardArrowRight';
import MapImage from '../../Google-Maps/MapImage';
import { convertDate } from '../../../utils/formatDate';
import { toast } from 'react-toastify';
import { handleStatus } from '../../../utils/utils';
import {
  approverejectProduct,
  detailProducts,
  editProductName,
  getProductImage,
  rejectProductRequest,
} from '../../../core/api/apiService';
import Loader from '../../../utils/Loader/Loader';

const AdminProductDetailView = () => {
  const [searchParams] = useSearchParams();
  const id = searchParams.get('id');
  const [product, setProduct] = useState([]);
  const [images, setImages] = useState([]);
  const CollectionSize = images?.length;
  const [index, setActiveStep] = useState(0);
  const [title, setTitle] = useState('Details');
  const [imageModal, setImageModal] = useState(false);
  const [apiCall, setApiCall] = useState(false);

  const [open, setOpen] = React.useState(false);
  const handleOpen = () => setOpen(true);
  const handleClose = useCallback(() => {
    setOpen(false);
    reset();
  }, []);
  const [openReasonModal, setOpenReasonModal] = React.useState(false);
  const handleOpenReasonModal = () => setOpenReasonModal(true);
  const handleCloseReasonModal = useCallback(() => {
    setOpenReasonModal(false);
  }, []);
  const modalstyle = {
    position: 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    width: 400,
    height: '240px',
    bgcolor: 'background.paper',
    borderRadius: '20px',
  };

  const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;
  let navigate = useNavigate();

  useEffect(() => {
    getImage();
    getProducts();
  }, []);

  useEffect(() => {
    const handleEsc = (event) => {
      if (event.keyCode === 27) {
        setImageModal(false);
      }
    };
    window.addEventListener('keydown', handleEsc);
    return () => {
      window.removeEventListener('keydown', handleEsc);
    };
  }, []);

  useEffect(() => {
    document.title = `Product Details`;
  }, [title]);

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    reset,
  } = UseForm({ mode: 'onChange' });

  //getproducts//
  const getProducts = async () => {
    setApiCall(true);
    detailProducts(id ? id : 0)
      .then((response) => {
        setApiCall(false);
        setTitle(response.data.data.productName);
        setProduct(response?.data.data);
        setValue('productName', response?.data.data.productName);
      })
      .catch((err) => {
        if (err.response.data.serviceStatus === 404) {
          navigate('/dashboard/products');
        }
      });
  };
  //get product image//
  const getImage = async () => {
    getProductImage(id ? id : 0)
      .then((response) => {
        setImages(response?.data.data);
      })
      .catch((err) => {
        console.log(err);
      });
  };
  //approve/reject//
  const handleApproval = async (productId, productName, status) => {
    let messsage = status === 1 ? 'Approve' : 'Reject';
    Swal.fire({
      title: `${messsage} ${productName} ?`,
      icon: 'question',
      heightAuto: false,
      customClass: style['styleTitle'],
      showCancelButton: true,
      allowOutsideClick: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: `Yes, ${messsage} it!`,
    }).then(async (result) => {
      if (result.isConfirmed) {
        setApiCall(true);
        approverejectProduct(productId)
          .then((response) => {
            if (response?.data.status) {
              Swal.fire(`${messsage}d!`, `${productName} has been ${messsage}d `, 'success');
              getProducts();
              setApiCall(false);
            }
          })
          .catch((err) => {
            console.log(err);
          });
      }
    });
  };
  //edit product name//
  const onSubmit = async (data) => {
    if (data.productName.trim() === '') {
      toast.error('Whitespaces are not allowed!');
      return;
    }
    let editData = {
      productName: data.productName,
    };
    editProductName(id, editData.productName)
      .then((response) => {
        if (response?.data.status) {
          getProducts();
          handleClose();
        }
      })
      .catch((err) => {
        console.log(err);
        handleClose();
      });
  };
  //extras//
  const goToNextPicture = useCallback(() => {
    if (index === CollectionSize - 1) {
      setActiveStep(0);
      return;
    }
    setActiveStep((prevActiveStep) => prevActiveStep + 1);
  });
  const goToPrevPicture = useCallback(() => {
    if (index === 0) {
      setActiveStep(CollectionSize - 1);
      return;
    }
    setActiveStep((prevActiveStep) => prevActiveStep - 1);
  });

  const toggleModal = () => {
    setImageModal(!imageModal);
  };

 
  const handleRejection = (e) => {
    handleCloseReasonModal();
    setApiCall(true);
    setApiCall(true);
    rejectProductRequest(id, e.reason)
      .then((response) => {
        setApiCall(false);
        if (response.data.status) {
          getProducts();
          setApiCall(false);
          toast.success('Request rejected.');
        }

        handleCloseReasonModal();
      })
      .catch((err) => {
        console.log(err);
        setApiCall(false);
      });
  };

  return (
    <>
      <div data-testid="adminproductdetailspage">
        <div className={style['pagecontainer']}>
          <div className={style['contentwrapper']}>
            <div className={style['leftsection']}>
              <div className={style['imageouter']}>
                <div className={style['imagediv']}>
                  <img
                    onClick={toggleModal}
                    data-testid="imagetag"
                    src={images?.length ? `${baseImageUrl}${images[index]?.photo}` : notfound}
                    alt=""
                    className={`${style['productimage']} ${style['loaded']}`}
                  />
                  <div className={style['imagestepper']}>
                    <div className={style['previousimage']}>
                      {images.length > 1 ? (
                        <KeyboardArrowLeftIcon
                          onClick={goToPrevPicture}
                          data-testid="previmagebtn"
                          style={{
                            color: 'black',
                            width: '50px',
                            height: '120px',
                            cursor: 'pointer',
                          }}
                        />
                      ) : (
                        <KeyboardArrowLeftIcon
                          style={{
                            color: 'White',
                            width: '50px',
                            height: '120px',
                            cursor: 'pointer',
                          }}
                        />
                      )}
                    </div>
                    <div className={style['pageno']}>
                      {images.length > 0 ? (
                        <MobileStepper
                          variant="text"
                          position="static"
                          activeStep={index}
                          steps={CollectionSize ? CollectionSize : 0}
                          style={{
                            marginRight: '0px',
                            padding: '1px',
                            display: 'flex',
                            flexDirection: 'coloum',
                            overflow: 'hidden',
                            width: '60px',
                            backgroundColor: 'white',
                            color: 'black',
                          }}
                        />
                      ) : (
                        ''
                      )}
                    </div>
                    <div className={style['nextimage']}>
                      {images.length > 1 ? (
                        <KeyboardArrowRightIcon
                          data-testid="nextimagebtn"
                          onClick={goToNextPicture}
                          style={{
                            color: 'black',
                            width: '50px',
                            height: '120px',
                            cursor: 'pointer',
                          }}
                        />
                      ) : (
                        ''
                      )}
                    </div>
                  </div>
                </div>
              </div>

              <div className={style['detailcontainer']}>
                <div className={style['productdetails']}>
                  <span style={{ fontSize: '23px' }}>
                    <b style={{ wordBreak: 'break-word' }}>{product?.productName}</b>
                    <button
                      disabled={product.status === 4}
                      data-testid="editIcon"
                      style={{ background: 'none', border: 'none' }}
                      onClick={() => {
                        setValue('productName', product?.productName);
                        handleOpen();
                      }}
                    >
                      {(product.status === 2 || product.status === 1) && (
                        <Tooltip disabled={product.status === 4} title="Edit Name" placement="top">
                          <EditIcon className={style['editIcon']}></EditIcon>
                        </Tooltip>
                      )}
                    </button>
                  </span>
                  <div className={style['fontstyle']}>{product?.productDescription}</div>
                  <div className={style['fontstyle']}>
                    <b>Category: </b> {product?.categoryName}
                  </div>
                  <div className={style['fontstyle']}>
                    <b> Price:</b> {product?.price}
                  </div>
                  <div className={style['approvebuttonmain']} data-testid='rejectstatus'>
                    {product.status === 2 && (
                      <div className={style['approvediv']}>
                        <button
                        data-testid="approvebutton"
                          className={style['approvebutton']}
                          onClick={() => {
                            handleApproval(product.productId, product.productName, 1);
                          }}
                        >
                          Approve
                        </button>
                      </div>
                    )}

                    {product.status === 2 && (
                      <div className={style['approvediv']}>
                        <button className={style['rejectbutton']} onClick={handleOpenReasonModal} data-testid='rejectButton'>
                          Reject
                        </button>
                      </div>
                    )}
                    <Modal
                      open={openReasonModal}
                      onClose={handleCloseReasonModal}
                      aria-labelledby="modal-modal-title"
                      aria-describedby="modal-modal-description"
                    >
                      <Box sx={modalstyle}>
                        <div>
                          <div className={style['head']}>
                            <Typography className={style['heading']}>Reject Request</Typography>
                          </div>
                          <div className={style['inputdiv']}>
                            <form onSubmit={handleSubmit(handleRejection)}>
                              <div className={style['inputerrordiv']}>
                                <input

                                  className={style['modalinputfield']}
                                  type="text"
                                  data-testid='reason-input'
                                  placeholder="Reason (Optional)"
                                  {...register('reason', {
                                    maxLength: {
                                      value: 255,
                                      message: 'Maximum 255 Characters',
                                    },
                                  })}
                                ></input>
                                {errors.reason ? (
                                  <div className={style['logged-error']}>{errors.reason.message}</div>
                                ) : (
                                  <br />
                                )}
                              </div>

                              <div
                                style={{
                                  display: 'grid',
                                  paddingBottom: '20px',
                                }}
                              >
                                <button className={style['modalsubmitbutton']} type="submit" value="submit" data-testid='submitButton'>
                                  Submit
                                </button>{' '}
                                <button
                                  onClick={() => {
                                    handleCloseReasonModal();
                                    setValue('reason', '');
                                    reset();
                                  }}
                                  className={style['modalcancelButton']}
                                >
                                  Cancel
                                </button>
                              </div>
                            </form>
                          </div>
                        </div>
                      </Box>
                    </Modal>
                  </div>
                  <div className={style['datepadding']}>
                    <div className={style['statusofproduct']}> {handleStatus(product.status)}</div>
                    <div className={style['dateshow']}>
                      <p className={style['date']}>{convertDate(product.createdDate)}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div className={style['rightsection']}>
              <div className={style['dealerside']}>
                <div className={style['pricebox']}>
                  <b> ₹ {product?.price}</b>
                </div>
                <div className={style['dealerbox']}>
                  <div className={style['dealerdetails']}>
                    <h2
                      style={{
                        color: '#002f34',
                      }}
                    >
                      <b style={{ fontWeight: '600' }}>Dealer Details</b>
                    </h2>
                    <div className={style['fontstyle']}>
                      <b> Dealer:</b> {product?.createdUser?.firstName} &nbsp;
                      {product?.createdUser?.lastName}
                    </div>
                    <div className={style['fontstyle']}>
                      <b> Contact:</b>&nbsp;
                      {product?.createdUser?.email}
                    </div>
                    <div className={style['fontstyle']}>
                      <b>Location: </b> {product?.location}
                    </div>
                  </div>
                </div>
              </div>

              <div className={style['locationBox']}>
                <div className={style['locationDetails']}>
                  <b style={{ fontWeight: '600' }}>Posted In</b>
                  <h4 className={style['locationBoxh2']}>{product?.location}</h4>
                  <div>
                    <MapImage latitude={product?.latitude} longitude={product?.longitude} />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      {/*  */}
      <div>
        <Modal
          open={open}
          onClose={handleClose}
          aria-labelledby="modal-modal-title"
          aria-describedby="modal-modal-description"
        >
          <Box sx={modalstyle}>
            <div>
              <div className={style['head']}>
                <Typography className={style['heading']}>Edit Product Name</Typography>
              </div>
              <div className={style['inputdiv']}>
                <form onSubmit={handleSubmit(onSubmit)}>
                  <div className={style['errorspace']}>
                    <input
                      data-testid="productname-input"
                      type="text"
                      className={style['inputfield']}
                      name="zipCode"
                      {...register('productName', {
                        required: 'Product name required ',
                        maxLength: {
                          value: 200,
                          message: 'Maximum 200 characters allowed',
                        },
                        pattern: {
                          value: /^[a-z0-9!@%&()_\-,."'+|:/\s]+$/i,
                          message: 'Invalid Product Name',
                        },
                      })}
                    ></input>
                    {errors.productName && <div className={style['logged-error']}>{errors.productName.message}</div>}
                  </div>
                  <div
                    style={{
                      display: 'grid',
                      paddingBottom: '20px',
                    }}
                  >
                    <button type="submit" value="submit" className={style['submitbutton']} data-testid='editsubmitbutton'>
                      Submit
                    </button>
                    <button
                      className={style['cancelButton']}
                      onClick={() => {
                        reset();
                        handleClose();
                      }}
                    >
                      Cancel
                    </button>
                  </div>
                </form>
              </div>
            </div>
          </Box>
        </Modal>
      </div>
      {/*  */}

      {imageModal && (
        <div className={style['container']} data-testid="fullscreenimage">
          <button className={style['close']} onClick={toggleModal}>
            <CloseIcon style={{ fontSize: '38px' }} />
          </button>
          <div className={style['fullscreen-imgouter-container']}>
            <div className={style['fullscreen-img-container']}>
              <img
                src={images?.length ? `${baseImageUrl}${images[index]?.photo}` : notfound}
                alt=""
                className={style['productimage']}
              />
            </div>
            <div className={style['fullscreenimagestepper']}>
              <div onClick={goToPrevPicture}>
                {images.length > 1 ? (
                  <KeyboardArrowLeftIcon
                    style={{
                      color: 'white',
                      width: '50px',
                      height: '120px',
                      cursor: 'pointer',
                    }}
                  />
                ) : (
                  ''
                )}
              </div>
              <div>
                <div className={style['fullscreenpageno']}>
                  {images.length > 0 ? (
                    <MobileStepper
                      variant="text"
                      position="static"
                      activeStep={index}
                      steps={CollectionSize ? CollectionSize : 0}
                      style={{
                        marginRight: '0px',
                        padding: '1px',
                        display: 'flex',
                        flexDirection: 'coloum',
                        overflow: 'hidden',
                        width: '60px',
                        backgroundColor: ' rgb(255 252 252 / 0%)',
                        color: 'white',
                      }}
                    />
                  ) : (
                    ''
                  )}
                </div>
              </div>
              <div onClick={goToNextPicture}>
                {images.length > 1 ? (
                  <KeyboardArrowRightIcon
                    style={{
                      color: 'white',
                      width: '50px',
                      height: '120px',
                      cursor: 'pointer',
                    }}
                  />
                ) : (
                  ''
                )}
              </div>
            </div>
          </div>
        </div>
      )}
      {apiCall && <Loader />}
    </>
  );
};

export default AdminProductDetailView;
