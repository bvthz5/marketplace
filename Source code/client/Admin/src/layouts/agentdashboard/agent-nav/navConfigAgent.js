// component
import SvgColor from '../../../components/svg-color';
import React from 'react';

// ----------------------------------------------------------------------

const icon = (name) => <SvgColor src={`/assets/icons/navbar/${name}.svg`} sx={{ width: 1, height: 1 }} />;
const navConfig = [
  {
    title: ' Dashboard',
    path: '/agentdashboard/home',
    icon: icon('ic_dashboardagent'),
  },
  {
    title: ' Order List',
    path: '/agentdashboard/orderlist',
    icon: icon('ic_order'),
  },
  {
    title: ' Change Password',
    path: '/agentdashboard/changepassword',
    icon: icon('ic_password'),
  },
];

export default navConfig;
