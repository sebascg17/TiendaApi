using TiendaApi.Constants;
using TiendaApi.Models;

namespace TiendaApi.Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Asegurarse de que la BD existe
            context.Database.EnsureCreated();

            // Verificar si ya existen roles
            if (context.Roles.Any())
            {
                return;   // Datos ya han sido sembrados
            }

            var roles = new Rol[]
            {
                new Rol { Nombre = RolesConsts.Admin, Descripcion = "Administrador del sistema", FechaCreacion = DateTime.UtcNow },
                new Rol { Nombre = RolesConsts.Tendero, Descripcion = "Vendedor con tienda propia", FechaCreacion = DateTime.UtcNow },
                new Rol { Nombre = RolesConsts.Cliente, Descripcion = "Comprador estándar", FechaCreacion = DateTime.UtcNow }
            };

            foreach (var r in roles)
            {
                context.Roles.Add(r);
            }

            context.SaveChanges();
        }
    }
}
