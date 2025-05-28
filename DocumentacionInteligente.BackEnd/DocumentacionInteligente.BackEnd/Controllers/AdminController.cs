using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DocumentacionInteligente.BackEnd.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DocumentacionInteligente.BackEnd.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Policy = "Admin")] // Solo los usuarios con la política "Admin" podrán acceder
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // 1.endpoint obtener la lista de usuarios
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.USUARIOS.Select(u => new
            {
                u.ID,
                u.NOMBRE,
                u.CORREO,
                u.ROL,
                u.CREATE_DATE
            }).ToList();

            return Ok(users);
        }

        // 2.endpoint obtener un usuario por ID
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.USUARIOS.Select(u => new
            {
                u.ID,
                u.NOMBRE,
                u.CORREO,
                u.ROL,
                u.CREATE_DATE
            }).FirstOrDefault(u => u.ID == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // 3. Actualizar el rol de un usuario
        [HttpPut("{id}/role")]
        public IActionResult UpdateUserRole(int id, [FromBody] UpdateRoleRequest request)
        {
            var user = _context.USUARIOS.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            string nombreUsuario = user.NOMBRE; // Guardamos el nombre del usuario antes de modificarlo
            user.ROL = request.NewRole.ToLower();
            _context.SaveChanges();

            return Ok($"Rol del usuario '{nombreUsuario}' (ID: {id}) actualizado a '{request.NewRole}'."); // Usamos la variable nombreUsuario
        }

        // 4. Eliminar un usuario
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.USUARIOS.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            string nombreUsuario = user.NOMBRE; // Guardamos el nombre del usuario antes de eliminarlo
            _context.USUARIOS.Remove(user);
            _context.SaveChanges();

            return Ok($"Usuario '{nombreUsuario}' (ID: {id}) eliminado."); // Usamos la variable nombreUsuario
        }
    }

    public class UpdateRoleRequest
    {
        public string NewRole { get; set; }
    }
}
