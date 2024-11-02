using Face.AccesoADatos;
using Face.EntidadesDeNegocio;
using Face.LogicaDeNegocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing; // Para Bitmap
using System.IO;
using System.Drawing.Imaging; // Para MemoryStream




namespace Face.UserInterface.Controllers
{
    public class EmpleadosController : Controller
    {
        EmpleadosBL empleadosBL = new EmpleadosBL();
        AsistenciasBL asistenciasBL = new AsistenciasBL();
        HorariosBL horariosBL = new HorariosBL();
        ReportesBL reportesBL = new ReportesBL();
        FotosBL fotosBL = new FotosBL();
        public async Task<IActionResult> Index(Empleados empleados = null)
        {
            if (empleados == null)
                empleados = new Empleados();
            if (empleados.Top_Aux == 0)
                empleados.Top_Aux = 10;
            else if (empleados.Top_Aux == -1)
                empleados.Top_Aux = 0;

            var empleado = await empleadosBL.BuscarAsync(empleados);

            foreach (var empleadoss in empleado)
            {
                empleadoss.Fotos = await fotosBL.ObtenerPorEmpleadoIdAsync(empleadoss.Id);
            }

            var asistencias = await asistenciasBL.ObtenerTodosAsync();
            var horarios = await horariosBL.ObtenerTodosAsync();
            var fotos = await fotosBL.ObtenerTodosAsync();
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

        // Método GET para mostrar la vista de confirmación de eliminación
        public async Task<ActionResult> Delete(int id)
        {
            var empleados = await empleadosBL.ObtenerPorIdAsync(new Empleados { Id = id });
            return View(empleados);
        }

        // Método POST para realizar la eliminación (renombrado a DeleteConfirmed)
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
        // Acción para cargar la vista de captura de fotos
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

        // Acción para guardar las fotos capturadas
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

        // Conversión de Base64 a byte[]
        private byte[] ConvertBase64ToByteArray(string base64Image)
        {
            var base64Data = base64Image.Split(',')[1];
            return Convert.FromBase64String(base64Data);
        }


    }
}