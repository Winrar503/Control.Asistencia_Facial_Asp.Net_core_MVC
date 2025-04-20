using Face.EntidadesDeNegocio;
using Face.LogicaDeNegocio;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Face.UserInterface.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ReportesBL _reportesBL = new ReportesBL();
        private readonly EmpleadosBL _empleadosBL = new EmpleadosBL();
        private readonly AsistenciasBL _asistenciasBL = new AsistenciasBL();
        private readonly RendimientoEmpleadosBL _rendimientoEmpleadosBL = new RendimientoEmpleadosBL();
        public async Task<IActionResult> Create(int empleadoId)
        {
            var empleado = await _empleadosBL.ObtenerPorIdAsync(new Empleados { Id = empleadoId });
            if (empleado == null) return NotFound("Empleado no encontrado.");

            ViewBag.EmpleadoNombre = empleado.Nombre;
            ViewBag.EmpleadoId = empleadoId;

            return View(new Reportes { EmpleadosId = empleadoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reportes reporte)
        {
            if (!ModelState.IsValid)
            {
                return View(reporte);
            }
            await _rendimientoEmpleadosBL.CalcularRendimientoAsync(reporte.EmpleadosId, reporte.FechaInicio, reporte.FechaFin);
            var rendimiento = await _rendimientoEmpleadosBL.ObtenerPorEmpleadoYRangoAsync(reporte.EmpleadosId, reporte.FechaInicio, reporte.FechaFin);
            if (!rendimiento.Any())
            {
                Console.WriteLine($"No se encontraron datos de rendimiento para el empleado {reporte.EmpleadosId} entre {reporte.FechaInicio} y {reporte.FechaFin}");
            }
            else
            {
                foreach (var r in rendimiento)
                {
                    Console.WriteLine($"Rendimiento encontrado: Asistencias Totales = {r.AsistenciasTotales}, Ausencias = {r.Ausencias}");
                }
            }
            reporte.Resumen = $"Total de Asistencias: {rendimiento.Sum(r => r.AsistenciasTotales)}, " +
                              $"Asistencias Tardías: {rendimiento.Sum(r => r.AsistenciasTardias)}, " +
                              $"Asistencias Exitosas: {rendimiento.Sum(r => r.AsistenciasExitosas)}, " +
                              $"Ausencias: {rendimiento.Sum(r => r.Ausencias)}.";
            await _reportesBL.CrearAsync(reporte);
            return RedirectToAction("Index", new { empleadoId = reporte.EmpleadosId });
        }
        public async Task<IActionResult> Index(int empleadoId)
        {
            if (empleadoId <= 0)
            {
                return NotFound("ID de empleado inválido.");
            }
            var empleado = await _empleadosBL.ObtenerPorIdAsync(new Empleados { Id = empleadoId });
            if (empleado == null)
            {
                return NotFound("Empleado no encontrado.");
            }
            var reportes = (await _reportesBL.ObtenerTodosAsync())
                .Where(r => r.EmpleadosId == empleadoId)
                .ToList();

            ViewBag.EmpleadoNombre = empleado.Nombre;
            ViewBag.EmpleadoId = empleadoId;
            if (!reportes.Any())
            {
                ViewBag.ErrorMessage = "No hay reportes generados para este empleado.";
            }

            return View(reportes);
        }
      

        // Eliminar reporte
        public async Task<IActionResult> Delete(int id)
        {
            var reporte = await _reportesBL.ObtenerPorIdAsync(new Reportes { Id = id });
            if (reporte == null) return NotFound("Reporte no encontrado.");

            return View(reporte);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reporte = await _reportesBL.ObtenerPorIdAsync(new Reportes { Id = id });
            if (reporte != null)
            {
                await _reportesBL.EliminarAsync(reporte);
            }

            return RedirectToAction("Index", new { empleadoId = reporte.EmpleadosId });
        }
    }
}
