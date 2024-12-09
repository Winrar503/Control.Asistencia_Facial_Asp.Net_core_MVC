using Face.LogicaDeNegocio;
using Face.UserInterface.Models;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewEngines;
namespace Face.UserInterface.Controllers
{
    public class ReporteGeneralController : Controller
    {
        private readonly EmpleadosBL empleadosBL = new EmpleadosBL();
        private readonly AsistenciasBL asistenciasBL = new AsistenciasBL();
        private readonly CargosBL cargosBL = new CargosBL();
        private readonly ICompositeViewEngine _viewEngine;
        public ReporteGeneralController(ICompositeViewEngine viewEngine, EmpleadosBL empleadosBL, AsistenciasBL asistenciasBL, CargosBL cargosBL)
        {
            _viewEngine = viewEngine;
            empleadosBL = empleadosBL;
            asistenciasBL = asistenciasBL;
            cargosBL = cargosBL;
        }

        public async Task<IActionResult> Index()
        {
            var totalEmpleados = (await empleadosBL.ObtenerTodosAsync()).Count;
            var empleadosActivos = (await empleadosBL.ObtenerTodosAsync()).Count(e => e.Estado);
            var totalAsistencias = (await asistenciasBL.ObtenerTodosAsync()).Count;
            var asistenciasExitosas = (await asistenciasBL.ObtenerTodosAsync()).Count(a => a.EstadoReconocimiento == "Exitoso");
            var asistenciasFallidas = (await asistenciasBL.ObtenerTodosAsync()).Count(a => a.EstadoReconocimiento == "Fallido");
            var totalCargos = (await cargosBL.ObtenerTodosAsync()).Count;

            var datosPorCargo = (await empleadosBL.ObtenerTodosConRelacionesAsync())
                .GroupBy(e => e.Cargo?.Nombre ?? "Sin Cargo")
                .Select(g => new { Cargo = g.Key, TotalEmpleados = g.Count() })
                .ToList();
            ViewBag.TotalEmpleados = totalEmpleados;
            ViewBag.EmpleadosActivos = empleadosActivos;
            ViewBag.TotalAsistencias = totalAsistencias;
            ViewBag.AsistenciasExitosas = asistenciasExitosas;
            ViewBag.AsistenciasFallidas = asistenciasFallidas;
            ViewBag.TotalCargos = totalCargos;
            ViewBag.DatosPorCargo = datosPorCargo;

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerDatosReporte()
        {
            var totalEmpleados = (await empleadosBL.ObtenerTodosAsync()).Count;
            var empleadosActivos = (await empleadosBL.ObtenerTodosAsync()).Count(e => e.Estado);
            var totalAsistencias = (await asistenciasBL.ObtenerTodosAsync()).Count;
            var asistenciasExitosas = (await asistenciasBL.ObtenerTodosAsync()).Count(a => a.EstadoReconocimiento == "Exitoso");
            var asistenciasFallidas = (await asistenciasBL.ObtenerTodosAsync()).Count(a => a.EstadoReconocimiento == "Fallido");
            var totalCargos = (await cargosBL.ObtenerTodosAsync()).Count;

            return Json(new
            {
                totalEmpleados,
                empleadosActivos,
                totalAsistencias,
                asistenciasExitosas,
                asistenciasFallidas,
                totalCargos
            });
        }

        [HttpGet]
        public IActionResult DescargarReportePdf()
        {
            string htmlContent = RenderViewToString("Index");
            using (var memoryStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(memoryStream);
                PdfDocument pdf = new PdfDocument(writer);
                ConverterProperties properties = new ConverterProperties();
                HtmlConverter.ConvertToPdf(htmlContent, pdf, properties);
                pdf.Close();
                return File(memoryStream.ToArray(), "application/pdf", "ReporteGeneral.pdf");
            }
        }
        private string RenderViewToString(string viewName)
        {
            using (var stringWriter = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                if (!viewResult.Success) throw new FileNotFoundException($"Vista '{viewName}' no encontrada.");
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, stringWriter, new HtmlHelperOptions());
                viewResult.View.RenderAsync(viewContext).Wait();
                return stringWriter.GetStringBuilder().ToString();
            }
        }

    }
}
