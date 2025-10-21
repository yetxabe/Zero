using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zero.Api.Contracts.Admin;
using Zero.Api.Contracts.Common;
using Zero.Api.Models.Auth;

namespace Zero.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("users")]
    public async Task<ActionResult<PagedResult<UserListItemDto>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? role = null)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;

        var query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(u =>
                (u.Email != null && u.Email.ToLower().Contains(s)) ||
                (u.FirstName != null && u.FirstName.ToLower().Contains(s)) ||
                (u.LastName != null && u.LastName.ToLower().Contains(s)) ||
                (u.IzaroCode != null && u.IzaroCode.ToLower().Contains(s))
            );
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleEntity = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == role);
            if (roleEntity is null)
                return Ok(new PagedResult<UserListItemDto>(page, pageSize, 0, Enumerable.Empty<UserListItemDto>()));
        }

        var total = await query.CountAsync();

        var users = await query
            .OrderBy(u => u.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        // Cargar roles por usuario
        var items = new List<UserListItemDto>(users.Count);
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            if (!string.IsNullOrEmpty(role) && !roles.Contains(role))
                continue; // aplica filtro por rol aquí

            items.Add(new UserListItemDto(
                u.Id,
                u.Email ?? "",
                u.FirstName,
                u.LastName,
                u.IzaroCode,
                roles
            ));
        }
        return Ok(new PagedResult<UserListItemDto>(page, pageSize, total, items));
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing is not null)
            return Conflict(new { message = "El email ya está en uso por otro usuario." });

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IzaroCode = dto.IzaroCode
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
            return BadRequest(new { errors = createResult.Errors.Select(e => e.Description) });

        

        var finalRoles = await _userManager.GetRolesAsync(user);

        return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, new UserListItemDto(
            user.Id,
            user.Email ?? "",
            user.FirstName,
            user.LastName,
            user.IzaroCode,
            finalRoles
        ));
    }
    
    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(new { message = "Usuario no encontrado." });
        var finalRoles = await _userManager.GetRolesAsync(user);

        return Ok(new UserListItemDto(
            user.Id,
            user.Email ?? "",
            user.FirstName,
            user.LastName,
            user.IzaroCode,
            finalRoles
        ));
    }
    
    [HttpPut("users/{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(new { message = "Usuario no encontrado." });

        // Validación de email duplicado si cambia
        if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing is not null && existing.Id != user.Id)
                return Conflict(new { message = "El email ya está en uso por otro usuario." });
        }

        // Actualización de campos básicos
        user.Email = dto.Email;
        user.UserName = dto.Email;
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.IzaroCode = dto.IzaroCode;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return BadRequest(new { errors = updateResult.Errors.Select(e => e.Description) });

        // === NUEVO: actualización de roles (si se envían) ===
        if (dto.Roles is not null)
        {
            // Normaliza y valida existencia de roles
            var requested = dto.Roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Verifica que todos existan
            var allRoleNames = _roleManager.Roles.Select(r => r.Name!).ToList();
            var unknown = requested
                .Where(r => !allRoleNames.Contains(r, StringComparer.OrdinalIgnoreCase))
                .ToList();
            if (unknown.Count > 0)
                return BadRequest(new { message = "Roles desconocidos.", roles = unknown });

            var current = await _userManager.GetRolesAsync(user);

            // Salvaguarda: impedir que un admin se quite a sí mismo el rol Admin
            var actingUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var removingOwnAdmin = string.Equals(user.Id, actingUserId, StringComparison.Ordinal) &&
                               current.Contains("Admin") &&
                               !requested.Contains("Admin", StringComparer.OrdinalIgnoreCase);
            if (removingOwnAdmin)
                return BadRequest(new { message = "No puedes quitarte a ti mismo el rol Admin." });

            // Opcional: evitar quedarse sin ningún Admin en el sistema
            var removingAdminFromTarget = current.Contains("Admin") &&
                                      !requested.Contains("Admin", StringComparer.OrdinalIgnoreCase);
            if (removingAdminFromTarget)
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                var otherAdmins = admins.Count(u => u.Id != user.Id);
                if (otherAdmins == 0)
                    return BadRequest(new { message = "Debe existir al menos un usuario con rol Admin." });
            }

            var rolesToAdd = requested.Except(current, StringComparer.OrdinalIgnoreCase).ToList();
            var rolesToRemove = current.Except(requested, StringComparer.OrdinalIgnoreCase).ToList();

            if (rolesToAdd.Count > 0)
            {
                var addRes = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addRes.Succeeded)
                    return BadRequest(new { errors = addRes.Errors.Select(e => e.Description) });
            }

            if (rolesToRemove.Count > 0)
            {
                var remRes = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!remRes.Succeeded)
                    return BadRequest(new { errors = remRes.Errors.Select(e => e.Description) });
            }
        }

        var finalRoles = await _userManager.GetRolesAsync(user);

        return Ok(new UserListItemDto(
            user.Id,
            user.Email ?? "",
            user.FirstName,
            user.LastName,
            user.IzaroCode,
            finalRoles
        ));
    }
}