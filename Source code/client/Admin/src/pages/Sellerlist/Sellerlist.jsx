import React, { useCallback, useEffect, useState } from 'react';
import ReactPaginate from 'react-paginate';
import SellerListcss from './Sellerlist.module.css';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableCell from '@mui/material/TableCell';
import TableRow from '@mui/material/TableRow';
import SellerDetail from './Seller-Detail-View/SellerDetail';
import Swal from 'sweetalert2';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import BlockIcon from '@mui/icons-material/Block';
import { Tooltip } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import LocalMallIcon from '@mui/icons-material/LocalMall';
import { allSellers, blockdeleteSellers } from '../../core/api/apiService';
import LoaderSeller from '../../utils/Loader/Loader';
import { statusBadge } from '../../utils/utils';
const Sellerlist = () => {
  const [sellers, setSeller] = useState([]);
  const [pageLimit] = useState(25);
  const [searchValue, setSearchValue] = useState('');
  const [status, setStatus] = useState('1');
  const [sortValue, setSortValue] = useState('CreatedDate');
  const [pageNo, setPageNo] = useState(1);
  const [pageCount, setPageCount] = useState(1);
  const [desc, setDesc] = useState(true);
  const [apiCallSellerList, setApiCallSellerList] = useState(false);

  useEffect(() => {
    document.title = 'Seller List';
    getUsers();
  }, [searchValue, status, sortValue, pageNo]);
  //get all sellers//
  const getUsers = async () => {
    setApiCallSellerList(true);
    const params = {
      pageNumber: pageNo,
      Role: 2,
      pageSize: pageLimit,
      search: searchValue.replace('+', '%2b'),
      SortBy: sortValue,
      Status: status ? status : '',
      SortByDesc: desc,
    };
    allSellers(params)
      .then((response) => {
        setSeller(response?.data.data.result);
        setPageCount(response?.data.data.totalPages);
        window.scrollTo({ top: 0, behavior: 'smooth' });
        setApiCallSellerList(false);
      })
      .catch((err) => {
        console.log(err);
        setApiCallSellerList(false);
      });
  };

  //block sellers//
  const blockUser = async (id, firstName, status) => {
    let messsage1 = status === 1 ? 'Activate' : 'Block';
    let messsage2 = status === 1 ? 'Activated' : 'Blocked';
    Swal.fire({
      title: `${messsage1} ${firstName}?`,
      icon: 'info',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: `Yes, ${messsage1}!`,
    }).then(async (result) => {
      if (result.isConfirmed) {
        blockdeleteSellers(id, status)
          .then((response) => {
            if (response?.data.data) Swal.fire(`${messsage2}!`, `${firstName} has been ${messsage2} `, 'success');
            getUsers();
          })
          .catch((err) => {
            console.log(err);
          });
      }
    });
  };
  //delete seller//
  const deleteUser = async (id, firstName, status) => {
    Swal.fire({
      title: `Delete ${firstName}?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: `Delete!`,
    }).then(async (result) => {
      if (result.isConfirmed) {
        blockdeleteSellers(id, status)
          .then((response) => {
            if (response?.data.data) Swal.fire(`Deleted!`, `${firstName} has been Deleted `, 'success');
            getUsers();
          })
          .catch((err) => {
            console.log(err);
          });
      }
    });
  };

  const handleSort = (data) => {
    if (data === 'CreatedDate') {
      setDesc(true);
    } else setDesc(false);
  };

  const handlePageClick = useCallback((data) => {
    let currentPage = data.selected + 1;
    setPageNo(currentPage);
  }, []);
  const dateConvertion = (date) => {
    let currentDate = new Date(date);
    return currentDate.toDateString();
  };

  const columns = [
    { id: 'id', label: 'User Id', minWidth: 1, maxLength: 3 },
    { id: 'FirstName', label: 'First Name', minWidth: 5 },
    {
      id: 'email',
      label: 'Email',
      minWidth: 5,
    },
    {
      id: 'createdDate',
      label: 'Created Date',
      minWidth: 5,
    },
    {
      id: 'status',
      label: 'Status',
      minWidth: 5,
    },
    {
      id: 'Actions',
      label: 'Actions',
      minWidth: 5,
    },
  ];

  return (
    <>
      <div className={SellerListcss['body']} data-testid="sellerlistpage">
        <div className={SellerListcss.boxproductlist}>
          <div className={SellerListcss.productbox}>
            <div className={SellerListcss.userlistbackground}>
              <div className={SellerListcss['items']}>
                <div>
                  <input
                    data-testid="search-input"
                    className={SellerListcss['search']}
                    maxLength={255}
                    type="search"
                    placeholder="Search here"
                    onChange={(e) => {
                      setSearchValue(e.target.value);
                      setPageNo(1);
                    }}
                  />
                </div>
                <div className={SellerListcss['selectdiv']}>
                  <select
                    data-testid="status-filter-dropdown"
                    className={SellerListcss.select}
                    onChange={(e) => {
                      setStatus(e.target.value);
                      setPageNo(1);
                    }}
                    name="cars"
                    id="cars"
                  >
                    <option className={SellerListcss.optionsort} defaultChecked value="1">
                      Active
                    </option>
                    <option className={SellerListcss.optionsort} value="2">
                      Blocked
                    </option>
                    <option className={SellerListcss.optionsort} value="3">
                      Deleted
                    </option>
                    <option className={SellerListcss.optionsort} value="">
                      All
                    </option>
                  </select>
                  <select
                    data-testid="sort-dropdown"
                    className={SellerListcss.select}
                    onChange={(e) => {
                      handleSort(e.target.value);
                      setSortValue(e.target.value);
                      setPageNo(1);
                    }}
                    name="cars"
                    id="cars"
                  >
                    <option
                      data-testid="createdDate"
                      className={SellerListcss.optionsort}
                      value="CreatedDate"
                      defaultChecked
                    >
                      Created Date
                    </option>
                    <option className={SellerListcss.optionsort} value="FirstName">
                      First Name
                    </option>
                    <option className={SellerListcss.optionsort} value="Email">
                      Email
                    </option>
                  </select>
                </div>
              </div>
              <Paper className={SellerListcss.papercss}>
                <TableContainer className={SellerListcss['TableContainer']}>
                  <Table stickyHeader>
                    <TableHead>
                      <TableRow>
                        {columns.map((column) => (
                          <TableCell
                            key={column.id}
                            align={column.align}
                            style={{
                              backgroundColor: '#4B4453',
                              color: 'white',
                            }}
                          >
                            {column.label}
                          </TableCell>
                        ))}
                      </TableRow>
                    </TableHead>
                    {sellers.length > 0 ? (
                      sellers.map((seller) => {
                        return (
                          <TableBody key={seller.userId} role="checkbox" tabIndex={-1}>
                            <TableRow className={SellerListcss['row']}>
                              <TableCell className={SellerListcss['TableCell']}>{seller.userId}</TableCell>
                              <TableCell className={SellerListcss['TableCell']} data-testid="profileicon20">
                                <div className={SellerListcss['tablecellprofilecss']}>
                                  <SellerDetail seller={seller} />
                                  &nbsp;&nbsp;
                                  <span
                                    style={{
                                      overflow: 'hidden',
                                      lineBreak: 'anywhere',
                                    }}
                                    onClick={() => {}}
                                  >
                                    {seller.firstName}
                                  </span>
                                </div>
                              </TableCell>
                              <TableCell
                                className={SellerListcss['TableCell']}
                                style={{
                                  overflow: 'hidden',
                                  lineBreak: 'anywhere',
                                }}
                              >
                                {seller.email}
                              </TableCell>
                              <TableCell
                                className={SellerListcss['TableCell']}
                                style={{
                                  overflow: 'hidden',
                                  lineBreak: 'anywhere',
                                }}
                              >
                                {dateConvertion(seller.createdDate)}
                              </TableCell>
                              <TableCell className={SellerListcss['TableCell']}>{statusBadge(seller.status)}</TableCell>

                              <TableCell className={SellerListcss['TableCell']}>
                                {seller.status !== 3 && (
                                  <>
                                    <button
                                      hidden={seller.status === 3}
                                      className={SellerListcss['button-55']}
                                      onClick={() => {
                                        window.open(`/dashboard/myproducts/?id=${seller.userId}`, '_blank').focus();
                                      }}
                                      disabled={seller.status === (3 || 2)}
                                    >
                                      <Tooltip title="show products" placement="top">
                                        <LocalMallIcon style={{ color: '#f8cf2b' }} />
                                      </Tooltip>
                                      <div
                                        style={{
                                          position: 'relative',
                                          bottom: '18px',
                                          color: 'green',
                                          fontSize: '10px',
                                        }}
                                      ></div>
                                    </button>
                                    <button
                                      data-testid="deletebtn"
                                      hidden={seller.status === 3 || seller.role === 1}
                                      className={SellerListcss['button-55']}
                                      onClick={() => {
                                        deleteUser(seller.userId, seller.firstName, 3);
                                      }}
                                    >
                                      <Tooltip title="Delete Seller" placement="top">
                                        <DeleteIcon
                                          style={{
                                            color: 'grey',
                                          }}
                                        />
                                      </Tooltip>
                                    </button>

                                    {seller.status === 1 && (
                                      <button
                                        data-testid="blockbtn"
                                        hidden={seller.status === 3 || seller.role === 1}
                                        className={SellerListcss['button-55']}
                                        onClick={() => {
                                          blockUser(seller.userId, seller.firstName, 2);
                                        }}
                                      >
                                        <Tooltip title="Block Seller" placement="top">
                                          <CheckCircleOutlineIcon style={{ color: 'green' }} />
                                        </Tooltip>
                                      </button>
                                    )}
                                    {(seller.status === 2 || seller.status === 3) && (
                                      <button
                                        data-testid="unblockbtn"
                                        hidden={seller.status === 3}
                                        className={SellerListcss['button-55']}
                                        onClick={() => {
                                          blockUser(seller.userId, seller.firstName, 1);
                                        }}
                                      >
                                        <Tooltip title="Activate Seller" placement="top">
                                          <BlockIcon style={{ color: 'red' }} />
                                        </Tooltip>
                                      </button>
                                    )}
                                  </>
                                )}
                              </TableCell>
                            </TableRow>
                          </TableBody>
                        );
                      })
                    ) : (
                      <>{!apiCallSellerList && <div className={SellerListcss['listloader']}>No Match Found</div>}</>
                    )}
                  </Table>
                </TableContainer>
              </Paper>
            </div>
          </div>

          {/* // */}
        </div>
      </div>
      {sellers.length !== 0 ? (
        <div type="button" className={SellerListcss['paginationdiv']}>
          <ReactPaginate
            className={SellerListcss['pagination']}
            previousLabel={'Previous'}
            nextLabel={'Next'}
            breakLabel={'...'}
            pageCount={pageCount}
            marginPagesDisplayed={2}
            pageRangeDisplayed={3}
            forcePage={pageNo - 1}
            onPageChange={handlePageClick}
            containerClassName={SellerListcss['paginationjustify']}
            pageClassName={SellerListcss['page-item']}
            pageLinkClassName={SellerListcss['page-link']}
            previousClassName={SellerListcss['page-item']}
            previousLinkClassName={SellerListcss['page-item']}
            nextClassName={SellerListcss['page-item']}
            nextLinkClassName={SellerListcss['page-item']}
            breakClassName={SellerListcss['page-item']}
            breakLinkClassName={SellerListcss['page-item']}
            activeClassName={SellerListcss['page-active']}
            disabledClassName={SellerListcss['page-prev-disabled']}
            disabledLinkClassName={SellerListcss['page-prev-disabled']}
            prevRel={null}
            prevPageRel={null}
          />
        </div>
      ) : null}
      {apiCallSellerList && <LoaderSeller />}
    </>
  );
};

export default Sellerlist;
