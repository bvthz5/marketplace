import React, { useEffect, useState, useCallback } from 'react';
import Swal from 'sweetalert2';
import deliveryagentcss from './DeliveryAgent.module.css';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableCell from '@mui/material/TableCell';
import TableRow from '@mui/material/TableRow';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import BlockIcon from '@mui/icons-material/Block';
import { Tooltip, Typography } from '@mui/material';
import { useForm as UseForm } from 'react-hook-form';
import { toast } from 'react-toastify';
import { addAgentsApi, allAgentsApi, deleteAgentsApi, editAgents } from '../../core/api/apiService';
import Loader from '../../utils/Loader/Loader';
import {statusBadge} from '../../utils/utils'
import Box from '@mui/material/Box';
import useWindowDimensions from '../../utils/WindowDimensions';
import Modal from '@mui/material/Modal';
import ReactPaginate from 'react-paginate';

const columns = [
  { id: 'id', label: 'ID', minWidth: 1 },
  { id: 'agent', label: 'Agent Name', minWidth: 5 },
  { id: 'email', label: 'Email', minWidth: 5 },
  { id: 'contact', label: 'Contact Number', minWidth: 5 },
  { id: 'createddate', label: 'Created Date', minWidth: 5 },

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

const Agent = () => {
  const { width } = useWindowDimensions();
  const [agents, setAgents] = useState([]);
  const [id, setId] = useState(null);
  const [loadingagentpage, setLoading] = useState(false);
  const [apiCallAgent, setApiCallAgent] = useState(false);
  const [openAgent, setOpenAgent] = React.useState(false);
  const [editopenAgent, setEditOpenAgent] = React.useState(false);
  const [pageCount, setPageCount] = useState(1);
  const [pageNo, setPageNo] = useState(1);
  const [pageLimit] = useState(25);
  const [searchValue, setSearchValue] = useState('');
  const [status, setStatus] = useState('1');
  const [sortValue, setSortValue] = useState('CreatedDate');
  const [desc, setDesc] = useState(true);

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    reset,
    getValues,
  } = UseForm({ mode: 'onChange' });

  const handleOpenAgent = () => setOpenAgent(true);
  const handleCloseAddModal = useCallback(() => {
    setOpenAgent(false);
  }, []);
  const handleEditAgentOpen = () => setEditOpenAgent(true);
  const handleEditCloseModal = useCallback(() => {
    setEditOpenAgent(false);
  }, []);

  const modalstyle = {
    position: 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    width: 400,
    height: '390px',
    bgcolor: 'background.paper',
    borderRadius: '20px',
  };
  const editmodalstyle = {
    position: 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    width: 400,
    height: '330px',
    bgcolor: 'background.paper',
    borderRadius: '20px',
  };
  useEffect(() => {
    document.title = 'Delivery Agent';
    localStorage.setItem('pillerValue', 'category');
    getAgents();
  }, [pageNo, searchValue, status, sortValue]);

  //   function for getting all agents
  const getAgents = async () => {
    setApiCallAgent(true);
    const params = {
      PageNumber: pageNo,
      PageSize: pageLimit,
      search: searchValue.replace('+', '%2b'),
      SortBy: sortValue,
      Status: status ? status : '',
      SortByDesc: desc,
    };
    allAgentsApi(params)
      .then((response) => {
        setAgents(response?.data.data.result);
        setPageCount(response?.data.data.totalPages);
        console.log(response);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      })
      .catch((err) => {
        console.log(err);
      });
    setApiCallAgent(false);
  };

  //   agent adding function
  const addAgents = (e) => {
    console.log(e);
    if (!e.name.trim()) {
      toast.error('Whitespaces are not allowed!', { toastId: '333' });
      return;
    }
    handleCloseAddModal();
    setLoading(true);
    addAgentsApi(e)
      .then(() => {
          setLoading(false);
          Swal.fire({
            icon: 'success',
            title: 'Added new Agent',
            text: `New Agent, "${e.name.trim()}" added `,
          });
          getAgents();
          handleCloseAddModal();
      })
      .catch((err) => {
        const error = err?.response?.data?.message;
        setLoading(false);
        if (error === 'Agent Already Exist') {
          Swal.fire({
            icon: 'warning',
            title: ' Email  already exists',
            backdrop: true,
            allowOutsideClick: false,
          });
        }
      });
    reset();
  };

  //   agent update function
  const updateAgent = (e) => {
    console.log(getValues('name'));
    console.log(e);
    if (e?.name?.trim() === '') {
      toast.error('Whitespaces are not allowed!', { toastId: '555' });
      return;
    }
    handleEditCloseModal();
    setLoading(true);
    editAgents(id, e)
      .then((response) => {
        if (response.data.status) {
          Swal.fire({
            icon: 'success',
            title: 'Agent Details Updated',
          });
          getAgents();
        }
        setLoading(false);
      })
      .catch((err) => {
        console.log('err');
        setLoading(false);
      });
    reset();
  };
  const patch = (agent) => {
    setValue('name', agent.name);
    setValue('email', agent.email);
    setValue('phoneNumber', agent.phoneNumber);
  };
  //agent delete//
  const deleteAgent = async (id, name, status) => {
    Swal.fire({
      title: `Delete ${name}?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: `Delete!`,
    }).then(async (result) => {
      if (result.isConfirmed) {
        deleteAgentsApi(id, status)
          .then((response) => {
            if (response?.data.data) Swal.fire(`Deleted!`, `${name} has been Deleted `, 'success');
            getAgents();
          })
          .catch((err) => {
            console.log(err);
          });
      }
    });
  };
  //bloack unblock agent//
  const blockAgent = async (id, name, status) => {
    let messsage1 = status === 2 ? 'Block' : 'Activate';
    let messsage2 = status === 1 ? 'Activated' : 'Blocked';
    Swal.fire({
      title: `${messsage1} ${name}?`,
      icon: 'info',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: `Yes, ${messsage1}!`,
    }).then(async (result) => {
      if (result.isConfirmed) {
        deleteAgentsApi(id, status)
          .then((response) => {
            if (response?.data.data) Swal.fire(`${messsage2}!`, `${name} has been ${messsage2} `, 'success');
            getAgents();
          })
          .catch((err) => {
            console.log(err);
          });
      }
    });
  };
  const dateConvertion = (date) => {
    let currentDate = new Date(date);
    return currentDate.toDateString();
  };
  const handlePageClick = useCallback((data) => {
    let currentPage = data.selected + 1;
    setPageNo(currentPage);
  }, []);

  const handleSort = (data) => {
    if (data === 'CreatedDate') {
      setDesc(true);
    } else setDesc(false);
  };

  return (
    <>
      <div>
        <div className={deliveryagentcss.boxproductlist}>
          <div className={deliveryagentcss.productbox}>
            <div className={deliveryagentcss['items']}>
              <div className={deliveryagentcss['itemfirsttwo']}>
                <div>
                  <input
                    className={deliveryagentcss['search']}
                    maxLength={255}
                    type="search"
                    placeholder="Search here"
                    onChange={(e) => {
                      setSearchValue(e.target.value);
                      setPageNo(1);
                    }}
                  />
                </div>
                <div className={deliveryagentcss['selectdiv']}>
                  <select
                    className={deliveryagentcss['select']}
                    onChange={(e) => {
                      setStatus(e.target.value);
                      setPageNo(1);
                    }}
                    name="cars"
                    id="cars"
                  >
                    <option className={deliveryagentcss['optionsort']} defaultChecked value="1">
                      Active
                    </option>
                    <option className={deliveryagentcss['optionsort']} value="2">
                      Blocked
                    </option>
                    <option className={deliveryagentcss['optionsort']} value="3">
                      Deleted
                    </option>
                    <option className={deliveryagentcss['optionsort']} value="0">
                      Inactive
                    </option>
                    <option className={deliveryagentcss['optionsort']} value="">
                      All
                    </option>
                  </select>
                  <select
                    data-testid="sortbtn"
                    className={deliveryagentcss.select}
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
                      className={deliveryagentcss.optionsort}
                      value="CreatedDate"
                      defaultChecked
                    >
                      Created Date
                    </option>
                    <option className={deliveryagentcss.optionsort} value="Name">
                      Name
                    </option>
                    <option className={deliveryagentcss.optionsort} value="Email">
                      Email
                    </option>
                  </select>
                </div>
              </div>
              <div className={deliveryagentcss.itemaddagent}>
                <div className={deliveryagentcss.addboxalign}>
                  <Tooltip title="Add Agent" placement="top">
                    <button
                      onClick={handleOpenAgent}
                      className={deliveryagentcss['addcategorybutton']}
                      data-testid="addagentbtn"
                    >
                      <AddIcon className={deliveryagentcss['inline-icon']} />
                      {width > 420 && 'Add Agent'}
                    </button>
                  </Tooltip>
                </div>
              </div>
            </div>
            <Modal open={openAgent} onClose={handleCloseAddModal}>
              <Box sx={modalstyle}>
                <div>
                  <div className={deliveryagentcss['head']}>
                    <Typography className={deliveryagentcss['heading']} data-testid="addagentmodal">
                      Add Agent
                    </Typography>
                  </div>
                  <div className={deliveryagentcss['formdiv']}>
                    <form onSubmit={handleSubmit(addAgents)}>
                      <div className={deliveryagentcss['errorhandle']}>
                        <input
                          className={deliveryagentcss['inputfield']}
                          type="text"
                          data-testid="name-input"
                          placeholder="Agent Name"
                          {...register('name', {
                            required: 'Agent Name required',
                            maxLength: 60,
                          })}
                        ></input>
                        {errors.name && <div className={deliveryagentcss['error']}>{errors.name.message}</div>}
                        {errors.name && errors.name.type === 'maxLength' && (
                          <div className={deliveryagentcss['error']}>Maximum 60 Characters</div>
                        )}
                      </div>
                      <div className={deliveryagentcss['errorhandle']}>
                        <input
                          className={deliveryagentcss['inputfield']}
                          type="text"
                          data-testid="email-input"
                          placeholder="Email"
                          {...register('email', {
                            required: 'Email required ',
                            pattern: {
                              value: /^[A-Z0-9._%+-]+@[A-z0-9.-]+\.[A-Z]{2,254}$/i,
                              message: 'Invalid Email address',
                            },
                            maxLength: 254,
                          })}
                        ></input>
                        {errors.email && <div className={deliveryagentcss['error']}>{errors.email.message}</div>}
                        {errors.email && errors.email.type === 'maxLength' && (
                          <div className={deliveryagentcss['error']}>Maximum Length Exceed</div>
                        )}
                      </div>
                      <div className={deliveryagentcss['errorhandle']}>
                        <input
                          className={deliveryagentcss['inputfield']}
                          type="tel"
                          onInput={(e) => {
                            e.target.value = e.target.value?.replace(/\D/g, '');
                          }}
                          placeholder="Phone Number"
                          data-testid="number-input"
                          {...register('phoneNumber', {
                            required: 'Phone Number Required',
                            maxLength: 10,
                            minLength: 10,
                          })}
                        ></input>
                        {errors.phoneNumber && (
                          <div className={deliveryagentcss['error']}>{errors.phoneNumber.message}</div>
                        )}
                        {errors.phoneNumber && errors.phoneNumber.type === 'maxLength' && (
                          <div className={deliveryagentcss['error']}>Maximum 10 Characters</div>
                        )}
                        {errors.phoneNumber && errors.phoneNumber.type === 'minLength' && (
                          <div className={deliveryagentcss['error']}>Minimum 10 Characters</div>
                        )}
                      </div>

                      <div className={deliveryagentcss['submitcanceldiv']}>
                        <button
                          className={deliveryagentcss['submitbutton']}
                          type="submit"
                          value="submit"
                          data-testid="submit"
                        >
                          Submit
                        </button>
                        <button
                       
                          value="cancel"
                          onClick={() => {
                            handleCloseAddModal();
                            reset();
                          }}
                          className={deliveryagentcss['cancelButton']}
                        >
                          Cancel
                        </button>
                      </div>
                    </form>
                  </div>
                </div>
              </Box>
            </Modal>
            <div>
              <Modal open={editopenAgent} onClose={handleEditCloseModal}>
                <Box sx={editmodalstyle}>
                  <div>
                    <div className={deliveryagentcss['head']}>
                      <Typography className={deliveryagentcss['heading']} data-testid="editagentmodal">
                        Edit Agent
                      </Typography>
                    </div>
                    <div className={deliveryagentcss['formdiv']}>
                      <form onSubmit={handleSubmit(updateAgent)}>
                        <div className={deliveryagentcss['errorhandle']}>
                          <input
                            className={deliveryagentcss['inputfield']}
                            type="tel"
                            data-testid="name-input"
                            placeholder="Agent Name"
                            {...register('name', {
                              required: 'Agent Name required',
                              maxLength: 60,
                            })}
                          ></input>
                          {errors.name && <div className={deliveryagentcss['error']}>{errors.name.message}</div>}
                          {errors.name && errors.name.type === 'maxLength' && (
                            <div className={deliveryagentcss['error']}>Maximum 60 Characters</div>
                          )}
                        </div>
                        <div className={deliveryagentcss['errorhandle']}>
                          <input
                            className={deliveryagentcss['inputfield']}
                            type="text"
                            onInput={(e) => {
                              e.target.value = e.target.value?.replace(/\D/g, '');
                            }}
                            placeholder="Phone Number"
                            data-testid="number-input"
                            {...register('phoneNumber', {
                              required: 'Phone Number Required',
                              maxLength: 10,
                              minLength: 10,
                            })}
                          ></input>
                          {errors.phoneNumber && (
                            <div className={deliveryagentcss['error']}>{errors.phoneNumber.message}</div>
                          )}
                          {errors.phoneNumber && errors.phoneNumber.type === 'maxLength' && (
                            <div className={deliveryagentcss['error']}>Maximum 10 Characters</div>
                          )}
                          {errors.phoneNumber && errors.phoneNumber.type === 'minLength' && (
                            <div className={deliveryagentcss['error']}>Minimum 10 Characters</div>
                          )}
                        </div>
                        <div className={deliveryagentcss['submitcanceldiv']}>
                          <button
                            type="submit"
                            value="submit"
                            data-testid="submitbuttonedit"
                            onClick={() => {
                              console.log(getValues('name'));
                            }}
                            className={deliveryagentcss['submitbutton']}
                          >
                            Submit
                          </button>
                          <button
                            value="cancel"
                            onClick={() => {
                              handleEditCloseModal();
                              reset();
                            }}
                            className={deliveryagentcss['cancelButton']}
                            data-testid="cancelbuttonedit"
                          >
                            Cancel
                          </button>
                        </div>
                      </form>
                    </div>
                  </div>
                </Box>
              </Modal>
            </div>
            <div>
              <div>
                <Paper
                  style={{
                    width: '95%',
                    overflow: 'hidden',
                    margin: 'auto',

                    zIndex: '0',
                  }}
                >
                  <TableContainer className={deliveryagentcss['TableContainer']}>
                    <Table stickyHeader aria-label="sticky table">
                      <TableHead className={deliveryagentcss['TableHead']}>
                        <TableRow>
                          {columns.map((column) => (
                            <TableCell
                              key={column.id}
                              align={column.align}
                              className={deliveryagentcss['tablecellmain']}
                            >
                              {column.label}
                            </TableCell>
                          ))}
                        </TableRow>
                      </TableHead>
                      <TableBody role="checkbox" tabIndex={-1}>
                        {agents.length > 0 ? (
                          agents.map((agent) => {
                            return (
                              <TableRow key={agent.agentId} className={deliveryagentcss['row']}>
                                <TableCell className={deliveryagentcss['TableCell']}>{agent.agentId}</TableCell>
                                <TableCell
                                  className={deliveryagentcss['TableCell']}
                                  style={{
                                    overflow: 'hidden',
                                    lineBreak: 'anywhere',
                                  }}
                                >
                                  {agent.name}
                                </TableCell>
                                <TableCell
                                  className={deliveryagentcss['TableCell']}
                                  style={{
                                    overflow: 'hidden',
                                    lineBreak: 'anywhere',
                                  }}
                                >
                                  {agent.email}
                                </TableCell>

                                <TableCell
                                  className={deliveryagentcss['TableCell']}
                                  style={{
                                    overflow: 'hidden',
                                    lineBreak: 'anywhere',
                                  }}
                                >
                                  {agent.phoneNumber}
                                </TableCell>
                                <TableCell
                                  className={deliveryagentcss['TableCell']}
                                  style={{
                                    overflow: 'hidden',
                                    lineBreak: 'anywhere',
                                  }}
                                >
                                  {dateConvertion(agent.createdDate)}
                                </TableCell>

                                <TableCell className={deliveryagentcss['TableCell']}>
                                  {statusBadge(agent.status)}
                                </TableCell>
                                <TableCell className={deliveryagentcss['TableCell']}>
                                  {agent.status !== 3 && (
                                    <button
                                      disabled={agent.status === 2}
                                      className={deliveryagentcss['button-55']}
                                      data-testid="editagentbutton"
                                      onClick={() => {
                                        setId(agent.agentId);
                                        patch(agent);
                                        handleEditAgentOpen();
                                      }}
                                    >
                                      <Tooltip title="Edit Agent" placement="top" data-testid="editbtn">
                                        <EditIcon style={{ color: 'black' }} />
                                      </Tooltip>
                                    </button>
                                  )}
                                  {(agent.status === 1 || agent.status === 2 || agent.status === 0) && (
                                    <button
                                      data-testid="deletebtn"
                                      className={deliveryagentcss['button-55']}
                                      onClick={() => {
                                        deleteAgent(agent.agentId, agent.name, 3);
                                      }}
                                    >
                                      <Tooltip title="Delete Agent" placement="top">
                                        <DeleteIcon
                                          style={{
                                            color: 'grey',
                                          }}
                                        />
                                      </Tooltip>
                                    </button>
                                  )}
                                  {agent.status === 1 && (
                                    <button
                                      data-testid="blockbtn"
                                      className={deliveryagentcss['button-55']}
                                      onClick={() => {
                                        blockAgent(agent.agentId, agent.name, 2);
                                      }}
                                    >
                                      <Tooltip title="Disable Agent" placement="top">
                                        <CheckCircleOutlineIcon style={{ color: 'green' }} />
                                      </Tooltip>
                                    </button>
                                  )}
                                  {agent.status === 2 && (
                                    <button
                                      data-testid="unblockbtn"
                                      className={deliveryagentcss['button-55']}
                                      onClick={() => {
                                        blockAgent(agent.agentId, agent.name, 1);
                                      }}
                                    >
                                      <Tooltip title="Enable Agent" placement="top">
                                        <BlockIcon style={{ color: 'red' }} />
                                      </Tooltip>
                                    </button>
                                  )}
                                </TableCell>
                              </TableRow>
                            );
                          })
                        ) : (
                          <>
                            {!apiCallAgent && (
                              <tr className={deliveryagentcss['listloader']}>
                                <td>No Match Found</td>
                              </tr>
                            )}
                          </>
                        )}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </Paper>
              </div>
            </div>
          </div>
        </div>

        {loadingagentpage && <div className={deliveryagentcss.loading}>Loading…</div>}
        {apiCallAgent && <Loader />}
        {agents.length !== 0 ? (
          <div type="button" className={deliveryagentcss['paginationdiv']}>
            <ReactPaginate
              className={deliveryagentcss['pagination']}
              previousLabel={'Previous'}
              nextLabel={'Next'}
              breakLabel={'...'}
              pageCount={pageCount}
              marginPagesDisplayed={2}
              pageRangeDisplayed={3}
              forcePage={pageNo - 1}
              onPageChange={handlePageClick}
              containerClassName={deliveryagentcss['paginationjustify']}
              pageClassName={deliveryagentcss['page-item']}
              pageLinkClassName={deliveryagentcss['page-link']}
              previousClassName={deliveryagentcss['page-item']}
              previousLinkClassName={deliveryagentcss['page-item']}
              nextClassName={deliveryagentcss['page-item']}
              nextLinkClassName={deliveryagentcss['page-item']}
              breakClassName={deliveryagentcss['page-item']}
              breakLinkClassName={deliveryagentcss['page-item']}
              activeClassName={deliveryagentcss['page-active']}
              disabledClassName={deliveryagentcss['page-prev-disabled']}
              disabledLinkClassName={deliveryagentcss['page-prev-disabled']}
              prevRel={null}
              prevPageRel={null}
            />
          </div>
        ) : null}
      </div>
    </>
  );
};

export default Agent;
