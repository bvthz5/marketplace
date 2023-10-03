
import React from 'react';
// component
import SvgColor from '../../../components/svg-color';

// ----------------------------------------------------------------------

const icon = (name) => <SvgColor src={`/assets/icons/navbar/${name}.svg`} sx={{ width: 1, height: 1 }} />;
const navConfig = [
  {
    title: 'dashboard',
    path: '/dashboard/home',
    icon: icon('ic_analytics'),
  },
  {
    title: 'Product',
    path: '/dashboard/products',
    icon: icon('ic_cart'),
  },
  {
    title: 'User',
    path: '/dashboard/user',
    icon: icon('ic_user'),
  },
  {
    title: 'Seller',
    path: '/dashboard/seller',
    icon: icon('ic_seller'),
  },
  {
    title: 'Orders',
    path: '/dashboard/orders',
    icon: icon('ic_orders'),
  },
  {
    title: 'Product Requesition',
    path: '/dashboard/request',
    icon: icon('ic_request'),
  },
  {
    title: 'Seller Requesition',
    path: '/dashboard/sellerrequest',
    icon: icon('ic_sell'),
  },
  {
    title: 'Category',
    path: '/dashboard/category',
    icon: icon('ic_category'),
  },
  {
    title: 'Delivery Agent',
    path: '/dashboard/deliveryagent',
    icon: icon('ic_truck'),
  },
  {
    title: 'Change Password',
    path: '/dashboard/changepassword',
    icon: icon('ic_password'),
  },
];



export default navConfig;
