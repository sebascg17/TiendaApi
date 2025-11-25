// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using TiendaApi.Infrastructure;
// using TiendaApi.Models;


// [ApiController]
// [Route("api/[controller]")]
// public class ServiciosController : ControllerBase {
//     private readonly AppDbContext _db;
//     public ServiciosController(AppDbContext db) => _db = db;


//     [HttpGet]
//     public async Task<IEnumerable<Servicio>> Get() => await _db.Servicios.ToListAsync();


//     [HttpPost]
//     public async Task<ActionResult<Servicio>> Create(Servicio s) {
//         _db.Servicios.Add(s);
//         await _db.SaveChangesAsync();
//         return CreatedAtAction(nameof(Get), new { id = s.Id }, s);
//     }
// }