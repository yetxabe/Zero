import { Routes, Route } from 'react-router-dom';
import RequireAuth from './components/RequireAuth';
import RequireRole from './components/RequireRole';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import AppLayout from './components/layout/AppLayout';
import StartAtLogin from './components/StartAtLogin';
import UsersList from './pages/admin/UsersList';
import UserCreate from './pages/admin/UserCreate';
import UserEdit from './pages/admin/UserEdit';
import RolesCreate from './pages/admin/RolesCreate';
import RolesAssign from './pages/admin/RolesAssign';

export default function App(){
    return (
        <AppLayout>
            <Routes>
                {/* Ruta raíz decide adónde ir según token */}
                <Route path="/" element={<StartAtLogin />} />

                <Route path="/login" element={<Login />} />

                <Route path="/dashboard" element={
                    <RequireAuth>
                        <Dashboard />
                    </RequireAuth>
                } />

                <Route path="/admin/users" element={
                    <RequireAuth>
                        <RequireRole roles={["Admin"]}>
                            <UsersList />
                        </RequireRole>
                    </RequireAuth>
                } />
                <Route path="/admin/users/new" element={
                    <RequireAuth>
                        <RequireRole roles={["Admin"]}>
                            <UserCreate />
                        </RequireRole>
                    </RequireAuth>
                } />
                <Route path="/admin/users/edit" element={
                    <RequireAuth>
                        <RequireRole roles={["Admin"]}>
                            <UserEdit />
                        </RequireRole>
                    </RequireAuth>
                } />
                <Route path="/admin/roles/new" element={
                    <RequireAuth>
                        <RequireRole roles={["Admin"]}>
                            <RolesCreate />
                        </RequireRole>
                    </RequireAuth>
                } />
                <Route path="/admin/roles/assign" element={
                    <RequireAuth>
                        <RequireRole roles={["Admin"]}>
                            <RolesAssign />
                        </RequireRole>
                    </RequireAuth>
                } />

                {/* catch-all opcional */}
                <Route path="*" element={<StartAtLogin />} />
            </Routes>
        </AppLayout>
    );
}