import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createUser } from '@/api/users.js';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Switch } from '@/components/ui/switch';
import { Eye, EyeOff } from 'lucide-react';

export default function UserCreate(){
    const navigate = useNavigate();
    const [showPwd, setShowPwd] = useState(false);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const [form, setForm] = useState({
        email: '',
        password: '',
        firstName: '',
        lastName: '',
        izaroCode: '',
        rolesText: '', // "Admin, HR"
    });

    const onChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

    const onSubmit = async (e) => {
        e.preventDefault();
        setError(''); setSuccess('');
        try {
            setSaving(true);
            const roles = form.rolesText.split(',').map(r => r.trim()).filter(Boolean);
            const dto = {
                email: form.email,
                password: form.password,
                firstName: form.firstName,
                lastName: form.lastName,
                izaroCode: form.izaroCode,
                phoneNumber: form.phoneNumber || null,
                emailConfirmed: form.emailConfirmed,
                roles: roles.length ? roles : null,
            };
            await createUser(dto);
            setSuccess('Usuario creado correctamente.');
            setTimeout(()=> navigate('/admin/users'), 800);
        } catch (e) {
            const status = e?.response?.status;
            const data = e?.response?.data;
            if (status === 409 && data?.message) {
                setError(data.message); // email duplicado
            } else if (status === 400 && data?.message) {
                const extra = Array.isArray(data.roles) ? `: ${data.roles.join(', ')}` : '';
                setError(`${data.message}${extra}`);
            } else if (Array.isArray(data?.errors)) {
                setError(data.errors.join(''));
            } else {
                setError(e.message || 'No se pudo crear el usuario');
            }
        } finally { setSaving(false); }
    };

    return (
        <div className="max-w-2xl space-y-5">
            <h1 className="text-xl font-semibold">Añadir usuario</h1>
            <form onSubmit={onSubmit} className="space-y-5">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <div>
                        <Label htmlFor="email">Email</Label>
                        <Input id="email" name="email" type="email" value={form.email} onChange={onChange} required />
                    </div>
                    <div>
                        <Label htmlFor="password">Contraseña</Label>
                        <div className="relative">
                            <Input id="password" name="password" type={showPwd?'text':'password'} value={form.password} onChange={onChange} required className="pr-10" />
                            <button type="button" onClick={()=>setShowPwd(v=>!v)} className="absolute inset-y-0 right-0 pr-3 grid place-items-center text-slate-500">
                                {showPwd ? <EyeOff className="size-5"/> : <Eye className="size-5"/>}
                            </button>
                        </div>
                    </div>
                    <div>
                        <Label htmlFor="firstName">Nombre</Label>
                        <Input id="firstName" name="firstName" value={form.firstName} onChange={onChange} />
                    </div>
                    <div>
                        <Label htmlFor="lastName">Apellidos</Label>
                        <Input id="lastName" name="lastName" value={form.lastName} onChange={onChange} />
                    </div>
                    <div>
                        <Label htmlFor="izaroCode">IzaroCode</Label>
                        <Input id="izaroCode" name="izaroCode" value={form.izaroCode} onChange={onChange} />
                    </div>
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