import React, { useEffect } from 'react';
import ordercss from './ProductCard.module.css';
import Grid from '@mui/material/Unstable_Grid2';
import { styled } from '@mui/material/styles';
import Paper from '@mui/material/Paper';
import MyImage from './../../../../assets/Image_not_available.png';
import { handleItemStatus } from '../../../../utils/utils';
import { orderStatus } from '../../../../utils/Enums';
import StepperView from './AdminStepperview';

const ProductCard = ({ item, paymentStatus }) => {
  const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;

  const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(3),
    textAlign: 'center',
    color: theme.palette.text.secondary,
  }));
  useEffect(() => {
    console.log('hiiiiiii', item);
  }, []);

  return (
    <div data-testid="productcard" style={{ width: '100%' }}>
      <Grid lg={5} sm={10} style={{ width: '100%', marginLeft: '10px' }} key={item?.product?.productId}>
        <Item
          className={ordercss['data-card']}
          key={item?.product?.productId}
          style={{ minHeight: '235px', display: 'flex', flexDirection: 'column', gap: '50px' }}
        >
          <div className={ordercss.productdetailsss}>
            <div className={ordercss['imagealign']}>
              <img
                src={item?.product?.thumbnail ? `${baseImageUrl}${item?.product?.thumbnail.toString()}` : MyImage}
                className={ordercss['image']}
                alt=""
              />
            </div>
            <div className={ordercss['datadiv']}>
              <div className={ordercss['fordata']}>{item?.product?.productName}</div>
              <div className={ordercss['categorydesign']}>{item?.product?.categoryName}</div>
              <div className={ordercss['forprice']}>
                <span>&#8377;{item?.product?.price}</span>
              </div>
              <div style={{ textAlign: 'left', width: '100%' }}>
                {item?.status !== orderStatus.CANCELLED ? (
                  <span style={{ color: '#409063' }}>{handleItemStatus(item?.status)}</span>
                ) : (
                  <div>
                    <div style={{ color: 'red' }}>{handleItemStatus(item?.status)}</div>
                    <div>
                      <div className={ordercss.cancelReason}>
                        <span className={ordercss.reasonTitle}>Reason :&nbsp;</span>
                        {item?.reason}
                      </div>
                    </div>
                  </div>
                )}
              </div>
              <div className={ordercss['sellerd']}>
                <div className={ordercss['h4']}>Seller details</div>
                <div className={ordercss['width']}>
                  {item?.product?.createdUser.firstName}&nbsp;{item?.product?.createdUser.lastName}
                </div>
                <div className={ordercss['width']}> {item?.product?.createdUser.email}</div>
              </div>
            </div>
          </div>
        </Item>
        {paymentStatus !== 0 && (
          <Item style={{ border: '1px solid #c5c4c4aa' }}>
            <div style={{ width: '100%' }}>
              <StepperView orderDetails={item} orderStatus={item?.status} horizontal={true} />
            </div>
          </Item>
        )}
      </Grid>
    </div>
  );
};

export default ProductCard;
