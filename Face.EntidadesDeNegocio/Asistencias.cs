using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.EntidadesDeNegocio
{
    public class Asistencias
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Empleados")]
        public int EmpleadosId { get; set; }

        [Display(Name = "Comentarios")]
        public string Comentarios { get; set; }
        public DateTime Fecha { get; set; }
        [Required(ErrorMessage = "Es requerido si es de salida oh entrada")]
        [MaxLength(50, ErrorMessage = "El largo maximo es 100 caracteres")]
        public string Tipo { get; set; }
        public string EstadoReconocimiento { get; set; }

        [NotMapped]
        public int Top_Aux { get; set; }
        public Empleados Empleados { get; set; }

    }
}
