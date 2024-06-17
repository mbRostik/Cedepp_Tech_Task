import React, { useState } from 'react';
import axios from 'axios';
import { useAuth } from '../AuthProvider';
import config from '../../config.json';
import userManager from '../../AuthFiles/authConfig';
import './FinishUserRegistrationForm.css';
import { useNavigate } from 'react-router-dom';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import 'bootstrap/dist/css/bootstrap.min.css';



const FinishUserRegistrationForm = () => {
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

    const { user } = useAuth();
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);

            const response = await fetch(`${config.apiBaseUrl}/FinishRegistration`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                toast.success('Registration finished successfully');
                await userManager.signoutRedirect();
                navigate('/');
            } else {
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
            
            }
        } catch (error) {
            console.error('Error:', error);
            toast.error('An error occurred while finishing registration.');
        }
    };

    return (
        <div className="container mt-5">
            <form onSubmit={handleSubmit} className="registration-form">
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

                <button type="submit" className="btn btn-primary">Finish Registration</button>
            </form>
            <ToastContainer />
        </div>
    );
};

export default FinishUserRegistrationForm;
