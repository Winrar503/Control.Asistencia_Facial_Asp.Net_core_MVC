using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.EntidadesDeNegocio
{
    public class Cargo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del cargo es obligatorio")]
        [MaxLength(100, ErrorMessage = "El largo máximo es de 100 caracteres")]
        public string Nombre { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [NotMapped]

        public ICollection<Empleados> Empleados { get; set; } = new List<Empleados>();
    }
}
