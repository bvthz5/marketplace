import React, { useCallback, useEffect, useState } from 'react';
import ReactPaginate from 'react-paginate';
import UserListcss from './Userlist.module.css';
import { statusBadge } from '../../utils/utils';
import UserDetail from './User-Detail-View/UserDetail';
import Swal from 'sweetalert2';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import LoaderUser from '../../utils/Loader/Loader';
import BlockIcon from '@mui/icons-material/Block';
import { Tooltip, Paper, Table, TableBody, TableContainer, TableHead, TableCell, TableRow } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import { allUserList, blockdeleteUseraccess } from '../../core/api/apiService';

const UserList = () => {
  const [users, setUsers] = useState([]);
  const [pageLimit] = useState(25);
  const [searchValue, setSearchValue] = useState('');
  const [status, setStatus] = useState('1');
  const [sortValue, setSortValue] = useState('CreatedDate');
  const [pageNo, setPageNo] = useState(1);
  const [pageCount, setPageCount] = useState(1);

  const [desc, setDesc] = useState(true);
  const [apiCallUser, setApiCallUser] = useState(false);

  useEffect(() => {
    document.title = 'User List';
    getUsers();
  }, [searchValue, status, sortValue, pageNo]);
  //get all users//
  const getUsers = async () => {
    setApiCallUser(true);
    const params = {
      pageNumber: pageNo,
      Role: [0, 1],
      pageSize: pageLimit,
      search: searchValue.replace('+', '%2b'),
      SortBy: sortValue,
      Status: status ? status : '',
      SortByDesc: desc,
    };
    allUserList(params)
      .then((response) => {
        setUsers(response?.data.data.result);
        setPageCount(response?.data.data.totalPages);
        window.scrollTo({ top: 0, behavior: 'smooth' });
        setApiCallUser(false);
      })
      .catch((err) => {
        console.log(err);
        setApiCallUser(false);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      });
  };
  //bloack users//
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
        blockdeleteUseraccess(id, status)
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
  //delete user//
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
        blockdeleteUseraccess(id, status)
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
      <div className={UserListcss['body']} data-testid="userlistpage">
        <div className={UserListcss.boxproductlist}>
          <div className={UserListcss.productbox}>
            <div className={UserListcss.userlistbackground}>
              <div className={UserListcss['items']}>
                <div>
                  <input
                    data-testid="search-input"
                    className={UserListcss['search']}
                    maxLength={255}
                    type="search"
                    placeholder="Search here"
                    onChange={(e) => {
                      setSearchValue(e.target.value);
                      setPageNo(1);
                    }}
                  />
                </div>
                <div className={UserListcss['selectdiv']}>
                  <select
                    data-testid="status-filter-dropdown"
                    className={UserListcss.select}
                    onChange={(e) => {
                      setStatus(e.target.value);
                      setPageNo(1);
                    }}
                    name="cars"
                    id="cars"
                  >
                    <option className={UserListcss.optionsort} defaultChecked value="1">
                      Active
                    </option>
                    <option className={UserListcss.optionsort} value="0">
                      Inactive
                    </option>
                    <option className={UserListcss.optionsort} value="2">
                      Blocked
                    </option>
                    <option className={UserListcss.optionsort} value="3">
                      Deleted
                    </option>
                    <option className={UserListcss.optionsort} value="">
                      All
                    </option>
                  </select>
                  <select
                    data-testid="sort-dropdown"
                    className={UserListcss.select}
                    onChange={(e) => {
                      handleSort(e.target.value);
                      setSortValue(e.target.value);
                      setPageNo(1);
                    }}
                    name="cars"
                    id="cars"
                  >
                    <option
                      className={UserListcss.optionsort}
                      value="CreatedDate"
                      defaultChecked
                      data-testid="createddate"
                    >
                      Created Date
                    </option>
                    <option className={UserListcss.optionsort} value="FirstName">
                      First Name
                    </option>
                    <option className={UserListcss.optionsort} value="Email">
                      Email
                    </option>
                  </select>
                </div>
              </div>

              <Paper
                style={{
                  width: '95%',
                  overflow: 'hidden',
                  margin: 'auto',

                  maxHeight: '70%',
                  zIndex: '0',
                }}
              >
                <TableContainer className={UserListcss['TableContainer']}>
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
                    {users.length > 0 ? (
                      users.map((user) => {
                        return (
                          <TableBody key={user.userId} role="checkbox" tabIndex={-1}>
                            <TableRow className={UserListcss['row']}>
                              <TableCell className={UserListcss['TableCell']}>{user.userId}</TableCell>
                              <TableCell className={UserListcss['TableCell']} data-testid="profileicon20">
                                <div className={UserListcss['tablecellprofilecss']}>
                                  <UserDetail user={user} />
                                  &nbsp;&nbsp;
                                  <span
                                    style={{
                                      overflow: 'hidden',
                                      lineBreak: 'anywhere',
                                    }}
                                    onClick={() => {}}
                                  >
                                    {user.firstName}
                                  </span>
                                </div>
                              </TableCell>
                              <TableCell
                                className={UserListcss['TableCell']}
                                style={{
                                  overflow: 'hidden',
                                  lineBreak: 'anywhere',
                                }}
                              >
                                {user.email}
                              </TableCell>
                              <TableCell
                                className={UserListcss['TableCell']}
                                style={{
                                  overflow: 'hidden',
                                  lineBreak: 'anywhere',
                                }}
                              >
                                {dateConvertion(user.createdDate)}
                              </TableCell>
                              <TableCell className={UserListcss['TableCell']}>{statusBadge(user.status)}</TableCell>

                              <TableCell className={UserListcss['TableCell']}>
                                {user.status !== 3 && (
                                  <>
                                    <button
                                      data-testid="deletebtn"
                                      hidden={user.status === 3}
                                      className={UserListcss['button-55']}
                                      onClick={() => {
                                        deleteUser(user.userId, user.firstName, 3);
                                      }}
                                    >
                                      <Tooltip title="Delete user" placement="top">
                                        <DeleteIcon
                                          style={{
                                            color: 'grey',
                                          }}
                                        />
                                      </Tooltip>
                                    </button>

                                    {user.status === 1 && (
                                      <button
                                        data-testid="blockbtn"
                                        hidden={user.status === 3}
                                        className={UserListcss['button-55']}
                                        onClick={() => {
                                          blockUser(user.userId, user.firstName, 2);
                                        }}
                                      >
                                        <Tooltip title="Block User" placement="top">
                                          <CheckCircleOutlineIcon style={{ color: 'green' }} />
                                        </Tooltip>
                                      </button>
                                    )}
                                    {(user.status === 2 || user.status === 3) && (
                                      <button
                                        data-testid="unblockbtn"
                                        hidden={user.status === 3}
                                        className={UserListcss['button-55']}
                                        onClick={() => {
                                          blockUser(user.userId, user.firstName, 1);
                                        }}
                                      >
                                        <Tooltip title="Activate User" placement="top">
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
                      <>{!apiCallUser && <div className={UserListcss['listloader']}>No Match Found</div>}</>
                    )}
                  </Table>
                </TableContainer>
              </Paper>
            </div>
          </div>

          {/* // */}
        </div>
      </div>
      {users.length !== 0 ? (
        <div type="button" className={UserListcss['paginationdiv']} data-testid="paginationbutton">
          <ReactPaginate
            className={UserListcss['pagination']}
            previousLabel={'Previous'}
            nextLabel={'Next'}
            breakLabel={'...'}
            pageCount={pageCount}
            marginPagesDisplayed={2}
            pageRangeDisplayed={3}
            forcePage={pageNo - 1}
            onPageChange={handlePageClick}
            containerClassName={UserListcss['paginationjustify']}
            pageClassName={UserListcss['page-item']}
            pageLinkClassName={UserListcss['page-link']}
            previousClassName={UserListcss['page-item']}
            previousLinkClassName={UserListcss['page-item']}
            nextClassName={UserListcss['page-item']}
            nextLinkClassName={UserListcss['page-item']}
            breakClassName={UserListcss['page-item']}
            breakLinkClassName={UserListcss['page-item']}
            activeClassName={UserListcss['page-active']}
            disabledClassName={UserListcss['page-prev-disabled']}
            disabledLinkClassName={UserListcss['page-prev-disabled']}
            prevRel={null}
            prevPageRel={null}
          />
        </div>
      ) : null}

      <br />
      <br />
      {apiCallUser && <LoaderUser />}
    </>
  );
};

export default UserList;
