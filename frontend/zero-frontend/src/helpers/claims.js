export function extractRolesFromJwtPayload(payload) {
    if (!payload || typeof payload !== 'object') return [];


// Posibles claves donde pueden venir los roles
    const roleKeys = [
        'role',
        'roles',
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role',
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/groups', // a veces grupos
    ];


    let collected = [];
    for (const key of roleKeys) {
        const val = payload[key];
        if (!val) continue;
        if (Array.isArray(val)) collected = collected.concat(val);
        else collected.push(val);
    }


// Normaliza: quita duplicados, trim y mayúsc/minúsc.
    return [...new Set(collected.map(r => String(r).trim()))];
}


export function extractEmailFromJwtPayload(payload) {
// Busca email/usuario en claims habituales
    const keys = ['email', 'upn', 'unique_name', 'name', 'sub'];
    for (const k of keys) {
        if (payload?.[k]) return payload[k];
    }
    return null;
}