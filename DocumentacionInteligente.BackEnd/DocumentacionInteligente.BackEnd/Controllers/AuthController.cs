using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocumentacionInteligente.BackEnd.Data;
using DocumentacionInteligente.BackEnd.Models;

namespace DocumentacionInteligente.BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // endpoint para iniciar sesión
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.USUARIOS.FirstOrDefault(u => u.CORREO == request.Correo);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PASSWORD_HASH))
                return Unauthorized(new { message = "Credenciales incorrectas" });

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.NOMBRE),
                new Claim("Correo", user.CORREO),
                new Claim("Rol", user.ROL),
                new Claim("User", "true"),
                new Claim("Admin", user.ROL == "Admin" ? "true" : "false")
            };

            var token = GenerateJwtToken(claims);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        // endpoint para registrar un nuevo usuario
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (_context.USUARIOS.Any(u => u.CORREO == request.Correo))
                return BadRequest("Ya existe un usuario con este correo.");

            var nuevoUsuario = new USUARIOS
            {
                NOMBRE = request.Nombre,
                CORREO = request.Correo,
                PASSWORD_HASH = BCrypt.Net.BCrypt.HashPassword(request.Password),
                ROL = "User",
                CREATE_DATE = DateTime.UtcNow
            };

            _context.USUARIOS.Add(nuevoUsuario);
            _context.SaveChanges();

            return Ok("Usuario registrado exitosamente.");
        }

        // Endpoint TEMPORAL para crear el primer administrador
        [HttpPost("create-first-admin")]
        public IActionResult CreateFirstAdmin([FromBody] RegisterRequest request)
        {
            if (_context.USUARIOS.Any(u => u.CORREO == request.Correo))
                return BadRequest("Ya existe un usuario con este correo.");

            var nuevoAdmin = new USUARIOS
            {
                NOMBRE = request.Nombre,
                CORREO = request.Correo,
                PASSWORD_HASH = BCrypt.Net.BCrypt.HashPassword(request.Password),
                ROL = "Admin",
                CREATE_DATE = DateTime.UtcNow
            };

            _context.USUARIOS.Add(nuevoAdmin);
            _context.SaveChanges();

            return Ok("Primer usuario administrador creado exitosamente.");
        }

        // Test para verificar la conexión a la base de datos
        [HttpGet("testdb")]
        public IActionResult TestDB()
        {
            try
            {
                var usuarios = _context.USUARIOS.ToList();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // Método para generar el token JWT 
        private JwtSecurityToken GenerateJwtToken(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));// Clave secreta
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);// Algoritmo de firma

            return new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],// Emisor
                audience: _config["JwtSettings:Audience"],// Público objetivo
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );
        }
    }

    // Modelos para el login y registro
    public class LoginRequest
    {
        public string Correo { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
    }
}