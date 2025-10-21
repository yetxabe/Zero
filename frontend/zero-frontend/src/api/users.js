import api from './axios';

// Ajusta las rutas si en tu backend son diferentes
export const getUsers = async ({ page = 1, pageSize = 20, search = '', role = '' } = {}) => {
    const params = { page, pageSize };
    if (search) params.search = search;
    if (role) params.role = role;
    const { data } = await api.get('/admin/users', { params });
    return data; // PagedResult
};

export const getUserById = async (id) => {
    const { data } = await api.get(`/admin/users/${id}`);
    return data; // { id, email, firstName, lastName, roles: [] }
};

export const updateUser = async (id, dto) => {
    // PUT /api/admin/users/{userId}
    // dto = { email, firstName, lastName, izaroCode, roles? }
    const res = await api.put(`/admin/users/${id}`, dto);
    return res.data; // UserListItemDto (con EmailConfirmed y Roles actualizados)
};

// Helper para normalizar diferentes nombres de propiedades en PagedResult
export function normalizePaged(result, fallback = {}) {
    const items = result.items ?? result.data ?? result.results ?? [];
    const total = result.total ?? result.totalCount ?? result.count ?? items.length ?? 0;
    const page = result.page ?? result.currentPage ?? fallback.page ?? 1;
    const pageSize = result.pageSize ?? result.page_size ?? fallback.pageSize ?? 20;
    const totalPages = result.totalPages ?? result.total_pages ?? (pageSize ? Math.ceil(total / pageSize) : 1);
    return { items, total, page, pageSize, totalPages };
}