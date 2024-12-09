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
        // Descargar reporte en PDF
        //public async Task<IActionResult> DescargarPDF(int id)
        //{
        //    var reporte = await _reportesBL.ObtenerPorIdAsync(new Reportes { Id = id });
        //    if (reporte == null) return NotFound("Reporte no encontrado.");

        //    using (var memoryStream = new MemoryStream())
        //    {
        //        var doc = new Document(PageSize.A4, 25, 25, 30, 30);
        //        PdfWriter.GetInstance(doc, memoryStream).CloseStream = false;
        //        doc.Open();

        //        // Título del documento
        //        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
        //        var textFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
        //        doc.Add(new Paragraph("Reporte de Asistencias", titleFont));
        //        doc.Add(new Paragraph($"Empleado: {reporte.Empleados.Nombre}", textFont));
        //        doc.Add(new Paragraph($"Rango de Fechas: {reporte.FechaInicio.ToShortDateString()} - {reporte.FechaFin.ToShortDateString()}", textFont));
        //        doc.Add(new Paragraph(" "));

        //        // Resumen del reporte
        //        doc.Add(new Paragraph("Resumen del Rendimiento:", titleFont));
        //        doc.Add(new Paragraph(reporte.Resumen, textFont));
        //        doc.Add(new Paragraph(" "));

        //        // Cierre del documento
        //        doc.Close();

        //        return File(memoryStream.ToArray(), "application/pdf", $"Reporte_{reporte.Empleados.Nombre}.pdf");
        //    }
        //}
        //public async Task<IActionResult> DescargarPDF(int id)
        //{
        //    var reporte = await _reportesBL.ObtenerPorIdAsync(new Reportes { Id = id });
        //    if (reporte == null) return NotFound("Reporte no encontrado.");

        //    using (var memoryStream = new MemoryStream())
        //    {
        //        var doc = new Document(PageSize.A4, 25, 25, 30, 30);
        //        var writer = PdfWriter.GetInstance(doc, memoryStream);
        //        writer.CloseStream = false;

        //        doc.Open();

        //        // Título del documento
        //        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
        //        var textFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
        //        doc.Add(new Paragraph("Reporte de Asistencias", titleFont));
        //        doc.Add(new Paragraph($"Empleado: {reporte.Empleados.Nombre}", textFont));
        //        doc.Add(new Paragraph($"Rango de Fechas: {reporte.FechaInicio:yyyy-MM-dd} - {reporte.FechaFin:yyyy-MM-dd}", textFont));
        //        doc.Add(new Paragraph(" "));

        //        // Resumen del reporte
        //        doc.Add(new Paragraph("Resumen del Rendimiento:", titleFont));
        //        doc.Add(new Paragraph(reporte.Resumen, textFont));
        //        doc.Add(new Paragraph(" "));

        //        // Tabla de detalles
        //        PdfPTable table = new PdfPTable(3);
        //        table.WidthPercentage = 100;
        //        table.AddCell("Fecha");
        //        table.AddCell("Tipo");
        //        table.AddCell("Estado");

        //        // Aquí podrías iterar sobre una lista de asistencias y agregar filas
        //        var asistencias = await _asistenciasBL.ObtenerTodosAsync();
        //        foreach (var asistencia in asistencias.Where(a => a.EmpleadosId == reporte.EmpleadosId && a.Fecha >= reporte.FechaInicio && a.Fecha <= reporte.FechaFin))
        //        {
        //            table.AddCell(asistencia.Fecha.ToString("yyyy-MM-dd"));
        //            table.AddCell(asistencia.Tipo);
        //            table.AddCell(asistencia.EstadoReconocimiento);
        //        }

        //        doc.Add(table);
        //        doc.Close();

        //        return File(memoryStream.ToArray(), "application/pdf", $"Reporte_{reporte.Empleados.Nombre}.pdf");
        //    }
        //}


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
