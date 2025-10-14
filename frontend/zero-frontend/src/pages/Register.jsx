import { useState } from 'react';
import { register as registerApi } from '../api/auth';
import { useNavigate } from 'react-router-dom';


export default function Register() {
    const [form, setForm] = useState({
        email: '',
        password: '',
        firstName: '',
        lastName: '',
        izaroCode: ''
    });
    const [msg, setMsg] = useState('');
    const navigate = useNavigate();


    const onChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });


    const onSubmit = async (e) => {
        e.preventDefault();
        setMsg('');
        try {
            await registerApi(form);
            setMsg('Registro completado. Ahora inicia sesión.');
            setTimeout(() => navigate('/login'), 800);
        } catch (err) {
            setMsg('No se pudo registrar.');
        }
    };


    return (
        <div style={{ maxWidth: 460, margin: '4rem auto' }}>
            <h1>Registro</h1>
            <form onSubmit={onSubmit}>
                <label>Nombre</label>
                <input name="firstName" value={form.firstName} onChange={onChange} required />
                <label>Apellidos</label>
                <input name="lastName" value={form.lastName} onChange={onChange} required />
                <label>Izaro Code</label>
                <input name="izaroCode" value={form.izaroCode} onChange={onChange} />
                <label>Email</label>
                <input name="email" value={form.email} onChange={onChange} type="email" required />
                <label>Contraseña</label>
                <input name="password" value={form.password} onChange={onChange} type="password" required />
                <button type="submit">Crear cuenta</button>
            </form>
            {msg && <p>{msg}</p>}
        </div>
    );
}