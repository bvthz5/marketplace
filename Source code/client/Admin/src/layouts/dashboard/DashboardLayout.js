import React,{ useCallback, useState } from 'react';
import { Outlet } from 'react-router-dom';

// @mui
import { styled } from '@mui/material/styles';
//
import Header from './header';
import Nav from './nav';
import RouteProtection from '../../../src/RouteProtection';

// ----------------------------------------------------------------------

const APP_BAR_MOBILE = 64;
const APP_BAR_DESKTOP = 92;

const StyledRoot = styled('div')({
  display: 'flex',
  minHeight: '100vh',
  overflow: 'hidden',
  backgroundColor: ' #f1f3f6',
});

const Main = styled('div')(({ theme }) => ({
  flexGrow: 1,
  overflow: 'auto',
  minHeight: '100%',
  paddingTop: APP_BAR_MOBILE + 24,
  paddingBottom: theme.spacing(10),
  [theme.breakpoints.up('lg')]: {
    paddingTop: APP_BAR_DESKTOP + 24,
    paddingLeft: theme.spacing(2),
    paddingRight: theme.spacing(2),
  },
}));

// ----------------------------------------------------------------------

export default function DashboardLayout() {
  const [open, setOpen] = useState(false);
  const closeHeader = useCallback(() => {
    setOpen(true);
  },[]);

  const openNav = useCallback(() => {
    setOpen(false);
  },[]);

  return (
    <RouteProtection>
      <StyledRoot>
        <Header onOpenNav={closeHeader} />
        <Nav openNav={open} onCloseNav={openNav} />
        <Main>
          <Outlet />
        </Main>
      </StyledRoot>
    </RouteProtection>
  );
}
