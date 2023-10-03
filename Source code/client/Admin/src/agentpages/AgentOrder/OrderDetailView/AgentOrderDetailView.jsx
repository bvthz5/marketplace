import orderDetailViewCss from './AgentOrderDetailView.module.css';
import React, { useCallback, useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import Grid from '@mui/material/Unstable_Grid2';
import { toast } from 'react-toastify';
import { getAgentDetailOrder, assignOrder, unAssignOrder, changeDeliveryStatus } from '../../../core/api/apiService';
import RequestLoader from '../../../utils/Loader/Loader';
import AgentOrdersCard from '../OrdersCard/AgentOrdersCard';
import Swal from 'sweetalert2';
import { Button, Box, Typography } from '@mui/material';
import { orderStatus } from '../../../utils/Enums';
import DeliveryOtp from './DeliveryOtp/DeliveryOtp';

const AgentOrderDetailView = () => {
  const [searchParams] = useSearchParams();
  const orderId = searchParams.get('orderId');
  const [details, setDetails] = useState({});
  const [apiCallRequest, setApiCallRequest] = useState(false);

  let navigate = useNavigate();

  useEffect(() => {
    document.title = 'Order Details';
    getOrder();
  }, [orderId]);

  const getOrder = useCallback(async () => {
    setApiCallRequest(true);
    getAgentDetailOrder(orderId)
      .then((response) => {
        setDetails(response?.data.data);
        setApiCallRequest(false);
      })
      .catch((err) => {
        console.error(err);
        toast.error('Order details not found!');
        navigate(`/agentdashboard/orderlist`);
      });
  }, [orderId]);

  const handleAssignOrder = useCallback(async () => {
    setApiCallRequest(true);
    assignOrder(details?.ordersId)
      .then(() => {
        Swal.fire({
          icon: 'success',
          title: 'Assigned to my orders',
        });
      })
      .catch((err) => {
        const limitError = 'Max Limit (10) exceed';
        if (err.response.data.message === limitError) {
          Swal.fire({
            icon: 'warning',
            title: 'Max Limit Reached',
            text: 'Maximum 10 orders can be assigned',
          });
        }
      })
      .finally(() => {
        setApiCallRequest(false);
        getOrder();
      });
  }, [details]);

  const handleUnassignOrder = useCallback(async () => {
    setApiCallRequest(true);
    unAssignOrder(details?.ordersId)
      .then(() => {
        Swal.fire({
          icon: 'success',
          title: 'Order Unassigned',
        });
      })
      .catch((err) => {
        const newError = `Order Not Found`;
        if (err.response.data.message === newError) {
          Swal.fire({
            icon: 'warning',
            title: 'Order Not Found',
          });
        }
        console.error(err);
      })
      .finally(() => {
        setApiCallRequest(false);
        getOrder();
      });
  }, [details]);

  const handleOutForDelivery = useCallback(async () => {
    setApiCallRequest(true);
    changeDeliveryStatus(details?.ordersId, orderStatus.OUTFORDELIVERY)
      .then(() => {
        Swal.fire({
          icon: 'success',
          title: 'Status Changed to Out for Delivery',
        });
      })
      .catch((err) => {
        console.error(err);
      })
      .finally(() => {
        setApiCallRequest(false);

        getOrder();
      });
  }, [details]);

  const handleStatusToInTransit = useCallback(async () => {
    setApiCallRequest(true);
    changeDeliveryStatus(details?.ordersId, orderStatus.INTRANSIT)
      .then(() => {
        Swal.fire({
          icon: 'success',
          title: 'Status Changed to IN-TRANSIT',
        });
      })
      .catch((err) => {
        console.error(err);
      })
      .finally(() => {
        setApiCallRequest(false);
        getOrder();
      });
  }, [details]);

  const handlepaymentStatus = (status) => {
    if (status === 0) {
      return <p className={orderDetailViewCss['badge-danger']}>UnPaid</p>;
    }
    if (status === 1) {
      return <p className={orderDetailViewCss['badge-success']}>Paid</p>;
    }
  };
  return (
    <>
      <div className={orderDetailViewCss.maindiv} data-testid="orderdetail">
        <div className={orderDetailViewCss.productcardmaindiv}>
          <div className={orderDetailViewCss.productcontaineralign}>
            <Box>
              <Grid container spacing={2} style={{ width: '100%', justifyContent: 'center' }}>
                {details.items
                  ? details.items.map((item) => {
                      return <AgentOrdersCard key={item?.orderDetailsId} item={item} />;
                    })
                  : !apiCallRequest && <div>No details Found!</div>}
              </Grid>
            </Box>
          </div>
        </div>
        <div className={orderDetailViewCss['otherdetails']}>
          <div className={orderDetailViewCss.orderStatusDiv}>
            {details?.orderStatus === orderStatus.CONFIRMED && (
              <Button
                variant="outlined"
                color="success"
                data-testid="assinbtn"
                onClick={handleAssignOrder}
                style={{ width: '100%' }}
              >
                Assign
              </Button>
            )}

            <>
              {details?.orderStatus === orderStatus.WAITING_FOR_PICKUP && (
                <Button variant="outlined" onClick={handleUnassignOrder} color="error" style={{ width: '45%' }}>
                  UnAssign
                </Button>
              )}

              {details?.orderStatus === orderStatus.WAITING_FOR_PICKUP && (
                <Button variant="outlined" onClick={handleStatusToInTransit} style={{ width: '45%' }}>
                  In-Transit
                </Button>
              )}
            </>

            {details?.orderStatus === orderStatus.INTRANSIT && (
              <Button variant="outlined" onClick={handleOutForDelivery} style={{ width: '100%' }}>
                Out for Delivery
              </Button>
            )}

            {details?.orderStatus === orderStatus.OUTFORDELIVERY && (
              <DeliveryOtp onCompleted={getOrder} orderId={orderId} email={details?.buyer?.email} />
            )}

            {details?.orderStatus === orderStatus.DELIVERED && (
              <Typography
                style={{
                  width: '100%',
                  display: 'flex',
                  justifyContent: 'center',
                  border: '1px solid green',
                  borderRadius: '5px',
                  padding: '6px',
                }}
                color={'green'}
                fontWeight={'bold'}
              >
                Order Delivered
              </Typography>
            )}

            {details?.orderStatus === orderStatus.CANCELLED && (
              <Typography
                style={{
                  width: '100%',
                  display: 'flex',
                  justifyContent: 'center',
                  border: '1px solid red',
                  borderRadius: '5px',
                  padding: '6px',
                }}
                color={'red'}
                fontWeight={'bold'}
              >
                Order Cancelled
              </Typography>
            )}
          </div>

          <div className={orderDetailViewCss['ordersummarybox']}>
            <div className={orderDetailViewCss['h4head']}>Order Summary</div>
            <div style={{ marginTop: '20px' }}>
              <div className={orderDetailViewCss['div']}>
                <div>
                  <span className={orderDetailViewCss['totalprice']}>
                    {' '}
                    Price of {details?.items?.length} product(s) :{' '}
                  </span>
                </div>
                <div className={orderDetailViewCss['divamount']}>
                  <span className={orderDetailViewCss['paddingprice']}>
                    {details?.items?.reduce((partialSum, { product: { price } }) => partialSum + price, 0)}
                  </span>
                </div>
              </div>
              <div className={orderDetailViewCss['div']}>
                <div>
                  <span className={orderDetailViewCss['totalprice']}> Delivery Charges: </span>
                </div>
                <div className={orderDetailViewCss['divamount']}>
                  <span className={orderDetailViewCss['free']}>FREE</span>
                </div>
              </div>
              <div className={orderDetailViewCss['div']}>
                <div>
                  <span className={orderDetailViewCss['totalprice']}> Other Charges:</span>
                </div>
                <div className={orderDetailViewCss['divamount']}>
                  <span className={orderDetailViewCss['othercharges']}>FREE</span>
                </div>
              </div>
              {details?.items?.some((item) => item.status === orderStatus.CANCELLED) && (
                <div className={orderDetailViewCss['refunddiv']}>
                  <div>
                    <span className={orderDetailViewCss['totalprice']}> Refunded :</span>
                  </div>
                  <div className={orderDetailViewCss['divamount']}>
                    <span className={orderDetailViewCss['refundprice']}>
                      {
                        -details?.items
                          ?.filter((item) => item.status === orderStatus.CANCELLED)
                          ?.reduce((partialSum, { product: { price } }) => partialSum + price, 0)
                      }
                    </span>
                  </div>
                </div>
              )}
              {details?.totalPrice !== 0 && (
                <div className={orderDetailViewCss['totaldiv']}>
                  <div>
                    <span className={orderDetailViewCss['totalprice']}> Total price:</span>
                  </div>
                  <div className={orderDetailViewCss['divamount']}>
                    <span className={orderDetailViewCss['paddingprice']}>{details.totalPrice}</span>
                  </div>
                </div>
              )}

              <div className={orderDetailViewCss['paymentstatus']}>{handlepaymentStatus(details.paymentStatus)}</div>
            </div>
          </div>

          <div className={orderDetailViewCss['buyersummarybox']}>
            <div className={orderDetailViewCss['detailshead']}>Buyer Details </div>
            <div className={orderDetailViewCss['buyerddress']}>
              <div className={orderDetailViewCss['buyerfl']}>
                {details?.buyer?.firstName}&nbsp; {details?.buyer?.lastName}
              </div>
              <div className={orderDetailViewCss['buyerfl']}> {details?.buyer?.email}</div>
              <div className={orderDetailViewCss['buyername']}>
                <div className={orderDetailViewCss['buyerdetails']}>{details?.buyer?.address}</div>
                <div className={orderDetailViewCss['buyerdetails']}>{details?.buyer?.state}</div>
                <div className={orderDetailViewCss['buyerdetails']}>{details?.buyer?.district}</div>
                <div className={orderDetailViewCss['buyerdetails']}>{details?.buyer?.city}</div>
                <div className={orderDetailViewCss['buyerdetails']}> {details?.buyer?.phoneNumber}</div>
              </div>
            </div>
            <div className={orderDetailViewCss['addresshead']}>Delivery Address </div>
            <div className={orderDetailViewCss['divaddress']}>
              <div>{details?.deliveryAddress?.name}</div>
              <div>{details?.deliveryAddress?.address}</div>
              <div>{details?.deliveryAddress?.streetAddress}</div>
              <div>{details?.deliveryAddress?.city}</div>
              <div>{details?.deliveryAddress?.state}</div>
              <div>{details?.deliveryAddress?.zipCode}</div>
              <div>{details?.deliveryAddress?.phone}</div>
            </div>
          </div>
        </div>
      </div>
      {apiCallRequest && <RequestLoader />}
    </>
  );
};

export default AgentOrderDetailView;
