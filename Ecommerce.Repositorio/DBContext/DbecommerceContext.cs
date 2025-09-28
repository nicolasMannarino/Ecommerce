using System;
using System.Collections.Generic;
using Ecommerce.Modelo;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositorio.DBContext;

public partial class DbecommerceContext : DbContext
{
    public DbecommerceContext()
    {
    }

    public DbecommerceContext(DbContextOptions<DbecommerceContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Categoria> Categoria { get; set; }
    public virtual DbSet<DetalleVenta> DetalleVenta { get; set; }
    public virtual DbSet<Producto> Productos { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Venta> Venta { get; set; }
    public virtual DbSet<Filtro> Filtro { get; set; }
    public virtual DbSet<FiltroOpcion> FiltroOpcion { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FiltroOpcion>(entity =>
        {
            entity.ToTable("FiltroOpcion");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Valor).IsRequired();

            entity.HasOne(e => e.Filtro)
                  .WithMany(f => f.FiltroOpciones)
                  .HasForeignKey(e => e.IdFiltro)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Filtro>()
            .HasMany(f => f.FiltroOpciones)
            .WithOne(fo => fo.Filtro)
            .HasForeignKey(fo => fo.IdFiltro)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductoFiltroValor>()
            .HasKey(p => new { p.IdProducto, p.IdFiltro });

        modelBuilder.Entity<ProductoFiltroValor>()
            .HasOne(pfv => pfv.Producto)
            .WithMany(p => p.ProductoFiltroValores)
            .HasForeignKey(pfv => pfv.IdProducto);

        modelBuilder.Entity<ProductoFiltroValor>()
            .HasOne(pfv => pfv.Filtro)
            .WithMany()
            .HasForeignKey(pfv => pfv.IdFiltro);

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__A3C02A1021B329DD");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.IdDetalleVenta).HasName("PK__DetalleV__AAA5CEC254C0E45B");

            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK__DetalleVe__IdPro__5812160E");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdVenta)
                .HasConstraintName("FK__DetalleVe__IdVen__571DF1D5");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__09889210657A7DD9");

            entity.ToTable("Producto");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PrecioOferta).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK__Producto__IdCate__4CA06362");
        });

        modelBuilder.Entity<ProductoImagen>(entity =>
        {
            entity.HasKey(e => e.IdProductoImagen).HasName("PK__ProductoImagen");

            entity.ToTable("ProductoImagen");

            entity.Property(e => e.RutaImagen)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.NumeroImagen)
                .IsRequired();

            // Relación con Producto
            entity.HasOne(d => d.Producto)
                .WithMany(p => p.ProductoImagenes)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK__ProductoImagen__IdProducto");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF97B3E7FA6B");

            entity.ToTable("Usuario");

            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Rol)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__Venta__BC1240BDAFA8A3A5");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Venta__IdUsuario__534D60F1");
        });

        modelBuilder.Entity<Filtro>(entity =>
        {
            entity.HasKey(e => e.IdFiltro).HasName("PK__Filtro__1CB2D349");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.TipoFiltro)
                .HasMaxLength(50)
                .IsUnicode(false);

            // Relación con CategoriaFiltro
            entity.HasMany(e => e.CategoriasFiltro)
                .WithOne(cf => cf.Filtro)
                .HasForeignKey(cf => cf.IdFiltro)
                .HasConstraintName("FK__CategoriaFiltro__IdFiltro");
        });

        modelBuilder.Entity<CategoriaFiltro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CategoriaFiltro__0F975522");

            // Propiedades
            entity.Property(e => e.IdFiltro).IsRequired();
            entity.Property(e => e.IdCategoria).IsRequired();

            // Relaciones
            entity.HasOne(cf => cf.Filtro)
                .WithMany(f => f.CategoriasFiltro)
                .HasForeignKey(cf => cf.IdFiltro)
                .HasConstraintName("FK__CategoriaFiltro__IdFiltro");

            entity.HasOne(cf => cf.Categoria)
                .WithMany(c => c.CategoriasFiltro)
                .HasForeignKey(cf => cf.IdCategoria)
                .HasConstraintName("FK__CategoriaFiltro__IdCategoria");
        });




        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
