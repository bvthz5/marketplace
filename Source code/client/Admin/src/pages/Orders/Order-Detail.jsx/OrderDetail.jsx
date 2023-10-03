import ordercss from './OrderDetail.module.css';
import React, { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Unstable_Grid2';
import { toast } from 'react-toastify';
import { orderDetailProducts } from '../../../core/api/apiService';
import RequestLoader from '../../../utils/Loader/Loader';
import ProductCard from './ProductCard/ProductCard';
import { orderStatus } from '../../../utils/Enums';

function OrderDetail() {
  const [searchParams] = useSearchParams();
  const id = searchParams.get('id');
  const [details, setDetails] = useState([]);
  const [apiCallRequest, setApiCallRequest] = useState(false);

  let navigate = useNavigate();

  useEffect(() => {
    document.title = 'Order Details';
    getProducts();
  }, [id]);

  //get products with details//
  const getProducts = async () => {
    setApiCallRequest(true);
    orderDetailProducts(id)
      .then((response) => {
        setDetails(response?.data.data);
        setApiCallRequest(false);
      })
      .catch((err) => {
        toast.error('Order details not found!');
        navigate(`/dashboard/orders`);
        setDetails([]);
        setApiCallRequest(false);
      });
  };

  const handlepaymentStatus = (status) => {
    if (status === 0) {
      return <p className={ordercss['badge-danger']}>UnPaid</p>;
    }
    if (status === 1) {
      return <p className={ordercss['badge-success']}>Paid</p>;
    }
    if (status === 2) {
      return <p className={ordercss['badge-info']}>Refunded</p>;
    }
  };

  return (
    <>
      <div className={ordercss.maindiv} data-testid="orderdetailpage">
        <div className={ordercss.productcardmaindiv}>
          <div className={ordercss.productcontaineralign}>
            <Box sx={{ marginTop: '15px' }}>
              <Grid container spacing={2} style={{ width: '100%', justifyContent: 'center' }}>
                {details.items
                  ? details.items.map((item) => {
                      return (
                        <ProductCard key={item?.orderDetailsId} item={item} paymentStatus={details.paymentStatus} />
                      );
                    })
                  : !apiCallRequest && <div>No details Found!</div>}
              </Grid>
            </Box>
          </div>
        </div>
        <div className={ordercss['otherdetails']}>
          <div className={ordercss['ordersummarybox']}>
            <div className={ordercss['h4head']}>Order Summary</div>
            <div style={{ marginTop: '20px' }}>
              <div className={ordercss['div']}>
                <div>
                  <span className={ordercss['totalprice']}> Price of {details?.items?.length} product(s) : </span>
                </div>
                <div className={ordercss['divamount']}>
                  <span className={ordercss['paddingprice']}>
                    {details?.items?.reduce((partialSum, { product: { price } }) => partialSum + price, 0)}
                  </span>
                </div>
              </div>
              <div className={ordercss['div']}>
                <div>
                  <span className={ordercss['totalprice']}> Delivery Charges: </span>
                </div>
                <div className={ordercss['divamount']}>
                  <span className={ordercss['free']}>FREE</span>
                </div>
              </div>
              <div className={ordercss['div']}>
                <div>
                  <span className={ordercss['totalprice']}> Other Charges:</span>
                </div>
                <div className={ordercss['divamount']}>
                  <span className={ordercss['othercharges']}>FREE</span>
                </div>
              </div>
              {details?.items?.some((item) => item.status === orderStatus.CANCELLED) && (
                <div className={ordercss['refunddiv']}>
                  <div>
                    <span className={ordercss['totalprice']}> Refunded :</span>
                  </div>
                  <div className={ordercss['divamount']}>
                    <span className={ordercss['refundprice']}>
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
                <div className={ordercss['totaldiv']}>
                  <div>
                    <span className={ordercss['totalprice']}> Total price:</span>
                  </div>
                  <div className={ordercss['divamount']}>
                    <span className={ordercss['paddingprice']}>{details.totalPrice}</span>
                  </div>
                </div>
              )}

              <div className={ordercss['paymentstatus']}>{handlepaymentStatus(details.paymentStatus)}</div>
            </div>
          </div>

          <div className={ordercss['buyersummarybox']}>
            <div className={ordercss['detailshead']}>Buyer Details </div>
            <div className={ordercss['buyerddress']}>
              <div className={ordercss['buyerfl']}>
                {details?.buyer?.firstName}&nbsp; {details?.buyer?.lastName}
              </div>
              <div className={ordercss['buyerfl']}> {details?.buyer?.email}</div>
              <div className={ordercss['buyername']}>
                <div className={ordercss['buyerdetails']}>{details?.buyer?.address}</div>
                <div className={ordercss['buyerdetails']}>{details?.buyer?.state}</div>
                <div className={ordercss['buyerdetails']}>{details?.buyer?.district}</div>
                <div className={ordercss['buyerdetails']}>{details?.buyer?.city}</div>
                <div className={ordercss['buyerdetails']}> {details?.buyer?.phoneNumber}</div>
              </div>
            </div>
            <div className={ordercss['addresshead']}>Delivery Address </div>
            <div className={ordercss['divaddress']}>
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
}

export default OrderDetail;
