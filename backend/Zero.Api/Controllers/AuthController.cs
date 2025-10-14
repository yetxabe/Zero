using Zero.Api.Contracts.Auth;
using Zero.Api.Services;
using Zero.Api.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Zero.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _jwt;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthController(
        IJwtTokenService jwt,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt;
        _roleManager = roleManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var exists = await _userManager.FindByEmailAsync(dto.Email);
        if (exists is not null)
            return BadRequest(new { message = "Email ya registrado." });

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IzaroCode = dto.IzaroCode
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        
        var (token, exp) = await _jwt.CreateAccessTokenAsync(user);
        return Ok(new AuthResponseDto(token, exp));
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return Unauthorized(new { message = "Credenciales inválidas." });

        var check = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
        if (!check.Succeeded)
            return Unauthorized(new { message = "Credenciales inválidas." });

        var (token, exp) = await _jwt.CreateAccessTokenAsync(user);
        return Ok(new AuthResponseDto(token, exp));
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        return Ok(new
        {
            user.Email,
            user.FirstName,
            user.LastName,
            user.IzaroCode
        });
    }
    
    [HttpPost("roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "El nombre de rol es requerido." });

        var exists = await _roleManager.RoleExistsAsync(dto.Name);
        if (exists) return Conflict(new { message = "El rol ya existe." });

        var result = await _roleManager.CreateAsync(new IdentityRole(dto.Name));
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return CreatedAtAction(nameof(GetRoles), new { name = dto.Name }, new { dto.Name });
    }
    [HttpGet("roles")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetRoles()
    {
        var roles = _roleManager.Roles.Select(r => r.Name).ToList();
        return Ok(roles);
    }
    [HttpPost("users/{userId}/roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddUserToRole(string userId, [FromBody] AddUserToRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(new { message = "Usuario no encontrado." });

        var exists = await _roleManager.RoleExistsAsync(dto.RoleName);
        if (!exists) return NotFound(new { message = "Rol no existe." });

        var result = await _userManager.AddToRoleAsync(user, dto.RoleName);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        // Regenerar token si quieres devolver uno con el nuevo rol:
        var (token, exp) = await _jwt.CreateAccessTokenAsync(user);
        return Ok(new { message = "Rol asignado.", token, expiresAtUtc = exp });
    }
    [HttpDelete("users/{userId}/roles/{roleName}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(new { message = "Usuario no encontrado." });

        var inRole = await _userManager.IsInRoleAsync(user, roleName);
        if (!inRole) return NotFound(new { message = "El usuario no tiene ese rol." });

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        var (token, exp) = await _jwt.CreateAccessTokenAsync(user);
        return Ok(new { message = "Rol eliminado.", token, expiresAtUtc = exp });
    }
    [HttpGet("users/{userId}/roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(new { message = "Usuario no encontrado." });

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(roles);
    }

}