import React, { useState, useCallback, useEffect } from 'react';
import ReactPaginate from 'react-paginate';
import OrderListcss from './AgentOrderList.module.css';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Unstable_Grid2';
import { styled } from '@mui/material/styles';
import Paper from '@mui/material/Paper';
import { agentGetOrders } from '../../../core/api/apiService';
import { convertDatesmall } from '../../../utils/formatDate';
import ArrowUpwardIcon from '@mui/icons-material/ArrowUpward';
import ArrowDownwardIcon from '@mui/icons-material/ArrowDownward';
import LoaderOrder from '../../../utils/Loader/Loader';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import { Tooltip } from '@mui/material';
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import { useNavigate } from 'react-router-dom';
import ArrowForwardIosIcon from '@mui/icons-material/ArrowForwardIos';
import { orderStatus } from '../../../utils/Enums';

const AgentOrderList = () => {
  let navigate = useNavigate();
  const [orders, setOrders] = useState([]);
  const [searchValue, setSearchValue] = useState('');
  const [searchError, setSearchError] = useState('');
  const [pageCount, setPageCount] = useState(1);
  const [apicallOrderList, setApiCallOrder] = useState(false);
  const [desc, setDesc] = useState(true);
  const [pageNo, setPageNo] = useState(1);
  const [myProducts, setMyProducts] = useState(localStorage.getItem('myOrders') === 'true');

  useEffect(() => {
    document.title = 'Order List';
    getAllOrders();
  }, [searchValue, pageNo, desc, myProducts]);

  const handleSort = () => {
    setDesc(!desc);
    setPageNo(1);
  };

  const navigateToOrderDetail = (ordersId) => {
    navigate(`/agentdashboard/orderDetailView?orderId=${ordersId}`);
  };

  //order list//
  const getAllOrders = () => {
    setApiCallOrder(true);
    const params = {
      PageNumber: pageNo,
      PageSize: 25,
      Search: searchValue,
      SortBy: 'OrderDate',
      SortByDesc: desc,
      MyProductsOnly: myProducts,
    };
    agentGetOrders(params)
      .then((response) => {
        console.log(response);
        setOrders(response?.data?.data?.result);
        setPageCount(response?.data?.data?.totalPages);
        setApiCallOrder(false);
        window.scrollTo({
          top: 0,
          behavior: 'smooth',
        });
      })
      .catch((err) => {
        console.log(err);
        setApiCallOrder(false);
      });
  };

  const handlePageClick = useCallback((data) => {
    let currentPage = data.selected + 1;
    setPageNo(currentPage);
  }, []);

  const handleMyOrders = useCallback(() => {
    setPageNo(1);
    setMyProducts(true);
    localStorage.setItem('myOrders', true);
  }, []);

  const handleAllOrders = useCallback(() => {
    setPageNo(1);
    setMyProducts(false);
    localStorage.setItem('myOrders', false);
  }, []);

  const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(3),
    textAlign: 'center',
    color: theme.palette.text.secondary,
  }));
  return (
    <div data-testid="orderlist">
      <div className={OrderListcss['items']}>
        {/* toggle button start */}
        <div className={OrderListcss['togglediv']}>
          <ToggleButtonGroup exclusive aria-label="text alignment">
            <ToggleButton
              value={myProducts ? true : false}
              onClick={handleMyOrders}
              aria-label="left aligned"
              style={{ color: 'black', backgroundColor: myProducts ? '#b6c3e0' : '#ffffff' }}
            >
              My Orders
            </ToggleButton>
            <ToggleButton
              value={myProducts ? false : true}
              onClick={handleAllOrders}
              aria-label="right aligned"
              style={{ color: 'black', backgroundColor: !myProducts ? '#cde1f2' : '#ffffff' }}
            >
              All
            </ToggleButton>
          </ToggleButtonGroup>
        </div>
        {/* toggle button end */}
        <div className={OrderListcss['searchdiv']}>
          <input
            data-testid="search-Zip"
            className={OrderListcss['search']}
            maxLength={255}
            onInput={(e) => {
              e.target.value = e.target.value?.replace(/\D/g, '');
            }}
            type="search"
            placeholder="Search By 6-digit zipcode"
            onChange={(e) => {
              const value = e.target.value;
              console.log(value, value.length);
              if (value === '' || value.length === 0) {
                setSearchError('');
                setSearchValue('');
              } else {
                if (value.length === 6) {
                  setSearchValue(value);
                  setPageNo(1);
                  setSearchError('');
                } else if (value.length < 6 || value.length > 6) {
                  setSearchValue('');
                  setSearchError('Enter valid 6-digit zipcode');
                }
              }
            }}
          />
          {searchError && <div className={OrderListcss['error']}>{searchError}</div>}
        </div>
        <div className={OrderListcss['selectdiv']} data-testid="sort-arrow">
          OrderDate :{' '}
          <div style={{ cursor: 'pointer' }}>
            {desc ? (
              <div onClick={handleSort}>
                <ArrowUpwardIcon style={{ color: '#5856d6' }} />
              </div>
            ) : (
              <div onClick={handleSort}>
                <ArrowDownwardIcon style={{ color: '#5856d6' }} />
              </div>
            )}
          </div>
        </div>
      </div>
      <div className={OrderListcss.productcontaineralign}>
        <Box sx={{ marginTop: '15px' }}>
          <Grid container spacing={4} style={{ width: '100%', justifyContent: 'center', marginLeft: '0px' }}>
            <Grid
              lg={5}
              sm={10}
              style={{ width: '100%', marginLeft: '10px', height: '70px' }}
              className={OrderListcss.Heading}
            >
              <Item className={OrderListcss['data-card1']}>
                <div className={OrderListcss['datamain']}>
                  <div className={OrderListcss['fordata']}>
                    <div className={OrderListcss['h3design1']}>Order Number</div>
                  </div>
                  <div className={OrderListcss['namediv']}>Customer Name</div>
                  <div className={OrderListcss['forprice1']}>
                    <span>Total Price </span>
                  </div>
                  <div className={OrderListcss['zipCodeBtn']}>
                    <span className={OrderListcss.pspan1}>Zipcode</span>
                  </div>
                  <div className={OrderListcss['categorydesign1']}>Order Date</div>
                  <div className={OrderListcss['aligninfo']}>Order Status</div>
                </div>
                <div className={OrderListcss['orderDetail']}>
                  <span>Detail View</span>
                </div>
              </Item>
            </Grid>
            {orders.length > 0 ? (
              orders.map((order) => {
                return (
                  <Grid lg={5} sm={10} style={{ width: '100%', marginLeft: '10px' }} key={order.ordersId}>
                    <div className={OrderListcss.orderItem} onClick={() => navigateToOrderDetail(order?.ordersId)}>
                      <Item className={OrderListcss['data-card']} key={order.ordersId}>
                        <div className={OrderListcss['datamain']}>
                          <div className={OrderListcss['fordata']}>
                            <div className={OrderListcss['h3design']}>{order?.orderNumber}</div>
                          </div>
                          <div className={OrderListcss['namediv']}> &nbsp;{order?.deliveryAddress?.name}</div>
                          <div className={OrderListcss['forprice']}>
                            <span>&nbsp; &#8377; {order.totalPrice}</span>
                          </div>
                          <div className={OrderListcss['zipCodeBtn']}>
                            <LocationOnIcon style={{ fontSize: '18px' }} />
                            <Tooltip title="ZIPCODE" placement="top">
                              <span className={OrderListcss.pspan}>{order?.deliveryAddress?.zipCode}</span>
                            </Tooltip>
                          </div>
                          <div className={OrderListcss['categorydesign']}>
                            <span>&nbsp; {convertDatesmall(order.orderDate)}</span>
                          </div>
                          <div className={OrderListcss['aligninfo']}>
                            {order.orderStatus === orderStatus.CREATED && (
                              <button
                                style={{ color: 'yellow', backgroundColor: 'rgba(251, 255, 8, 0.16)' }}
                                className={OrderListcss['statuscss']}
                              >
                                Order created
                              </button>
                            )}
                            {order.orderStatus === orderStatus.CONFIRMED && (
                              <button
                                style={{ color: 'black', backgroundColor: 'rgba(3, 3, 1, 0.16)' }}
                                className={OrderListcss['statuscss']}
                              >
                                Order confirmed
                              </button>
                            )}

                            {order.orderStatus === orderStatus.WAITING_FOR_PICKUP && (
                              <button
                                style={{ color: 'orange', backgroundColor: '#f7750329' }}
                                className={OrderListcss['statuscss']}
                              >
                                Waiting For Pickup
                              </button>
                            )}

                            {order.orderStatus === orderStatus.CANCELLED && (
                              <button
                                style={{ color: 'red', backgroundColor: 'rgba(255, 86, 48, 0.16)' }}
                                className={OrderListcss['statuscss']}
                              >
                                Order Cancelled
                              </button>
                            )}
                            {order.orderStatus === orderStatus.INTRANSIT && (
                              <button
                                style={{ color: 'green', backgroundColor: 'rgba(7, 92, 49, 0.16)' }}
                                className={OrderListcss['statuscss']}
                              >
                                In-Transit
                              </button>
                            )}

                            {order.orderStatus === orderStatus.DELIVERED && (
                              <button
                                style={{ color: 'blue', backgroundColor: 'rgba(10, 38, 244, 0.16)' }}
                                className={OrderListcss['statuscss']}
                              >
                                Delivered
                              </button>
                            )}

                            {order.orderStatus === orderStatus.OUTFORDELIVERY && (
                              <button
                                style={{ color: 'orange', backgroundColor: 'rgba(247, 143, 39, 0.16)' }}
                                className={OrderListcss['statuscss']}
                              >
                                Out for Delivery
                              </button>
                            )}
                          </div>
                        </div>
                        <div className={OrderListcss['orderDetail']}>
                          <span>
                            <button className={OrderListcss['button-55']}>
                              <Tooltip title="View Details" placement="top">
                                <ArrowForwardIosIcon
                                  style={{
                                    color: 'blue',
                                  }}
                                />
                              </Tooltip>
                            </button>
                          </span>
                        </div>
                      </Item>
                    </div>
                  </Grid>
                );
              })
            ) : (
              <div className={OrderListcss['noprductsfound']}>No Orders Found!</div>
            )}
          </Grid>
        </Box>
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
      {apicallOrderList && <LoaderOrder />}
    </div>
  );
};

export default AgentOrderList;
