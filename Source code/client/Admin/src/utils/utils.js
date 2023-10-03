import React from 'react';
import style from '../pages/Product-List-Admin/AdminProductDetailView/AdminProductDetailView.module.css';
import productliststyle from '../pages/Product-List-Admin/ProductList.module.css';
import deliveryagentcss from '../pages/Delivery-Agent/DeliveryAgent.module.css';
import { orderStatus } from './Enums';

export const handleStatus = (status) => {
  if (status === 0) {
    return <span className={style['logged-rejected']}>Rejected</span>;
  }
  if (status === 1) {
    return <span className={style['logged-in']}>Approved</span>;
  }
  if (status === 2) {
    return <span className={style['warning']}>Pending</span>;
  }
  if (status === 3) {
    return <span className={style['logged-sold']}>Sold</span>;
  }
  if (status === 4) {
    return <span className={style['logged-out']}>Deleted</span>;
  }
  if (status === 6) {
    return <span className={style['warning']}>Order Processing</span>;
  }
};

export const handleItemStatus = (status) => {
  switch (status) {
    case orderStatus.CREATED:
      return 'Order created';

    case orderStatus.CONFIRMED:
      return 'Order Confirmed';

    case orderStatus.INTRANSIT:
      return 'In Transit';

    case orderStatus.CANCELLED:
      return 'Order Cancelled';

    case orderStatus.WAITING_FOR_PICKUP:
      return 'Waiting For Pickup';

    case orderStatus.OUTFORDELIVERY:
      return 'Out For Delivery';

    case orderStatus.DELIVERED:
      return 'Delivered';

    default:
      return '';
  }
};

export const PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?#&]{8,16}$/;

export const handleProductStatus = (status) => {
  if (status === 0) {
    return <span className={productliststyle['logged-rejected']}>Rejected</span>;
  }
  if (status === 1) {
    return <span className={productliststyle['logged-in']}>Approved</span>;
  }
  if (status === 2) {
    return <span className={productliststyle['logged-pending']}>Pending</span>;
  }
  if (status === 3) {
    return <span className={productliststyle['logged-sold']}>Sold</span>;
  }
  if (status === 4) {
    return <span className={productliststyle['logged-out']}>Deleted</span>;
  }
  if (status === 6) {
    return <span className={productliststyle['logged-orderprocessing']}>Order Processing</span>;
  }
};

export const statusBadge = (status) => {
  switch (status) {
    case 3:
      return (
        <div
          className={deliveryagentcss['statuscss']}
          style={{ color: 'red', backgroundColor: 'rgba(255, 86, 48, 0.16)' }}
        >
          Deleted
        </div>
      );
    case 2:
      return (
        <div className={deliveryagentcss['statuscss']} style={{ color: 'orange', backgroundColor: '#e9981629' }}>
          Blocked
        </div>
      );
    case 1:
      return (
        <div
          className={deliveryagentcss['statuscss']}
          style={{ color: 'green', backgroundColor: 'rgba(7, 92, 49, 0.16)' }}
        >
          Active
        </div>
      );
    case 0:
    default:
      return (
        <div className={deliveryagentcss['statuscss']} style={{ color: '#4a63ee', backgroundColor: '#0f1ef129' }}>
          Inactive
        </div>
      );
  }
};

export function convertTime(time) {
  const date = new Date(time);
  const hours = date.getHours();
  const minutes = date.getMinutes();
  let period = "AM";

  let hour = hours;
  if (hour >= 12) {
    period = "PM";
    hour = hour === 12 ? 12 : hour - 12;
  } else if (hour === 0) {
    hour = 12;
  }

  return `${hour}:${formatTwoDigits(minutes)} ${period}`;
}

function formatTwoDigits(value) {
  return value.toString().padStart(2, "0");
}