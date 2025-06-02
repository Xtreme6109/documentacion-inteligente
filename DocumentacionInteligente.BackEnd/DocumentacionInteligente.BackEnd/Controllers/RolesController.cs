using DocumentacionInteligente.BackEnd.Data;
using DocumentacionInteligente.BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DocumentacionInteligente.BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/roles
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.ROLES.ToListAsync();
            return Ok(roles);
        }

        // GET: api/roles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRol(int id)
        {
            var rol = await _context.ROLES.FindAsync(id);
            if (rol == null) return NotFound();
            return Ok(rol);
        }

        // POST: api/roles
        [HttpPost]
        public async Task<IActionResult> CrearRol([FromBody] ROL nuevoRol)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existe = await _context.ROLES.AnyAsync(r => r.NOMBRE == nuevoRol.NOMBRE);
            if (existe)
                return Conflict(new { message = "Ya existe un rol con ese nombre" });

            nuevoRol.CREATE_DATE = DateTime.Now;
            nuevoRol.ESTADO = true;

            _context.ROLES.Add(nuevoRol);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRol), new { id = nuevoRol.ID }, nuevoRol);
        }

        // PUT: api/roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarRol(int id, [FromBody] ROL rolActualizado)
        {
            if (id != rolActualizado.ID)
                return BadRequest();

            var rolExistente = await _context.ROLES.FindAsync(id);
            if (rolExistente == null) return NotFound();

            rolExistente.NOMBRE = rolActualizado.NOMBRE;
            rolExistente.DESCRIPCION = rolActualizado.DESCRIPCION;
            rolExistente.ESTADO = rolActualizado.ESTADO;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Conflict(new { message = ex.Message });
            }

            return NoContent();
        }

        // DELETE: api/roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarRol(int id)
        {
            var rol = await _context.ROLES.FindAsync(id);
            if (rol == null) return NotFound();

            _context.ROLES.Remove(rol);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
