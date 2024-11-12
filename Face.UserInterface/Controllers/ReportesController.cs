using Face.EntidadesDeNegocio;
using Face.LogicaDeNegocio;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Face.UserInterface.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ReportesBL _reportesBL = new ReportesBL();
        private readonly EmpleadosBL _empleadosBL = new EmpleadosBL();
        private readonly RendimientoEmpleadosBL _rendimientoEmpleadosBL = new RendimientoEmpleadosBL();

        // Vista para seleccionar el empleado y el rango de fechas
        public async Task<IActionResult> Create(int empleadoId)
        {
            var empleado = await _empleadosBL.ObtenerPorIdAsync(new Empleados { Id = empleadoId });
            if (empleado == null)
            {
                return NotFound();
            }
            ViewBag.EmpleadoNombre = empleado.Nombre;
            ViewBag.EmpleadoId = empleadoId;
            return View(new Reportes { EmpleadosId = empleadoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reportes reporte)
        {
            if (ModelState.IsValid)
            {
                // Calcula el rendimiento del empleado para el rango de fechas
                var rendimiento = await _rendimientoEmpleadosBL.ObtenerRendimientoPorEmpleadoAsync(reporte.EmpleadosId);

                // Filtra el rendimiento del rango de fechas seleccionado
                var rendimientoEnRango = rendimiento
                    .Where(r => r.FechaInicio >= reporte.FechaInicio && r.FechaFin <= reporte.FechaFin)
                    .ToList();

                // Si no hay datos de rendimiento, muestra un mensaje
                if (!rendimientoEnRango.Any())
                {
                    ModelState.AddModelError("", "No se encontraron registros de rendimiento en el rango de fechas seleccionado.");
                    return View(reporte);
                }

                // Genera un resumen del rendimiento
                reporte.Resumen = $"Total Asistencias: {rendimientoEnRango.Sum(r => r.AsistenciasTotales)}, " +
                                  $"Asistencias Tardías: {rendimientoEnRango.Sum(r => r.AsistenciasTardias)}, " +
                                  $"Asistencias Exitosas: {rendimientoEnRango.Sum(r => r.AsistenciasExitosas)}, " +
                                  $"Asistencias Fallidas: {rendimientoEnRango.Sum(r => r.AsistenciasFallidas)}, " +
                                  $"Ausencias: {rendimientoEnRango.Sum(r => r.Ausencias)}";

                await _reportesBL.CrearAsync(reporte);
                return RedirectToAction("Index", new { empleadoId = reporte.EmpleadosId });
            }

            return View(reporte);
        }

        // Listado de reportes de un empleado específico
        public async Task<IActionResult> Index(int empleadoId)
        {
            // Obtener todos los reportes
            var reportes = await _reportesBL.ObtenerTodosAsync();

            // Filtrar reportes por el empleado específico
            var reportesEmpleado = reportes.Where(r => r.EmpleadosId == empleadoId).ToList();

            // Obtener la información del empleado
            var empleado = await _empleadosBL.ObtenerPorIdAsync(new Empleados { Id = empleadoId });

            // Verificar si el empleado existe
            if (empleado == null)
            {
                ViewBag.ErrorMessage = "Empleado no encontrado.";
                return View("Error");
            }

            ViewBag.EmpleadoNombre = empleado.Nombre;
            ViewBag.EmpleadoId = empleadoId;

            return View(reportesEmpleado);
        }


        // Eliminar reporte
        public async Task<IActionResult> Delete(int id)
        {
            var reporte = await _reportesBL.ObtenerPorIdAsync(new Reportes { Id = id });
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

        // Generar PDF del reporte
        public async Task<IActionResult> DescargarPDF(int id)
        {
            var reporte = await _reportesBL.ObtenerPorIdAsync(new Reportes { Id = id });
            if (reporte == null)
            {
                return NotFound();
            }

            using (var memoryStream = new MemoryStream())
            {
                var doc = new Document();
                PdfWriter.GetInstance(doc, memoryStream).CloseStream = false;
                doc.Open();

                doc.Add(new Paragraph($"Reporte de Asistencia para el Empleado: {reporte.Empleados.Nombre}"));
                doc.Add(new Paragraph($"Fecha Inicio: {reporte.FechaInicio.ToShortDateString()}"));
                doc.Add(new Paragraph($"Fecha Fin: {reporte.FechaFin.ToShortDateString()}"));
                doc.Add(new Paragraph($"Resumen: {reporte.Resumen}"));

                doc.Close();
                byte[] pdfBytes = memoryStream.ToArray();
                return File(pdfBytes, "application/pdf", $"Reporte_{reporte.Empleados.Nombre}_{reporte.FechaInicio.ToShortDateString()}_{reporte.FechaFin.ToShortDateString()}.pdf");
            }
        }
    }
}
