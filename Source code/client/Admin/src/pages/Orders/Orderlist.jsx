import React, { useCallback, useEffect, useState } from 'react';
import ReactPaginate from 'react-paginate';
import { useNavigate } from 'react-router-dom';
import OrderListcss from './Orderlist.module.css';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableCell from '@mui/material/TableCell';
import TableRow from '@mui/material/TableRow';
import DriveFileMoveIcon from '@mui/icons-material/DriveFileMove';
import { Tooltip } from '@mui/material';
import { ordersList } from '../../core/api/apiService';
import OrderLoader from '../../utils/Loader/Loader';
import { convertDatesmall } from '../../utils/formatDate';
import { orderStatus } from '../../utils/Enums';

const Orderlist = () => {
  const [orders, setOrders] = useState([]);
  const pageLimit = 25;
  const [searchValue, setSearchValue] = useState('');
  const [status, setStatus] = useState('1');
  const [sortValue, setSortValue] = useState('OrdersId');
  const [pageNo, setPageNo] = useState(1);
  const [pageCount, setPageCount] = useState(1);
  const [desc, setDesc] = useState(true);
  const [apiCallOrder, setApiCallOrder] = useState(false);

  let navigate = useNavigate();

  useEffect(() => {
    document.title = 'Order List';
    getAllOrders();
  }, [searchValue, status, sortValue, pageNo]);

  //order list//
  const getAllOrders = () => {
    setApiCallOrder(true);
    const params = {
      pageNumber: pageNo,
      pageSize: pageLimit,
      search: searchValue,
      SortBy: sortValue,
      PaymentStatus: status ? status : '',
      SortByDesc: desc,
    };
    ordersList(params)
      .then((response) => {
        setOrders(response?.data.data.result);
        setPageCount(response?.data.data.totalPages);
        setApiCallOrder(false);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      })
      .catch((err) => {
        console.log(err);
        setApiCallOrder(false);
      });
  };
  //
  const handleSort = (data) => {
    if (data === 'OrdersId') {
      setDesc(true);
    } else setDesc(false);
  };

  const handlePageClick = useCallback((data) => {
    let currentPage = data.selected + 1;
    setPageNo(currentPage);
  }, []);

  const columns = [
    { id: 'id', label: 'Order Id', minWidth: 1, maxLength: 3 },
    { id: 'Order Number', label: 'Order Number', minWidth: 5 },
    {
      id: 'buyer',
      label: 'Buyer',
      minWidth: 5,
    },
    {
      id: 'price',
      label: 'Price',
      minWidth: 5,
    },
    {
      id: 'Order Date',
      label: 'Order Date',
      minWidth: 5,
    },
    {
      id: 'status',
      label: 'Order Status',
      minWidth: 5,
    },

    {
      id: 'payment',
      label: 'Payment Status',
      minWidth: 5,
    },

    {
      id: 'Actions',
      label: 'Actions',
      minWidth: 5,
    },
  ];

  return (
    <>
      <div className={OrderListcss['body']} data-testid="orderlistpage">
        <div className={OrderListcss.boxproductlist}>
          <div className={OrderListcss.productbox}>
            <div className={OrderListcss.userlistbackground}>
              <div className={OrderListcss['items']}>
                <div className={OrderListcss['searchdiv']}>
                  <input
                    data-testid="search-input"
                    className={OrderListcss['search']}
                    maxLength={255}
                    type="search"
                    placeholder="Search here"
                    onChange={(e) => {
                      setSearchValue(e.target.value);
                      setPageNo(1);
                    }}
                  />
                </div>
                <div className={OrderListcss['selectdiv']}>
                  <select
                    data-testid="status-filter-dropdown"
                    className={OrderListcss.select}
                    onChange={(e) => {
                      setStatus(e.target.value);
                      setPageNo(1);
                    }}
                    name="cars"
                    id="cars"
                  >
                    <option className={OrderListcss.optionsort} defaultChecked value="1">
                      Paid
                    </option>
                    <option className={OrderListcss.optionsort} value="0">
                      Unpaid
                    </option>
                    <option className={OrderListcss.optionsort} value="2">
                      Refunded
                    </option>
                    <option className={OrderListcss.optionsort} value="">
                      All
                    </option>
                  </select>
                  <select
                    data-testid="sort-dropdown"
                    className={OrderListcss.select}
                    onChange={(e) => {
                      handleSort(e.target.value);
                      setSortValue(e.target.value);
                      setPageNo(1);
                    }}
                    name="cars"
                    id="cars"
                  >
                    <option className={OrderListcss.optionsort} value="OrdersId" defaultChecked>
                      Order Id
                    </option>
                    <option className={OrderListcss.optionsort} value="Price">
                      Price
                    </option>
                    <option className={OrderListcss.optionsort} value="OrderDate">
                      Order Date
                    </option>
                  </select>
                </div>
              </div>
              <Paper className={OrderListcss.papercss}>
                <TableContainer className={OrderListcss['TableContainer']}>
                  <Table stickyHeader>
                    <TableHead>
                      <TableRow>
                        {columns.map((column) => (
                          <TableCell
                            key={column.id}
                            align={column.align}
                            style={{
                              backgroundColor: '#4B4453',
                              color: 'white',
                            }}
                          >
                            {column.label}
                          </TableCell>
                        ))}
                      </TableRow>
                    </TableHead>
                    {orders.length > 0 ? (
                      orders.map((order) => {
                        return (
                          <TableBody key={order.ordersId} role="checkbox" tabIndex={-1}>
                            <TableRow className={OrderListcss['row']}>
                              <TableCell className={OrderListcss['TableCell']} data-testid="ordersid">
                                {order.ordersId}
                              </TableCell>
                              <TableCell className={OrderListcss['TableCell']} data-testid="ordernumber">
                                &nbsp;
                                <span
                                  style={{
                                    overflow: 'hidden',
                                    lineBreak: 'anywhere',
                                  }}
                                >
                                  {order.orderNumber}
                                </span>
                              </TableCell>
                              <TableCell
                                className={OrderListcss['TableCell']}
                                style={{
                                  overflow: 'hidden',
                                  lineBreak: 'anywhere',
                                }}
                              >
                                {order.buyer.firstName}&nbsp;{order.buyer.lastName}
                              </TableCell>

                              <TableCell
                                className={OrderListcss['TableCell']}
                                style={{
                                  overflow: 'hidden',
                                  lineBreak: 'anywhere',
                                }}
                              >
                                {order.totalPrice ? `₹${order.totalPrice}` : '----'}
                              </TableCell>
                              <TableCell
                                className={OrderListcss['TableCell']}
                                style={{
                                  overflow: 'hidden',
                                  lineBreak: 'anywhere',
                                }}
                              >
                                {convertDatesmall(order.orderDate)}
                              </TableCell>
                              <TableCell className={OrderListcss['TableCell']}>
                                {order.orderStatus === orderStatus.CREATED && 'Created'}
                                {order.orderStatus === orderStatus.CONFIRMED && 'Confirmed'}
                                {order.orderStatus === orderStatus.CANCELLED && 'Cancelled'}
                                {order.orderStatus === orderStatus.FAILED && 'Failed'}
                                {order.orderStatus === orderStatus.INTRANSIT && 'In Transit'}
                                {order.orderStatus === orderStatus.WAITING_FOR_PICKUP && 'Waiting For Pickup'}
                                {order.orderStatus === orderStatus.OUTFORDELIVERY && 'Out for Delivery'}
                                {order.orderStatus === orderStatus.DELIVERED && 'Delivered'}
                              </TableCell>

                              <TableCell className={OrderListcss['TableCell']}>
                                {order.paymentStatus === 0 && <p className={OrderListcss['badge-danger']}>UnPaid</p>}
                                {order.paymentStatus === 1 && <p className={OrderListcss['badge-success']}>Paid</p>}
                                {order.paymentStatus === 2 && <p className={OrderListcss['badge-info']}>Refunded</p>}
                              </TableCell>
                              <TableCell className={OrderListcss['TableCell']}>
                                <button
                                  className={OrderListcss['button-55']}
                                  onClick={() => {
                                    navigate(`/dashboard/orderdetail/?id=${order.ordersId}`);
                                  }}
                                >
                                  <Tooltip title="View Details" placement="top">
                                    <DriveFileMoveIcon
                                      style={{
                                        color: '#3b89f7',
                                      }}
                                    />
                                  </Tooltip>
                                </button>
                              </TableCell>
                            </TableRow>
                          </TableBody>
                        );
                      })
                    ) : (
                      <>
                        {!apiCallOrder && (
                          <TableBody className={OrderListcss['listloader']}>
                            <tr>
                              <td>No Match Found</td>
                            </tr>
                          </TableBody>
                        )}
                      </>
                    )}
                  </Table>
                </TableContainer>
              </Paper>
            </div>
          </div>
        </div>
      </div>
      {orders.length !== 0 ? (
        <div type="button" className={OrderListcss['paginationdiv']}>
          <ReactPaginate
            className={OrderListcss['pagination']}
            previousLabel={'Previous'}
            nextLabel={'Next'}
            breakLabel={'...'}
            pageCount={pageCount}
            marginPagesDisplayed={2}
            pageRangeDisplayed={3}
            forcePage={pageNo - 1}
            onPageChange={handlePageClick}
            containerClassName={OrderListcss['paginationjustify']}
            pageClassName={OrderListcss['page-item']}
            pageLinkClassName={OrderListcss['page-link']}
            previousClassName={OrderListcss['page-item']}
            previousLinkClassName={OrderListcss['page-item']}
            nextClassName={OrderListcss['page-item']}
            nextLinkClassName={OrderListcss['page-item']}
            breakClassName={OrderListcss['page-item']}
            breakLinkClassName={OrderListcss['page-item']}
            activeClassName={OrderListcss['page-active']}
            disabledClassName={OrderListcss['page-prev-disabled']}
            disabledLinkClassName={OrderListcss['page-prev-disabled']}
            prevRel={null}
            prevPageRel={null}
          />
        </div>
      ) : null}
      {apiCallOrder && <OrderLoader />}
    </>
  );
};

export default Orderlist;
