using BookStoreApi.Models;
using BookStoreApi.Services;
using BookStoreApi.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UsersService _usersService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UsersService usersService,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthController> logger)
    {
        _usersService = usersService;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        try
        {
            // Verificar si el email ya existe
            var existingUser = await _usersService.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new ErrorResponse { Message = "El email ya está registrado" });
            }

            // Crear nuevo usuario
            var user = new User
            {
                Email = request.Email,
                Username = request.Username,
                Password = request.Password
            };

            await _usersService.CreateAsync(user);

            // Generar token
            var token = GenerateJwtToken(user);

            return Ok(new AuthResponse
            {
                Token = token,
                Username = user.Username
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en el registro de usuario");
            return StatusCode(500, new ErrorResponse { Message = "Error al registrar usuario" });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        try
        {
            var user = await _usersService.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new ErrorResponse { Message = "Credenciales inválidas" });
            }

            var isValid = _usersService.ValidatePassword(user, request.Password);
            if (!isValid)
            {
                return BadRequest(new ErrorResponse { Message = "Credenciales inválidas" });
            }

            var token = GenerateJwtToken(user);

            return Ok(new AuthResponse
            {
                Token = token,
                Username = user.Username
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en el login");
            return StatusCode(500, new ErrorResponse { Message = "Error al iniciar sesión" });
        }
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 