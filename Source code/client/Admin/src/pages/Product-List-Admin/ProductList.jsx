import React, { useCallback, useEffect, useState } from 'react';
import MyImage from './../../assets/Image_not_available.png';
import productliststyle from './ProductList.module.css';
import Loader from 'react-js-loader';
import Box from '@mui/material/Box';
import Paper from '@mui/material/Paper';
import Grid from '@mui/material/Unstable_Grid2';
import { styled } from '@mui/material/styles';
import PlacesAutocomplete from 'react-places-autocomplete';
import Topbar from './../Topbar/Topbar';
import { useLoadScript } from '@react-google-maps/api';
import ScrollToTopButton from './../../utils/ScrollToTopButton/ScrollToTopButton';
import { getAllProducts } from '../../core/api/apiService';
import {getRelativeDate} from '../../utils/formatDate'
import { handleProductStatus } from '../../utils/utils';

const baseImageUrl = process.env.REACT_APP_IMAGE_PATH;
const mapKey = process.env.REACT_APP_MAPS_API_KEY;
const libraries = ['places'];
const componentRestrictions = { country: 'in' };

const ProductList = () => {
  const { isLoaded } = useLoadScript({
    googleMapsApiKey: mapKey,
    libraries: libraries,
  });
  const [products, setProducts] = useState([]);
  const [hasNext, sethasNext] = useState(false);
  const [loaderState, setLoaderState] = useState(true);
  const [apiCall, setApiCall] = useState(false);

  // variables for filters
  const [searchValue, setSearchValue] = useState('');

  const [selectedCategory, setCategory] = useState(null);
  const [priceStartRange, setStartRange] = useState(0);
  const [priceEndRange, setEndRange] = useState(0);
  const [sortValue, setSort] = useState('CreatedDate');
  const [status, setStatus] = useState(null);
  const [desc, setDesc] = useState(true);

  const [address, setAddress] = useState('');
  const [location, setLocation] = useState('');
  const [toggle, setToggle] = useState(true);

  useEffect(() => {
    setFilter();
  }, [selectedCategory, searchValue, address, priceStartRange, priceEndRange, sortValue, status, desc,toggle]);

  useEffect(() => {
    if (location === '' && address === '') return;
    else if (location === '') {
      setProducts([]);
      setLocation('');
      setAddress('');
    }
  }, [location]);

  useEffect(() => {
    localStorage.setItem('pillerValue', 'productlist');
    document.title = 'Product List';
    const handleScroll = (e) => {
      const scrollHeight = e.target.documentElement.scrollHeight;
      const currentHeight = e.target.documentElement.scrollTop + window.innerHeight;
      if (currentHeight >= (scrollHeight * 39) / 40 && !apiCall) {
        if (hasNext && !apiCall) {
          console.log(apiCall);
          setApiCall(true);
          sethasNext(false);
          setFilter();
        }
      }
    };
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  });

  // function for fetching products based on the applied filters
  const setFilter = async () => {
    const params = {
      Offset: products.length !== 0 ? products[products.length - 1].productId : 0,
      PageSize: products.length > 20 ? 12 : 24,
      CategoryId: selectedCategory ? selectedCategory : '',
      Search: searchValue,
      Location: address,
      StartPrice: priceStartRange ? priceStartRange : 0,
      EndPrice: priceEndRange ? priceEndRange : 0,
      SortBy: sortValue,
      Status: status ? status : '',
      SortByDesc: desc,
    };
    setLoaderState(true);
    sethasNext(false);
    getAllProducts(params)
      .then((response) => {
        let data = response?.data?.data.result;
        console.log(data);
        if (products.length === 0) {
          setProducts([...data], executeScroll());
        } else {
          setProducts([...products, ...data]);
        }
        sethasNext(response?.data.data.hasNext);
        setLoaderState(false);
        setApiCall(false);
      })
      .catch((err) => console.log(err));
    setApiCall(false);
  };
  const executeScroll = async () => {
    window.scrollTo({ top: 5, behavior: 'smooth' });
  };

  const handleSelect = useCallback((value) => {
    let selectedAddress = value.split(',').splice(0, 2).join();
    setLocation(selectedAddress);
    if (address === selectedAddress) {
      return;
    }
    console.log(address === selectedAddress);
    setProducts([]);
    setAddress(selectedAddress);
  }, []);

  // function handling search bar
  const handleProductSearch = async (data) => {
    setProducts([]);
    setSearchValue(data);
  };

  const handleCategory = useCallback((category) => {
    setCategory(category);
    setProducts([]);
  }, []);

  const handleStartRange = useCallback((range) => {
    if (priceStartRange === range) return;
    setProducts([]);
    setStartRange(range);
  }, [priceStartRange]);
  const handleEndRange = useCallback((endrange) => {
    if (priceEndRange === endrange) return;
    setProducts([]);
    setEndRange(endrange);
  }, [priceEndRange]);
  const handleSort = useCallback((sort) => {
    if (sort === 'Newest to Oldest') {
      setDesc(true);
      setSort('CreatedDate');
      setProducts([]);
    }
    if (sort === 'Oldest to Newest') {
      console.log(desc, 'bef');
      setDesc(false);
      console.log('carr ssssssssss');
      setSort('CreatedDate');
      setProducts([]);
      console.log(desc, 'aft');
    }
    if (sort === 'Price:low to high') {
      setDesc(false);
      setSort('Price');
      setProducts([]);
    }
    if (sort === 'Price:high to low') {
      setDesc(true);
      setSort('Price');
      setProducts([]);
    }
  }, []);
  const handleStatus = useCallback((status) => {
    setProducts([]);
    setStatus(status);
  }, []);
  const clearFilters = useCallback(() => {
    setProducts([]);
    setSearchValue('');
    setCategory(null);
    setStartRange(0);
    setEndRange(0);
    setSort('CreatedDate');
    setAddress('');
    setLocation('');
    setStatus(null);
    setDesc(true);
    setFilter();
    setToggle(toggle ? false : true);
  }, []);

  const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(3),
    textAlign: 'center',
    color: theme.palette.text.secondary,
  }));


  const detailViewsNewTab = (productId) => () => {
    window.open(`/dashboard/adminproductdetailview/?id=${productId}`, '_blank');
  };

  return (
    <>
      <div data-testid="productlistpage">
        <ScrollToTopButton />
        <div className={productliststyle.Title}></div>
        <div className={productliststyle.Maindiv}>
          <div className={productliststyle.box1}>
            <div className={productliststyle.box2}>
              <div>
                <div className={productliststyle['twobar']}>
                  <div className={productliststyle['twobar2']}>
                    <div className={productliststyle['select']}>
                      {isLoaded && (
                        <PlacesAutocomplete
                          searchOptions={{ componentRestrictions }}
                          highlightFirstSuggestion
                          value={location}
                          onChange={setLocation}
                          onSelect={handleSelect}
                        >
                          {({ getInputProps, suggestions, getSuggestionItemProps, loading }) => (
                            <div key={suggestions.description} style={{ width: '20px' }}>
                              <input
                                {...getInputProps({
                                  placeholder: 'Search Places ...',
                                  className: productliststyle.locationsearchinput,
                                })}
                              />
                              <div className={productliststyle['locationdropdown']}>
                                {loading && <div>Loading...</div>}
                                {suggestions.map((suggestion) => {
                                  // inline style for demonstration purpose
                                  const style = suggestion.active
                                    ? {
                                        backgroundColor: '#d3d3d3',
                                        cursor: 'pointer',
                                      }
                                    : {
                                        backgroundColor: '#ffffff',
                                        cursor: 'pointer',
                                      };
                                  return (
                                    <div
                                      className={productliststyle['input-suggestion']}
                                      key={suggestion.placeId}
                                      {...getSuggestionItemProps(suggestion, {
                                        style,
                                      })}
                                    >
                                      <i className="material-icons">location_on </i>
                                      <span>{suggestion.description.split(',').splice(0, 2).join()}</span>
                                    </div>
                                  );
                                })}
                              </div>
                            </div>
                          )}
                        </PlacesAutocomplete>
                      )}
                    </div>

                    {/* /// */}
                    <div>
                      <input
                      data-testid='search-input'
                        className={productliststyle['search']}
                        type="search"
                        maxLength={255}
                        value={searchValue}
                        placeholder="Find products here..."
                        onChange={(e) => {
                          handleProductSearch(e.target.value);
                        }}
                      />
                    </div>
                  </div>
                </div>
              </div>
              <div className={productliststyle.topbar}>
                
                  <Topbar
                    setCategory={handleCategory}
                    setStartRange={handleStartRange}
                    setEndRange={handleEndRange}
                    setSort={handleSort}
                    setStatus={handleStatus}
                    clearFilters={clearFilters}
                  />
               
              </div>
              <Box sx={{ marginTop: '10px' }}>
                <Grid container spacing={2} className={productliststyle['gridcss']}>
                  {products.length > 0 ? (
                    products.map((product) => {
                      return (
                        <Grid
                          lg={5}
                          sm={10}
                          className={productliststyle['cardwidth']}
                          key={product.productId}
                          onClick={detailViewsNewTab(product?.productId)}
                        >
                          <Item
                            className={productliststyle['data-card']}
                            style={{
                              height: '280px',
                              cursor: 'pointer',
                              padding: '10px',
                            }}
                          >
                            <div className={productliststyle['datas']}   data-testid="product-card">
                              <img
                                src={product?.thumbnail ? `${baseImageUrl}${product.thumbnail}` : MyImage}
                                className={productliststyle['image']}
                                alt=""
                              />
                              <div className={productliststyle['fontsize']}>
                                <h2 style={{ margin: '0', padding: '0' }}>&#8377; {product?.price}</h2>
                                <p className={productliststyle.productname}>{product?.productName}</p>
                              </div>
                              {handleProductStatus(product?.status)}
                              <p className={productliststyle['dateshow']}>{getRelativeDate(product?.createdDate)}</p>
                            </div>
                          </Item>
                        </Grid>
                      );
                    })
                  ) : (
                    <div className={productliststyle['item']} key={'1'}>
                      {!loaderState && <div> No Match Found! </div>}
                    </div>
                  )}
                </Grid>
                {loaderState ? <Loader type="spinner-cub" bgColor={'#008296'} color={'black'} size={50} /> : ''}
              </Box>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};
export default ProductList;
