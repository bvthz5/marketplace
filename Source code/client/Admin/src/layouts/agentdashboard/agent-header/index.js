import React from 'react';
import PropTypes from 'prop-types';
// @mui
import { styled } from '@mui/material/styles';
import { Box, Stack, AppBar, Toolbar, IconButton } from '@mui/material';
// utils
import { bgBlur } from '../../../utils/cssStyles';
// components
import Iconify from '../../../components/iconify/Iconify';
//

import AgentAccountPopover from './AgentAccountPopover';
import { useLocation } from 'react-router-dom';

// ----------------------------------------------------------------------

const NAV_WIDTHAGENT = 280;

const HEADER_MOBILEAGENT = 64;

const HEADER_DESKTOPAGENT = 72;

const StyledRootAgent = styled(AppBar)(({ theme }) => ({
  ...bgBlur({ color: theme.palette.background.default }),
  boxShadow: 'none',
  [theme.breakpoints.up('lg')]: {
    width: `calc(100% - ${NAV_WIDTHAGENT + 1}px)`,
  },
}));

const StyledToolbarAgent = styled(Toolbar)(({ theme }) => ({
  minHeight: HEADER_MOBILEAGENT,
  [theme.breakpoints.up('lg')]: {
    minHeight: HEADER_DESKTOPAGENT,
    padding: theme.spacing(0, 5),
  },
}));

// ----------------------------------------------------------------------

Header.propTypes = {
  onOpenNav: PropTypes.func,
};

export default function Header({ onOpenNav }) {
  const location = useLocation();

  const getNavTitle = () => {
    const mapping = [
      {
        pathname: '/agentdashboard/profile',
        title: 'My Profile',
      },
      {
        pathname: '/agentdashboard/home',
        title: 'Dashboard',
      },
      {
        pathname: '/agentdashboard/orderlist',
        title: 'Order List',
      },
      {
        pathname: '/agentdashboard/changepassword',
        title: 'Change Password',
      },
    ];

    return mapping.find((obj) => obj.pathname === location.pathname)?.title;
  };

  const navTitle = getNavTitle();

  return (
    <div>
      <StyledRootAgent>
        <StyledToolbarAgent>
          <IconButton
            onClick={onOpenNav}
            sx={{
              mr: 1,
              color: 'text.primary',
              display: { lg: 'none' },
            }}
          >
            <Iconify icon="eva:menu-2-fill" />
          </IconButton>

          <Box sx={{ flexGrow: 1, color: 'darkblue', fontSize: 20 }}>{navTitle}</Box>
          <Stack
            direction="row"
            alignItems="center"
            spacing={{
              xs: 0.5,
              sm: 1,
            }}
          >
            <AgentAccountPopover />
          </Stack>
        </StyledToolbarAgent>
      </StyledRootAgent>
    </div>
  );
}
