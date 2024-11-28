using Face.LogicaDeNegocio;
using Face.UserInterface.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Face.EntidadesDeNegocio;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using iText.Html2pdf;
using iText.Kernel.Pdf;



namespace Face.UserInterface.Controllers
{
    public class HomeController : Controller
    {
        private readonly CargosBL _cargosBL = new CargosBL();
        private readonly EmpleadosBL _empleadosBL = new EmpleadosBL();
        private readonly AsistenciasBL _asistenciasBL = new AsistenciasBL();
        private readonly ILogger<HomeController> _logger;
        private readonly ICompositeViewEngine _viewEngine;


        public async Task<IActionResult> GetDashboardMetrics()
        {
            var totalEmpleadosTask = _empleadosBL.ObtenerTodosAsync();
            var totalAsistenciasTask = _asistenciasBL.ObtenerTodosAsync();
            var totalCargosTask = _cargosBL.ObtenerCantidadAsync();

            await Task.WhenAll(totalEmpleadosTask, totalAsistenciasTask, totalCargosTask);

            var totalEmpleados = totalEmpleadosTask.Result.Count;
            var totalAsistencias = totalAsistenciasTask.Result.Count;
            var totalCargos = totalCargosTask.Result;

            // Calcular total de comentarios registrados
            var totalComentarios = totalAsistenciasTask.Result
                .Count(a => !string.IsNullOrEmpty(a.Comentarios));

            return Json(new
            {
                totalEmpleados,
                totalAsistencias,
                totalCargos,
                totalComentarios // Incluir comentarios registrados
            });
        }



        [HttpGet]
        public async Task<IActionResult> ObtenerComentarios()
        {
            var asistencias = await _asistenciasBL.ObtenerTodosConRelacionesAsync();

            var datos = asistencias
                .Where(a => !string.IsNullOrEmpty(a.Comentarios))
                .Select(a => new
                {
                    Empleado = a.Empleados != null ? a.Empleados.Nombre : "Desconocido",
                    Comentario = !string.IsNullOrEmpty(a.Comentarios) ? a.Comentarios : "Sin comentario"
                })
                .ToList();

            return Json(datos);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarComentario([FromBody] ComentarioDto comentarioDto)
        {
            var empleado = await _empleadosBL.ObtenerPorNombreAsync(comentarioDto.Empleado);

            if (empleado == null)
            {
                return Json(new { success = false, message = "Empleado no encontrado" });
            }
            var asistencia = new Asistencias
            {
                EmpleadosId = empleado.Id,
                Comentarios = comentarioDto.Comentario,
                Fecha = DateTime.Now,
                Tipo = "Reconocimiento",
                EstadoReconocimiento = "Exitoso"
            };

            await _asistenciasBL.CrearAsync(asistencia);
            return Json(new { success = true });
        }

        public class ComentarioDto
        {
            public string Empleado { get; set; }
            public string Comentario { get; set; }
        }

        public async Task<IActionResult> Inicio()
        {
            var cargos = await _cargosBL.ObtenerTodosAsync();
            ViewBag.HayCargos = cargos != null && cargos.Count > 0;
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> GetAsistenciasMensuales()
        {
            var asistencias = await _asistenciasBL.ObtenerTodosAsync();
            var asistenciasPorMes = asistencias
                .Where(a => a.Fecha.Year == DateTime.Now.Year) 
                .GroupBy(a => a.Fecha.Month)
                .Select(g => new { Mes = g.Key, Total = g.Count() })
                .ToDictionary(g => g.Mes, g => g.Total);
            var asistenciasMensuales = Enumerable.Range(1, 12).Select(m => asistenciasPorMes.ContainsKey(m) ? asistenciasPorMes[m] : 0).ToArray();

            return Json(asistenciasMensuales);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetDistribucionEmpleados()
        //{
        //    var empleados = await _empleadosBL.ObtenerTodosConRelacionesAsync();
        //    var distribucionPorCargo = empleados
        //        .GroupBy(e => e.Cargo?.Nombre ?? "Sin Cargo")
        //        .Select(g => new { Cargo = g.Key, Total = g.Count() })
        //        .ToDictionary(g => g.Cargo, g => g.Total);

        //    return Json(distribucionPorCargo);
        //}
        [HttpGet]
        public async Task<IActionResult> GetDistribucionEmpleados()
        {
            // Obtener todos los cargos
            var cargos = await _cargosBL.ObtenerTodosAsync();

            // Obtener todos los empleados con sus relaciones
            var empleados = await _empleadosBL.ObtenerTodosConRelacionesAsync();

            // Crear la distribución por cargo, asegurando que todos los cargos estén representados
            var distribucionPorCargo = cargos
                .Select(cargo => new
                {
                    Cargo = cargo.Nombre,
                    Total = empleados.Count(e => e.CargoId == cargo.Id) // Contar empleados asignados al cargo
                })
                .ToDictionary(d => d.Cargo, d => d.Total);

            return Json(distribucionPorCargo);
        }


        public async Task<IActionResult> Dashboard()
        {
            var cantidadCargos = await _cargosBL.ObtenerCantidadAsync();
            ViewBag.HayCargos = cantidadCargos > 0;
            return View();
        }


        public HomeController(ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
        }

        public IActionResult Manual()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DescargarManualPdf()
        {
            string htmlContent = RenderViewToString("Manual");

            using (var memoryStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(memoryStream);
                PdfDocument pdf = new PdfDocument(writer);
                ConverterProperties properties = new ConverterProperties();

                HtmlConverter.ConvertToPdf(htmlContent, pdf, properties);
                pdf.Close();
                return File(memoryStream.ToArray(), "application/pdf", "ManualDeUsuario.pdf");
            }
        }
        private string RenderViewToString(string viewName)
        {
            using (var stringWriter = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

                if (!viewResult.Success)
                {
                    throw new FileNotFoundException($"La vista '{viewName}' no fue encontrada.");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    stringWriter,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext).Wait();
                return stringWriter.GetStringBuilder().ToString();
            }
        }
        [HttpGet]
        public async Task<IActionResult> VerificarEmpleados()
        {
            var totalEmpleados = (await _empleadosBL.ObtenerTodosAsync()).Count;
            return Json(new { hayEmpleados = totalEmpleados > 0 });
        }


    }
}
