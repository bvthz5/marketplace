import React from 'react';
import PropTypes from 'prop-types';
import { styled } from '@mui/material/styles';
import { Card, Typography } from '@mui/material';
import Iconify from '../../../components/iconify/Iconify';

const StyledCard = styled(Card)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
  padding: theme.spacing(3),
  boxShadow: '0px 4px 12px rgba(0, 0, 0, 0.2)',
  borderRadius: theme.spacing(1),
  textAlign: 'center',
  color: theme.palette.text.primary,
  backgroundColor: theme.palette.background.paper,
  transition: 'transform 0.3s, box-shadow 0.3s',
  '&:hover': {
    transform: 'scale(1.05)',
    boxShadow: '0px 4px 12px rgba(0, 0, 0, 0.3)',
  },
}));

const StyledIcon = styled('div')(({ theme, color }) => ({
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  width: theme.spacing(8),
  height: theme.spacing(8),
  borderRadius: '50%',
  backgroundColor: color,
  marginBottom: theme.spacing(3),
}));

const StyledTitle = styled(Typography)(({ colors }) => ({
  fontWeight: 'bold',
  color: colors,
  textTransform: 'uppercase',
}));

AppWidgetSummary.propTypes = {
  color: PropTypes.string,
  icon: PropTypes.string,
  title: PropTypes.string.isRequired,
  total: PropTypes.number.isRequired,
  colors: PropTypes.string,
};

export default function AppWidgetSummary({ title, total, icon, color = 'secondary', colors = 'primary' }) {
  

  return (
    <StyledCard>
      <StyledIcon color={color}>
        <Iconify icon={icon} width={30} height={30} color="#fff" />
      </StyledIcon>
      <Typography variant="h5" component="div">
        {total}
      </Typography>
      <StyledTitle colors={colors} variant="body2" sx={{ opacity: 1.72 }}>
        {title}
      </StyledTitle>
    </StyledCard>
  );
}

