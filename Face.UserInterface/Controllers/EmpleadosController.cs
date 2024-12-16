using Face.EntidadesDeNegocio;
using Face.LogicaDeNegocio;
using Microsoft.AspNetCore.Mvc;

namespace Face.UserInterface.Controllers
{

    public class EmpleadosController : Controller
    {
        EmpleadosBL empleadosBL = new EmpleadosBL();
        AsistenciasBL asistenciasBL = new AsistenciasBL();
        HorariosBL horariosBL = new HorariosBL();
        ReportesBL reportesBL = new ReportesBL();
        FotosBL fotosBL = new FotosBL();
        CargosBL cargosBL = new CargosBL();

        public async Task<IActionResult> Index(int? cargoId, Empleados empleados = null)
        {

            if (empleados == null)
                empleados = new Empleados();
            if (empleados.Top_Aux == 0)
                empleados.Top_Aux = 10;
            else if (empleados.Top_Aux == -1)
                empleados.Top_Aux = 0;


            List<Empleados> empleado;

            if (cargoId == null || cargoId == 0)
            {

                empleado = await empleadosBL.ObtenerTodosAsync();
            }
            else
            {

                empleado = await empleadosBL.BuscarAsync(new Empleados { CargoId = cargoId.Value });

                if (empleado == null || !empleado.Any())
                {
                    empleado = new List<Empleados>();
                }
            }

            foreach (var empleadoss in empleado)
            {
                empleadoss.Fotos = await fotosBL.ObtenerPorEmpleadoIdAsync(empleadoss.Id);
                empleadoss.Cargo = await cargosBL.ObtenerPorIdAsync(empleadoss.CargoId);
            }

            var cargos = await cargosBL.ObtenerTodosAsync();
            ViewBag.Cargos = cargos;
            ViewBag.CargoSeleccionado = cargoId ?? 0;
            ViewBag.TotalCargos = cargos.Count;
            ViewBag.Top = empleados.Top_Aux;
            return View(empleado);
        }

        //public async Task<IActionResult> Index(int? cargoId, int page = 1, int pageSize = 5, Empleados empleadosFiltros = null)
        //{

        //    var empleadosFiltro = new Empleados();
        //    if (empleadosFiltro.Top_Aux == 0)
        //        empleadosFiltro.Top_Aux = 10;

        //    List<Empleados> empleados;

        //    // Filtrar empleados por cargo si se especifica un cargoId
        //    if (cargoId == null || cargoId == 0)
        //    {
        //        empleados = await empleadosBL.ObtenerTodosAsync();
        //    }
        //    else
        //    {
        //        empleados = await empleadosBL.BuscarAsync(new Empleados { CargoId = cargoId.Value });
        //        if (empleados == null || !empleados.Any())
        //        {
        //            empleados = new List<Empleados>();
        //        }
        //    }

        //    // Paginación
        //    var totalEmpleados = empleados.Count; // Total de empleados sin paginación
        //    empleados = empleados.Skip((page - 1) * pageSize).Take(pageSize).ToList(); // Aplica paginación

        //    // Cargar relaciones (Fotos y Cargos) solo para empleados paginados
        //    foreach (var empleado in empleados)
        //    {
        //        empleado.Fotos = await fotosBL.ObtenerPorEmpleadoIdAsync(empleado.Id);
        //        empleado.Cargo = await cargosBL.ObtenerPorIdAsync(empleado.CargoId);
        //    }

        //    // Obtener lista de cargos para filtros
        //    var cargos = await cargosBL.ObtenerTodosAsync();
        //    ViewBag.Cargos = cargos;
        //    ViewBag.CargoSeleccionado = cargoId ?? 0;

        //    // Datos para la paginación en la vista
        //    ViewBag.TotalPages = (int)Math.Ceiling((double)totalEmpleados / pageSize); // Total de páginas
        //    ViewBag.CurrentPage = page; // Página actual
        //    ViewBag.PageSize = pageSize; // Tamaño de página

        //    return View(empleados);
        //}


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
            ViewBag.Cargos = await cargosBL.ObtenerTodosAsync(); 
            ViewBag.Error = "";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Empleados empleados)
        {
            try
            {
                empleados.FechaRegistro = DateTime.Now;
                int result = await empleadosBL.CrearAsync(empleados);

                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = "No se pudo crear el empleado.";
                return View(empleados);
            }
            catch (Exception ex)
            {
                ViewBag.Cargos = await cargosBL.ObtenerTodosAsync();
                ViewBag.Error = ex.Message;
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
            if (id != pEmpleados.Id)
            {
                return NotFound();
            }
            try
            {
                await empleadosBL.ModificarAsync(pEmpleados);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
            }

            return View(pEmpleados);
        }
        public async Task<ActionResult> Delete(int id)
        {
            var empleados = await empleadosBL.ObtenerPorIdAsync(new Empleados { Id = id });
            return View(empleados);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var empleado = await empleadosBL.ObtenerPorIdAsync(new Empleados { Id = id });
                if (empleado == null)
                {
                    return NotFound();
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
        [HttpPost]
        [Route("Empleados/DeleteConfirmed/{id}")]
        public async Task<IActionResult> DeleteConfirmedJson(int id)
        {
            try
            {   var empleado = await empleadosBL.ObtenerPorIdAsync(new Empleados { Id = id });
                if (empleado == null)
                {
                    return Json(new { success = false, message = "El empleado no fue encontrado en la base de datos." });
                }
                int result = await empleadosBL.EliminarAsync(empleado);
                if (result > 0)
                {
                    return Json(new { success = true, message = "Empleado eliminado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo eliminar el empleado. Inténtalo de nuevo." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar el empleado.", details = ex.Message });
            }
        }


        public async Task<IActionResult> CapturarFotos(int empleadoId)
        {
            var empleado = await empleadosBL.ObtenerPorIdAsync(new Empleados { Id = empleadoId });
            if (empleado == null)
            {
                return NotFound();
            }
            var fotos = empleado.Fotos;
            return View(empleado);
        }
        [HttpPost]
        public async Task<IActionResult> GuardarFotos(int empleadoId, string image1, string image2, string image3)
        {
            try
            {
                if (string.IsNullOrEmpty(image1) || string.IsNullOrEmpty(image2) || string.IsNullOrEmpty(image3))
                {
                    ViewBag.Error = "No se han recibido todas las fotos requeridas.";
                    return View("Error");
                }
                var foto1Bytes = ConvertBase64ToByteArray(image1);
                var foto2Bytes = ConvertBase64ToByteArray(image2);
                var foto3Bytes = ConvertBase64ToByteArray(image3);
                if (foto1Bytes.Length == 0 || foto2Bytes.Length == 0 || foto3Bytes.Length == 0)
                {
                    ViewBag.Error = "Una o más fotos no se han capturado correctamente.";
                    return View("Error");
                }
                var nuevaFoto1 = new Fotos { EmpleadosId = empleadoId, Foto = foto1Bytes, NombreFoto = "lado izquierdo" };
                var nuevaFoto2 = new Fotos { EmpleadosId = empleadoId, Foto = foto2Bytes, NombreFoto = "centro" };
                var nuevaFoto3 = new Fotos { EmpleadosId = empleadoId, Foto = foto3Bytes, NombreFoto = "lado derecho" };
                await fotosBL.CrearAsync(nuevaFoto1);
                await fotosBL.CrearAsync(nuevaFoto2);
                await fotosBL.CrearAsync(nuevaFoto3);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error: " + ex.Message;
                return View("Error");
            }
        }
        private byte[] ConvertBase64ToByteArray(string base64Image)
        {
            var base64Data = base64Image.Split(',')[1];
            return Convert.FromBase64String(base64Data);
        }
    }
}