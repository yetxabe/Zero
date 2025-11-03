using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Zero.Api.Contracts.Form;
using Zero.Api.Data;
using Zero.Api.Models.Form;

namespace Zero.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class FormController : ControllerBase
{
    private readonly AppDbContext _db;
    public FormController(AppDbContext db) => _db = db;

    /// Crea un formulario con secciones, campos y opciones (transacción + reintentos)
    [HttpPost("forms")]
    public async Task<ActionResult<FormDetailsDto>> Create([FromBody] CreateFormDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "El nombre del formulario es obligatorio." });
        if (dto.Sections is null || dto.Sections.Count == 0)
            return BadRequest(new { message = "Debes incluir al menos una sección." });

        var allFields = dto.Sections.SelectMany(s => s.Fields).ToList();
        if (allFields.Count == 0)
            return BadRequest(new { message = "Debes incluir al menos un campo." });

        // ✅ VALIDACIONES FUERA DE LA TRANSACCIÓN
        var categoryOk = await _db.FormCategories.AsNoTracking()
            .AnyAsync(c => c.Id == dto.CategoryId, ct);
        if (!categoryOk)
            return NotFound(new { message = $"Categoría {dto.CategoryId} no encontrada." });

        var typeIds = allFields.Select(f => f.FormFieldTypeId).Distinct().ToList();
        var existingTypeIds = await _db.FormFieldTypes
            .Where(t => typeIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(ct);
        var missing = typeIds.Except(existingTypeIds).ToList();
        if (missing.Any())
            return BadRequest(new { message = "Tipos de campo inválidos.", missingTypeIds = missing });

        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync<ActionResult<FormDetailsDto>>(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                // ✅ SOLO INSERCIONES / UPDATES AQUÍ DENTRO
                var form = new Form
                {
                    Name = dto.Name.Trim(),
                    CategoryId = dto.CategoryId,
                    Sections = new List<FormSection>()
                };
                await _db.Forms.AddAsync(form, ct);
                await _db.SaveChangesAsync(ct); // obtener form.Id

                foreach (var s in dto.Sections)
                {
                    var section = new FormSection
                    {
                        Name = s.Name.Trim(),
                        FormId = form.Id,
                        Fields = new List<FormField>()
                    };

                    foreach (var f in s.Fields)
                    {
                        var field = new FormField
                        {
                            Name = f.Name.Trim(),
                            Description = string.IsNullOrWhiteSpace(f.Description) ? null : f.Description.Trim(),
                            FormFieldTypeId = f.FormFieldTypeId,
                            FormFieldOptions = f.FormFieldOptions?.Select(o => new FormFieldOptions { Name = o.Trim() }).ToList()
                        };
                        section.Fields.Add(field);
                    }

                    form.Sections.Add(section);
                }

                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct); // ✅ único commit

                // Cargar DTO (fuera de la tx ya confirmada)
                var created = await _db.Forms
                    .AsNoTracking()
                    .Where(f => f.Id == form.Id)
                    .Select(f => new FormDetailsDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                        CategoryId = f.CategoryId,
                        CategoryName = f.Category.Name,
                        Sections = f.Sections
                            .AsQueryable()
                            .OrderBy(s => s.Id)
                            .Select(s => new FormSectionDetailsDto
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Fields = s.Fields
                                    .AsQueryable()
                                    .OrderBy(ff => ff.Id)
                                    .Select(ff => new FormFieldDetailsDto
                                    {
                                        Id = ff.Id,
                                        Name = ff.Name,
                                        Description = ff.Description,
                                        FormFieldTypeId = ff.FormFieldTypeId,
                                        FormFieldTypeName = ff.FormFieldType.Name,
                                        Options = ff.FormFieldOptions
                                            .AsQueryable()
                                            .Select(o => o.Name)
                                            .ToList()
                                    }).ToList()
                            }).ToList()
                    })
                    .FirstAsync(ct);

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch 
            {
                // 🔒 Solo intenta rollback si sigue activa
                try
                {
                    // Disponible desde EF Core (using Microsoft.EntityFrameworkCore.Storage)
                    if (_db.Database.CurrentTransaction?.GetDbTransaction()?.Connection is not null)
                        await _db.Database.CurrentTransaction!.RollbackAsync(ct);
                }
                catch
                {
                    // si ya está “zombie”, ignoramos esta segunda excepción
                }
                throw;
            }
        });
    }

    /// Obtiene un formulario (secciones, campos y opciones)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FormDetailsDto>> GetById([FromRoute] int id, CancellationToken ct)
    {
        var form = await _db.Forms
            .AsNoTracking()
            .Include(f => f.Category)
            .Include(f => f.Sections)
                .ThenInclude(s => s.Fields!)
                    .ThenInclude(ff => ff.FormFieldOptions)
            .Include(f => f.Sections)
                .ThenInclude(s => s.Fields!)
                    .ThenInclude(ff => ff.FormFieldType)
            .Where(f => f.Id == id)
            .Select(f => new FormDetailsDto
            {
                Id = f.Id,
                Name = f.Name,
                CategoryId = f.CategoryId,
                CategoryName = f.Category.Name,
                Sections = f.Sections
                    .OrderBy(s => s.Id)
                    .Select(s => new FormSectionDetailsDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Fields = s.Fields
                            .OrderBy(ff => ff.Id)
                            .Select(ff => new FormFieldDetailsDto
                            {
                                Id = ff.Id,
                                Name = ff.Name,
                                Description = ff.Description,
                                FormFieldTypeId = ff.FormFieldTypeId,
                                FormFieldTypeName = ff.FormFieldType.Name,
                                Options = (ff.FormFieldOptions ?? new List<FormFieldOptions>()).Select(o => o.Name).ToList()
                            }).ToList()
                    }).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (form is null) return NotFound(new { message = $"Formulario {id} no encontrado." });
        return Ok(form);
    }

    /// Listado básico
    [HttpGet("forms")]
    public async Task<ActionResult<IEnumerable<object>>> GetAll(CancellationToken ct)
    {
        var list = await _db.Forms
            .AsNoTracking()
            .Include(f => f.Category)
            .OrderBy(f => f.Name)
            .Select(f => new
            {
                f.Id,
                f.Name,
                Category = f.Category.Name,
                Sections = f.Sections.Count
            })
            .ToListAsync(ct);

        return Ok(list);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<FormCategoryDto>>> GetAllCategories(CancellationToken ct)
    {
        var list = await _db.FormCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new FormCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                FormsCount = c.Forms.Count
            })
            .ToListAsync(ct);
        return Ok(list);   
    }
    [HttpPost("create-category")]
    public async Task<ActionResult<FormCategoryDto>> CreateCategory(
        [FromBody] CreateFormCategoryDto dto,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "El nombre de la categoría es obligatorio." });

        // Estrategia de reintentos + transacción (coherente con FormsController)
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync<ActionResult<FormCategoryDto>>(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                // (Opcional) Validación de unicidad por nombre case-insensitive
                var exists = await _db.FormCategories
                    .AsNoTracking()
                    .AnyAsync(c => c.Name.ToLower() == dto.Name.Trim().ToLower(), ct);
                if (exists)
                {
                    await tx.RollbackAsync(ct);
                    return Conflict(new { message = $"Ya existe una categoría con el nombre '{dto.Name.Trim()}'." });
                }

                var category = new FormCategory
                {
                    Name = dto.Name.Trim()
                };

                _db.FormCategories.Add(category);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                var result = new FormCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    FormsCount = 0
                };

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        });
    }
}