using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DocumentacionInteligente.BackEnd.Data;

[Route("api/menu")]
[ApiController]
public class MenuController : ControllerBase
{
    private readonly AppDbContext _context;

    public MenuController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("menu-por-rol")]
    [Authorize]
    public async Task<IActionResult> ObtenerMenuPorRol()
    {
        var userId = User.FindFirst("UsuarioId")?.Value;

        var usuario = await _context.USUARIOS
            .FirstOrDefaultAsync(u => u.ID.ToString() == userId);

        if (usuario == null)
            return Unauthorized();

        var menus = await _context.PERMISOS_MENU_ROLES
            .Where(p => p.ROL == usuario.ROL && p.CAN_VIEW)
            .Include(p => p.Menu)
            .OrderBy(p => p.Menu.Title)
            .Select(p => new
            {
                title = p.Menu.Title,
                caption = p.Menu.Caption,
                icon = p.Menu.Icon,
                link = p.Menu.Link
            })
            .ToListAsync();

        return Ok(menus);


        return Ok(menus);
    }
}
