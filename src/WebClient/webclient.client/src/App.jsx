import 'bootstrap/dist/css/bootstrap.min.css';
import { useEffect, useState } from 'react';
import './App.css';
import { Link, useNavigate } from 'react-router-dom';
import userManager from './AuthFiles/authConfig';
import { useAuth } from './Components/AuthProvider';
import { ThreeDots } from 'react-loader-spinner';
import FinishUserRegistrationForm from './Components/FinishUserRegistration/FinishUserRegistrationForm.jsx';
import Profile from './Components/Profile/Profile';

function App() {
    const navigate = useNavigate();
    const { user, userData, loading, isAuthorized } = useAuth();

    const onLogin = () => {
        userManager.signinRedirect();
    };

    const onLogout = async () => {
        await userManager.signoutRedirect();
        navigate('/');
    };

    useEffect(() => {
        if (userData) {
            console.log(userData);
        }
    }, [userData]);

    return (
        <div className="container">
            {loading ? (
                <div className="d-flex justify-content-center align-items-center vh-100">
                    <ThreeDots color="orange" height={80} width={80} />
                </div>
            ) : isAuthorized === false ? (
                    <div className="text-center mt-5">
                        <h1 className="text-center">Cedepp - Test task</h1>
                        <br></br>
                    <div className="btn-group mb-4">
                        <button onClick={onLogin} className="btn btn-primary">Login</button>
                        <button onClick={onLogin} className="btn btn-secondary">Sign Up</button>
                    </div>
                </div>
            ) : userData === null ? (
                <div className="d-flex justify-content-center align-items-center vh-100">
                    <ThreeDots color="#00BFFF" height={80} width={80} />
                </div>
            ) : (
                <div className="mt-5">
                    {userData && userData.isFinished ? (
                        <>
                            <div className="d-flex justify-content-center align-items-center mb-4">
                                <h1 className="text-center">Cedepp - Test task</h1>
                            </div>
                            <Profile />
                        </>
                    ) : (
                        <div>
                            <FinishUserRegistrationForm />
                            <br />
                            <button onClick={onLogout} className="btn btn-danger mt-3">LogOut</button>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
}

export default App;
