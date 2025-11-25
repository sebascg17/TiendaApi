using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaApi.Infrastructure;
using TiendaApi.Models;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactoController : ControllerBase
    {
        // ✅ Cualquier usuario autenticado
        [HttpGet("todos")]
        [Authorize]
        public IActionResult GetTodos()
        {
            return Ok("Acceso permitido a cualquier usuario logueado");
        }

        // ✅ Solo Admin
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetSoloAdmins()
        {
            return Ok("Acceso permitido solo a Admin");
        }

        // ✅ Solo User
        [HttpGet("solo-user")]
        [Authorize(Roles = "User")]
        public IActionResult GetSoloUser()
        {
            return Ok("Acceso permitido solo a User role");
        }
    }
}
