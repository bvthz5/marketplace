import React from 'react';
import cardcss from './AgentOrdersCard.module.css';
import Grid from '@mui/material/Unstable_Grid2';
import { styled } from '@mui/material/styles';
import Paper from '@mui/material/Paper';
import MyImage from '../../../assets/Image_not_available.png';
import { handleItemStatus } from '../../../utils/utils';
import { orderStatus } from '../../../utils/Enums';

const AgentOrdersCard = ({ item }) => {
  const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;

  const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(3),
    textAlign: 'center',
    color: theme.palette.text.secondary,
  }));
  return (
    <div data-testid="AgentOrdersCard" style={{ width: '98%' }}>
      <Grid lg={5} sm={10} style={{ width: '100%', marginLeft: '10px' }} key={item?.product?.productId}>
        <Item
          className={cardcss['data-card']}
          key={item?.product?.productId}
          style={{ minHeight: '235px', display: 'flex', flexDirection: 'row' }}
        >
          <div className={cardcss['imagealign']}>
            <img
              src={item?.product?.thumbnail ? `${baseImageUrl}${item?.product?.thumbnail.toString()}` : MyImage}
              className={cardcss['image']}
              alt=""
            />
          </div>
          <div className={cardcss['datadiv']}>
            <div className={cardcss['fordata']}>{item?.product?.productName}</div>
            <div className={cardcss['categorydesign']}>{item?.product?.categoryName}</div>
            <div className={cardcss['forprice']}>
              <span>&#8377;{item?.product?.price}</span>
            </div>
            <div style={{ textAlign: 'left', width: '100%' }}>
              {item?.status !== orderStatus.CANCELLED && <span style={{ color: '#409063' }}>{handleItemStatus(item?.status)}</span>}
              {item?.status === orderStatus.CANCELLED && (
                <div>
                  <div style={{ color: 'red' }}>{handleItemStatus(item?.status)}</div>
                </div>
              )}
            </div>

            <div className={cardcss['sellerd']}>
              <div className={cardcss['h4']}>Seller details</div>
              <div className={cardcss['width']}>
                {item?.product?.createdUser.firstName}&nbsp;{item?.product?.createdUser.lastName}
              </div>
              <div className={cardcss['width']}> {item?.product?.createdUser.email}</div>
            </div>
          </div>
        </Item>
      </Grid>
    </div>
  );
};

export default AgentOrdersCard;
