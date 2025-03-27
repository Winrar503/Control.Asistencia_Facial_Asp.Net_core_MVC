using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.EntidadesDeNegocio
{
    public class Fotos
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Empleados")]
        public int EmpleadosId { get; set; }
        public byte[] Foto { get; set; }
        public string NombreFoto { get; set; }


        [NotMapped]
        public int Top_Aux { get; set; }
        public Empleados Empleados { get; set; }
    }
}
