using Face.AccesoADatos;
using Face.EntidadesDeNegocio;
using Face.LogicaDeNegocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Face.UserInterface.Controllers
{
    public class EmpleadosController : Controller
    {
       EmpleadosBL empleadosBL = new EmpleadosBL();
       AsistenciasBL asistenciasBL = new AsistenciasBL();
       HorariosBL horariosBL = new HorariosBL();
       ReportesBL reportesBL = new ReportesBL();
       public async Task<IActionResult> Index(Empleados empleados = null)
       {
            //var reportes = await reportesBL.ObtenerTodosAsync();
            if (empleados == null) 
                empleados= new Empleados();
            if (empleados.Top_Aux == 0)
                empleados.Top_Aux = 10;
            else if (empleados.Top_Aux == -1)
                empleados.Top_Aux = 0;

            var empleado = await empleadosBL.BuscarAsync(empleados);
            var asistencias = await asistenciasBL.ObtenerTodosAsync();
            var horarios = await horariosBL.ObtenerTodosAsync();
            var reportes = await reportesBL.ObtenerTodosAsync();
            ViewBag.Top = empleados.Top_Aux;
            return View(empleado);
       }


        public async Task<IActionResult> Details(int id)
        {
            var empleado = await empleadosBL.ObtenerPorIdConRelacionesAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }
        public async Task<IActionResult> Crear()
        {
            ViewBag.Error = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Empleados empleados, IFormFile imagen)
        {
            try
            {
                empleados.FechaRegistro = DateTime.Now;
                int result = await empleadosBL.CrearAsync(empleados);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View(empleados);
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            var empleado = await empleadosBL.ObtenerPorIdAsync(new Empleados { Id = id });
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Empleados pEmpleados)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pEmpleados.Cargo))
                {
                    ModelState.AddModelError("Cargo", "El campo Cargo es obligatorio.");
                    return View(pEmpleados);
                }

                if (ModelState.IsValid)
                {
                    // Actualiza los datos del empleado
                    int result = await empleadosBL.ModificarAsync(pEmpleados);
                    return RedirectToAction(nameof(Index));
                }
                return View(pEmpleados);
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View(pEmpleados);
            }
        }


        // GET: LibrosController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var empleados = await empleadosBL.ObtenerPorIdAsync(new Empleados { Id = id });
            return View(empleados);
        }

        // POST: LibrosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Empleados empleados)
        {
            try
            {
                var empleado = await empleadosBL.ObtenerPorIdAsync(new Empleados { Id = id });
                if (empleado == null)
                {
                    return NotFound();  // Si no se encuentra el empleado, retorna NotFound
                }
                int result = await empleadosBL.EliminarAsync(empleado);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al eliminar el empleado: " + ex.Message;
                var empleadoDb = await empleadosBL.ObtenerPorIdAsync(new Empleados { Id = id });
                return View(empleadoDb);
            }
        }
    }
}
