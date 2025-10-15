import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function StartAtLogin(){
    const { token, loading } = useAuth();
    if (loading) return null; // o un spinner
    return <Navigate to={token ? '/dashboard' : '/login'} replace />;
}