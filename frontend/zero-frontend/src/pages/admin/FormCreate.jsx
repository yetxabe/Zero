import React, { useEffect, useMemo, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import {
    Select,
    SelectTrigger,
    SelectContent,
    SelectItem,
    SelectValue,
} from "@/components/ui/select";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

// --- API base + auth (ajusta si usas otro método) ---
const api = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:5000",
});
api.interceptors.request.use((config) => {
    const token = localStorage.getItem("access_token");
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

// Tipos que requieren opciones
const TYPES_WITH_OPTIONS = new Set(["select", "radio", "checkbox"]);

// Fallbacks si aún no expones catálogos
const FALLBACK_TYPES = [
    { id: 1, name: "checkbox" },
    { id: 2, name: "radio" },
    { id: 3, name: "true-false" },
    { id: 4, name: "textarea" },
    { id: 5, name: "firma" },
];
const FALLBACK_CATEGORIES = [
    { id: 1, name: "Instalaciones" },
];

export default function FormCreate() {
    const navigate = useNavigate();

    // catálogos
    const [types, setTypes] = useState(FALLBACK_TYPES);
    const [categories, setCategories] = useState(FALLBACK_CATEGORIES);

    // estado UI
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    // datos básicos
    const [name, setName] = useState("");
    const [categoryId, setCategoryId] = useState("");

    // secciones: [{ name, fields: [{ name, description, formFieldTypeId, formFieldOptions: [""] }] }]
    const [sections, setSections] = useState([
        {
            name: "",
            fields: [
                {
                    name: "",
                    description: "",
                    formFieldTypeId: "",
                    formFieldOptions: [""], // strings
                },
            ],
        },
    ]);

    useEffect(() => {
        (async () => {
            try {
                const [catRes, typeRes] = await Promise.allSettled([
                    api.get("/api/Form/categories"),
                    api.get("/api/Form/form-field-types"),
                ]);
                if (catRes.status === "fulfilled" && Array.isArray(catRes.value.data))
                    setCategories(catRes.value.data);
                if (typeRes.status === "fulfilled" && Array.isArray(typeRes.value.data))
                    setTypes(typeRes.value.data);
            } catch {}
        })();
    }, []);

    const typeNameById = useMemo(() => {
        const m = new Map();
        types.forEach((t) => m.set(String(t.id), String(t.name).toLowerCase()));
        return m;
    }, [types]);
    const requiresOptions = (formFieldTypeId) =>
        TYPES_WITH_OPTIONS.has(typeNameById.get(String(formFieldTypeId || "")));

    // ------- helpers mutación -------
    const addSection = () =>
        setSections((prev) => [
            ...prev,
            {
                name: "",
                fields: [
                    { name: "", description: "", formFieldTypeId: "", formFieldOptions: [""] },
                ],
            },
        ]);
    const removeSection = (sIdx) =>
        setSections((prev) => prev.filter((_, i) => i !== sIdx));
    const patchSection = (sIdx, patch) =>
        setSections((prev) =>
            prev.map((s, i) => (i === sIdx ? { ...s, ...patch } : s))
        );

    const addField = (sIdx) =>
        setSections((prev) =>
            prev.map((s, i) =>
                i === sIdx
                    ? {
                        ...s,
                        fields: [
                            ...s.fields,
                            {
                                name: "",
                                description: "",
                                formFieldTypeId: "",
                                formFieldOptions: [""],
                            },
                        ],
                    }
                    : s
            )
        );
    const removeField = (sIdx, fIdx) =>
        setSections((prev) =>
            prev.map((s, i) =>
                i === sIdx ? { ...s, fields: s.fields.filter((_, j) => j !== fIdx) } : s
            )
        );
    const patchField = (sIdx, fIdx, patch) =>
        setSections((prev) =>
            prev.map((s, i) =>
                i === sIdx
                    ? {
                        ...s,
                        fields: s.fields.map((f, j) => (j === fIdx ? { ...f, ...patch } : f)),
                    }
                    : s
            )
        );

    // opciones (array de strings)
    const addOption = (sIdx, fIdx) =>
        setSections((prev) =>
            prev.map((s, i) =>
                i === sIdx
                    ? {
                        ...s,
                        fields: s.fields.map((f, j) =>
                            j === fIdx
                                ? { ...f, formFieldOptions: [...(f.formFieldOptions || []), ""] }
                                : f
                        ),
                    }
                    : s
            )
        );
    const updateOption = (sIdx, fIdx, oIdx, value) =>
        setSections((prev) =>
            prev.map((s, i) =>
                i === sIdx
                    ? {
                        ...s,
                        fields: s.fields.map((f, j) =>
                            j === fIdx
                                ? {
                                    ...f,
                                    formFieldOptions: (f.formFieldOptions || []).map((o, k) =>
                                        k === oIdx ? value : o
                                    ),
                                }
                                : f
                        ),
                    }
                    : s
            )
        );
    const removeOption = (sIdx, fIdx, oIdx) =>
        setSections((prev) =>
            prev.map((s, i) =>
                i === sIdx
                    ? {
                        ...s,
                        fields: s.fields.map((f, j) =>
                            j === fIdx
                                ? {
                                    ...f,
                                    formFieldOptions: (f.formFieldOptions || []).filter(
                                        (_, k) => k !== oIdx
                                    ),
                                }
                                : f
                        ),
                    }
                    : s
            )
        );

    // ------- submit -------
    const onSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setSuccess("");

        if (!name.trim()) return setError("El nombre del formulario es obligatorio.");
        if (!categoryId) return setError("Debes seleccionar una categoría.");

        // validaciones básicas
        for (const s of sections) {
            if (!s.name?.trim()) return setError("Cada sección debe tener un nombre.");
            for (const f of s.fields) {
                if (!f.name?.trim() || !f.formFieldTypeId)
                    return setError("Cada campo necesita nombre y tipo.");
                if (requiresOptions(f.formFieldTypeId)) {
                    const opts = (f.formFieldOptions || []).map((x) => String(x).trim());
                    if (opts.length === 0 || opts.every((x) => x.length === 0)) {
                        return setError("Los tipos con opciones requieren al menos una opción.");
                    }
                }
            }
        }

        // construir DTO EXACTO
        const dto = {
            name: name.trim(),
            categoryId: Number(categoryId),
            sections: sections.map((s) => ({
                name: s.name.trim(),
                fields: s.fields.map((f) => ({
                    name: f.name.trim(),
                    description: (f.description || "").trim(),
                    formFieldTypeId: Number(f.formFieldTypeId),
                    formFieldOptions: requiresOptions(f.formFieldTypeId)
                        ? (f.formFieldOptions || [])
                            .map((o) => String(o || "").trim())
                            .filter((o) => o.length > 0)
                        : [], // envía [] cuando no aplica
                })),
            })),
        };

        setLoading(true);
        try {
            await api.post("/Form/forms", dto);
            setSuccess("Formulario creado correctamente.");
            setTimeout(() => navigate("/forms"), 900);
        } catch (err) {
            const msg =
                err?.response?.data?.message ||
                err?.response?.data?.error ||
                err?.message ||
                "Error al crear el formulario.";
            setError(msg);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="mx-auto max-w-5xl p-4 space-y-6">
            <Card>
                <CardHeader>
                    <CardTitle className="text-2xl">Crear formulario</CardTitle>
                </CardHeader>
                <CardContent>
                    <form onSubmit={onSubmit} className="space-y-8">
                        {/* Datos básicos */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                            <div>
                                <Label>Nombre</Label>
                                <Input
                                    value={name}
                                    onChange={(e) => setName(e.target.value)}
                                    placeholder="p. ej. Solicitud de vacaciones"
                                />
                            </div>
                            <div>
                                <Label>Categoría</Label>
                                <Select value={String(categoryId)} onValueChange={setCategoryId}>
                                    <SelectTrigger>
                                        <SelectValue placeholder="Selecciona categoría" />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {categories.map((c) => (
                                            <SelectItem key={c.id} value={String(c.id)}>
                                                {c.name}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            </div>
                        </div>

                        {/* Secciones + Campos */}
                        <div className="space-y-6">
                            <div className="flex items-center justify-between">
                                <h3 className="text-lg font-semibold">Secciones</h3>
                                <Button type="button" variant="secondary" onClick={addSection}>
                                    Añadir sección
                                </Button>
                            </div>

                            {sections.map((s, sIdx) => (
                                <Card key={sIdx} className="border">
                                    <CardContent className="pt-6 space-y-5">
                                        <div className="grid grid-cols-1 md:grid-cols-6 gap-4">
                                            <div className="md:col-span-4">
                                                <Label>Nombre de la sección</Label>
                                                <Input
                                                    value={s.name}
                                                    onChange={(e) =>
                                                        patchSection(sIdx, { name: e.target.value })
                                                    }
                                                    placeholder="p. ej. Datos básicos"
                                                />
                                            </div>
                                            <div className="md:col-span-2 flex items-end justify-end">
                                                <Button
                                                    type="button"
                                                    variant="ghost"
                                                    onClick={() => removeSection(sIdx)}
                                                >
                                                    Eliminar sección
                                                </Button>
                                            </div>
                                        </div>

                                        {/* Campos dentro de la sección */}
                                        <div className="space-y-3">
                                            <div className="flex items-center justify-between">
                                                <Label className="font-medium">
                                                    Campos ({s.fields.length})
                                                </Label>
                                                <Button
                                                    type="button"
                                                    size="sm"
                                                    variant="outline"
                                                    onClick={() => addField(sIdx)}
                                                >
                                                    Añadir campo
                                                </Button>
                                            </div>

                                            {s.fields.map((f, fIdx) => {
                                                const needsOpts = requiresOptions(f.formFieldTypeId);
                                                return (
                                                    <Card key={`${sIdx}-${fIdx}`} className="border-dashed">
                                                        <CardContent className="pt-6 space-y-4">
                                                            <div className="grid grid-cols-1 md:grid-cols-12 gap-4">
                                                                <div className="md:col-span-4">
                                                                    <Label>Nombre</Label>
                                                                    <Input
                                                                        value={f.name}
                                                                        onChange={(e) =>
                                                                            patchField(sIdx, fIdx, { name: e.target.value })
                                                                        }
                                                                        placeholder="p. ej. Motivo"
                                                                    />
                                                                </div>
                                                                <div className="md:col-span-5">
                                                                    <Label>Descripción</Label>
                                                                    <Textarea
                                                                        value={f.description}
                                                                        onChange={(e) =>
                                                                            patchField(sIdx, fIdx, {
                                                                                description: e.target.value,
                                                                            })
                                                                        }
                                                                        placeholder="Texto de ayuda…"
                                                                    />
                                                                </div>
                                                                <div className="md:col-span-3">
                                                                    <Label>Tipo</Label>
                                                                    <Select
                                                                        value={String(f.formFieldTypeId || "")}
                                                                        onValueChange={(val) => {
                                                                            const next = { formFieldTypeId: Number(val) };
                                                                            if (!requiresOptions(val))
                                                                                next.formFieldOptions = [];
                                                                            else if (
                                                                                !f.formFieldOptions ||
                                                                                f.formFieldOptions.length === 0
                                                                            )
                                                                                next.formFieldOptions = [""];
                                                                            patchField(sIdx, fIdx, next);
                                                                        }}
                                                                    >
                                                                        <SelectTrigger>
                                                                            <SelectValue placeholder="Selecciona tipo" />
                                                                        </SelectTrigger>
                                                                        <SelectContent>
                                                                            {types.map((t) => (
                                                                                <SelectItem key={t.id} value={String(t.id)}>
                                                                                    {t.name}
                                                                                </SelectItem>
                                                                            ))}
                                                                        </SelectContent>
                                                                    </Select>
                                                                </div>
                                                            </div>

                                                            {/* Opciones si aplica */}
                                                            {needsOpts && (
                                                                <div className="space-y-3">
                                                                    <div className="flex items-center justify-between">
                                                                        <Label>Opciones</Label>
                                                                        <Button
                                                                            type="button"
                                                                            size="sm"
                                                                            variant="outline"
                                                                            onClick={() => addOption(sIdx, fIdx)}
                                                                        >
                                                                            Añadir opción
                                                                        </Button>
                                                                    </div>
                                                                    <div className="space-y-2">
                                                                        {(f.formFieldOptions || []).map((opt, oIdx) => (
                                                                            <div key={oIdx} className="flex items-center gap-2">
                                                                                <Input
                                                                                    placeholder={`Opción ${oIdx + 1}`}
                                                                                    value={opt}
                                                                                    onChange={(e) =>
                                                                                        updateOption(
                                                                                            sIdx,
                                                                                            fIdx,
                                                                                            oIdx,
                                                                                            e.target.value
                                                                                        )
                                                                                    }
                                                                                />
                                                                                <Button
                                                                                    type="button"
                                                                                    variant="ghost"
                                                                                    onClick={() =>
                                                                                        removeOption(sIdx, fIdx, oIdx)
                                                                                    }
                                                                                >
                                                                                    Quitar
                                                                                </Button>
                                                                            </div>
                                                                        ))}
                                                                    </div>
                                                                </div>
                                                            )}

                                                            <div className="flex justify-end">
                                                                <Button
                                                                    type="button"
                                                                    variant="ghost"
                                                                    onClick={() => removeField(sIdx, fIdx)}
                                                                >
                                                                    Eliminar campo
                                                                </Button>
                                                            </div>
                                                        </CardContent>
                                                    </Card>
                                                );
                                            })}
                                        </div>
                                    </CardContent>
                                </Card>
                            ))}
                        </div>

                        {/* mensajes */}
                        {error && <p className="text-red-600 whitespace-pre-line">{error}</p>}
                        {success && <p className="text-green-600">{success}</p>}

                        {/* acciones */}
                        <div className="flex justify-end gap-2">
                            <Button
                                type="button"
                                variant="outline"
                                onClick={() => navigate(-1)}
                                disabled={loading}
                            >
                                Cancelar
                            </Button>
                            <Button type="submit" disabled={loading}>
                                {loading ? "Guardando…" : "Crear formulario"}
                            </Button>
                        </div>
                    </form>
                </CardContent>
            </Card>
        </div>
    );
}