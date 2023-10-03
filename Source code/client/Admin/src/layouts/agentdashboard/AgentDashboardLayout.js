import React, { useCallback, useState } from 'react';
import { Outlet } from 'react-router-dom';
// @mui
import { styled } from '@mui/material/styles';
//
import AgentHeader from './agent-header';
import AgentNav from './agent-nav';
import RouteProtectionAgent from '../../RouterProtectionAgent';

// ----------------------------------------------------------------------

const APP_BAR_MOBILE = 64;
const APP_BAR_DESKTOP = 92;

const StyledRoot = styled('div')({
  display: 'flex',
  minHeight: '100%',
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

export default function AgentDashboardLayout() {
  const [open, setOpen] = useState(false);
  const closeHeader = useCallback(() => {
    setOpen(true);
  }, []);

  const openNav = useCallback(() => {
    setOpen(false);
  }, []);

  return (
    <RouteProtectionAgent>
      <StyledRoot>
        <AgentHeader onOpenNav={closeHeader} />
        <AgentNav openNav={open} onCloseNav={openNav} />
        <Main style={{ minHeight: '100vh', paddingBottom: '40px' }}>
          <Outlet />
        </Main>
      </StyledRoot>
    </RouteProtectionAgent>
  );
}
