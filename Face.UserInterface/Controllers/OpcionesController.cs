//using Face.EntidadesDeNegocio;
//using Face.LogicaDeNegocio;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Face.UserInterface.Controllers
//{
//    public class OpcionesController : Controller
//    {
//        private readonly AsistenciasBL asistenciasBL;
//        private readonly EmpleadosBL empleadosBL;

//        public OpcionesController(AsistenciasBL asistenciasBL, EmpleadosBL empleadosBL)
//        {
//            this.asistenciasBL = asistenciasBL;
//            this.empleadosBL = empleadosBL;
//        }

//        // GET: OpcionesController
//        public async Task<IActionResult> Index()
//        {
//            var asistencias = await asistenciasBL.ObtenerTodosAsync();
//            foreach (var asistencia in asistencias)
//            {
//                asistencia.Empleado = await empleadosBL.ObtenerPorIdAsync(new Empleados { Id = asistencia.EmpleadoId });
//            }
//            return View(asistencias);
//        }

//        // GET: OpcionesController/Details/5
//        public async Task<IActionResult> Details(int id)
//        {
//            var asistencia = await asistenciasBL.ObtenerPorIdAsync(new Asistencias { Id = id });
//            if (asistencia == null)
//            {
//                return NotFound();
//            }

//            asistencia.Empleado = await empleadosBL.ObtenerPorIdAsync(new Empleados { Id = asistencia.EmpleadoId });

//            return View(asistencia);
//        }

//        // GET: OpcionesController/Create
//        public async Task<IActionResult> Create()
//        {
//            ViewBag.Empleados = await ObtenerEmpleadosSelectList();
//            return View();
//        }

//        // POST: OpcionesController/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Asistencias asistencia)
//        {
//            if (ModelState.IsValid)
//            {
//                asistencia.FechaHora = DateTime.Now;
//                int result = await asistenciasBL.CrearAsync(asistencia);
//                if (result > 0)
//                {
//                    return RedirectToAction(nameof(Index));
//                }
//                ViewBag.Error = "No se pudo crear la asistencia.";
//            }
//            ViewBag.Empleados = await ObtenerEmpleadosSelectList();
//            return View(asistencia);
//        }

//        // GET: OpcionesController/Edit/5
//        public async Task<IActionResult> Edit(int id)
//        {
//            var asistencia = await asistenciasBL.ObtenerPorIdAsync(new Asistencias { Id = id });
//            if (asistencia == null)
//            {
//                return NotFound();
//            }

//            ViewBag.Empleados = await ObtenerEmpleadosSelectList();
//            return View(asistencia);
//        }

//        // POST: OpcionesController/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, Asistencias pAsistencia)
//        {
//            if (id != pAsistencia.Id)
//            {
//                return NotFound();
//            }
//            try
//            {
//                await asistenciasBL.ModificarAsync(pAsistencia);
//                return RedirectToAction(nameof(Index));
//            }
//            catch (Exception ex)
//            {
//                ViewBag.Empleados = await ObtenerEmpleadosSelectList();
//                ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
//            }

//            return View(pAsistencia);
//        }

//        // GET: OpcionesController/Delete/5
//        public async Task<IActionResult> Delete(int id)
//        {
//            var asistencia = await asistenciasBL.ObtenerPorIdAsync(new Asistencias { Id = id });
//            if (asistencia == null)
//            {
//                return NotFound();
//            }

//            return View(asistencia);
//        }

//        // POST: OpcionesController/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            try
//            {
//                var asistencia = await asistenciasBL.ObtenerPorIdAsync(new Asistencias { Id = id });
//                if (asistencia == null)
//                {
//                    return NotFound();
//                }
//                int result = await asistenciasBL.EliminarAsync(asistencia);
//                return RedirectToAction(nameof(Index));
//            }
//            catch (Exception ex)
//            {
//                ViewBag.Error = "Ocurrió un error al eliminar la asistencia: " + ex.Message;
//                var asistenciaDb = await asistenciasBL.ObtenerPorIdAsync(new Asistencias { Id = id });
//                return View(asistenciaDb);
//            }
//        }

//        private async Task<List<SelectListItem>> ObtenerEmpleadosSelectList()
//        {
//            var empleados = await empleadosBL.ObtenerTodosAsync();
//            return empleados.Select(e => new SelectListItem
//            {
//                Value = e.Id.ToString(),
//                Text = e.Nombre
//            }).ToList();
//        }
//    }
//}