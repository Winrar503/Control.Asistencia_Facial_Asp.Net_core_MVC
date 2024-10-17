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
        //Metodos de captura de empleado y registrarlo 
        // Vista para capturar las fotos
        public IActionResult Capturar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GuardarFotos(string image1, string image2, string image3)
        {
            try
            {
                // Validar que al menos una de las imágenes no sea nula o vacía
                if (string.IsNullOrEmpty(image1) || string.IsNullOrEmpty(image2) || string.IsNullOrEmpty(image3))
                {
                    ViewBag.Error = "No se han recibido todas las fotos requeridas.";
                    return View("Error");
                }

                // Convierte las imágenes base64 a byte arrays
                var foto1Bytes = ConvertBase64ToByteArray(image1);
                var foto2Bytes = ConvertBase64ToByteArray(image2);
                var foto3Bytes = ConvertBase64ToByteArray(image3);

                // Realizar reconocimiento facial
                var empleadoReconocido = await RealizarReconocimientoFacial(foto1Bytes);

                if (empleadoReconocido != null)
                {
                    ViewBag.Message = "Empleado reconocido: " + empleadoReconocido.Nombre;
                }
                else
                {
                    ViewBag.Message = "No se encontró coincidencia";
                }

                // Retornar la vista de resultado
                return View("Resultado", empleadoReconocido);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                ViewBag.Error = "Ocurrió un error: " + ex.Message;
                return View("Error");
            }
        }


        private byte[] ConvertBase64ToByteArray(string base64Image)
        {
            var base64Data = base64Image.Split(',')[1]; // Remueve la metadata de la imagen base64
            return Convert.FromBase64String(base64Data);
        }

        private async Task<Empleados> RealizarReconocimientoFacial(byte[] fotoCapturada)
        {
            var recognizer = new Emgu.CV.Face.LBPHFaceRecognizer();
            var empleados = await empleadosBL.ObtenerTodosAsync();

            foreach (var empleado in empleados)
            {
                if (empleado.Foto != null)
                {
                    using (var msEmpleado = new MemoryStream(empleado.Foto))
                    {
                        using (var bitmapEmpleado = new Bitmap(msEmpleado))
                        {
                            var fotoBase = BitmapToMat(bitmapEmpleado); // Usamos el método BitmapToMat que te proporciono abajo
                        }
                    }

                    using (var msCapturada = new MemoryStream(fotoCapturada))
                    {
                        using (var bitmapCapturada = new Bitmap(msCapturada))
                        {
                            var fotoComparar = BitmapToMat(bitmapCapturada);

                            var result = recognizer.Predict(fotoComparar);

                            if (result.Label == empleado.Id)
                            {
                                return empleado; 
                            }
                        }
                    }
                }
            }

            return null;
        }

        private Mat BitmapToMat(Bitmap bitmap)
        {
            Mat mat = new Mat(); 

            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat
            );

            using (Image<Bgr, Byte> tempImage = new Image<Bgr, Byte>(bitmap.Width, bitmap.Height, bitmapData.Stride, bitmapData.Scan0))
            {
                tempImage.Mat.CopyTo(mat);
            }

            bitmap.UnlockBits(bitmapData);

            return mat; 
        }


    }
}

