import api from '../api/axios';
import { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext.jsx';


export default function Dashboard() {
    const [message, setMessage] = useState('');
    const { user, logout } = useAuth();


    useEffect(() => {
        api.get('/api/secure/ping')
            .then(res => setMessage(res.data?.message || 'OK'))
            .catch(() => setMessage('Error llamando API protegida'));
    }, []);


    return (
        <div style={{ maxWidth: 720, margin: '2rem auto' }}>
            <h1>Dashboard</h1>
            <p>Hola {user?.email}</p>
            <p>Respuesta API protegida: {message}</p>
            <button onClick={logout}>Cerrar sesión</button>
        </div>
    );
}