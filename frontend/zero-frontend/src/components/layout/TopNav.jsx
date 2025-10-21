import { NavLink, Link } from 'react-router-dom';
import { ChevronDown, LogOut, User2 } from 'lucide-react';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger } from '@/components/ui/dropdown-menu';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { useAuth } from '../../context/AuthContext';

// Menú principal (centrado). "children" genera submenú.
const NAV_ITEMS = [
    {
        label: 'Admin',
        roles: ['Admin'],
        children: [
            { label: 'Listar usuarios', to: '/admin/users' },
            { label: 'Añadir roles', to: '/admin/roles/new' },
            { label: 'Asignar roles', to: '/admin/roles/assign' },
        ],
    },
    {
        label: 'Instalaciones',
        roles: ['Admin', 'Instalaciones'],
        children: [
            { label: 'Listar instalaciones', to: '/installations' },
        ],
    },
];

function classNames(...cn){ return cn.filter(Boolean).join(' '); }
function initialsFromEmail(email){
    if(!email) return 'U';
    const name = email.split('@')[0];
    return name.slice(0,2).toUpperCase();
}

export default function TopNav(){
    const { user, logout } = useAuth();
    const roles = user?.roles ?? [];
    const allowed = (item) => !item.roles || item.roles.some(r => roles.includes(r));
    const menu = NAV_ITEMS.filter(allowed);

    return (
        <header className="sticky top-0 z-50 bg-[oklch(var(--primary))] text-[oklch(var(--primary-foreground))]">
            <div className="mx-auto max-w-7xl px-4">
                <div className="h-14 grid grid-cols-[auto_1fr_auto] items-center gap-3">
                    {/* Logo izq */}
                    <Link to="/" className="inline-flex items-center gap-2">
                        <img src="/logo.png" alt="Logo" className="h-7 w-auto drop-shadow-sm" />
                    </Link>

                    {/* Menú centrado */}
                    <nav className="justify-self-center">
                        <ul className="flex items-center gap-8">
                            {menu.map(item => (
                                <li key={item.label}>
                                    {item.children ? (
                                        <DropdownMenu>
                                            <DropdownMenuTrigger className="inline-flex items-center gap-1 text-sm opacity-90 hover:opacity-100">
                                                {item.label}
                                                <ChevronDown className="h-4 w-4" />
                                            </DropdownMenuTrigger>
                                            <DropdownMenuContent align="center">
                                                {item.children.filter(allowed).map(child => (
                                                    <DropdownMenuItem key={child.to} asChild>
                                                        <NavLink to={child.to}>{child.label}</NavLink>
                                                    </DropdownMenuItem>
                                                ))}
                                            </DropdownMenuContent>
                                        </DropdownMenu>
                                    ) : (
                                        <NavLink
                                            to={item.to}
                                            className={({isActive}) => classNames(
                                                'text-sm transition-opacity hover:opacity-100',
                                                isActive ? 'opacity-100 underline underline-offset-8 decoration-[oklch(var(--primary-foreground))]' : 'opacity-90'
                                            )}
                                        >
                                            {item.label}
                                        </NavLink>
                                    )}
                                </li>
                            ))}
                        </ul>
                    </nav>

                    {/* Usuario der */}
                    <div className="justify-self-end">
                        <DropdownMenu>
                            <DropdownMenuTrigger className="inline-flex items-center gap-2 rounded-full bg-white/95 text-slate-900 px-3 py-1.5 text-sm shadow hover:bg-white focus:outline-none">
                                <Avatar className="h-6 w-6">
                                    <AvatarImage />
                                    <AvatarFallback className="text-[10px]">{initialsFromEmail(user?.email)}</AvatarFallback>
                                </Avatar>
                                <span className="hidden sm:inline max-w-[160px] truncate">{user?.email || 'Usuario'}</span>
                                <ChevronDown className="h-4 w-4" />
                            </DropdownMenuTrigger>
                            <DropdownMenuContent align="end" className="w-56">
                                <DropdownMenuLabel className="truncate">{user?.email}</DropdownMenuLabel>
                                <DropdownMenuSeparator />
                                <DropdownMenuItem onClick={() => {}}>
                                    <User2 className="mr-2 h-4 w-4" /> Perfil (próx.)
                                </DropdownMenuItem>
                                <DropdownMenuSeparator />
                                <DropdownMenuItem onClick={logout} className="text-red-600 focus:text-red-600">
                                    <LogOut className="mr-2 h-4 w-4" /> Cerrar sesión
                                </DropdownMenuItem>
                            </DropdownMenuContent>
                        </DropdownMenu>
                    </div>
                </div>
            </div>
        </header>
    );
}