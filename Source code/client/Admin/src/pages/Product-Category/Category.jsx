import React, { useEffect, useState, useCallback } from 'react';
import Swal from 'sweetalert2';
import style from './Category.module.css';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableCell from '@mui/material/TableCell';
import TableRow from '@mui/material/TableRow';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import BlockIcon from '@mui/icons-material/Block';
import { Tooltip, Typography } from '@mui/material';
import { useForm as UseForm } from 'react-hook-form';
import { toast } from 'react-toastify';
import { addCategories, allCategories, deleteCategories, editCategories } from '../../core/api/apiService';
import Loader from '../../utils/Loader/Loader';
import Box from '@mui/material/Box';
import useWindowDimensions from '../../utils/WindowDimensions';
import Modal from '@mui/material/Modal';

const Category = () => {
  const { width } = useWindowDimensions();
  const [categories, setCategories] = useState({});
  const [id, setId] = useState(null);
  const [loading, setLoading] = useState(false);
  const [apiCall, setApiCall] = useState(false);
  const [open, setOpen] = React.useState(false);
  const [editopen, setEditOpen] = React.useState(false);
  const handleOpen = () => setOpen(true);
  const handleClose = useCallback(() => {
    setOpen(false);
  }, []);
  const handleEditOpen = () => setEditOpen(true);
  const handleEditClose = useCallback(() => {
    setEditOpen(false);
  }, []);

  const modalstyle = {
    position: 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    width: 400,
    height: '240px',
    bgcolor: 'background.paper',
    borderRadius: '20px',
  };
  useEffect(() => {
    document.title = 'Category';
    localStorage.setItem('pillerValue', 'category');
    getCategories();
  }, []);

  //   function for getting all categories
  const getCategories = async () => {
    setApiCall(true);
    allCategories()
      .then((response) => {
        setCategories(response?.data.data);
      })
      .catch((err) => {
        console.log(err);
      });
    setApiCall(false);
  };

  //   category adding function
  const addCategory = async (e) => {
    if (!e.categoryName.trim()) {
      toast.error('Whitespaces are not allowed!');
      return;
    }
    setLoading(true);
    addCategories(e)
      .then((response) => {
        setLoading(false);
        if (response.data.status) {
          Swal.fire({
            icon: 'success',
            title: 'Added new Category',
            text: `New Category, "${e.categoryName.trim()}" added `,
          });

          getCategories();
          setValue('categoryName', '');
          handleClose();
        }
      })
      .catch((err) => {
        const error = err.response.data.message;
        if (error === 'Category Already Exists') {
          Swal.fire({
            icon: 'warning',
            title: 'This category  already exists',
            text: `"${e.categoryName}"  already exist `,
            backdrop: true,
            allowOutsideClick: false,
          });
        }
        handleClose();
      });
    setLoading(false);
    reset();
  };

  //   category update function
  const updateCategory = async (e) => {
    if (e.categoryName.trim() === '') {
      toast.error('Whitespaces are not allowed!');
      return;
    }
    setLoading(true);
    editCategories(id, e)
      .then((response) => {
        if (response.data.status) {
          Swal.fire({
            icon: 'success',
            title: 'Category name changed',
            text: `Category name changed to "${e.categoryName.trim()}"`,
          });
          handleEditClose();
          getCategories();
        }
        setLoading(false);
      })
      .catch((err) => {
        const newError = `Category Already Exists`;
        if (err.response.data.message === newError) {
          Swal.fire({
            icon: 'warning',
            title: 'This category already exists',
            text: `"${e.categoryName}" already exist `,
          });
        }
        handleEditClose();
      });
    setLoading(false);
    reset();
  };
  const patch = (categoryName) => {
    setValue('categoryName', categoryName);
  };

  //   category enable/disable function //
  const deleteCategory = async (id, category, status) => {
    let messsage = status === 0 ? 'Enable' : 'Disable';

    Swal.fire({
      title: `${messsage} ${category} ?`,
      icon: 'info',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: `Yes, ${messsage} it!`,
    }).then(async (result) => {
      if (result.isConfirmed) {
        deleteCategories(id, status)
          .then((response) => {
            if (response?.data.status) {
              Swal.fire(`${messsage}d!`, `${category} has been ${messsage}d `, 'success');
              getCategories();
            }
          })
          .catch((err) => {
            console.log(err);
          });
      }
    });
  };
  const columns = [
    { id: 'id', label: 'ID', minWidth: 1 },
    { id: 'category', label: 'Category Name', minWidth: 5 },
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

  const statusBadge = (status) => {
    switch (status) {
      case 1:
        return (
          <div className={style['statuscss']} style={{ color: 'green', backgroundColor: 'rgba(7, 92, 49, 0.16)' }}>
            Active
          </div>
        );
      case 0:
      default:
        return (
          <div className={style['statuscss']} style={{ color: '#4a63ee', backgroundColor: '#0f1ef129' }}>
            Inactive
          </div>
        );
    }
  };
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    reset,
  } = UseForm({ mode: 'onChange' });

  return (
    <>
      <div data-testid="categorypage">
        <div className={style.boxproductlist}>
          <div className={style.productbox}>
            <div className={style.addboxalign}>
              <Tooltip title="Add Category" placement="top">
                <button onClick={handleOpen} className={style['addcategorybutton']} data-testid="addcategorybutton">
                  <AddIcon className={style['inline-icon']} />
                  {width > 420 && 'Add Category'}
                </button>
              </Tooltip>
            </div>
            <Modal open={open} onClose={handleClose}>
              <Box sx={modalstyle}>
                <div>
                  <div className={style['head']}>
                    <Typography className={style['heading']}>Add Category</Typography>
                  </div>
                  <div className={style['formdiv']}>
                    <form onSubmit={handleSubmit(addCategory)}>
                      <div className={style['errorhandle']}>
                        <input
                          data-testid="Add-input"
                          className={style['inputfield']}
                          type="text"
                          placeholder="Category"
                          {...register('categoryName', {
                            required: 'Category Name required',
                            maxLength: 20,
                            pattern: {
                              value: /^[a-z\s]*$/i,
                              message: 'Only Alphabets and spaces are allowed',
                            },
                          })}
                        ></input>
                        {errors.categoryName && <div className={style['error']}>{errors.categoryName.message}</div>}
                        {errors.categoryName && errors.categoryName.type === 'maxLength' && (
                          <div className={style['error']}>Maximum 20 Characters</div>
                        )}
                      </div>
                      <div className={style['submitcanceldiv']}>
                        <button
                          className={style['submitbutton']}
                          type="submit"
                          value="submit"
                          data-testid="submitbutton"
                        >
                          Submit
                        </button>
                        <button
                          data-testid="cancelbutton"
                          onClick={() => {
                            handleClose();
                            setValue('categoryName', '');
                            reset();
                          }}
                          className={style['cancelButton']}
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
              <Modal open={editopen} onClose={handleEditClose}>
                <Box sx={modalstyle}>
                  <div>
                    <div className={style['head']}>
                      <Typography className={style['heading']}>Edit Category</Typography>
                    </div>
                    <div className={style['formdiv']}>
                      <form onSubmit={handleSubmit(updateCategory)}>
                        <div className={style['errorhandle']}>
                          <input
                            data-testid="category-input"
                            className={style['inputfield']}
                            type="text"
                            placeholder="Category"
                            {...register('categoryName', {
                              required: 'Category Name Required',
                              maxLength: 20,
                              pattern: {
                                value: /^[a-z\s]*$/i,
                                message: 'Only Alphabets and spaces are allowed',
                              },
                            })}
                          ></input>
                          {errors.categoryName && <div className={style['error']}>{errors.categoryName.message}</div>}
                          {errors.categoryName && errors.categoryName.type === 'maxLength' && (
                            <div className={style['error']}>Maximum 20 Characters</div>
                          )}
                        </div>
                        <div className={style['submitcanceldiv']}>
                          <button
                            type="submit"
                            value="submit"
                            className={style['submitbutton']}
                            data-testid="editsubmitbutton"
                          >
                            Submit
                          </button>
                          <button
                            data-testid="canceleditmodal"
                            onClick={() => {
                              handleEditClose();
                              setValue('categoryName', '');
                              reset();
                            }}
                            className={style['cancelButton']}
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
                  <TableContainer className={style['TableContainer']}>
                    <Table stickyHeader aria-label="sticky table">
                      <TableHead className={style['TableHead']}>
                        <TableRow>
                          {columns.map((column) => (
                            <TableCell key={column.id} align={column.align} className={style['tablecellmain']}>
                              {column.label}
                            </TableCell>
                          ))}
                        </TableRow>
                      </TableHead>
                      <TableBody role="checkbox" tabIndex={-1}>
                        {categories.length > 0 ? (
                          categories.map((category) => {
                            return (
                              <TableRow key={category.categoryId} className={style['row']}>
                                <TableCell className={style['TableCell']}>{category.categoryId}</TableCell>
                                <TableCell
                                  className={style['TableCell']}
                                  style={{
                                    overflow: 'hidden',
                                    lineBreak: 'anywhere',
                                  }}
                                >
                                  {category.categoryName}
                                </TableCell>
                                <TableCell className={style['TableCell']}>{statusBadge(category.status)}</TableCell>
                                <TableCell className={style['TableCell']}>
                                  <button
                                    data-testid={`editcategorybutton${category.categoryId}`}
                                    disabled={category.status === 0}
                                    className={style['button-55']}
                                    onClick={() => {
                                      setId(category.categoryId);
                                      handleEditOpen();
                                      patch(category.categoryName);
                                    }}
                                  >
                                    <Tooltip title="Edit category" placement="top">
                                      <EditIcon style={{ color: 'black' }} />
                                    </Tooltip>
                                  </button>
                                  {category.status === 0 && (
                                    <button
                                      data-testid="unblkbtn"
                                      className={style['button-55']}
                                      onClick={() => {
                                        deleteCategory(category.categoryId, category.categoryName, category.status);
                                      }}
                                    >
                                      <Tooltip title="Enable Category" placement="top">
                                        <BlockIcon style={{ color: 'red' }} />
                                      </Tooltip>
                                    </button>
                                  )}
                                  {category.status === 1 && (
                                    <button
                                      data-testid="blkbtn"
                                      className={style['button-55']}
                                      onClick={() => {
                                        deleteCategory(category.categoryId, category.categoryName, category.status);
                                      }}
                                    >
                                      <Tooltip title="Disable Category" placement="top">
                                        <CheckCircleOutlineIcon style={{ color: 'green' }} />
                                      </Tooltip>
                                    </button>
                                  )}
                                </TableCell>
                              </TableRow>
                            );
                          })
                        ) : (
                          <>
                            {!apiCall && (
                              <TableRow className={style['listloader']}>
                                <TableCell>No Match Found</TableCell>
                              </TableRow>
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
        {loading && <div className={style.loading}>Loading…</div>}
        {apiCall && <Loader />}
      </div>
    </>
  );
};

export default Category;
