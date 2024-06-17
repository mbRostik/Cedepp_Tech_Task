import React from 'react';
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import userManager from '../../AuthFiles/authConfig';
import { ThreeDots } from 'react-loader-spinner';
import { useAuth } from './../AuthProvider';
import config from '../../config.json';
import './Profile.css';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const Profile = () => {
    const [isHovered, setIsHovered] = useState(false);
    const { user, userData, loading, isAuthorized, setLoadingState, setIsAuthorizedState, setUserState, setUserDataState } = useAuth();

    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        phoneNumber: '',
        address: '',
        codiceFiscale: '',
        cap: '',
        dayOfBirth: '',
        workplace: ''
    });

    const handleMouseEnter = () => {
        setIsHovered(true);
    }

    const handleMouseLeave = () => {
        setIsHovered(false);
    }

    const handleImageUpload = (e) => {
        const file = e.target.files[0];
        const maxSize = 5 * 1024 * 1024;
        const maxResolution = 1920;

        if (file && !isImageFile(file)) {
            toast.error('Allowed extensions: image/jpeg, image/png, image/svg+xml, image/webp.', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
            e.target.value = null;
            setLoadingState(false);
            return;
        }

        if (file && file.size > maxSize) {
            toast.error('The max size of photo: 2mb.', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
            e.target.value = null;
            setLoadingState(false);
            return;
        }

        if (file) {
            const img = new Image();
            img.onload = () => {
                const width = img.width;
                const height = img.height;
                if (width > maxResolution || height > maxResolution) {
                    toast.error(`The maximum image resolution must be ${maxResolution}x${maxResolution} pixels.`, {
                        position: "top-right",
                        autoClose: 5000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                        progress: undefined,
                    });
                    e.target.value = null;
                    setLoadingState(false);
                    return;
                } else {
                    const reader = new FileReader();
                    reader.onloadend = async () => {
                        const imageData = reader.result;
                        const blob = new Blob([new Uint8Array(imageData)], { type: file.type });
                        const base64Avatar = await new Promise((resolve) => {
                            const reader = new FileReader();
                            reader.onloadend = () => resolve(reader.result.split(',')[1]);
                            reader.readAsDataURL(blob);
                        });
                        try {
                            const accessToken = await userManager.getUser().then(user => user.access_token);
                            const response = await fetch(`${config.apiBaseUrl}/UploadProfilePhoto`, {
                                method: 'POST',
                                headers: {
                                    'Authorization': `Bearer ${accessToken}`,
                                    'Content-Type': 'application/json'
                                },
                                body: JSON.stringify({ photo: base64Avatar })
                            });

                            const data = await response.json();
                            setUserDataState(data);
                            toast.success('Profile photo uploaded successfully.');

                        } catch (err) {
                            toast.error(`Error occurred: ${err.message}`, {
                                position: "top-right",
                                autoClose: 5000,
                                hideProgressBar: false,
                                closeOnClick: true,
                                pauseOnHover: true,
                                draggable: true,
                                progress: undefined,
                            });
                        }
                    };
                    reader.readAsArrayBuffer(file);
                }
            };
            img.src = URL.createObjectURL(file);
        }
    };

    const isImageFile = (file) => {
        const acceptedImageTypes = ['image/jpeg', 'image/png', 'image/svg+xml', 'image/webp'];
        return acceptedImageTypes.includes(file.type);
    };

    const handleImageDelete = async (e) => {
        if (userData.photo == null || (Array.isArray(userData.photo) && userData.photo.length === 0) || userData.photo == '') {
            toast.error('You do not have a photo', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
            e.target.value = null;
            setLoadingState(false);
            return;
        }

        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            await fetch(`${config.apiBaseUrl}/UploadProfilePhoto`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ avatar: "" })
            });

            userData.photo = null;
            toast.success('Profile photo was deleted successfully.', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
        } catch (err) {
            toast.error(err, {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
        }
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value
        });
    };

    const [isEditing, setIsEditing] = useState(false);

    useEffect(() => {
        if (userData) {
            setFormData({
                firstName: userData.firstName,
                lastName: userData.lastName,
                phoneNumber: userData.phoneNumber,
                address: userData.address,
                codiceFiscale: userData.codiceFiscale,
                cap: userData.cap,
                dayOfBirth: userData.dayOfBirth,
                workplace: userData.workplace
            });
        }
    }, [userData]);

    const handleSaveChanges = async (e) => {
        e.preventDefault();
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const response = await fetch(`${config.apiBaseUrl}/ChangeUserProfile`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(formData)
            });

            if (!response.ok) {
                const errorData = await response.json();
                const errors = errorData.errors;

                for (const key in errors) {
                    if (errors.hasOwnProperty(key)) {
                        const errorMessages = errors[key];
                        errorMessages.forEach(message => {
                            toast.error(`${key}: ${message}`, {
                                position: "top-right",
                                autoClose: 5000,
                                hideProgressBar: false,
                                closeOnClick: true,
                                pauseOnHover: true,
                                draggable: true,
                                progress: undefined,
                            });
                        });
                    }
                }
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const accessToken2 = await userManager.getUser().then(user => user.access_token);
            const response2 = await fetch(`${config.apiBaseUrl}/GetUserProfile`, {
                headers: { 'Authorization': `Bearer ${accessToken2}` }
            });

            if (!response2.ok) {
                throw new Error(`HTTP error! Status: ${response2.status}`);
            }

            setUserDataState(await response2.json());
            toast.success('Profile updated successfully');
            setIsEditing(false);
        } catch (error) {
            console.error('Error while sending the request to the UserService', error);
            toast.error('An error occurred while updating the profile');
        } finally {
            setLoadingState(false);
        }
    };
    const onLogout = async () => {
        await userManager.signoutRedirect();
        navigate('/');
    };
    return (
        <div className="ProfileMain">
            <ToastContainer />
            {loading ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="orange" height={80} width={80} />
                </div>
            ) : !isAuthorized ? (
                <div>UnAuthorized</div>
            ) : (
                <div className="container mt-5">
                    <div className="row">
                        <div className="col-lg-4">
                            <div className="Left_ProfileDiv">
                                <div className="avatar-container" onMouseEnter={handleMouseEnter} onMouseLeave={handleMouseLeave}>
                                    <img src={userData.photo ? `data:image/jpeg;base64,${userData.photo}` : "/NoPhoto.jpg"} alt="Avatar" className="avatar img-thumbnail" />
                                    <div className="buttons-container">
                                        <label className="edit-button btn btn-primary">
                                            New
                                            <input type="file" name="clientAvatar" accept="image/*" onChange={handleImageUpload} style={{ display: 'none' }} capture="false" />
                                        </label>
                                        <button className="delete-button btn btn-danger" onClick={handleImageDelete}>Delete</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="col-lg-8">
                            <div className="Right_ProfileDiv card">
                                <div className="card-header">
                                    <h2>Update Profile</h2>
                                </div>
                                <div className="card-body">
                                    <form onSubmit={handleSaveChanges} className="registration-form">
                                        <div className="row">
                                            <div className="col-md-6">
                                                <div className="form-group mb-3">
                                                    <label htmlFor="firstName" className="form-label">First Name:</label>
                                                    <input
                                                        type="text"
                                                        name="firstName"
                                                        value={formData.firstName}
                                                        onChange={handleChange}
                                                        className="form-control"
                                                        id="firstName"
                                                        required
                                                    />
                                                </div>

                                                <div className="form-group mb-3">
                                                    <label htmlFor="lastName" className="form-label">Last Name:</label>
                                                    <input
                                                        type="text"
                                                        name="lastName"
                                                        value={formData.lastName}
                                                        onChange={handleChange}
                                                        className="form-control"
                                                        id="lastName"
                                                        required
                                                    />
                                                </div>

                                                <div className="form-group mb-3">
                                                    <label htmlFor="phoneNumber" className="form-label">Phone Number:</label>
                                                    <input
                                                        type="text"
                                                        name="phoneNumber"
                                                        value={formData.phoneNumber}
                                                        onChange={handleChange}
                                                        className="form-control"
                                                        id="phoneNumber"
                                                        required
                                                    />
                                                </div>

                                                <div className="form-group mb-3">
                                                    <label htmlFor="address" className="form-label">Address:</label>
                                                    <input
                                                        type="text"
                                                        name="address"
                                                        value={formData.address}
                                                        onChange={handleChange}
                                                        className="form-control"
                                                        id="address"
                                                        required
                                                    />
                                                </div>
                                            </div>
                                            <div className="col-md-6">
                                                <div className="form-group mb-3">
                                                    <label htmlFor="codiceFiscale" className="form-label">Codice Fiscale:</label>
                                                    <input
                                                        type="text"
                                                        name="codiceFiscale"
                                                        value={formData.codiceFiscale}
                                                        onChange={handleChange}
                                                        className="form-control"
                                                        id="codiceFiscale"
                                                        required
                                                    />
                                                </div>

                                                <div className="form-group mb-3">
                                                    <label htmlFor="cap" className="form-label">CAP:</label>
                                                    <input
                                                        type="text"
                                                        name="cap"
                                                        value={formData.cap}
                                                        onChange={handleChange}
                                                        className="form-control"
                                                        id="cap"
                                                        required
                                                    />
                                                </div>

                                                <div className="form-group mb-3">
                                                    <label htmlFor="dayOfBirth" className="form-label">Day Of Birth:</label>
                                                    <input
                                                        type="date"
                                                        name="dayOfBirth"
                                                        value={formData.dayOfBirth}
                                                        onChange={handleChange}
                                                        className="form-control"
                                                        id="dayOfBirth"
                                                        required
                                                    />
                                                </div>

                                                <div className="form-group mb-3">
                                                    <label htmlFor="workplace" className="form-label">Workplace:</label>
                                                    <input
                                                        type="text"
                                                        name="workplace"
                                                        value={formData.workplace}
                                                        onChange={handleChange}
                                                        className="form-control"
                                                        id="workplace"
                                                        required
                                                    />
                                                </div>
                                            </div>
                                        </div>
                                        <div className="d-flex justify-content-between mt-4">
                                            <button type="submit" className="btn btn-primary">Save Changes</button>
                                            <button type="button" onClick={onLogout} className="btn btn-danger">LogOut</button>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Profile;