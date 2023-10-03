import React, { useEffect, useState } from 'react';
import { agentOrdersCount } from '../../core/api/apiService';
import ScrollToTopButton from '../../utils/ScrollToTopButton/ScrollToTopButton';
import AppWidgetSummary from './Graph/AppWidgetSummary';
import Grid from '@mui/material/Unstable_Grid2';
import style from './AgentDashboard.module.css';
import CountUp from 'react-countup';

const initValue = [
  {
    property: 'Delivered Orders',
    count: 0,
  },
  {
    property: 'Canceled Orders',
    count: 0,
  },
  {
    property: 'Assigned Orders',
    count: 0,
  },
];

const AgentDashboard = () => {
  const [orderCount, setOrderCount] = useState(initValue);


  useEffect(() => {
    document.title = 'Dashboard';
    getordersCount();
  }, []);

  const getordersCount = async () => {
    agentOrdersCount()
      .then((response) => {
        console.log(response.data);
        setOrderCount(response?.data.data);
      })
      .catch((err) => {
        console.log(err);
      });
  };
  return (
    <>
      <div style={{ width: '100%' }} data-testid="myorderscount">
        <div className={style.counts}>
          <div>
          <Grid container className={style.cardcountmain}>
              <Grid>
                <AppWidgetSummary
                  className={`${style.cardcount} ${style.totalOrders} ` }
                  title={'Delivered Orders'}
                  
                  total={<CountUp end={orderCount[0]?.count} />}
                  color="green"
                  icon={'clarity:assign-user-solid'}
                  colors='rgb(83, 207, 74);'
                 
                />
              </Grid>      
              <Grid>
                <AppWidgetSummary
                  data-testid="total"
                  className={`${style.cardcount} ${style.assignedOrders}`}
                  title={'Assigned Orders'}
                  total={<CountUp end={orderCount[2]?.count} />}
                  color="blue"
                  icon={'ant-design:smile-outlined'}
                  colors='rgb(88, 88, 224);'
                
                />
              </Grid>
              <Grid>
                <AppWidgetSummary
                  className={`${style.cardcount} ${style.canceledOrders}`}
                  title={'Cancelled Orders'}
                  total={<CountUp end={orderCount[1]?.count} />}
                  color="orange"
                  icon={'mdi:book-cancel-outline'}
                  colors=' rgb(224, 88, 88);'
                />
              </Grid>
            </Grid>
          </div>
        </div>
        <ScrollToTopButton />
      </div>
    </>
  );
};

export default AgentDashboard;
