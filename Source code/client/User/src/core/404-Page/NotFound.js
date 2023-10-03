import React, { useCallback } from "react";
import Footer from "../../components/Common-Components/Footer/Footer";
import notfound from "./NotFound.module.css";
import { useNavigate } from "react-router-dom";

const NotFound = () => {
  let navigate = useNavigate();
  const goBack = useCallback(() => navigate(-1), [navigate]);
  const goHome = useCallback(() => navigate("/home"), [navigate]);

  return (
    <>
      <div data-testid="notfoundpage">
        <main className={notfound.maindiv} data-aut-id="page-main-content">
          <div className={notfound.div}>
            <div
              type="image/webp"
              src="https://statics.olx.in/external/base/img/404.webp"
            >
              <img
                src="https://statics.olx.in/external/base/img/404.png"
                alt=""
              />
            </div>

            <div className={notfound.div4}>
              <div className={notfound.div5}>
                <span>Oops!</span>
              </div>
              <div className={notfound.div6}>
                <span>We can't seem to find that.</span>
                <br />
                <span>Try searching for it.</span>
              </div>
              <div className={notfound.div7}>
                <span>Error 404</span>
              </div>
              <div className={notfound.div8}>
                <div className={notfound.div9}>
                  <span>Here are some helpful links:</span>
                  &nbsp;{" "}
                  <span
                    style={{ color: "blue", cursor: "pointer" }}
                    onClick={goHome}
                  >
                    Home
                  </span>
                  &nbsp;&nbsp;
                  <br />
                  <span
                    style={{ color: "red", cursor: "pointer" }}
                    onClick={goBack}
                  >
                    Go Back
                  </span>
                </div>
              </div>
            </div>
          </div>
        </main>
        <Footer />
      </div>
    </>
  );
};

export default NotFound;
  