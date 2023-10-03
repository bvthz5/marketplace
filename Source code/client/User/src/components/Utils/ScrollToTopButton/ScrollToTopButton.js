import React, { useState, useEffect, useCallback } from "react";
import { FaAngleUp } from "react-icons/fa";
import styles from "./ScrollToTopButton.module.css";

const ScrollToTopButton = () => {
  const [showTopBtn, setShowTopBtn] = useState(false);
  useEffect(() => {
    window.addEventListener("scroll", () => {
      if (window.scrollY > 400) {
        setShowTopBtn(true);
      } else {
        setShowTopBtn(false);
      }
    });
  }, []);
  const goToTop = useCallback(() => {
    window.scrollTo({
      top: 0,
      behavior: "smooth",
    });
  }, []);
  return (
    <div className={styles["top-to-btm"]} data-testid="scrolltotopbtn">
      {showTopBtn && (
        <FaAngleUp
          data-testid="go-to-top-btn"
          title="Go To Top"
          className={styles["icon-style"]}
          onClick={goToTop}
        />
      )}
    </div>
  );
};

export default ScrollToTopButton;
