using Microsoft.EntityFrameworkCore;
using TiendaApi.Models;

namespace TiendaApi.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // 🧑‍💼 Usuarios y Roles
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
        public DbSet<Permiso> Permisos => Set<Permiso>();
        public DbSet<RolPermiso> RolPermisos => Set<RolPermiso>();
        public DbSet<DireccionUsuario> DireccionesUsuarios { get; set; }

        // 🏪 Tiendas y Planes
        public DbSet<Tienda> Tiendas => Set<Tienda>();
        public DbSet<Plan> Planes => Set<Plan>();
        public DbSet<TipoTienda> TipoTiendas => Set<TipoTienda>();

        // 🛒 Productos y Categorías
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<ProductoCategoria> ProductoCategorias => Set<ProductoCategoria>();

        // 📦 Pedidos
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<PedidoDetalle> PedidoDetalles => Set<PedidoDetalle>();

        // 💬 Contactos
        public DbSet<Contacto> Contactos => Set<Contacto>();

        // 💸 Transacciones, Comisiones y Referidos
        public DbSet<Transaccion> Transacciones => Set<Transaccion>();
        public DbSet<Comision> Comisiones => Set<Comision>();
        public DbSet<Referido> Referidos => Set<Referido>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======== USUARIO - ROL (N:N) ========
            modelBuilder.Entity<UsuarioRol>()
                .HasKey(ur => new { ur.UsuarioId, ur.RolId });

            modelBuilder.Entity<UsuarioRol>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.UsuarioRoles)
                .HasForeignKey(ur => ur.UsuarioId);

            modelBuilder.Entity<UsuarioRol>()
                .HasOne(ur => ur.Rol)
                .WithMany(r => r.UsuarioRoles)
                .HasForeignKey(ur => ur.RolId);

            // ======== ROL - PERMISO (N:N) ========
            modelBuilder.Entity<RolPermiso>()
                .HasKey(rp => new { rp.RolId, rp.PermisoId });

            modelBuilder.Entity<RolPermiso>()
                .HasOne(rp => rp.Rol)
                .WithMany()
                .HasForeignKey(rp => rp.RolId);

            modelBuilder.Entity<RolPermiso>()
                .HasOne(rp => rp.Permiso)
                .WithMany(p => p.RolPermisos)
                .HasForeignKey(rp => rp.PermisoId);

            // ======== PRODUCTO - CATEGORIA (N:N) ========
            modelBuilder.Entity<ProductoCategoria>()
                .HasKey(pc => new { pc.ProductoId, pc.CategoriaId });

            modelBuilder.Entity<ProductoCategoria>()
                .HasOne(pc => pc.Producto)
                .WithMany(p => p.ProductoCategorias)
                .HasForeignKey(pc => pc.ProductoId);

            modelBuilder.Entity<ProductoCategoria>()
                .HasOne(pc => pc.Categoria)
                .WithMany(c => c.ProductoCategorias)
                .HasForeignKey(pc => pc.CategoriaId);

            // ======== PEDIDO - CLIENTE / REPARTIDOR ========
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Repartidor)
                .WithMany()
                .HasForeignKey(p => p.RepartidorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ======== COMISIONES ========
            modelBuilder.Entity<Comision>()
                .HasOne(c => c.UsuarioDestino)
                .WithMany(u => u.Comisiones)
                .HasForeignKey(c => c.UsuarioDestinoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comision>()
                .HasOne(c => c.UsuarioOrigen)
                .WithMany()
                .HasForeignKey(c => c.UsuarioOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comision>()
                .HasOne(c => c.Pedido)
                .WithMany()
                .HasForeignKey(c => c.PedidoId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Comision>()
                .HasOne(c => c.Referido)
                .WithMany(r => r.Comisiones)
                .HasForeignKey(c => c.ReferidoId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Comision>()
                .HasOne(c => c.Transaccion)
                .WithMany()
                .HasForeignKey(c => c.TransaccionId)
                .OnDelete(DeleteBehavior.SetNull);

            // ======== TRANSACCIONES ========
            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Transacciones)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // ======== REFERIDOS ========
            modelBuilder.Entity<Referido>()
                .HasOne(r => r.UsuarioReferente)
                .WithMany()
                .HasForeignKey(r => r.UsuarioReferenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Referido>()
                .HasOne(r => r.UsuarioReferido)
                .WithMany(u => u.Referidos)
                .HasForeignKey(r => r.UsuarioReferidoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ======== TIENDA - PLAN ========
            modelBuilder.Entity<Tienda>()
                .HasOne(t => t.Plan)
                .WithMany(p => p.Tiendas)
                .HasForeignKey(t => t.PlanId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Tienda>()
                .HasOne(t => t.TipoTienda)
                .WithMany(tp => tp.Tiendas)
                .HasForeignKey(t => t.TipoTiendaId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Tienda>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Tiendas)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ======== TRANSACCIONES ========
            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Transacciones)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Saldo)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Plan>()
                .Property(p => p.PrecioMensual)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PedidoDetalle>()
                .Property(p => p.PrecioUnitario)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaccion>()
                .Property(t => t.Monto)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Comision>()
                .Property(c => c.Monto)
                .HasPrecision(18, 2);

            // ======== PEDIDO - TIENDA ========
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Tienda)
                .WithMany(t => t.Pedidos)
                .HasForeignKey(p => p.TiendaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
