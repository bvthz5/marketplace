import React, { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import Box from '@mui/material/Box';
import Paper from '@mui/material/Paper';
import Grid from '@mui/material/Unstable_Grid2';
import { styled } from '@mui/material/styles';
import style from './MyProducts.module.css';
import MyImage from './../../assets/Image_not_available.png';
import { sellerProductCount, sellerProductList } from '../../core/api/apiService';
import { convertDate } from '../../utils/formatDate';
import ScrollToTopButton from '../../utils/ScrollToTopButton/ScrollToTopButton';
import AppWidgetSummary from '../../utils/AppWidgetSummary';

const initValue = [
  {
    property: 'INACTIVE',
    count: 0,
  },
  {
    property: 'ACTIVE',
    count: 0,
  },
  {
    property: 'SOLD',
    count: 0,
  },
  {
    property: 'PENDING',
    count: 0,
  },
];
const MyProducts = () => {
  const [desc] = useState(true);
  const [searchParams] = useSearchParams();
  const id = searchParams.get('id');
  const [products, setProducts] = useState([]);
  const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;
  const [apiCall, setApiCall] = useState(false);
  const [hasNext, sethasNext] = useState(false);
  const [statusCount, setStatusCount] = useState(initValue);

  useEffect(() => {
    console.log(initValue);
    console.log(statusCount);
  }, [statusCount]);

  useEffect(() => {
    document.title = 'Seller Products';
    getProducts();
    getproductCount();
  }, [id]);

  useEffect(() => {
    const handleScroll = (e) => {
      const scrollHeight = e.target.documentElement.scrollHeight;
      const currentHeight = e.target.documentElement.scrollTop + window.innerHeight;
      console.log(scrollHeight);
      console.log(currentHeight);
      if (currentHeight >= (scrollHeight * 39) / 40 && !apiCall) {
        if (hasNext && !apiCall) {
          console.log(apiCall);
          setApiCall(true);
          sethasNext(false);
          getProducts();
        }
      }
    };
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  });

  //getproduct//
  const getProducts = async () => {
    const params = {
      UserId: id,
      SortByDesc: desc,
      Offset: products.length !== 0 ? products[products.length - 1].productId : 0,
      pageSize: products.length > 20 ? 12 : 24,
    };
    sellerProductList(params)
      .then((response) => {
        let data = response?.data?.data.result;
        console.log(data);
        if (products.length === 0) {
          setProducts([...data]);
        } else {
          setProducts([...products, ...data]);
        }
        sethasNext(response.data.data.hasNext);
      })
      .catch((err) => console.log(err));
  };
  const getproductCount = async () => {
    sellerProductCount(id)
      .then((response) => {
        console.log(response.data);
        setStatusCount(response?.data.data);
      })
      .catch((err) => {
        console.log(err);
      });
  };

  const handleProductStatus = (status) => {
    if (status === 0) {
      return <span className={style['logged-rejected']}>Rejected</span>;
    }
    if (status === 1) {
      return <span className={style['logged-in']}>Approved</span>;
    }
    if (status === 2) {
      return <span className={style['logged-pending']}>Pending for approval</span>;
    }
    if (status === 3) {
      return <span className={style['logged-sold']}>Sold</span>;
    }
    if (status === 4) {
      return <span className={style['logged-out']}>Deleted</span>;
    }
    if (status === 5) {
      return <span className={style['logged-pending']}>draft</span>;
    }
    if (status === 6) {
      return <span className={style['logged-pending']}>Order Processing</span>;
    }
  };
  const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(3),
    textAlign: 'center',
    color: theme.palette.text.secondary,
  }));

  const detailViewNewTab = (productId) => () => {
    window.open(`/dashboard/adminproductdetailview/?id=${productId}`, '_blank').focus();
  };

  return (
    <>
      <div style={{ width: '100%' }} data-testid="myProductspage">
        <div className={style.counts}>
          <div>
            <Grid container className={style.cardcountmain}>
              <Grid>
                <AppWidgetSummary
                  data-testid="total"
                  className={style.cardcount}
                  title={'APPROVED'}
                  total={statusCount[1]?.count.toString()}
                  color="success"
                  icon={'ant-design:check-circle-outlined'}
                />
              </Grid>
              <Grid>
                <AppWidgetSummary
                  className={style.cardcount}
                  title={statusCount[3]?.property}
                  total={statusCount[3]?.count.toString()}
                  color="info"
                  icon={'ant-design:smile-outlined'}
                />
              </Grid>
              <Grid>
                <AppWidgetSummary
                  className={style.cardcount}
                  title={'PENDING'}
                  total={statusCount[2]?.count.toString()}
                  color="warning"
                  icon={'ant-design:clock-circle-outlined'}
                />
              </Grid>
            </Grid>
          </div>
        </div>
        <ScrollToTopButton />
        <div className={style.productcontaineralign}>
          <Box sx={{ marginTop: '15px' }}>
            <Grid container spacing={4} style={{ width: '100%', justifyContent: 'center' }}>
              {products.length > 0 ? (
                products.map((product) => {
                  return (
                    <Grid
                      lg={5}
                      sm={10}
                      style={{ width: '100%', marginLeft: '10px' }}
                      key={product.productId}
                      onClick={detailViewNewTab(product.productId)}
                      data-testid="gridclick"
                    >
                      <Item className={style['data-card']} key={product.productId}>
                        <div className={style['imagealign']}>
                          <img
                            src={product?.thumbnail ? `${baseImageUrl}${product.thumbnail}` : MyImage}
                            className={style['image']}
                            alt=""
                          />
                        </div>
                        <div className={style['datamain']}>
                          <div className={style['fordata']}>
                            <div className={style['h3design']}>{product.productName}</div>
                            <div className={style['categorydesign']}>{product.categoryName}</div>
                          </div>
                          <div className={style['forprice']}>
                            <span>&#8377;{product.price}</span>
                          </div>
                          <div className={style['categorydesign']}>
                            <span> {convertDate(product.createdDate)}</span>
                          </div>
                          <div className={style['aligninfo']}>{handleProductStatus(product.status)}</div>
                        </div>
                      </Item>
                    </Grid>
                  );
                })
              ) : (
                <div className={style['noprductsfound']}>No Products Found!</div>
              )}
            </Grid>
          </Box>
        </div>
      </div>
    </>
  );
};

export default MyProducts;
