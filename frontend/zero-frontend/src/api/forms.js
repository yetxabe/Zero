import api from "@/api/axios.js";

export async function getForms({ page=1, pageSize=20, search='', category='' } = {}) {
    // Intenta con endpoint paginado
    try {
        const { data } = await api.get('/Form/forms', {
            params: { page, pageSize, search, category }
        });
        return data;
    } catch {
        // Fallback a un array simple
        const { data } = await api.get('/forms');
        return data;
    }
}

// Igual que createUser -> POST
export async function createForm(dto) {
    const { data } = await api.post('/Form/forms', dto);
    return data;
}

// Normalizador compatible con UsersList.jsx
export function normalizePaged(raw, { page, pageSize }) {
    if (Array.isArray(raw)) {
        const total = raw.length;
        const start = (page - 1) * pageSize;
        const end = start + pageSize;
        const items = raw.slice(start, end);
        return {
            items,
            total,
            page,
            pageSize,
            totalPages: Math.max(1, Math.ceil(total / pageSize)),
        };
    }
    const items = raw?.items ?? [];
    const total = Number(raw?.total ?? items.length);
    const p = Number(raw?.page ?? page);
    const ps = Number(raw?.pageSize ?? pageSize);
    return {
        items,
        total,
        page: p,
        pageSize: ps,
        totalPages: Math.max(1, Math.ceil(total / ps)),
    };
}