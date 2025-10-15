import { useLocation } from 'react-router-dom';
import TopNav from './TopNav';

export default function AppLayout({ children }){
    const { pathname } = useLocation();
    const hideNav = pathname.startsWith('/login') || pathname.startsWith('/register') || pathname.startsWith('/forgot-password');
    return (
        <div className="min-h-dvh">
            {!hideNav && <TopNav />}
            <main className="mx-auto max-w-6xl px-4 py-6">{children}</main>
        </div>
    );
}