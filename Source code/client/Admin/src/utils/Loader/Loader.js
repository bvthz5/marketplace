import React from 'react';
import Styles from './Loader.module.css';

const Loader = () => {
  return (
    <div className={Styles['apiloader']} data-testid="loaderpage">
      <div className={Styles['load-row']}>
        <span></span>
        <span></span>
        <span></span>
        <span></span>
      </div>
    </div>
  );
};

export default Loader;
