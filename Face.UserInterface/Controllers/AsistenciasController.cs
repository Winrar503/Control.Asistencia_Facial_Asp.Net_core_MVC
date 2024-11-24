using Face.AccesoADatos;
using Face.EntidadesDeNegocio;
using Face.LogicaDeNegocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Face.UserInterface.Controllers
{
    public class AsistenciasController : Controller
    {
        AsistenciasBL asistenciasBL = new AsistenciasBL();
        EmpleadosBL empleadosBL = new EmpleadosBL();
        public async Task<IActionResult> Index(Asistencias asistencias = null)
        {
            if (asistencias == null)
                asistencias = new Asistencias();
            if (asistencias.Top_Aux == 0)
                asistencias.Top_Aux = 10;
            else if (asistencias.Top_Aux == -1)
                asistencias.Top_Aux = 0;
            var lisAsistencia = await asistenciasBL.BuscarAsync(asistencias);
            var empleados = await empleadosBL.ObtenerTodosAsync();

            ViewBag.Empleados = empleados;
            ViewBag.Top = asistencias.Top_Aux;
            return View(lisAsistencia);
        }
        public async Task<IActionResult> Details(int id)
        {
            var asistencia = await asistenciasBL.ObtenerPorIdConRelacionesAsync(id);
            if (asistencia == null)
            {
                return NotFound();
            }
            return View(asistencia);
        }
        public async Task<IActionResult> Create()
        {
            var empleados = await empleadosBL.ObtenerTodosAsync();
            ViewBag.Empleados = empleados;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Asistencias asistencias)
        {
            try
            {
                int result = await asistenciasBL.CrearAsync(asistencias);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                var empleados = await empleadosBL.ObtenerTodosAsync();
                ViewBag.Empleados = empleados;
                return View(asistencias);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            var asistencia = await asistenciasBL.ObtenerPorIdAsync(new Asistencias { Id = id });
            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Asistencias pAsistencias)
        {
            if (id != pAsistencias.Id)
            {
                return NotFound();
            }
            try
            {
                await asistenciasBL.ModificarAsync(pAsistencias);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
            }

            return View(pAsistencias);
        }
        public async Task<ActionResult> Delete(int id)
        {
            var asistencia = await asistenciasBL.ObtenerPorIdAsync(new Asistencias { Id = id });
            return View(asistencia);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> Delete(int id, Asistencias asistencias)
        {
            try
            {
                var asistencia = await asistenciasBL.ObtenerPorIdAsync(new Asistencias { Id = id });
                if (asistencia == null)
                {
                    return NotFound();
                }
                int result = await asistenciasBL.EliminarAsync(asistencia);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al eliminar el empleado: " + ex.Message;
                var asistenciaBd = await asistenciasBL.ObtenerPorIdAsync(new Asistencias { Id = id });
                return View(asistenciaBd);
            }
        }
    }
}
