using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.EntidadesDeNegocio
{
    public class RendimientoEmpleados
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Empleados")]
        public int EmpleadosId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int AsistenciasTotales { get; set; }
        public int AsistenciasTardias { get; set; }
        public int AsistenciasExitosas { get; set; }
        public int AsistenciasFallidas { get; set; }
        public int Ausencias { get; set; }

        [NotMapped]
        public int Top_Aux { get; set; }
        public Empleados Empleado { get; set; }
    }
}
