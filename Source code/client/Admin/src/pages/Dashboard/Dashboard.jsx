import React, { useEffect, useState } from 'react';
import Charts from 'react-apexcharts';
import dashboardcss from './Dashboard.module.css';
import { categoriesGraph, salesCountGraph } from '../../core/api/apiService';
const Dashboard = () => {
  const [categories, setCategories] = useState([]);
  const [categoryData, setCategoryData] = useState([]);
  const [categoryCount, setCategoryCount] = useState([]);
  const [sales, setSales] = useState([]);
  const [salesData, setSalesData] = useState([]);
  const [salesCount, setSalesCount] = useState([]);

  useEffect(() => {
    document.title = 'CART_IN';
    getCategories();
    getSales();
  }, []);

  useEffect(() => {
    setCategoriesValue();
  }, [categories]);

  useEffect(() => {
    setSalesValues();
  }, [sales]);
//get categories //
  const getCategories = async () => {
    categoriesGraph()
      .then((response) => {
        setCategories(response?.data.data);
      })
      .catch((err) => console.log(err));
  };
//get sales count//
  const getSales = async () => {
    let date = new Date();
    let to = date.toISOString().split('T')[0];
    date.setDate(date.getDate() - 7);
    let from = date.toISOString().split('T')[0];
    const params = {
      From: from,
      To: to,
    };
    salesCountGraph(params)
      .then((response) => {
        setSales(response?.data.data);
      })
      .catch((err) => console.log(err));
  };

  const setCategoriesValue = () => {
    let category = [];
    let count = [];
    categories.forEach((data) => {
      category.push(data?.property);
      count.push(data?.count);
    });

    setCategoryData(category);
    setCategoryCount(count);
  };

  const setSalesValues = () => {
    let sale = [];
    let count = [];
    sales.forEach((data) => {
      sale.push(data.property);
      count.push(data.count);
    });
    setSalesData(sale);
    setSalesCount(count);
  };

  let categoryChart = {
    options: {
      chart: {
        id: 'category_wise_products_count',
      },
      xaxis: {
        categories: categoryData,
        hideOverlappingLabels: false,
        labels: {
          trim: true,
          rotate: -45,
          style: {
            fontSize: '14px',
          },
        },
    
      },
      theme: {
        palette: 'palette1',
        monochrome: {
          enabled: true,
          color: '#4B4453', // Set color to red
          shadeTo: 'light',
          shadeIntensity: 0.65,
        },
      },
      responsive: [{
        breakpoint: undefined,
        options: {},
    }],
    
    },
    series: [
      {
        name: 'No.of products',
        data: categoryCount,
      },
    ],
    fill: {
      colors: ['black'],
    },
  };

  let salesChart = {
    options: {
      chart: {
        id: 'sales_summary',
      },
      xaxis: {
        categories: salesData,
        hideOverlappingLabels: true,
        labels: {
          trim: true,
          rotate: -45,
          style: {
            fontSize: '14px',
          },

          stroke: {
            width: 1,
          },
        },
      },
      theme: {
        palette: 'palette1',
        monochrome: {
          enabled: true,
          color: '#4B4453', // Set color to red
          shadeTo: 'light',
          shadeIntensity: 0.65,
        },
      },
      responsive: [{
        breakpoint: 1200,
        options: {},
    }]
    },
    series: [
      {
        name: 'Total sales',
        data: salesCount,
      },
    ],
  };

  return (
    <>
      <div>
        <div className={dashboardcss['chartbox']}>
          <div className={dashboardcss['report']}>Product count based on category</div>
          <div className={dashboardcss['chartdiv']}>
            <Charts
              options={categoryChart.options}
              series={categoryChart.series}
              type="bar"
              height={300}
              width={1200}
              fontSize={8}
              
            />
          </div>
        </div>
        <div className={dashboardcss['chartbox2']}>
          <div className={dashboardcss['report']}>Sales report of last seven days</div>

          <div>
            <Charts
              options={salesChart.options}
              series={salesChart.series}
              type="line"
              height={300}
              width={1200}
              fontSize={8}
            />
          </div>
        </div>
      </div>
    </>
  );
};
export default Dashboard;
