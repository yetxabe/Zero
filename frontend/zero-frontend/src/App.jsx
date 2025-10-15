import { Routes, Route, Link, useLocation } from 'react-router-dom';
import RequireAuth from './components/RequireAuth';
import RequireRole from './components/RequireRole';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Admin from './pages/Admin';

function Layout({ children }) {
    const { pathname } = useLocation();
// Ocultar el menú en páginas de autenticación
    const hideNav = pathname.startsWith('/login') || pathname.startsWith('/forgot-password');
    return (
        <>
            {!hideNav && (
                <nav style={{ display: 'flex', gap: 12, padding: 12 }}>
                    <Link to="/">Inicio</Link>
                    <Link to="/dashboard">Dashboard</Link>
                    <Link to="/admin">Admin</Link>
                    <Link to="/login">Login</Link>
                    <Link to="/register">Registro</Link>
                </nav>
            )}
            {children}
        </>
    );
}


export default function App() {
    return (
        <Layout>
            <Routes>
                <Route path="/" element={<div style={{ padding: 24 }}>Home pública</div>} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/dashboard" element={
                    <RequireAuth>
                        <Dashboard />
                    </RequireAuth>
                } />
                <Route path="/admin" element={
                    <RequireAuth>
                        <RequireRole roles={["Admin"]}>
                            <Admin />
                        </RequireRole>
                    </RequireAuth>
                } />
            </Routes>
        </Layout>
    );
}