using DocumentacionInteligente.BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace DocumentacionInteligente.BackEnd.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<USUARIOS> USUARIOS { get; set; }
        public DbSet<CATEGORIAS> CATEGORIAS { get; set; }
        public DbSet<DOCUMENTOS> DOCUMENTOS { get; set; }
        public DbSet<VERSIONES> VERSIONES { get; set; }
        public DbSet<IA_PROCESAMIENTOS> IA_PROCESAMIENTOS { get; set; }
        public DbSet<PALABRAS_CLAVE> PALABRAS_CLAVE { get; set; }
        public DbSet<LOGS_ACCESO> LOGS_ACCESO { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){  }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<USUARIOS>().ToTable("USUARIOS");
            modelBuilder.Entity<CATEGORIAS>().ToTable("CATEGORIAS");
            modelBuilder.Entity<DOCUMENTOS>().ToTable("DOCUMENTOS");
            modelBuilder.Entity<VERSIONES>().ToTable("VERSIONES");
            modelBuilder.Entity<IA_PROCESAMIENTOS>().ToTable("IA_PROCESAMIENTOS");
            modelBuilder.Entity<PALABRAS_CLAVE>().ToTable("PALABRAS_CLAVE");
            modelBuilder.Entity<LOGS_ACCESO>().ToTable("LOGS_ACCESO");
            base.OnModelCreating(modelBuilder);
        }
    }
}
