import React, { useCallback, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
// @mui
import { alpha } from '@mui/material/styles';
import { Box, Divider, Typography, MenuItem, Avatar, IconButton, Popover } from '@mui/material';
// mocks_

// ----------------------------------------------------------------------
import Swal from 'sweetalert2';
import { useDispatch, useSelector } from 'react-redux';
import { agentDetails, fetchAgentDetails } from '../../../agentpages/MyProfile/MyProfileSlice';
// ----------------------------------------------------------------------

function AgentAccountPopover() {
  const navigate = useNavigate();
  const [openModal, setOpenModal] = useState(null);

  const baseImageUrl = process.env.REACT_APP_PROFILEIMAGE_PATH;

  const handleOpen = useCallback((event) => {
    setOpenModal(event.currentTarget);
  }, []);

  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(fetchAgentDetails());
    console.log('jjj55555555555555');
  }, []);

  const handleClose = useCallback(() => {
    setOpenModal(null);
  }, []);
  const logOut = useCallback(() => {
    Swal.fire({
      title: 'Logout?',
      text: 'You will be logged out!',
      icon: 'info',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, Logout!',
    }).then((result) => {
      dispatch(fetchAgentDetails());
      if (result.isConfirmed) {
        localStorage.clear();
        navigate('/agent/login');
      }
    });
  }, []);

  const navigateMyProfile = useCallback(() => {
    handleClose();
    navigate('/agentdashboard/profile');
  }, []);

  const agent = useSelector(agentDetails);

  const logOutFunctionsAdmin = useCallback(() => {
    handleClose();
    logOut();
  }, []);
  return (
    <>
      <div>
        <IconButton
          onClick={handleOpen}
          sx={{
            p: 0,
            ...(openModal && {
              '&:before': {
                zIndex: 1,
                content: "''",
                width: '100%',
                height: '100%',
                borderRadius: '50%',
                position: 'absolute',
                bgcolor: (theme) => alpha(theme.palette.grey[900], 0.8),
              },
            }),
          }}
        >
          <Avatar
            alt={agent?.name}
            src={agent?.profilePic ? `${baseImageUrl}agent/profile-pic/${agent.profilePic}` : agent?.name}
          />
        </IconButton>

        <Popover
          open={Boolean(openModal)}
          anchorEl={openModal}
          onClose={handleClose}
          anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
          transformOrigin={{ vertical: 'top', horizontal: 'right' }}
          PaperProps={{
            sx: {
              p: 0,
              mt: 1.5,
              ml: 0.75,
              width: 180,
              '& .MuiMenuItem-root': {
                typography: 'body2',
                borderRadius: 0.75,
              },
            },
          }}
        >
          <Box sx={{ my: 1.5, px: 2.5 }}>
            <Typography variant="subtitle2" noWrap>
              Agent
            </Typography>
            <Typography variant="body2" sx={{ color: 'text.secondary' }} noWrap></Typography>
          </Box>
          <Divider sx={{ borderStyle: 'dashed' }} />
          <Divider sx={{ borderStyle: 'dashed' }} />
          <MenuItem onClick={navigateMyProfile} sx={{ m: 1 }}>
            My Profile
          </MenuItem>
          <MenuItem onClick={logOutFunctionsAdmin} sx={{ m: 1 }}>
            Logout
          </MenuItem>
        </Popover>
      </div>
    </>
  );
}
export default AgentAccountPopover;
