using Face.LogicaDeNegocio;
using Face.UserInterface.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Face.EntidadesDeNegocio;
using System.Linq;

namespace Face.UserInterface.Controllers
{
    public class HomeController : Controller
    {
        private readonly CargosBL _cargosBL = new CargosBL();
        private readonly EmpleadosBL _empleadosBL = new EmpleadosBL();
        private readonly AsistenciasBL _asistenciasBL = new AsistenciasBL();
        private readonly ILogger<HomeController> _logger;

        [HttpGet]
        public async Task<IActionResult> GetDashboardMetrics()
        {
            var totalEmpleados = (await _empleadosBL.ObtenerTodosAsync()).Count;
            var totalAsistencias = (await _asistenciasBL.ObtenerTodosAsync()).Count;
            var totalCargos = await _cargosBL.ObtenerCantidadAsync();
            var totalComentarios = (await _asistenciasBL.ObtenerTodosAsync())
                .Count(a => !string.IsNullOrEmpty(a.Comentarios));

            return Json(new
            {
                totalEmpleados,
                totalAsistencias,
                totalCargos,
                totalComentarios
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

            var empleados = await _empleadosBL.ObtenerTodosAsync();


            ViewBag.HayEmpleados = empleados.Any();

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

        [HttpGet]
        public async Task<IActionResult> GetDistribucionEmpleados()
        {
            var empleados = await _empleadosBL.ObtenerTodosConRelacionesAsync();
            var distribucionPorCargo = empleados
                .GroupBy(e => e.Cargo?.Nombre ?? "Sin Cargo")
                .Select(g => new { Cargo = g.Key, Total = g.Count() })
                .ToDictionary(g => g.Cargo, g => g.Total);

            return Json(distribucionPorCargo);
        }
        public async Task<IActionResult> Dashboard()
        {

            var empleados = await _empleadosBL.ObtenerTodosAsync();


            ViewBag.HayEmpleados = empleados.Any();

            return View();
        }
    }
}
