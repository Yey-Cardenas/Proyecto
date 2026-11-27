using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Proyecto.Modules.Autenticacion.Entities;
using Proyecto.Modules.Beneficiarios.Entities;
using Proyecto.Modules.Revalidacion.Entities;

namespace Proyecto.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Comite> Comites => Set<Comite>();
    public DbSet<Beneficiario> Beneficiarios => Set<Beneficiario>();
    public DbSet<Programa> Programas => Set<Programa>();
    public DbSet<HistorialCambio> HistorialCambios => Set<HistorialCambio>();
    public DbSet<RevalidacionBeneficiario> Revalidaciones => Set<RevalidacionBeneficiario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Comite>()
            .Property(comite => comite.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        modelBuilder.Entity<Beneficiario>()
            .Property(beneficiario => beneficiario.Dni)
            .HasMaxLength(8)
            .IsRequired();

        modelBuilder.Entity<Beneficiario>()
            .HasIndex(beneficiario => beneficiario.Dni)
            .IsUnique()
            .HasFilter("[Activo] = 1");

        modelBuilder.Entity<Beneficiario>()
            .Property(beneficiario => beneficiario.Nombres)
            .HasMaxLength(150)
            .IsRequired();

        modelBuilder.Entity<Beneficiario>()
            .Property(beneficiario => beneficiario.Apellidos)
            .HasMaxLength(150)
            .IsRequired();

        modelBuilder.Entity<Programa>()
            .Property(programa => programa.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        modelBuilder.Entity<HistorialCambio>()
            .Property(cambio => cambio.Motivo)
            .HasMaxLength(500)
            .IsRequired();

        modelBuilder.Entity<HistorialCambio>()
            .Property(cambio => cambio.Operacion)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Comite>()
            .HasMany(comite => comite.Beneficiarios)
            .WithOne(beneficiario => beneficiario.Comite)
            .HasForeignKey(beneficiario => beneficiario.ComiteId);

        modelBuilder.Entity<Comite>()
            .HasMany(comite => comite.Dirigentes)
            .WithOne(usuario => usuario.Comite)
            .HasForeignKey(usuario => usuario.ComiteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Beneficiario>()
            .HasMany(beneficiario => beneficiario.Programas)
            .WithMany(programa => programa.Beneficiarios);

        modelBuilder.Entity<HistorialCambio>()
            .HasOne(cambio => cambio.Beneficiario)
            .WithMany(beneficiario => beneficiario.HistorialCambios)
            .HasForeignKey(cambio => cambio.BeneficiarioId);

        modelBuilder.Entity<RevalidacionBeneficiario>()
            .HasOne(revalidacion => revalidacion.Beneficiario)
            .WithMany()
            .HasForeignKey(revalidacion => revalidacion.BeneficiarioId);
    }
}
