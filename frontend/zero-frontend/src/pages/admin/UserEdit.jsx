import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { getUserById, updateUser } from '../../api/users';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Switch } from '@/components/ui/switch';
import { useAuth } from '../../context/AuthContext';

export default function UserEdit(){
    const { id } = useParams();
    const navigate = useNavigate();
    const { user: currentUser } = useAuth();

    const [form, setForm] = useState({
        email: '',
        firstName: '',
        lastName: '',
        izaroCode: '',
        rolesText: '', // fallback si no tienes selector de roles
    });

    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    useEffect(() => {
        (async () => {
            try { setLoading(true); setError('');
                const u = await getUserById(id);
                setForm({
                    email: u.email || '',
                    firstName: u.firstName || '',
                    lastName:  u.lastName  || '',
                    izaroCode: u.izaroCode || '',
                    rolesText: Array.isArray(u.roles) ? u.roles.join(', ') : ''
                });
            } catch (e) { setError('No se pudo cargar el usuario'); }
            finally { setLoading(false); }
        })();
    }, [id]);

    const onChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

    const onSubmit = async (e) => {
        e.preventDefault();
        setError(''); setSuccess('');
        try {
            setSaving(true);
            // Construye el DTO para UpdateUserDto
            const roles = form.rolesText.split(',').map(r => r.trim()).filter(Boolean);

            const dto = {
                email: form.email,
                firstName: form.firstName,
                lastName: form.lastName,
                izaroCode: form.izaroCode,
                roles: roles.length ? roles : null,
            };

            await updateUser(id, dto);
            setSuccess('Usuario actualizado correctamente.');
            setTimeout(() => navigate('/admin/users'), 800);
        } catch (e) {
            // Manejo de errores específicos del backend
            const status = e?.response?.status;
            const data = e?.response?.data;
            if (status === 409 && data?.message) {
                setError(data.message); // "El email ya está en uso por otro usuario."
            } else if (status === 400 && data?.message) {
                // Puede ser: "Roles desconocidos.", "No puedes quitarte...", etc.
                const extra = Array.isArray(data.roles) ? `: ${data.roles.join(', ')}` : '';
                setError(`${data.message}${extra}`);
            } else if (status === 404 && data?.message) {
                setError(data.message);
            } else if (Array.isArray(data?.errors)) {
                setError(data.errors.join(' '));
            } else {
                setError('No se pudo guardar');
            }
        } finally {
            setSaving(false);
        }
    };

    if (loading) return <p>Cargando…</p>;
    if (error && !saving && !success && !form.email) return <p className="text-red-600">{error}</p>;

    const isEditingSelf = currentUser?.email && currentUser.email.toLowerCase() === form.email?.toLowerCase();

    return (
        <div className="max-w-2xl space-y-5">
            <h1 className="text-xl font-semibold">Editar usuario</h1>

            <form onSubmit={onSubmit} className="space-y-5">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <div>
                        <Label htmlFor="firstName">Nombre</Label>
                        <Input id="firstName" name="firstName" value={form.firstName} onChange={onChange} />
                    </div>
                    <div>
                        <Label htmlFor="lastName">Apellidos</Label>
                        <Input id="lastName" name="lastName" value={form.lastName} onChange={onChange} />
                    </div>
                    <div>
                        <Label htmlFor="email">Email</Label>
                        <Input id="email" name="email" type="email" value={form.email} onChange={onChange} required />
                    </div>
                    <div>
                        <Label htmlFor="izaroCode">IzaroCode</Label>
                        <Input id="izaroCode" name="izaroCode" value={form.izaroCode} onChange={onChange} />
                    </div>
                </div>

                {/* Roles: entrada simple separada por comas */}
                <div>
                    <Label htmlFor="rolesText">Roles (separados por coma)</Label>
                    <Input id="rolesText" name="rolesText" value={form.rolesText} onChange={onChange} placeholder="Admin, HR" />
                    {isEditingSelf && form.rolesText && !form.rolesText.toLowerCase().includes('admin') && (
                        <p className="text-xs text-amber-600 mt-1">Aviso: estás quitándote el rol Admin a ti mismo. El backend lo impedirá si eres el último Admin.</p>
                    )}
                </div>

                {error && <p className="text-red-600 whitespace-pre-line">{error}</p>}
                {success && <p className="text-emerald-600">{success}</p>}

                <div className="flex gap-2">
                    <Button type="submit" disabled={saving}>{saving ? 'Guardando…' : 'Guardar cambios'}</Button>
                    <Button type="button" variant="secondary" onClick={()=>navigate('/admin/users')}>Cancelar</Button>
                </div>
            </form>
        </div>
    );
}