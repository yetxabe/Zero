import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { getForms, normalizePaged } from '@/api/forms';
import { Button } from '@/components/ui/button';

export default function FormsList(){
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(20);
    const [search, setSearch] = useState('');
    const [category, setCategory] = useState(''); // análogo al filtro "role"

    const [data, setData] = useState({ items: [], total: 0, page: 1, pageSize: 20, totalPages: 1 });
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        (async () => {
            try {
                setLoading(true); setError('');
                const raw = await getForms({ page, pageSize, search, category });
                setData(normalizePaged(raw, { page, pageSize }));
            } catch (e) {
                setError(e.message || 'No se pudo cargar el listado');
            } finally {
                setLoading(false);
            }
        })();
    }, [page, pageSize, search, category]);

    const from = useMemo(() => (data.total === 0 ? 0 : (data.page - 1) * data.pageSize + 1), [data]);
    const to = useMemo(() => Math.min(data.page * data.pageSize, data.total), [data]);

    const onSearchSubmit = (e) => { e.preventDefault(); setPage(1); };
    const onCategoryChange = (e) => { setCategory(e.target.value); setPage(1); };

    return (
        <div className="space-y-4">
            <div className="flex items-center justify-between gap-4 flex-wrap">
                <h1 className="text-xl font-semibold">Formularios</h1>
                <Button asChild>
                    <Link to="/admin/forms/new">Añadir formulario</Link>
                </Button>
            </div>

            {/* Filtros */}
            <form onSubmit={onSearchSubmit} className="flex flex-wrap items-center gap-2">
                <input
                    className="border rounded px-3 py-2 w-64"
                    placeholder="Buscar por nombre…"
                    value={search}
                    onChange={(e)=>setSearch(e.target.value)}
                />
                <input
                    className="border rounded px-3 py-2 w-48"
                    placeholder="Categoría (texto o id)"
                    value={category}
                    onChange={onCategoryChange}
                />
                <Button type="submit" variant="secondary">Buscar</Button>
            </form>

            {loading && <p>Cargando…</p>}
            {error && <p className="text-red-600">{error}</p>}

            {!loading && !error && (
                <>
                    <div className="overflow-x-auto">
                        <table className="min-w-full text-sm">
                            <thead>
                            <tr className="text-left border-b">
                                <th className="py-2 pr-4">Nombre</th>
                                <th className="py-2 pr-4">Categoría</th>
                                <th className="py-2 pr-4">Secciones</th>
                                <th className="py-2 pr-4">Acciones</th>
                            </tr>
                            </thead>
                            <tbody>
                            {data.items.map(f => (
                                <tr key={f.id} className="border-b last:border-b-0">
                                    <td className="py-2 pr-4">{f.name}</td>
                                    <td className="py-2 pr-4">{f.category ?? f.categoryId ?? '-'}</td>
                                    <td className="py-2 pr-4">{f.sections ?? (Array.isArray(f.fields) ? f.fields.length : '-')}</td>
                                    <td className="py-2 pr-4">
                                        <div className="flex gap-2">
                                            <Button variant="secondary" asChild>
                                                <Link to={`/forms/${f.id}`}>Ver</Link>
                                            </Button>
                                            <Button variant="secondary" asChild>
                                                <Link to={`/forms/${f.id}/edit`}>Editar</Link>
                                            </Button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    </div>

                    {/* Paginación */}
                    <div className="flex flex-wrap items-center justify-between gap-3 pt-2">
                        <div className="text-sm text-slate-600 dark:text-slate-400">
                            Mostrando <b>{from}</b>–<b>{to}</b> de <b>{data.total}</b>
                        </div>
                        <div className="flex items-center gap-2">
                            <select
                                className="border rounded px-2 py-1 text-sm"
                                value={pageSize}
                                onChange={(e)=>{ setPageSize(Number(e.target.value)); setPage(1); }}
                            >
                                {[10,20,50,100].map(n => <option key={n} value={n}>{n}/página</option>)}
                            </select>
                            <div className="flex items-center gap-2">
                                <Button
                                    type="button"
                                    variant="secondary"
                                    disabled={data.page <= 1}
                                    onClick={()=> setPage(p => Math.max(1, p-1))}
                                >
                                    Anterior
                                </Button>
                                <span className="text-sm">Página {data.page} / {data.totalPages}</span>
                                <Button
                                    type="button"
                                    variant="secondary"
                                    disabled={data.page >= data.totalPages}
                                    onClick={()=> setPage(p => Math.min(data.totalPages, p+1))}
                                >
                                    Siguiente
                                </Button>
                            </div>
                        </div>
                    </div>
                </>
            )}
        </div>
    );
}