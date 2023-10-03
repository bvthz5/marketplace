import React, { useEffect, useState, useContext, useCallback } from "react";
import Headercss from "./Header.module.css";
import { useNavigate } from "react-router-dom";
import UserDropdown from "../User-DropDown/UserDropdown";
import Swal from "sweetalert2";
import HomeIcon from "@mui/icons-material/Home";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import { cartCounts } from "../../../../App";
import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from "@mui/material/DialogTitle";
import { toast } from "react-toastify";
import { getAllCart, requestAsSeller } from "../../../../core/Api/apiService";
import { refreshToken } from "../../../../core/Api/coreApiService";
import MenuIcon from "@mui/icons-material/Menu";
import useWindowDimensions from "../../../../hooks/WindowSizeReader/WindowDimensions";
import DrawerData from "../Drawer-Right/DrawerData";
import { Badge, IconButton, SwipeableDrawer, Tooltip } from "@mui/material";
import { useLoadScript } from "@react-google-maps/api";
import PlacesAutocomplete from "react-places-autocomplete";
import NotificationBox from "../NotificationDrawer/NotificationBox";
import { HttpTransportType, HubConnectionBuilder } from "@microsoft/signalr";
import { useDispatch, useSelector } from "react-redux";
import {
  selectAllFilters,
  setFilters,
} from "../../Productlist/ProductList/ProductSlices/filterSlice";
import { clearAllProducts } from "../../Productlist/ProductList/ProductSlices/productSlice";
import { fetchUserDetails } from "../../User-Profile/userDetailSlice";

const URL = process.env.REACT_APP_HUB_ADDRESS
  ? process.env.REACT_APP_HUB_ADDRESS
  : "https://localhost:8080";
const mapKey = process.env.REACT_APP_MAPS_API_KEY;
const libraries = ["places"];

const componentRestrictions = { country: "in" };

const CommonHeader = ({ productlist }) => {
  const { isLoaded } = useLoadScript({
    googleMapsApiKey: mapKey,
    libraries: libraries,
  });
  const dispatch = useDispatch();
  const filters = useSelector(selectAllFilters);
  let navigate = useNavigate();
  const { width } = useWindowDimensions();
  const [loggedIn, setLoggedIn] = useState(false);
  const [count, setCount] = useContext(cartCounts);
  const [open, setOpen] = useState(false);
  const [seller, setSeller] = useState(false);
  const [checked, setChecked] = useState(false);
  const [userRole, setUserRole] = useState(0);
  const drawerSide = "right";
  const [state, setState] = useState(false);
  const [address, setAddress] = useState(filters.Location);
  const [callNotification, setCallNotification] = useState(false);
  const [connection, setConnection] = useState(null);

  useEffect(() => {
    const getRole = () => {
      let role = parseInt(localStorage.getItem("role"));
      setUserRole(role);
      if (role === 1) {
        refreshAccessToken();
      }
      if (role === 2) {
        setSeller(true);
      }
      return role;
    };
    const getCart = () => {
      getAllCart()
        .then((response) => {
          setCount(response.data.data.length);
        })
        .catch((err) => console.log(err));
    };
    if (localStorage.getItem("accessToken")) {
      setLoggedIn(true);
      getCart();
      getRole();
      dispatch(fetchUserDetails());
    }
    console.log("call");
  }, [dispatch, setCount]);

  useEffect(() => {
    if (loggedIn) {
      const newConnection = new HubConnectionBuilder()
        .withUrl(URL, {
          accessTokenFactory: () => `${localStorage.getItem("accessToken")}`,
          skipNegotiation: true,
          transport: HttpTransportType.WebSockets,
        })
        .withAutomaticReconnect()
        .build();

      setConnection(newConnection);
    }
  }, [loggedIn]);

  useEffect(() => {
    if (connection) {
      document.addEventListener("visibilitychange", () => {
        if (document.visibilityState === "visible") {
          connection
            .start()
            .then(() => {
              setCallNotification(true);
            })
            .catch((err) => {
              console.error(err);
            });
        } else if (document.visibilityState === "hidden") {
          connection
            .stop()
            .then(() => {
              setCallNotification(false);
            })
            .catch((err) => {
              console.error(err);
            });
        }
      });

      connection.on("Notification", () => {
        setCallNotification(true);
        toast("🔔 You have a new notification!", {
          position: "bottom-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
          progress: undefined,
          theme: "dark",
          toastId: "85",
        });
      });

      connection.on("Stop", () => {
        connection
          .stop()
          .then(() => {
            console.log("SignalR connection stopped");
          })
          .catch((err) => {
            console.error(err);
          });
      });

      connection
        .start()
        .then(() => {
          console.log("SignalR connection started");
        })
        .catch((err) => {
          console.error(err);
        });
      return () => {
        connection
          .stop()
          .then(() => {
            console.log("SignalR connection stopped");
          })
          .catch((err) => {
            console.error(err);
          });
      };
    }
  }, [connection]);

  useEffect(() => {
    if (!address && productlist) {
      dispatch(clearAllProducts());
      dispatch(setFilters({ Location: "" }));
    }
  }, [address, dispatch, productlist]);

  useEffect(() => {
    if (callNotification) setCallNotification(false);
  }, [callNotification]);

  const handleClose = useCallback(() => {
    setOpen(false);
    setChecked(false);
  }, []);

  // api call to become a seller
  const sellerRequest = useCallback(async () => {
    if (localStorage.getItem("role") === "1") {
      toast.warning("Request is already submitted");
      setChecked(false);
      handleClose();
    } else {
      setChecked(false);
      requestAsSeller()
        .then(() => {
          handleClose();
          toast.success("Request submitted");
          refreshAccessToken();
        })
        .catch((err) => {
          handleClose();
          console.log(err);
        });
    }
  }, [handleClose]);

  // calling refresh checking if the user userRole is changed or not

  const refreshAccessToken = async () => {
    refreshToken()
      .then((response) => {
        const accessToken = response?.data?.data.accessToken?.value;
        const newRefreshToken = response?.data?.data.refreshToken?.value;
        let responseRole = response.data.data.role;
        console.log(responseRole);
        setUserRole(responseRole);
        localStorage.setItem("accessToken", accessToken);
        localStorage.setItem("refreshToken", newRefreshToken);
        localStorage.setItem("role", responseRole);
        if (responseRole === 2) {
          setSeller(true);
        }
      })
      .catch((err) => console.log(err));
  };

  const preLoginMessage = useCallback(() => {
    Swal.fire({
      title: "Oops...",
      text: "You haven't logged in yet!",
      icon: "error",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Login Now",
    }).then((result) => {
      if (result.isConfirmed) {
        navigate("/login");
      }
    });
  },[navigate]);

  // cheking if user is logged in or not if logged in will navigate to sell page else will show a prompt to login

  const sellerClick = () => {
    if (loggedIn) {
      navigate("/addproduct");
    }
  };
  // cheking if user is logged in or not if logged in will navigate to cart page else will show a prompt to login

  const mycart = () => {
    if (loggedIn) {
      navigate("/cart");
    } else {
      preLoginMessage();
    }
  };

  // cheking if user is logged in or not if logged in will open the terms and conditions modal else will show a prompt to login
  const becomeSeller = () => {
    if (loggedIn) {
      if (userRole === 0) {
        setOpen(true);
      } else if (userRole === 1) {
        toast.warning("Request already submitted", { toastId: 15 });
      }
    } else {
      preLoginMessage();
    }
  };

  useEffect(() => {
    setAddress(filters.Location);
  }, [filters.Location]);

  const handleSelect = useCallback(
    (value) => {
      if (!value) {
        dispatch(clearAllProducts());
        dispatch(setFilters({ Location: "" }));
        setAddress("");
        return;
      }
      let selectedAddress = value.split(",").splice(0, 2).join();
      setAddress(selectedAddress);
      dispatch(clearAllProducts());
      dispatch(setFilters({ Location: selectedAddress }));
    },
    [dispatch]
  );

  const handleSearchValue = useCallback(
    (value) => {
      dispatch(clearAllProducts());
      dispatch(setFilters({ Search: value }));
    },
    [dispatch]
  );

  const handleCheckBox = () => {
    setChecked(!checked);
  };

  const handleClick = useCallback(() => {
    navigate("/home");
  }, [navigate]);

  const loginClick = () => {
    navigate("/login");
  };

  const goHome = useCallback(() => navigate("/home"), [navigate]);

  const toggleDrawer = useCallback((open) => (event) => {
    if (
      event &&
      event.type === "keydown" &&
      (event.key === "Tab" || event.key === "Shift")
    ) {
      return;
    }
    setState(open);
  },[]);

  const callNoLogin = useCallback(() => {
    preLoginMessage();
  },[preLoginMessage]);

  return (
    <>
      <div data-testid="commonheader" className={Headercss["headercontainer"]}>
        {/* //container// */}
        <header className={Headercss["containerheader"]}>
          {/* //header// */}
          <div className={Headercss["headerpadding"]}>
            {/* //icon// */}
            <div className={Headercss["leftsection"]}>
              <div
                data-testid="title"
                className={Headercss["logo"]}
                onClick={() => {
                  handleClick();
                }}
              >
                CART_IN
              </div>
              <div className={Headercss["homeicon"]}>
                <HomeIcon
                  style={{ cursor: "pointer" }}
                  data-testid="homeicon"
                  onClick={goHome}
                />
              </div>

              {productlist && (
                <div className={Headercss["searchbars"]}>
                  {/* //locationsearch// */}
                  <div className={Headercss["locationsearch"]}>
                    <div className={Headercss["locationdrop"]}>
                      {isLoaded && (
                        <PlacesAutocomplete
                          searchOptions={{ componentRestrictions }}
                          highlightFirstSuggestion
                          value={address}
                          onChange={setAddress}
                          onSelect={handleSelect}
                        >
                          {({
                            getInputProps,
                            suggestions,
                            getSuggestionItemProps,
                            loading,
                          }) => (
                            <div
                              key={suggestions.description}
                              className={Headercss["locationcontainer"]}
                            >
                              <input
                                {...getInputProps({
                                  placeholder: "Search Places ...",
                                  className: Headercss.locationsearchinput,
                                })}
                              />
                              <div
                                className={`autocomplete-dropdown-container ${Headercss["drop"]}`}
                              >
                                {loading && <div>Loading...</div>}
                                {suggestions.map((suggestion, index) => {
                                  // inline style for demonstration purpose
                                  const style = suggestion.active
                                    ? {
                                        backgroundColor: "#d3d3d3",
                                        cursor: "pointer",
                                      }
                                    : {
                                        backgroundColor: "#ffffff",
                                        cursor: "pointer",
                                      };
                                  return (
                                    <div
                                      className={Headercss["input-suggestion"]}
                                      key={suggestion.placeId}
                                      {...getSuggestionItemProps(suggestion, {
                                        style,
                                      })}
                                    >
                                      <i className="material-icons">
                                        location_on{" "}
                                      </i>
                                      <span>
                                        {suggestion.description
                                          .split(",")
                                          .splice(0, 2)
                                          .join()}
                                      </span>
                                    </div>
                                  );
                                })}
                              </div>
                            </div>
                          )}
                        </PlacesAutocomplete>
                      )}
                    </div>
                  </div>
                  {/* //normalsearch// */}

                  <div className={Headercss["textsearch"]}>
                    <div className={Headercss["textcontainer"]}>
                      <input
                        data-testid="search-input"
                        className={Headercss["searchbar"]}
                        type="search"
                        value={filters.Search}
                        maxLength={255}
                        placeholder="Search for products and more..."
                        onChange={(e) => {
                          handleSearchValue(e.target.value);
                        }}
                      />
                      <SearchButton fill={"white"} />
                    </div>
                  </div>
                </div>
              )}

              {width < 768 && <SearchButton fill={"#002f34"} />}
            </div>

            <div className={Headercss["rightsection"]}>
              <div className={Headercss["sellsection"]}>
                {seller && (
                  <div className={Headercss["login1"]}>
                    <div style={{ cursor: "pointer", marginTop: "2px" }}>
                      <div
                        rel=""
                        data-aut-id="btnSell"
                        className={Headercss["sell1"]}
                        href="/post"
                      >
                        <div
                          data-testid="sellbtn"
                          onClick={() => {
                            sellerClick();
                          }}
                        >
                          <svg
                            width="104"
                            height="48"
                            viewBox="0 0 1603 768"
                            className={Headercss["sell2"]}
                          >
                            <g>
                              <path
                                className={Headercss["sell3"]}
                                d="M434.442 16.944h718.82c202.72 0 367.057 164.337 367.057 367.058s-164.337 367.057-367.057 367.057h-718.82c-202.721 0-367.058-164.337-367.058-367.058s164.337-367.058 367.058-367.058z"
                              ></path>
                              <path
                                className={Headercss["sell4"]}
                                d="M427.241 669.489c-80.917 0-158.59-25.926-218.705-73.004l-0.016-0.014c-69.113-54.119-108.754-131.557-108.754-212.474 0-41.070 9.776-80.712 29.081-117.797 25.058-48.139 64.933-89.278 115.333-118.966l-52.379-67.581c-64.73 38.122-115.955 90.98-148.159 152.845-24.842 47.745-37.441 98.726-37.441 151.499 0 104.027 50.962 203.61 139.799 273.175h0.016c77.312 60.535 177.193 93.887 281.22 93.887h299.699l25.138-40.783-25.138-40.783h-299.698z"
                              ></path>
                              <path
                                className={Headercss["sell5"]}
                                d="M1318.522 38.596v0c-45.72-14.369-93.752-21.658-142.762-21.658h-748.511c-84.346 0-165.764 21.683-235.441 62.713l3.118 51.726 49.245 15.865c54.16-31.895 117.452-48.739 183.073-48.739h748.511c38.159 0 75.52 5.657 111.029 16.829v0c44.91 14.111 86.594 37.205 120.526 66.792l66.163-57.68c-43.616-38.010-97.197-67.703-154.957-85.852z"
                              ></path>
                              <path
                                className={Headercss["sell6"]}
                                d="M1473.479 124.453l-55.855 9.91-10.307 47.76c61.844 53.929 95.92 125.617 95.92 201.88 0 25.235-3.772 50.26-11.214 74.363-38.348 124.311-168.398 211.129-316.262 211.129h-448.812l25.121 40.783-25.121 40.783h448.812c190.107 0 357.303-111.638 406.613-271.498 9.572-31.009 14.423-63.162 14.423-95.559 0-98.044-43.805-190.216-123.317-259.551z"
                              ></path>
                            </g>
                          </svg>
                          <div className={Headercss["sell7"]}>
                            <span className={Headercss["sell8"]}>
                              <svg
                                width="16px"
                                height="16px"
                                viewBox="0 0 1024 1024"
                                data-aut-id="icon"
                              >
                                <path
                                  className={Headercss["h1"]}
                                  d="M414.898 123.739v291.218h-291.218l-97.014 97.014 97.014 97.131h291.218v291.16l97.073 97.071 97.073-97.071v-291.16h291.16l97.131-97.131-97.131-97.014h-291.16v-291.218l-97.073-97.073z"
                                ></path>
                              </svg>
                            </span>
                            <span>Sell</span>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                )}
                {!seller && (
                  <>
                    <div
                      data-testid="becomesellerbtn"
                      onClick={() => {
                        becomeSeller();
                      }}
                      className={Headercss["becomeseller"]}
                    >
                      Become a Seller
                    </div>
                  </>
                )}
              </div>
              <div className={Headercss["carticon"]}>
                <NotificationBox
                  callNotification={callNotification}
                  loggedIn={loggedIn}
                  callNoLogin={callNoLogin}
                />
              </div>
              <div
                data-testid="cartbtn"
                className={Headercss["carticon"]}
                onClick={() => {
                  mycart();
                }}
              >
                <Tooltip title="Go to Cart">
                  <IconButton>
                    <Badge
                      badgeContent={count}
                      sx={{
                        "& .MuiBadge-badge": {
                          color: "white",
                          backgroundColor: "red",
                        },
                      }}
                    >
                      <ShoppingCartIcon
                        style={{ fontSize: "27px", color: "#002f34" }}
                      />
                    </Badge>
                  </IconButton>
                </Tooltip>
              </div>
              <div className={Headercss["dropdownicon"]}>
                {loggedIn ? (
                  <>
                    {width < 500 ? (
                      <Button
                        data-testid="menu-icon"
                        onClick={toggleDrawer(true)}
                      >
                        <MenuIcon className={Headercss["menuicon"]} />
                      </Button>
                    ) : (
                      <UserDropdown setLoggedIn={setLoggedIn} />
                    )}
                  </>
                ) : (
                  <h3
                    data-testid="loginbtn"
                    className={Headercss["loginbtn"]}
                    onClick={() => {
                      loginClick();
                    }}
                  >
                    Login
                  </h3>
                )}
              </div>
            </div>
          </div>
        </header>
        <Dialog
          open={open}
          onClose={handleClose}
          aria-labelledby="alert-dialog-title"
          aria-describedby="alert-dialog-description"
        >
          <DialogTitle data-testid="modalheading" id="alert-dialog-title">
            {"Terms and Conditions"}
          </DialogTitle>
          <DialogContent>
            <DialogContentText id="alert-dialog-description">
              ● All products will be monitored by admin.
              <br />
              ● Admin have rights to approve /reject your products from this
              website.
              <br />● Money will be transfered to your account 7 days after the
              pickup.
              <div className={Headercss["termdiv"]}>
                <input
                  data-testid="checkbox"
                  onClick={handleCheckBox}
                  className={Headercss["checkbox"]}
                  value={checked}
                  type="checkbox"
                />
                I accept the Terms and Conditions.
              </div>
            </DialogContentText>
          </DialogContent>
          <DialogActions>
            <Button data-testid="disagree-button" onClick={handleClose}>
              Disagree
            </Button>
            <Button
              data-testid="agreebutton"
              onClick={sellerRequest}
              disabled={!checked}
            >
              Agree
            </Button>
          </DialogActions>
        </Dialog>
      </div>
      <SwipeableDrawer
        sx={{ width: "10px !important" }}
        anchor={drawerSide}
        open={state}
        onClose={toggleDrawer(false)}
        onOpen={toggleDrawer(true)}
      >
        <DrawerData toggleDrawer={toggleDrawer} setLoggedIn={setLoggedIn} />
      </SwipeableDrawer>
    </>
  );
};

export default CommonHeader;

const SearchButton = ({ fill }) => {
  const { width } = useWindowDimensions();

  const handleClick = () => {
    if (width > 768) return;
  };

  return (
    <button
      className={Headercss["buttonsearch"]}
      data-testid="search-btn"
      onClick={handleClick}
    >
      <span>
        <svg
          width="30px"
          height="24px"
          padding-top="12px"
          viewBox="0 0 1024 1024"
          data-aut-id="icon"
          fill={fill}
          rule="evenodd"
        >
          <path d="M448 725.333c-152.917 0-277.333-124.416-277.333-277.333s124.416-277.333 277.333-277.333c152.917 0 277.333 124.416 277.333 277.333s-124.416 277.333-277.333 277.333v0zM884.437 824.107v0.021l-151.915-151.936c48.768-61.781 78.144-139.541 78.144-224.192 0-199.979-162.688-362.667-362.667-362.667s-362.667 162.688-362.667 362.667c0 199.979 162.688 362.667 362.667 362.667 84.629 0 162.411-29.376 224.171-78.144l206.144 206.144h60.352v-60.331l-54.229-54.229z"></path>
        </svg>
      </span>
    </button>
  );
};
