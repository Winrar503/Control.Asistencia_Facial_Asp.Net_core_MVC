using Face.EntidadesDeNegocio;
using Face.LogicaDeNegocio;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Face.UserInterface.Controllers
{
    public class RendimientoEmpleadosController : Controller
    {
        private readonly RendimientoEmpleadosBL _rendimientoEmpleadosBL = new RendimientoEmpleadosBL();
        private readonly EmpleadosBL _empleadosBL = new EmpleadosBL();

        public async Task<IActionResult> Index(int empleadoId)
        {
            // Obtener el empleado y su rendimiento
            var empleado = await _empleadosBL.ObtenerPorIdConRelacionesAsync(empleadoId);
            var rendimientoTotal = await _rendimientoEmpleadosBL.ObtenerRendimientoPorEmpleadoAsync(empleadoId);

            // Verificar si el empleado existe
            if (empleado == null)
            {
                ViewBag.ErrorMessage = "Empleado no encontrado.";
                return View("Error");
            }

            ViewBag.EmpleadoNombre = empleado.Nombre;
            ViewBag.EmpleadoFoto = empleado.Fotos?.FirstOrDefault()?.Foto;

            // Filtrar rendimiento del día actual
            var hoy = DateTime.Today;
            var rendimientoHoy = rendimientoTotal
                .Where(r => r.FechaInicio.Date == hoy)
                .FirstOrDefault();

            // Filtrar rendimiento de la semana (lunes a viernes)
            var lunes = hoy.AddDays(-(int)hoy.DayOfWeek + (int)DayOfWeek.Monday);
            var viernes = lunes.AddDays(4);

            var rendimientoSemana = rendimientoTotal
                .Where(r => r.FechaInicio.Date >= lunes && r.FechaFin.Date <= viernes)
                .ToList();

            // Pasar los datos de rendimiento semanal y diario a la vista
            ViewBag.RendimientoHoy = rendimientoHoy;
            ViewBag.RendimientoSemana = rendimientoSemana;

            // Si no hay datos de rendimiento, mostrar mensaje
            if (!rendimientoTotal.Any())
            {
                ViewBag.ErrorMessage = "No hay datos de rendimiento disponibles para este empleado.";
            }

            return View(rendimientoTotal);
        }
    }
}
