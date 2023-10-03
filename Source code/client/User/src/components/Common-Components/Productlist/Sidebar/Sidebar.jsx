import Button from "@mui/material/Button";
import styles from "./Sidebar.module.css";
import React, { useCallback, useEffect, useState } from "react";
import { getAllCategories } from "../../../../core/Api/apiService";
import { productlistParams, sortValues } from "../../../Utils/Data/Data";
import { useDispatch, useSelector } from "react-redux";
import {
  clearAllFilters,
  selectAllFilters,
  setFilters,
} from "../ProductList/ProductSlices/filterSlice";
import { clearAllProducts } from "../ProductList/ProductSlices/productSlice";
import { hasDataChanged } from "../../../Utils/Utils";
import { Box, Divider, Slider } from "@mui/material";

function Sidebar() {
  const dispatch = useDispatch();
  const filters = useSelector(selectAllFilters);
  const [categories, setCategories] = useState({});
  const [priceRange, setPriceRange] = React.useState([
    filters.StartPrice,
    filters.EndPrice,
  ]);

  const minDistance = 10000;

  // sortable  values for sort filters
  useEffect(() => {
    getCategories();
  }, []);

  useEffect(() => {
    setPriceRange([filters.StartPrice, filters.EndPrice]);
  }, [filters]);

  // function for fetching all available categories to list on brand filter
  const getCategories = () => {
    getAllCategories()
      .then((response) => {
        setCategories(response?.data.data);
      })
      .catch((err) => console.log(err));
  };

  const applyPriceFilter = useCallback(() => {
    let min = Number(priceRange[0]);
    let max = Number(priceRange[1]);

    if (min === filters.StartPrice && max === filters.EndPrice) return;
    dispatch(clearAllProducts());
    dispatch(
      setFilters({
        StartPrice: min,
        EndPrice: max,
      })
    );
  },[dispatch, filters.EndPrice, filters.StartPrice, priceRange]);

  const handleSliderChange = useCallback((event, newValue, activeThumb) => {
    if (!Array.isArray(newValue)) {
      return;
    }

    if (newValue[1] - newValue[0] < minDistance) {
      if (activeThumb === 0) {
        const clamped = Math.min(newValue[0], 500000 - minDistance);
        setPriceRange([clamped, clamped + minDistance]);
      } else {
        const clamped = Math.max(newValue[1], minDistance);
        setPriceRange([clamped - minDistance, clamped]);
      }
    } else {
      setPriceRange(newValue);
    }
  },[]);

  const sortOptions = {
    "Newest to Oldest": ["CreatedDate", true],
    "Oldest to Newest": ["CreatedDate", false],
    "Price:low to high": ["Price", false],
    "Price:high to low": ["Price", true],
  };
  const handleSort = (sort) => {
    const [sortBy, sortByDesc] = sortOptions[sort] || [];
    if (sortBy) {
      dispatch(clearAllProducts());
      dispatch(setFilters({ SortBy: sortBy, SortByDesc: sortByDesc }));
    }
  };

  const sortByValue = () => {
    const targetValue = [filters.SortBy, filters.SortByDesc];
    const targetKey = Object.entries(sortOptions).find(
      ([, value]) => value.toString() === targetValue.toString()
    )?.[0];
    return targetKey;
  };

  const handleClearFilters = useCallback(() => {
    let dataChanged = hasDataChanged(filters, productlistParams);
    if (!dataChanged) return;
    dispatch(clearAllProducts());
    dispatch(clearAllFilters());
  },[dispatch, filters]);

  return (
    <div data-testid="sidebar" className={styles["sidebarContainer"]}>
      <Divider>Choose Category</Divider>
      <div className={styles["categoryDiv"]}>
        <select
          data-testid="category-sort-dropdown"
          className={styles["categorySelect"]}
          name="categoryId"
          value={filters.CategoryId}
          onChange={(e) => {
            dispatch(clearAllProducts());
            dispatch(setFilters({ CategoryId: e.target.value }));
          }}
        >
          <option defaultChecked hidden value="">
            select category
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
              categories not found
            </option>
          )}
        </select>
      </div>

      <Divider>Sort By</Divider>
      <div className={styles["categoryDiv"]}>
        <select
          data-testid="price-sort-dropdown"
          className={styles["categorySelect"]}
          name="pricerange"
          value={sortByValue()}
          onChange={(e) => {
            handleSort(e.target.value);
          }}
        >
          {sortValues.map((data) => {
            return (
              <option key={data.key} value={data.key}>
                {data.value}
              </option>
            );
          })}
        </select>
      </div>

      <Divider>Choose Price Range</Divider>
      <div className={styles["priceFilter-div"]}>
        <div className={styles.sliderDiv}>
          <Box sx={{ width: 250 }}>
            <Slider
              value={priceRange}
              onChange={handleSliderChange}
              valueLabelDisplay="auto"
              disableSwap
              step={minDistance}
              min={0}
              max={500000}
            />
          </Box>
        </div>

        <div className={styles["applyBtn-div"]}>
          <div>{`${priceRange[0]} - ${priceRange[1]}`}</div>
          <Button data-testid="apply-button" onClick={applyPriceFilter}>
            Apply
          </Button>
        </div>
      </div>
      <div className={styles["clearBtn-div"]}>
        <Divider />
        <Button data-testid="clear-all-button" onClick={handleClearFilters}>
          Clear All
        </Button>
      </div>
    </div>
  );
}

export default Sidebar;
