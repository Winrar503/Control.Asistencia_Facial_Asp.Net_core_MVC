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

        //private async Task RegistrarAsistencia(int empleadoId)
        //{
        //    var ahora = DateTime.Now;
        //    var tipo = ahora.Hour < 12 ? "Entrada" : "Salida";


        //    var horario = await horariosBL.ObtenerTodosAsync();
        //    var empleadoHorario = horario.FirstOrDefault(h => h.EmpleadosId == empleadoId);

        //    string comentarios = "";
        //    string estadoReconocimiento = "Exitoso";


        //    var nuevaAsistencia = new Asistencias
        //    {
        //        EmpleadosId = empleadoId,
        //        Fecha = ahora,
        //        Tipo = tipo,
        //        EstadoReconocimiento = estadoReconocimiento
        //    };

        //    if (empleadoHorario != null)
        //    {
        //        var horaEntradaProgramada = empleadoHorario.HoraEntrada;
        //        var horaSalidaProgramada = empleadoHorario.HoraSalida;

        //        if (tipo == "Entrada")
        //        {
        //            if (ahora.TimeOfDay <= horaEntradaProgramada + TimeSpan.FromMinutes(10))
        //            {

        //                comentarios = "Entrada puntual";
        //            }
        //            else if (ahora.TimeOfDay > horaEntradaProgramada + TimeSpan.FromMinutes(10))
        //            {

        //                comentarios = "Entrada tardía";
        //            }
        //        }
        //        else if (tipo == "Salida")
        //        {
        //            if (ahora.TimeOfDay > horaSalidaProgramada + TimeSpan.FromHours(1))
        //            {

        //                comentarios = "Salida con horas extras";
        //                var horasExtras = (ahora.TimeOfDay - horaSalidaProgramada).TotalHours;
        //                nuevaAsistencia.Comentarios = comentarios;
        //                nuevaAsistencia.HorasExtras = Math.Round((decimal)horasExtras, 2); 
        //            }
        //            else
        //            {
        //                comentarios = "Salida puntual";
        //            }
        //        }
        //    }
        //    else
        //    {

        //        comentarios = "Horario no definido";
        //        estadoReconocimiento = "Fallido";
        //    }
        //    nuevaAsistencia.Comentarios = comentarios;
        //    await asistenciasBL.CrearAsync(nuevaAsistencia);
        //}
        private async Task ActualizarEstadoEmpleado(int empleadoId, string tipo)
        {
            var empleado = await empleadosBL.ObtenerPorIdAsync(new Empleados { Id = empleadoId });
            if (empleado != null)
            {
                if (tipo == "Entrada")
                {
                    empleado.Estado = true; // Activo
                }
                else if (tipo == "Salida")
                {
                    empleado.Estado = false; // Inactivo
                }
                await empleadosBL.ModificarAsync(empleado);
            }
        }

        private async Task RegistrarAsistencia(int empleadoId)
        {
            var ahora = DateTime.Now;
            var tipo = ahora.Hour < 12 ? "Entrada" : "Salida";

            var horario = await horariosBL.ObtenerTodosAsync();
            var empleadoHorario = horario.FirstOrDefault(h => h.EmpleadosId == empleadoId);

            string comentarios = "";
            string estadoReconocimiento = "Exitoso";

            var nuevaAsistencia = new Asistencias
            {
                EmpleadosId = empleadoId,
                Fecha = ahora,
                Tipo = tipo,
                EstadoReconocimiento = estadoReconocimiento
            };

            if (empleadoHorario != null)
            {
                var horaEntradaProgramada = empleadoHorario.HoraEntrada;
                var horaSalidaProgramada = empleadoHorario.HoraSalida;

                if (tipo == "Entrada")
                {
                    if (ahora.TimeOfDay <= horaEntradaProgramada + TimeSpan.FromMinutes(10))
                    {
                        comentarios = "Entrada puntual";
                    }
                    else if (ahora.TimeOfDay > horaEntradaProgramada + TimeSpan.FromMinutes(10))
                    {
                        comentarios = "Entrada tardía";
                    }
                }
                else if (tipo == "Salida")
                {
                    if (ahora.TimeOfDay > horaSalidaProgramada + TimeSpan.FromHours(1))
                    {
                        comentarios = "Salida con horas extras";
                        var horasExtras = (ahora.TimeOfDay - horaSalidaProgramada).TotalHours;
                        nuevaAsistencia.Comentarios = comentarios;
                        nuevaAsistencia.HorasExtras = Math.Round((decimal)horasExtras, 2);
                    }
                    else
                    {
                        comentarios = "Salida puntual";
                    }
                }
            }
            else
            {
                comentarios = "Horario no definido";
                estadoReconocimiento = "Fallido";
            }

            nuevaAsistencia.Comentarios = comentarios;
            await asistenciasBL.CrearAsync(nuevaAsistencia);

            // Actualizar el estado del empleado
            await ActualizarEstadoEmpleado(empleadoId, tipo);
        }

    }
}
