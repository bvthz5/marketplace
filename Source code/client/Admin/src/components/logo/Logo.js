import PropTypes from 'prop-types';
import React,{ forwardRef } from 'react';
import { Link as RouterLink } from 'react-router-dom';
// @mui

import { Box, Link } from '@mui/material';
import Logocss from './Logo.module.css';

// ----------------------------------------------------------------------

const Logo = forwardRef(({ disabledLink = false, sx, ...other }, ref) => {
  const logo = (
    <Box
      ref={ref}
      component="div"
      sx={{
        width: 40,
        height: 40,
        display: 'inline-flex',
        ...sx,
      }}
      {...other}
    >
      <div className={Logocss['container']}>
        <div className={Logocss['row']}>
          <div className={Logocss['col-md-12 text-center']}>
            <div className={Logocss['animate-charcter']}>CART_IN</div>
          </div>
        </div>
      </div>
    </Box>
  );

  if (disabledLink) {
    return (
      <>
        <div data-testid="logo">{logo} </div>
      </>
    );
  }

  return (
    <Link to="/dashboard/home" component={RouterLink} sx={{ display: 'contents' }} data-testid="logo">
      {logo}
    </Link>
  );
});

Logo.propTypes = {
  sx: PropTypes.object,
  disabledLink: PropTypes.bool,
};

export default Logo;
