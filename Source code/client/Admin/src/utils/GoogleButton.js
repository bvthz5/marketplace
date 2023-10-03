import React from 'react';
import { GoogleLogin } from '@react-oauth/google';

export const GoogleButton = ({ response }) => {
  return (
    <>
     
        <GoogleLogin onSuccess={response} onError={console.log('Login Failed')} useOneTap />
   
    </>
  );
};
