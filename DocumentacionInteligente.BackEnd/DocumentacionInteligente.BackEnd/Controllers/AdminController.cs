using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DocumentacionInteligente.BackEnd.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DocumentacionInteligente.BackEnd.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Policy = "Administrador")] // Solo los usuarios con la política "Admin" podrán acceder
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

    }

    public class UpdateRoleRequest
    {
        public string NewRole { get; set; }
    }
}
