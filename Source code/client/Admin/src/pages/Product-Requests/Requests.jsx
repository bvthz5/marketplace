import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import style from './Request.module.css';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableCell from '@mui/material/TableCell';
import TableRow from '@mui/material/TableRow';
import TextSnippetIcon from '@mui/icons-material/TextSnippet';
import { Tooltip } from '@mui/material';
import RequestLoader from '../../utils/Loader/Loader';
import NoRequestFound from '../NoRequestFound/NoRequestFound';
import { allRequest } from '../../core/api/apiService';
import ReactPaginate from 'react-paginate';

const Requests = () => {
  let navigate = useNavigate();
  const [requests, setRequests] = useState([]);
  const [apiCallRequest, setApiCallRequest] = useState(false);
  const [pageCount, setPageCount] = useState(1);
  const [pageNo, setPageNo] = useState(1);
  useEffect(() => {
    document.title = 'Product Requesition';
    getRequests();
  }, [pageNo]);
  //get requests//
  const getRequests = async () => {
    setApiCallRequest(true);
    const params = {
      Status: 2,
      pageSize: 25,
      pageNumber: pageNo,
    };
    allRequest(params)
      .then((response) => {
        console.log(response?.data.data.result);
        setRequests(response?.data.data.result);
        setPageCount(response?.data.data.totalPages);
        setApiCallRequest(false);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      })
      .catch((err) => {
        console.log(err);
        setApiCallRequest(false);
      });
  };
  //
  const dateConvertion = (date) => {
    let currentDate = new Date(date);
    return currentDate.toDateString();
  };
  const columns = [
    { id: 'id', label: 'ID', minWidth: 1 },
    { id: 'productname', label: 'Product Name', minWidth: 5 },
    {
      id: 'createdate',
      label: 'Created Date',
      minWidth: 5,
    },
    {
      id: 'Details',
      label: 'Details',
      minWidth: 5,
    },
  ];
  const handlePageClick = useCallback(
    (data) => {
      let currentPage = data.selected + 1;
      setPageNo(currentPage);
    },
    [pageNo]
  );
  return (
    <>
      <div data-testid="requestpage">
        <div className={style.boxproductlist}>
          <div className={style.productbox}>
            <div style={{ minHeight: '70vh' }}>
              <div>
                {requests.length !== 0 ? (
                  <Paper
                    sx={{
                      width: '95%',
                      overflow: 'hidden',
                      margin: 'auto',
                      marginTop: '50px',
                      zIndex: '0',
                    }}
                  >
                    <TableContainer className={style['TableContainer']}>
                      <Table stickyHeader aria-label="sticky table">
                        <TableHead className={style['TableHead']}>
                          <TableRow>
                            {columns.map((column) => (
                              <TableCell
                                key={column.id}
                                align={column.align}
                                style={{
                                  backgroundColor: '#4B4453',
                                  color: 'white',
                                  zIndex: '0',
                                }}
                              >
                                {column.label}
                              </TableCell>
                            ))}
                          </TableRow>
                        </TableHead>
                        <TableBody role="checkbox" tabIndex={-1}>
                          {requests.length > 0 ? (
                            requests.map((product) => {
                              return (
                                <TableRow key={product.productId} className={style['row']}>
                                  <TableCell className={style['TableCell']}>{product.productId}</TableCell>
                                  <TableCell
                                    className={style['TableCell']}
                                    style={{ overflow: 'hidden', lineBreak: 'anywhere' }}
                                  >
                                    {product.productName}
                                  </TableCell>
                                  <TableCell className={style['TableCell']}>
                                    {dateConvertion(product.createdDate)}
                                  </TableCell>
                                  <TableCell className={style['TableCell']}>
                                    <button
                                    data-testid='details'
                                      className={style['button-55']}
                                      onClick={() => {
                                        navigate(`/dashboard/adminproductdetailview/?id=${product.productId}`);
                                      }}
                                    >
                                      <Tooltip title="View Details" placement="top">
                                        <TextSnippetIcon
                                          style={{
                                            color: 'blue',
                                          }}
                                        />
                                      </Tooltip>
                                    </button>
                                  </TableCell>
                                </TableRow>
                              );
                            })
                          ) : (
                            <>{!apiCallRequest && <div className={style['listloader']}>No Requests Found</div>}</>
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
            {requests.length !== 0 ? (
              <div type="button" className={style['paginationdiv']}>
                <ReactPaginate
                  className={style['pagination']}
                  previousLabel={'Previous'}
                  nextLabel={'Next'}
                  breakLabel={'...'}
                  pageCount={pageCount}
                  marginPagesDisplayed={2}
                  pageRangeDisplayed={3}
                  forcePage={pageNo - 1}
                  onPageChange={handlePageClick}
                  containerClassName={style['paginationjustify']}
                  pageClassName={style['page-item']}
                  pageLinkClassName={style['page-link']}
                  previousClassName={style['page-item']}
                  previousLinkClassName={style['page-item']}
                  nextClassName={style['page-item']}
                  nextLinkClassName={style['page-item']}
                  breakClassName={style['page-item']}
                  breakLinkClassName={style['page-item']}
                  activeClassName={style['page-active']}
                  disabledClassName={style['page-prev-disabled']}
                  disabledLinkClassName={style['page-prev-disabled']}
                  prevRel={null}
                  prevPageRel={null}
                />
              </div>
            ) : null}
          </div>
        </div>
        {apiCallRequest && <RequestLoader />}
      </div>
    </>
  );
};

export default Requests;
