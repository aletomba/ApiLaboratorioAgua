using Dominio.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class LabAguaDbContext:DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Muestra> Muestras { get; set; }
        public DbSet<LibroDeEntrada> LibroEntradas { get; set; }
        public DbSet<Bacteriologico> Bacteriologicos { get; set; }
        public DbSet<FisicoQuimico> FisicoQuimicos { get; set; }
        public DbSet<PlanillaDiaria> PlanillasDiarias { get; set; }
        public DbSet<EnsayoJarras> EnsayosJarras { get; set; }

        public LabAguaDbContext(DbContextOptions<LabAguaDbContext> options) : base(options) { }
     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar herencia TPH para BaseEntity
            modelBuilder.Entity<BaseEntities>().UseTphMappingStrategy();          

            // Relaci�n Cliente - Muestra (1 a muchos)
            modelBuilder.Entity<Muestra>()
                .HasOne(m => m.Cliente)
                .WithMany(c => c.Muestras)
                .HasForeignKey(m => m.ClienteId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminaci�n en cascada

            // Relaci�n Muestra - LibroEntrada (muchos a 1)
            modelBuilder.Entity<Muestra>()
                .HasOne(m => m.LibroEntrada)
                .WithMany(le => le.Muestras)
                .HasForeignKey(m => m.LibroEntradaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relaci�n Muestra - LibroBacteriologia (1 a 0..1)
            modelBuilder.Entity<Bacteriologico>()
                .HasOne(lb => lb.Muestra)
                .WithOne(m => m.Bacteriologia)
                .HasForeignKey<Bacteriologico>(lb => lb.MuestraId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relaci�n Muestra - LibroFisicoQuimico (1 a 0..1)
            modelBuilder.Entity<FisicoQuimico>()
                .HasOne(lf => lf.Muestra)
                .WithOne(m => m.FisicoQuimico)
                .HasForeignKey<FisicoQuimico>(lf => lf.MuestraId)
                .OnDelete(DeleteBehavior.Cascade);

            // PlanillaDiaria - LibroDeEntrada (1 a 1)
            modelBuilder.Entity<PlanillaDiaria>()
                .HasOne(p => p.LibroEntrada)
                .WithMany()
                .HasForeignKey(p => p.LibroEntradaId)
                .OnDelete(DeleteBehavior.Restrict);

            // PlanillaDiaria - EnsayoJarras (1 a 0..1)
            modelBuilder.Entity<EnsayoJarras>()
                .HasOne(e => e.PlanillaDiaria)
                .WithOne(p => p.EnsayoJarras)
                .HasForeignKey<EnsayoJarras>(e => e.PlanillaDiariaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice único: una planilla por día
            modelBuilder.Entity<PlanillaDiaria>()
                .HasIndex(p => p.Fecha)
                .IsUnique();
        }
        //TODO: Agregar DbSet para las entidades restantes
    }
}
