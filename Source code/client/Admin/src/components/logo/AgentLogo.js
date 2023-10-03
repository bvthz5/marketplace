import PropTypes from 'prop-types';
import React,{ forwardRef } from 'react';
import { Link as RouterLink } from 'react-router-dom';
// @mui

import { Box, Link } from '@mui/material';
import AgentLogocss from './AgentLogo.module.css';

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
      <div className={AgentLogocss['animate-charcter']}>CART_IN</div>
    </Box>
  );

  if (disabledLink) {
    return <>
    <div data-testid='agentlogo'>{logo}</div></>;
  }

  return (
    <Link to="/agentdashboard/home" component={RouterLink} sx={{ display: 'contents' }}>
      {logo}
    </Link>
  );
});

Logo.propTypes = {
  sx: PropTypes.object,
  disabledLink: PropTypes.bool,
};

export default Logo;
