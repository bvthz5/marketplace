import React, { useState, useEffect, useCallback } from 'react';
import sellercss from './SellerRequest.module.css';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableCell from '@mui/material/TableCell';
import { Tooltip } from '@mui/material';
import TableRow from '@mui/material/TableRow';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import HighlightOffIcon from '@mui/icons-material/HighlightOff';
import { toast } from 'react-toastify';
import { useForm } from 'react-hook-form';
import { allSellerRequest, approveSellerRequest, rejectSellerRequest } from '../../core/api/apiService';
import Loader from '../../utils/Loader/Loader';
import NoRequestFound from '../NoRequestFound/NoRequestFound';
import ReactPaginate from 'react-paginate';
import Box from '@mui/material/Box';

import Typography from '@mui/material/Typography';
import Modal from '@mui/material/Modal';

const SellerRequest = () => {
  const [requests, setRequests] = useState([]);
  const [pageLimit] = useState(25);
  const [pageNo, setPageNo] = useState(1);
  const [pageCount, setPageCount] = useState(1);
  const [loading, setLoading] = useState(false);
  const [userId, setuserId] = useState();
  const [apiCall, setApiCall] = useState(false);
  const [open, setOpen] = React.useState(false);
  const handleOpen = () => setOpen(true);
  const handleClose = useCallback(() => {
    setOpen(false);
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

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    reset,
  } = useForm({ mode: 'onChange' });
  useEffect(() => {
    document.title = 'Seller Requesition';
    getRequests();
  }, [pageNo]);
  ///all seller requests//
  const getRequests = async () => {
    setApiCall(true);
    const params = {
      pageNumber: pageNo,
      Role: 1,
      pageSize: pageLimit,
      Status: 1,
    };
    allSellerRequest(params)
      .then((response) => {
        setRequests(response?.data.data.result);
        setPageCount(response?.data.data.totalPages);
        setApiCall(false);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      })
      .catch((err) => {
        console.log(err);
        setApiCall(false);
      });
  };
  //approve seller request//
  const handleRequest = async (status, userId) => {
    setuserId(userId);
    if (!status) {
      handleOpen();
    } else {
      setLoading(true);
      approveSellerRequest(userId)
        .then((response) => {
          if (response.data.status) {
            getRequests();
            setLoading(false);
            toast.success('Request approved');
          }
        })
        .catch((err) => {
          console.log(err);
          setLoading(false);
        });
    }
  };
  //reject seller request//
  const handleRejection = async (e) => {
    handleClose();
    setLoading(true);
    rejectSellerRequest(userId, e.reason)
      .then((response) => {
        if (response.data.status) {
          getRequests();

          setLoading(false);
          toast.success('Request rejected.');
        }
        setLoading(false);
      })
      .catch((err) => {
        console.log(err);
        handleClose();
        setLoading(false);
      });
  };

  const dateConvertion = (date) => {
    let currentDate = new Date(date);
    return currentDate.toDateString();
  };
  const columns = [
    { id: 'id', label: 'ID', minWidth: 1 },
    { id: 'productname', label: 'Seller Name', minWidth: 5 },
    {
      id: 'createdate',
      label: 'Created Date',
      minWidth: 5,
    },
    {
      id: 'Details',
      label: 'Actions',
      minWidth: 5,
    },
  ];
  const handlePageClick = useCallback((data) => {
    let currentPage = data.selected + 1;
    setPageNo(currentPage);
  }, []);
  return (
    <>
      <div data-testid="sellerrequestpage">
        <div>
          <Modal
            open={open}
            onClose={handleClose}
            aria-labelledby="modal-modal-title"
            aria-describedby="modal-modal-description"
          >
            <Box sx={modalstyle}>
              <div className={sellercss['modalhead']}>
                <Typography className={sellercss['modalheading']}>Reject Request?</Typography>
              </div>
              <div className={sellercss['formdiv']}>
                <form onSubmit={handleSubmit(handleRejection)}>
                  <div className={sellercss['inputdiv']}>
                    <input
                    data-testid='reject-reason'
                      className={sellercss['modalinputfield']}
                      type="text"
                      placeholder="Reason  (optional)"
                      {...register('reason', {
                        maxLength: 255,
                      })}
                    ></input>

                    {errors.reason ? (
                      errors.reason.type === 'maxLength' && (
                        <div className={sellercss['error']}>Maximum 255 Characters</div>
                      )
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
                    <button className={sellercss['modalsubmitbutton']} type="submit" value="submit" data-testid='submitreasonbutton'>
                      Submit
                    </button>
                    <button
                    data-testid='cancelreasonbutton'
                      onClick={() => {
                        handleClose();
                        setValue('reason','');
                        reset();
                      }}
                      className={sellercss['modalcancelButton']}
                    >
                      Cancel
                    </button>
                  </div>
                </form>
              </div>
            </Box>
          </Modal>
        </div>
        <div>
          <div className={sellercss.boxproductlist}>
            <div className={sellercss.productbox}>
              <div>
                <div>
                  {requests.length !== 0 ? (
                    <Paper className={sellercss.papercss}>
                      <TableContainer className={sellercss['TableContainer']}>
                        <Table stickyHeader aria-label="sticky table">
                          <TableHead className={sellercss['TableHead']}>
                            <TableRow>
                              {columns.map((column) => (
                                <TableCell key={column.id} align={column.align} className={sellercss['tablecellmain']}>
                                  {column.label}
                                </TableCell>
                              ))}
                            </TableRow>
                          </TableHead>
                          <TableBody role="checkbox" tabIndex={-1}>
                            {requests.length > 0 ? (
                              requests.map((user) => {
                                return (
                                  <TableRow key={user.userId} className={sellercss['row']}>
                                    <TableCell className={sellercss['TableCell']}>{user.userId}</TableCell>
                                    <TableCell
                                      className={sellercss['TableCell']}
                                      style={{ overflow: 'hidden', lineBreak: 'anywhere' }}
                                    >
                                      {user.firstName}
                                    </TableCell>
                                    <TableCell className={sellercss['TableCell']}>
                                      {dateConvertion(user.createdDate)}
                                    </TableCell>
                                    <TableCell className={sellercss['TableCell']}>
                                      <button
                                      data-testid='rejectbutton'
                                        className={sellercss['button-55']}
                                        onClick={() => {
                                          handleRequest(false, user.userId);
                                          handleOpen();
                                        }}
                                      >
                                        <Tooltip title="Reject  seller" placement="top">
                                          <HighlightOffIcon
                                            style={{
                                              color: 'red',
                                            }}
                                          />
                                        </Tooltip>
                                      </button>
                                      <button
                                      data-testid='approvebutton'
                                        className={sellercss['button-55']}
                                        onClick={() => {
                                          handleRequest(true, user.userId);
                                        }}
                                      >
                                        <Tooltip title="Accept  seller" placement="top">
                                          <CheckCircleOutlineIcon
                                            style={{
                                              color: 'Green',
                                            }}
                                          />
                                        </Tooltip>
                                      </button>
                                    </TableCell>
                                  </TableRow>
                                );
                              })
                            ) : (
                              <>{!apiCall && <div className={sellercss['listloader']}>No Requests Found</div>}</>
                            )}
                          </TableBody>
                        </Table>
                      </TableContainer>
                    </Paper>
                  ) : (
                    <NoRequestFound />
                  )}
                </div>
              </div>
            </div>
          </div>
        </div>
        {loading && <div className={sellercss.loading}>Loading…</div>}
        {requests.length !== 0 ? (
          <div type="button" className={sellercss['paginationdiv']}>
            <ReactPaginate
              className={sellercss['pagination']}
              previousLabel={'Previous'}
              nextLabel={'Next'}
              breakLabel={'...'}
              pageCount={pageCount}
              marginPagesDisplayed={2}
              pageRangeDisplayed={3}
              forcePage={pageNo - 1}
              onPageChange={handlePageClick}
              containerClassName={sellercss['paginationjustify']}
              pageClassName={sellercss['page-item']}
              pageLinkClassName={sellercss['page-link']}
              previousClassName={sellercss['page-item']}
              previousLinkClassName={sellercss['page-item']}
              nextClassName={sellercss['page-item']}
              nextLinkClassName={sellercss['page-item']}
              breakClassName={sellercss['page-item']}
              breakLinkClassName={sellercss['page-item']}
              activeClassName={sellercss['page-active']}
              disabledClassName={sellercss['page-prev-disabled']}
              disabledLinkClassName={sellercss['page-prev-disabled']}
              prevRel={null}
              prevPageRel={null}
            />
          </div>
        ) : null}
        {apiCall && <Loader />}
      </div>
    </>
  );
};

export default SellerRequest;
