using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocumentacionInteligente.BackEnd.Data;
using DocumentacionInteligente.BackEnd.Models;
using Microsoft.AspNetCore.Authorization;


namespace DocumentacionInteligente.BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Usercontroller : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public Usercontroller(AppDbContext context, IConfiguration config)
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
                return Unauthorized(new { message = "Credenciales inválidas" });

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.NOMBRE),
                new Claim("Correo", user.CORREO),
                new Claim("Rol", user.ROL),
                new Claim("User", "true"),
                new Claim("Admin", user.ROL == "Admin" ? "true" : "false"),
                new Claim("UsuarioId", user.ID.ToString())  // <-- ¡Este es el que necesitas!
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

            // Aquí por seguridad solo permitimos rol "User" en registro estándar
            var rolPermitido = "User";

            var nuevoUsuario = new USUARIOS
            {
                NOMBRE = request.Nombre,
                CORREO = request.Correo,
                PASSWORD_HASH = BCrypt.Net.BCrypt.HashPassword(request.Password),
                ROL = rolPermitido,
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
        [HttpGet("load-users")]
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

        [HttpPut("update-user/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] RegisterRequest request)
        {
            var usuario = _context.USUARIOS.FirstOrDefault(u => u.ID == id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            // Validar si el correo está siendo cambiado y ya existe otro usuario con ese correo
            if (usuario.CORREO != request.Correo && _context.USUARIOS.Any(u => u.CORREO == request.Correo))
                return BadRequest("Ya existe un usuario con este correo.");

            // Actualizar campos (no cambiar CREATE_DATE ni ID)
            usuario.NOMBRE = request.Nombre;
            usuario.CORREO = request.Correo;

            // Si envían contraseña nueva, actualizar hash
            if (!string.IsNullOrWhiteSpace(request.Password))
                usuario.PASSWORD_HASH = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Para el rol, solo permitir "User" o "Admin" (puedes ajustar según permisos)
            if (request.Rol == "User" || request.Rol == "Admin")
                usuario.ROL = request.Rol;
            else
                usuario.ROL = "User"; // Rol por defecto o lanzar error si prefieres

            _context.SaveChanges();

            return Ok("Usuario actualizado correctamente.");
        }

        [HttpDelete("delete-user/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var usuario = _context.USUARIOS.FirstOrDefault(u => u.ID == id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            _context.USUARIOS.Remove(usuario);
            _context.SaveChanges();

            return Ok("Usuario eliminado correctamente.");
        }

        [Authorize] // Requiere token JWT válido
        [HttpPut("cambiar-contrasena")]
        public IActionResult CambiarContrasena([FromBody] CambiarContrasenaRequest request)
        {
            var correoUsuario = User.Claims.FirstOrDefault(c => c.Type == "Correo")?.Value;

            if (string.IsNullOrEmpty(correoUsuario))
                return Unauthorized("No se pudo identificar al usuario.");

            var usuario = _context.USUARIOS.FirstOrDefault(u => u.CORREO == correoUsuario);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            if (string.IsNullOrWhiteSpace(request.NuevaContrasena))
                return BadRequest("La nueva contraseña no puede estar vacía.");

            usuario.PASSWORD_HASH = BCrypt.Net.BCrypt.HashPassword(request.NuevaContrasena);
            _context.SaveChanges();

            return Ok("Contraseña actualizada correctamente.");
        }

        [Authorize]
        [HttpGet("perfil")]
        public IActionResult GetPerfil()
        {
            var correoUsuario = User.Claims.FirstOrDefault(c => c.Type == "Correo")?.Value;
            if (correoUsuario == null) return Unauthorized();

            var usuario = _context.USUARIOS.FirstOrDefault(u => u.CORREO == correoUsuario);
            if (usuario == null) return NotFound("Usuario no encontrado");

            return Ok(new
            {
                nombre = usuario.NOMBRE,
                correo = usuario.CORREO
            });
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
        public string Rol { get; set; }
    }

    public class CambiarContrasenaRequest
    {
        public string NuevaContrasena { get; set; }
    }
}