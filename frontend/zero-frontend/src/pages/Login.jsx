import { useState } from 'react';
import { login as loginApi } from '../api/auth';
import { useAuth } from '../context/AuthContext.jsx';
import { useLocation, useNavigate } from 'react-router-dom';


export default function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();
    const location = useLocation();
    const { loginAction } = useAuth();


    const onSubmit = async (e) => {
        e.preventDefault();
        setError('');
        try {
            const data = await loginApi({ email, password }); // { token }
            loginAction(data.token);
            const to = location.state?.from?.pathname || '/';
            navigate(to, { replace: true });
        } catch (err) {
            setError('Credenciales inválidas');
        }
    };


    return (
        <div style={{ maxWidth: 380, margin: '4rem auto' }}>
            <h1>Login</h1>
            <form onSubmit={onSubmit}>
                <label>Email</label>
                <input value={email} onChange={e => setEmail(e.target.value)} type="email" required />
                <label>Contraseña</label>
                <input value={password} onChange={e => setPassword(e.target.value)} type="password" required />
                <button type="submit">Entrar</button>
            </form>
            {error && <p style={{ color: 'crimson' }}>{error}</p>}
        </div>
    );
}