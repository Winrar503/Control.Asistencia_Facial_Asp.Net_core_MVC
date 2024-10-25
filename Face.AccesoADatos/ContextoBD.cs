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
        public DbSet<Asistencias> Asistencias {  get; set; }
        public DbSet<Horarios> Horarios { get; set; }
        public DbSet<Reportes> Reportes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-18G9CNA;Initial Catalog=ReconocimientoFacial;Integrated Security=True; encrypt = false; trustServerCertificate = True");
        }
    }
}
