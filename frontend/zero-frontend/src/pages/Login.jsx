import { useState } from 'react';
import { useLocation, useNavigate, Link } from 'react-router-dom';
import { Eye, EyeOff } from 'lucide-react';
import { motion } from 'framer-motion';
import { useAuth } from '../context/AuthContext';
import { login as loginApi } from '../api/auth';


// shadcn/ui
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';


export default function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [showPwd, setShowPwd] = useState(false);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const navigate = useNavigate();
    const location = useLocation();
    const { loginAction } = useAuth();


    const onSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);
        try {
            const data = await loginApi({ email, password });
            loginAction(data.token);
            const to = location.state?.from?.pathname || '/dashboard';
            navigate(to, { replace: true });
        } catch (err) {
            setError('Credenciales inválidas o servidor no disponible.');
        } finally {
            setLoading(false);
        }
    };


    return (
        <div className="min-h-dvh w-full px-4 py-8 grid place-items-center">
            <div className="w-full max-w-5xl bg-white/80 dark:bg-slate-900/70 border border-slate-200/70 dark:border-slate-800/70 rounded-2xl shadow-sm overflow-hidden grid grid-cols-1 md:grid-cols-2">
                {/* Columna izquierda: formulario */}
                <div className="p-6 md:p-10">
                    <div className="mb-6">
                        <h1 className="text-2xl font-semibold">Welcome back</h1>
                        <p className="text-sm text-slate-600 dark:text-slate-400">Login to your account</p>
                    </div>


                    <form onSubmit={onSubmit} className="space-y-5">
                        <div className="space-y-2">
                            <Label htmlFor="email">Email</Label>
                            <Input
                                id="email"
                                type="email"
                                placeholder="m@example.com"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                                autoFocus
                            />
                        </div>


                        <div className="space-y-2">
                            <div className="flex items-center justify-between">
                                <Label htmlFor="password">Password</Label>
                                <button
                                    type="button"
                                    className="text-xs text-slate-600 dark:text-slate-300 hover:underline"
                                    onClick={() => navigate('/forgot-password')}
                                >
                                    Forgot your password?
                                </button>
                            </div>
                            <div className="relative">
                                <Input
                                    id="password"
                                    type={showPwd ? 'text' : 'password'}
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    required
                                    className="pr-10"
                                />
                                <button
                                    type="button"
                                    onClick={() => setShowPwd((v) => !v)}
                                    className="absolute inset-y-0 right-0 pr-3 grid place-items-center text-slate-500 hover:text-slate-700 dark:hover:text-slate-200"
                                    aria-label={showPwd ? 'Hide password' : 'Show password'}
                                >
                                    {showPwd ? <EyeOff className="size-5" /> : <Eye className="size-5" />}
                                </button></div>
                        </div>


                        {error && <p className="text-sm text-red-600 dark:text-red-400">{error}</p>}


                        <Button type="submit" disabled={loading} className="w-full h-10">
                            {loading ? 'Logging in…' : 'Login'}
                        </Button>


                        <div className="pt-2 text-xs text-slate-500 dark:text-slate-400">
                            By clicking continue, you agree to our <a className="underline underline-offset-2" href="#">Terms of Service</a> and <a className="underline underline-offset-2" href="#">Privacy Policy</a>.
                        </div>
                    </form>
                </div>


                {/* Columna derecha: logotipo */}
                <div className="relative hidden md:block bg-slate-100 dark:bg-slate-800">
                    {/* círculo guía estilo mock */}
                    <div className="absolute inset-0 grid place-items-center opacity-30">
                        <div className="relative size-60">
                            <div className="absolute inset-0 rounded-full border border-slate-400/50"/>
                            <div className="absolute inset-6 rounded-full border border-slate-400/40"/>
                            <div className="absolute inset-12 rounded-full border border-slate-400/30"/>
                        </div>
                    </div>
                    {/* Logo */}
                    <div className="absolute inset-0 grid place-items-center">
                        {/* Sustituye /logo.png por tu ruta real o import */}
                        <motion.img
                            src="/logo.png"
                            alt="Company logo"
                            className="max-h-40 w-auto drop-shadow-sm"
                            initial={{ scale: 0.95, opacity: 0 }}
                            animate={{ scale: 1, opacity: 1 }}
                            transition={{ duration: 0.5 }}
                        />
                    </div>
                </div>
            </div>
        </div>
    );
}