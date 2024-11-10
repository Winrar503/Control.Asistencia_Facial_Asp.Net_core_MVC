using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.EntidadesDeNegocio
{
    public class EmpleadoHorario
    {
        [ForeignKey("Empleados")]
        public int EmpleadosId { get; set; }
        public Empleados Empleados { get; set; }
        [ForeignKey("Horarios")]
        public int HorariosId { get; set; }
        public Horarios Horarios { get; set; }
    }
}
