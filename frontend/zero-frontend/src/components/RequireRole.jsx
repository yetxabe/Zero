import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';


export default function RequireRole({ children, roles = [] }) {
    const { user, loading } = useAuth();
    if (loading) return <div>Cargando…</div>;
    const hasRole = roles.length === 0 || roles.some(r => user?.roles?.includes(r));
    return hasRole ? children : <Navigate to="/" replace />;
}