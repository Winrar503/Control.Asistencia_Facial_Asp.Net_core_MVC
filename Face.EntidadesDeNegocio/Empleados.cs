using Emgu.CV.Stitching;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.EntidadesDeNegocio
{
    public class Empleados
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El mombre es obligatorio")]
        [Display(Name = "Nombre")]
        [MaxLength(100, ErrorMessage = "El largo máximo es 100 caracteres")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "La edad es obligatorio")]
        [Display(Name = "Edad")]
        public int Edad { get; set; }
        [Required(ErrorMessage = "El correo es obligatorio")]
        [Display(Name = "Email")]
        [MaxLength(255, ErrorMessage = "El largo máximo es 255 caracteres")]
        public string Email { get; set; }
        //[Required(ErrorMessage = "El cargo es requerido")]
        //[Display(Name = "Cargo")]
        //[MaxLength(100, ErrorMessage = "El largo maximo es 100 caracteres")]
        //public string Cargo { get; set; } 
        [Required(ErrorMessage = "El numero telefonico es requerido")]
        [Display(Name = "Telefono")]
        public string Telefono { get; set; }
        [Display(Name = "Estado")]
        public bool Estado {  get; set; }
        public DateTime FechaRegistro { get; set; }

        [ForeignKey("Cargo")]
        public int CargoId { get; set; }
        public Cargo Cargo { get; set; }
        [NotMapped]
        public int Top_Aux { get; set; }
        public ICollection<Asistencias> Asistencias { get; set; } = new List<Asistencias>();
        public ICollection<Fotos> Fotos { get; set; } = new List<Fotos>();
        public ICollection<Horarios> Horarios { get; set; } = new List<Horarios>();
        public ICollection<Reportes> Reportes { get; set; } = new List<Reportes>();
        public ICollection<EmpleadoHorario> EmpleadoHorarios { get; set; }
       
    }
}
