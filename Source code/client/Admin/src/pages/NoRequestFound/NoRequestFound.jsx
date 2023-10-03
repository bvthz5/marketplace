import React from 'react';
import norequestcss from './NoRequestFound.module.css';
function NoRequestFound() {
  return (
    <div className={norequestcss['main']} data-testid="norequestfoundpage">
      <div className={norequestcss['second']}>
        <h1 style={{ fontSize: '50px' }}> No </h1>
        <span style={{ marginTop: '-25px' }}>Requests</span>
        <br />
      </div>
      <span style={{ marginTop: '-105px', marginRight: '-70px' }}>Found!!!!</span>
    </div>
  );
}

export default NoRequestFound;
