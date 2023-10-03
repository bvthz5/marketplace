import React from 'react';
import { Navigate, useRoutes } from 'react-router-dom';
// layouts
import DashboardLayout from './layouts/dashboard';
import Agentdashboard from './layouts/agentdashboard';
import ChangePasswordAgentLayout from './layouts/ChangePasswordAgent';
import SimpleLayout from './layouts/simple';
//

import LoginPage from './core/api/login/login-page/login';
import Page404 from './pages/Page404/Page404';

import ChangePassword from './pages/Change-Password/ChangePassword';
import UserList from './pages/Userlist/Userlist';
import ProductList from './pages/Product-List-Admin/ProductList';
import Adminproductdetailview from './pages/Product-List-Admin/AdminProductDetailView/AdminProductDetailView';
import Category from './pages/Product-Category/Category';
import SellerList from './pages/Sellerlist/Sellerlist';
import Home from './pages/Dashboard/Dashboard';
import Requests from './pages/Product-Requests/Requests';
import SellerRequest from './pages/Seller-Request/SellerRequest';
import MyProducts from './pages/MyProducts/MyProducts';
import Orders from './pages/Orders/Orderlist';
import VerifyToken from './core/api/login/forgot-pswd-tokenVerification/verify_token';
import VerifyTokenAgent from '../src/agentpages/forgot-pswd-tokenVerificationAgent/verify-TokenAgent';
import LoginLayout from './layouts/login/LoginLayout';
import LoginAgentLayout from './layouts/loginAgent/LoginAgentLayout';
import OrderDetail from './pages/Orders/Order-Detail.jsx/OrderDetail';
import DeliveryAgent from './pages/Delivery-Agent/DeliveryAgent';
import AgentLogin from './core/api/login-Agent/login-agent-page/AgentLogin';
import AgentChangePassword from './agentpages/ChangePassword/AgentChangePassword';
import AgentDashboard from './agentpages/Dashboard/AgentDashboard';
import AgentOrderList from './agentpages/AgentOrder/OrderList/AgentOrderList';
import { MyProfile } from './agentpages/MyProfile/MyProfile';
import AgentOrderDetailView from './agentpages/AgentOrder/OrderDetailView/AgentOrderDetailView';

// ----------------------------------------------------------------------

export default function Router() {
  return useRoutes([
    {
      path: '/dashboard',
      element: <DashboardLayout />,
      children: [
        { element: <Navigate to="/dashboard" />, index: true },
        { path: 'home', element: <Home /> },
        { path: 'user', element: <UserList /> },
        { path: 'products', element: <ProductList /> },
        { path: 'adminproductdetailview', element: <Adminproductdetailview /> },
        { path: 'category', element: <Category /> },
        { path: 'seller', element: <SellerList /> },
        { path: 'request', element: <Requests /> },
        { path: 'sellerrequest', element: <SellerRequest /> },
        { path: 'myproducts', element: <MyProducts /> },
        { path: 'orders', element: <Orders /> },
        { path: 'orderdetail', element: <OrderDetail /> },
        { path: 'changepassword', element: <ChangePassword /> },
        { path: 'deliveryagent', element: <DeliveryAgent /> },
      ],
    },
    {
      path: '/agentdashboard',
      element: <Agentdashboard />,
      children: [
        { element: <Navigate to="/agentdashboard" />, index: true },
        { path: 'changepassword', element: <AgentChangePassword changestyle={true} /> },
        { path: 'home', element: <AgentDashboard /> },
        { path: 'orderlist', element: <AgentOrderList /> },
        { path: 'orderDetailView', element: <AgentOrderDetailView /> },
        { path: 'profile', element: <MyProfile /> },
      ],
    },
    {
      element: <LoginLayout />,
      children: [{ path: '/login', element: <LoginPage /> }],
    },
    {
      element: <LoginAgentLayout />,
      children: [{ path: '/agent/login', element: <AgentLogin /> }],
    },
    {
      element: <ChangePasswordAgentLayout />,
      children: [{ path: '/changepassword', element: <AgentChangePassword /> }],
    },
    { path: '/forgot-password', element: <VerifyToken /> },
    { path: '/agent/forgot-password', element: <VerifyTokenAgent /> },
    {
      element: <SimpleLayout />,
      children: [
        { element: <Navigate to="/login" />, index: true },
        { path: '404', element: <Page404 /> },
        { path: '*', element: <Navigate to="/404" /> },
      ],
    },
    {
      path: '*',
      element: <Navigate to="/404" replace />,
    },
  ]);
}
