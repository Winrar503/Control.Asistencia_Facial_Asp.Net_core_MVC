using Face.EntidadesDeNegocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.AccesoADatos
{
    public class BDContexto : DbContext
    {
        public DbSet<Empleados> Empleados { get; set; }
        public DbSet<Fotos> Fotos { get; set; }
        public DbSet<RendimientoEmpleados> RendimientoEmpleados { get; set; }
        public DbSet<EmpleadoHorario> EmpleadoHorarios { get; set; }
        public DbSet<Asistencias> Asistencias {  get; set; }
        public DbSet<Horarios> Horarios { get; set; }
        public DbSet<Reportes> Reportes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-G786A4S\SQLEXPRESS;Initial Catalog=ReconocimientoFacial;Integrated Security=True; encrypt = false; trustServerCertificate = True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la clave compuesta en EmpleadoHorario
            modelBuilder.Entity<EmpleadoHorario>()
                .HasKey(eh => new { eh.EmpleadosId, eh.HorariosId });

            modelBuilder.Entity<EmpleadoHorario>()
                .HasOne(eh => eh.Empleados)
                .WithMany(e => e.EmpleadoHorarios)
                .HasForeignKey(eh => eh.EmpleadosId);

            modelBuilder.Entity<EmpleadoHorario>()
                .HasOne(eh => eh.Horarios)
                .WithMany(h => h.EmpleadoHorarios)
                .HasForeignKey(eh => eh.HorariosId);
        }
    }
}

