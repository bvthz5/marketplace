import React, { useCallback, useEffect, useState } from 'react';

import style from './MyProfile.module.css';
import { Avatar, Button, Card, CardActions, CardContent, CircularProgress, TextField, Tooltip } from '@mui/material';
import { useForm } from 'react-hook-form';
import { editAgentProfile, getAgentProfile, updateProfilePic } from '../../core/api/apiService';
import { toast } from 'react-toastify';
import { FaRegEdit } from 'react-icons/fa';
import Loader from '../../utils/Loader/Loader';
import { validateProfilePicture } from '../../utils/ImageValidation';
import { useDispatch } from 'react-redux';
import { fetchAgentDetails } from './MyProfileSlice';

export const MyProfile = () => {
  const [agent, setAgent] = useState({ name: '', phoneNumber: '', email: '' });

  const [apiCall, setApiCall] = useState(false);

  const [imageUploading, setImageUploading] = useState(false);

  const [newProfilePic, setNewProfilePic] = useState(null);

  const baseImageUrl = process.env.REACT_APP_PROFILEIMAGE_PATH;

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
    reset,
  } = useForm({
    mode: 'onChange',
    defaultValues: { agent: agent },
  });

  useEffect(() => {
    document.title = 'Profile';

    getProfile();
  }, []);

  useEffect(() => {
    if (newProfilePic) handleUpload();
  }, [newProfilePic]);

  const dispatch = useDispatch();

  const getProfile = () => {
    setApiCall(true);
    getAgentProfile()
      .then((response) => {
        setAgent(response?.data?.data);
        setApiCall(false);
        const { name, phoneNumber } = { ...response?.data?.data };
        setValue('agent', { name, phoneNumber });
      })
      .catch((err) => {
        setApiCall(false);
        console.log(err);
      });
  };

  //   uploading new profile picture
  const handleUpload = async () => {
    // image validation size limit 2 MB
    const fileSize = newProfilePic.size / 1024 / 1024; // in MiB
    if (fileSize > 2) {
      setNewProfilePic(null);
      toast.warning('File size exceeds 2 MB', {
        position: 'top-center',
        autoClose: 5000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
        theme: 'light',
      });
    } else {
      // call for image extension validation
      let validtaionResult = validateProfilePicture(newProfilePic);
      if (validtaionResult) {
        let data = new FormData();
        data.append('file', newProfilePic);

        setImageUploading(true);
        updateProfilePic(data)
          .then(() => {
            getAgentProfile()
              .then((response) => {
                dispatch(fetchAgentDetails());
                console.log(response.data);
                setAgent(response?.data?.data);
                setImageUploading(false);

                toast.success('Profile Picture Updated');
              })
              .catch((err) => {
                console.log(err);
                setImageUploading(false);
              });
          })
          .catch((err) => {
            console.log(err);
            toast.error('Update Failed');
          });
      }
    }
  };

  const onSubmit = async (data) => {
    console.log('new data', data.agent);
    console.log('old data', agent);

    if (Object.keys(data.agent).every((key) => agent[key] === data.agent[key].trim())) {
      toast.info('No changes', { toastId: 'no_changes' });
      return;
    }

    editAgentProfile(data.agent)
      .then((response) => {
        console.log(response);
        toast.success('Details updated', { toastId: 'details_updated_toast' });
        setAgent(response.data.data);
      })
      .catch((err) => {
        console.log(err);
        toast.error('Update Failed');
        getProfile();
      });
  };

  const onCancel = useCallback(async () => {
    const { name, phoneNumber } = agent;
    reset({ agent: { name, phoneNumber } });
  }, [agent]);

  return (
    <div className={style.container}>
      <Card sx={{ minWidth: 330 }}>
        <div className={style.profilePic}>
          {!imageUploading ? (
            <Avatar
              className={style.avatar}
              alt={agent.name}
              src={agent?.profilePic ? `${baseImageUrl}agent/profile-pic/${agent.profilePic}` : agent?.name}
            />
          ) : (
            <div className={style.avatarLoader}>
              <CircularProgress color="secondary" />
            </div>
          )}
        </div>
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', paddingTop: '10px' }}>
          <Tooltip title={imageUploading ? 'Please wait...' : 'Change profile picture'}>
            <label>
              <input
                data-testid="image-uploader"
                disabled={imageUploading}
                id="file"
                className={style.input}
                type="file"
                onChange={(event) => {
                  setNewProfilePic(event.target.files[0]);
                }}
              />
              <FaRegEdit style={{ cursor: 'pointer' }} />
            </label>
          </Tooltip>
        </div>

        <CardContent style={{ display: 'flex', flexDirection: 'column', gap: '20px', paddingBottom: '10px' }}>
          <TextField
            data-testid="name-input"
            label="Name"
            variant="outlined"
            {...register('agent.name', {
              validate: (value) => {
                return value.trim() !== '' ? null : 'Not a valid name';
              },
              required: 'Name is required',
              maxLength: {
                value: 60,
                message: 'Maximum 60 characters',
              },
            })}
            error={!!errors?.agent?.name}
            helperText={errors?.agent?.name?.message}
            InputLabelProps={{
              shrink: !!getValues('agent.name'),
            }}
          />

          <TextField
            disabled
            label="Email"
            variant="outlined"
            value={agent.email}
            InputProps={{
              readOnly: true,
            }}
            sx={{ 'input:disabled': { border: 0, fontWeight: 'bold' } }}
            InputLabelProps={{
              shrink: true,
            }}
          />

          <TextField
            data-testid="phoneNumber-input"
            label="Phone Number"
            variant="outlined"
            {...register('agent.phoneNumber', {
              required: 'Phone is required',
              pattern: {
                value: /^\d{10}$/i,
                message: 'Not a valid  phone number',
              },
            })}
            error={!!errors?.agent?.phoneNumber}
            helperText={errors?.agent?.phoneNumber?.message}
            InputLabelProps={{
              shrink: !!getValues('agent.phoneNumber'),
            }}
          />
        </CardContent>
        <CardActions style={{ display: 'flex', justifyContent: 'space-around', width: '100%', paddingBottom: '15px' }}>
          <Button
            style={{ width: '37%', color: '#004fd4', backgroundColor: '#c9d3f0' }}
            data-testid="update-btn"
            size="small"
            onClick={handleSubmit(onSubmit)}
          >
            Update
          </Button>
          <Button
            data-testid="cancel-btn"
            style={{ color: 'red', backgroundColor: 'antiquewhite', width: '37%' }}
            size="small"
            onClick={onCancel}
          >
            Cancel
          </Button>
        </CardActions>
      </Card>
      {apiCall && <Loader />}
    </div>
  );
};
