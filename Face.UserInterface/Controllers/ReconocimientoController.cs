using Face.LogicaDeNegocio;
using Face.EntidadesDeNegocio;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Face.UserInterface.Controllers
{
    public class ReconocimientoController : Controller
    {
        private readonly EmpleadosBL empleadosBL = new EmpleadosBL();
        private readonly FotosBL fotosBL = new FotosBL();
        private readonly AsistenciasBL asistenciasBL = new AsistenciasBL();
        private readonly HorariosBL horariosBL = new HorariosBL();
        private readonly ReconocimientoService reconocimientoService;

        public ReconocimientoController()
        {
            reconocimientoService = new ReconocimientoService();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IdentificarEmpleado([FromBody] string imageBase64)
        {
            if (string.IsNullOrEmpty(imageBase64))
            {
                return Json(new { success = false, message = "No se ha recibido ninguna imagen." });
            }

            try
            {
                var imageBytes = Convert.FromBase64String(imageBase64.Split(',')[1]);
                var empleados = await empleadosBL.ObtenerTodosAsync();

                foreach (var empleado in empleados)
                {
                    empleado.Fotos = await fotosBL.ObtenerPorEmpleadoIdAsync(empleado.Id);
                }

                reconocimientoService.EntrenarModelo(empleados);
                var nombreEmpleado = reconocimientoService.IdentificarEmpleado(imageBytes);

                if (nombreEmpleado != "Desconocido")
                {
                    var empleadoIdentificado = empleados.FirstOrDefault(e => e.Nombre == nombreEmpleado);
                    if (empleadoIdentificado != null)
                    {
                        await RegistrarAsistencia(empleadoIdentificado.Id);
                        return Json(new { success = true, empleado = nombreEmpleado });
                    }
                }

                return Json(new { success = false, message = "No se pudo identificar el rostro." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        private async Task RegistrarAsistencia(int empleadoId)
        {
            var ahora = DateTime.Now;
            var tipo = ahora.Hour < 12 ? "Entrada" : "Salida";

            var horario = await horariosBL.ObtenerPorIdAsync(new Horarios { EmpleadosId = empleadoId });

           string comentarios = "";
            if (horario != null)
            {
                comentarios = tipo == "Entrada" && ahora.TimeOfDay > horario.HoraEntrada
                    ? "Entrada tardía"
                    : "";
            }
            else
            {
                comentarios = "Horario no definido";
            }

            var nuevaAsistencia = new Asistencias
            {
                EmpleadosId = empleadoId,
                Comentarios = comentarios,
                Fecha = ahora,
                Tipo = tipo,
                EstadoReconocimiento = "Exitoso"
            };

            await asistenciasBL.CrearAsync(nuevaAsistencia);
        }

    }
}
