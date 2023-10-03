import Button from '@mui/material/Button';
import React, { useCallback, useEffect, useState } from 'react';
import { topbarCategories } from '../../core/api/apiService';
import Topbarcss from './Topbar.module.css';

function Topbar({ setCategory, setStartRange, setEndRange, setSort, setStatus, clearFilters }) {
  const [categories, setCategories] = useState({});
  const [selectedCategory, setSelectedCategory] = useState('');
  const [priceStartRange, setSelectedStartRange] = useState('');
  const [priceEndRange, setSelectedEndRange] = useState('');
  const [sortValue, setSelectedSort] = useState('Newest to Oldest');
  const [selectedStatus, setSelectedStatus] = useState('');
  const [errors, setErrors] = useState('');

  useEffect(() => {
    getCategories();
  }, []);

  useEffect(() => {
    validatePrice();
  }, [priceStartRange, priceEndRange]);

  const validatePrice = () => {
    const limit = 500000;
    if (Number.parseFloat(priceStartRange) > limit || Number.parseFloat(priceEndRange) > limit) {
      setErrors('price limit is 500000');
    } else if (priceStartRange && priceEndRange) {
      if (Number.parseFloat(priceStartRange) > limit || Number.parseFloat(priceEndRange) > limit) {
        setErrors('price limit is 500000');
      }
      if (Number.parseFloat(priceStartRange) === Number.parseFloat(priceEndRange)) {
        setErrors('minimum price should be less than maximum price');
      }

      if (Number.parseFloat(priceEndRange) < Number.parseFloat(priceStartRange)) {
        setErrors('minimum price should be less than maximum price');
      }
      if (Number.parseFloat(priceEndRange) > Number.parseFloat(priceStartRange)) {
        setErrors('');
        setStartRange(priceStartRange);
        setEndRange(priceEndRange);
      }
    } else {
      setErrors('');
      setStartRange(priceStartRange);
      setEndRange(priceEndRange);
    }
  };

  const sortValues = [
    { key: 'Newest to Oldest', value: 'Newest to Oldest' },
    { key: 'Oldest to Newest', value: 'Oldest to Newest' },
    { key: 'Price:low to high', value: 'Price:low to high' },
    { key: 'Price:high to low', value: 'Price:high to low' },
  ];

  const statuses = [
    { key: '1', value: 'Active' },
    { key: '0', value: 'Rejected' },
    { key: '2', value: 'Pending' },
    { key: '4', value: 'Deleted' },
    { key: '3', value: 'Sold' },
    { key: '6', value: 'Order Processing' },
  ];
  //categories for filter//
  const getCategories = async () => {
    topbarCategories()
      .then((response) => {
        setCategories(response?.data.data);
      })
      .catch((err) => {
        console.log(err);
      });
  };

  return (
    <div className={Topbarcss.maindiv} data-testid="topbarpage">
      <div className={Topbarcss.filterdiv}>
        <div className={Topbarcss.category}>
          <select
          data-testid='category-sort-dropdown'
            name="categoryId"
            value={selectedCategory}
            className={Topbarcss.selectdiv}
            onChange={(e) => {
              setCategory(e.target.value);
              setSelectedCategory(e.target.value);
            }}
          >
            <option defaultChecked hidden className={Topbarcss.options}>
              Select category
            </option>
            {categories.length > 0 ? (
              categories.map((category) => {
                return (
                  <option key={category.categoryId} value={category.categoryId}>
                    {category.categoryName}
                  </option>
                );
              })
            ) : (
              <option disabled value="">
                Categories not found
              </option>
            )}
          </select>
        </div>
        <div className={Topbarcss.category}>
          <select
          data-testid="price-sort-dropdown"
            name="pricerange"
            value={sortValue}
            className={Topbarcss.selectdiv}
            onChange={(e) => {
              setSort(e.target.value);
              setSelectedSort(e.target.value);
            }}
          >
            &nbsp;
            {sortValues.length > 0 ? (
              sortValues.map((data) => {
                return (
                  <option key={data.key} value={data.key}>
                    {data.value}
                  </option>
                );
              })
            ) : (
              <option disabled value="">
                0
              </option>
            )}
          </select>
        </div>
      </div>

      {/* //// */}

      <div style={{minWidth:'330px'}}>
        <div className={Topbarcss.selectdiv}>
          <div>
            <input
            data-testid='start-price'
              onKeyDown={(evt) => ['e', 'E', '+', '-', '.'].includes(evt.key) && evt.preventDefault()}
              type="number"
              className={Topbarcss.selectmin}
              name="pricerange"
              placeholder="Min"
              min="0.00"
              max="500000.00"
              value={priceStartRange}
              onChange={(e) => {
                setSelectedStartRange(e.target.value);
              }}
            />
          </div>
          <span className={Topbarcss.spanto}> to</span>

          <div>
            <input
            data-testid='end-price'
              onKeyDown={(evt) => ['e', 'E', '+', '-', '.'].includes(evt.key) && evt.preventDefault()}
              type="number"
              min="0.00"
              max="500000.00"
              className={Topbarcss.selectmax}
              name="pricerange"
              placeholder="Max"
              value={priceEndRange}
              onChange={(e) => {
                setSelectedEndRange(e.target.value);
              }}
            />
          </div>
          
        </div>
        <div className={Topbarcss['error']}>{errors}</div>
      </div>
      {/* //// */}
      <div className={Topbarcss.filterdiv}>
        <div className={Topbarcss.category}>
          <select
          data-testid='status-filter-dropdown'
            name="status"
            value={selectedStatus}
            className={Topbarcss.selectdiv}
            onChange={(e) => {
              setStatus(e.target.value);
              setSelectedStatus(e.target.value);
            }}
          >
            <option defaultChecked value="">
              &nbsp; All
            </option>
            {statuses.length > 0 ? (
              statuses.map((data) => {
                return (
                  <option key={data.key} value={data.key}>
                    {data.value}
                  </option>
                );
              })
            ) : (
              <option disabled value="">
                0
              </option>
            )}
          </select>
        </div>
        <div>
          <Button
          data-testid='clearbutton'
            className={Topbarcss.clearbutton}
            onClick={useCallback(() => {
              clearFilters();
              setSelectedCategory('');
              setSelectedStartRange('');
              setSelectedEndRange('');
              setSelectedSort('');
              setSelectedStatus('');
              setErrors('');
            }, [])}
          >
            Clear All
          </Button>
        </div>
      </div>
    </div>
  );
}

export default Topbar;
