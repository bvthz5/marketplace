import React from "react";
import Register from "..//register/register";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import Login from "../login/login-page/login";
import Verifyemail from "../register/Verify-email";
import VerifyToken from "../login/forgot-pswd-tokenVerification/verify_token";
import Addproduct from "../../components/Common-Components/AddProduct/Addproduct";
import UserProfile from "../../components/Common-Components/User-Profile/User-Profile/userProfile";
import ProductList from "../../components/Common-Components/Productlist/ProductList/Productlist";
import ProductImage from "../../components/Common-Components/AddProduct/ProductImgUpload";
import MyProducts from "../../components/Common-Components/MyProducts/MyProducts";
import ProductlistDetailView from "../../components/Common-Components/Productlist/ProductDetailView/ProductlistDetailView";
import EditProduct from "../../components/Common-Components/EditProduct/EditProduct";
import WishList from "../../components/Common-Components/WishList/WishList";
import MyCart from "../../components/Common-Components/MyCart/MyCart";
import EditProductImage from "../../components/Common-Components/EditProduct/EditProductImage";
import ChangePassword from "../../components/Common-Components/User-Profile/Change-Password/ChangePassword";
import EditProfile from "../../components/Common-Components/User-Profile/Edit-Profile/EditProfile";
import NotFound from "../404-Page/NotFound";
import ProtectedUsersRoutes from "../Routes/ProtectedUsersRoutes";
import Home from "../../components/Common-Components/Home/Home";
import RazorPay from "../../components/Common-Components/Razor-Pay/RazorPay";
import OrderSummary from "../../components/Common-Components/MyOrders/CheckOut/Check-Out/Checkout";
import OrderHistory from "../../components/Common-Components/MyOrders/Order-History/OrderHistory";
import OrderDetailView from "../../components/Common-Components/MyOrders/Order-History/Order-Detail-View/OrderDetailView";
import FilterMobile from "../../components/Common-Components/Productlist/Filter-Mobile/FilterMobile";
import LoginProtection from "./LoginProtection";

const RouterFile = () => {
  return (
    <Router>
      <Routes>
        {/* common routes */} {/* accesible after login*/}
        <Route element={<ProtectedUsersRoutes />}>
          <Route path="/addproduct" element={<Addproduct />} />
          <Route path="/productimage" element={<ProductImage />} />
          <Route path="/editproduct" element={<EditProduct />} />
          <Route path="/editproductimage" element={<EditProductImage />} />
          <Route path="/myproducts" element={<MyProducts />} />
          <Route path="/wishlist" element={<WishList />} />
          <Route path="/cart" element={<MyCart />} />
          <Route path="/profile" element={<UserProfile />} />
          <Route path="/editprofile" element={<EditProfile />} />
          <Route path="/changepassword" element={<ChangePassword />} />
          <Route path="/home" element={<ProductList />} />
          <Route path="/RazorPay" element={<RazorPay />} />
          <Route path="/summary" element={<OrderSummary />} />
          <Route path="/orders" element={<OrderHistory />} />
          <Route path="/orderdetail" element={<OrderDetailView />} />
        </Route>
        {/* common routes */} {/* accesible without login */}
        <Route element={<LoginProtection />}>
          <Route path="/login" element={<Login />} />
        </Route>
        <Route path="/register" element={<Register />} />
        <Route path="/verify" element={<Verifyemail />} />
        <Route path="/forgot-password" element={<VerifyToken />} />
        <Route path="/" exact element={<Home />} />
        <Route path="/productdetail" element={<ProductlistDetailView />} />
        <Route path="/header" element={<FilterMobile />} />
        {/* 404 page */}
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Router>
  );
};

export default RouterFile;
