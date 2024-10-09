using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.EntidadesDeNegocio
{
    public class Horarios
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Empleados")]
        public int EmpleadosId { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }

        [NotMapped]
        public int Top_Aux { get; set; }
        public Empleados Empleados { get; set; }
    }
}
