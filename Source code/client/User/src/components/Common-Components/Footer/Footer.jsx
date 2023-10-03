import React from "react";

import Footercss from "./Footer.module.css";

function Footer() {
  return (
    <div data-testid="footer" className={Footercss.footerParentDiv}>
      <div className={Footercss.footer}>
        <p>Other Countries- Japan - Canada - Indonesia</p>
        <p>© 2022-2023 CART.IN</p>
        <p>Free Classifieds in India </p>
      </div>
    </div>
  );
}

export default Footer;
