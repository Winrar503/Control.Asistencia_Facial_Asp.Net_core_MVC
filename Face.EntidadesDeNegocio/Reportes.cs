using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.EntidadesDeNegocio
{
    public class Reportes
    {
        [Key]
        public int Id {  get; set; }
        [ForeignKey("Empleado")]
        public int EmpleadosId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin {  get; set; }
        [Display(Name = "Reporte")]
        public string Resumen {  get; set; }
        [NotMapped]
        public int Top_Aux { get; set; }
        public Empleados Empleados { get; set; }

    }
}
