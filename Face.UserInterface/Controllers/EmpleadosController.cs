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
            //var reportes = await reportesBL.ObtenerTodosAsync();
            if (empleados == null)
                empleados = new Empleados();
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
        public async Task<IActionResult> Crear(int? foto1Id, int? foto2Id, int? foto3Id)
        {
            ViewBag.Foto1Id = foto1Id;
            ViewBag.Foto2Id = foto2Id;
            ViewBag.Foto3Id = foto3Id;

            ViewBag.Error = "";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Empleados empleados, int? foto1Id, int? foto2Id, int? foto3Id)
        {
            try
            {
                empleados.FechaRegistro = DateTime.Now;
                int result = await empleadosBL.CrearAsync(empleados);

                if (result > 0)
                {
                    if (foto1Id.HasValue && foto2Id.HasValue && foto3Id.HasValue)
                    {
                        var foto1 = await fotosBL.ObtenerPorIdAsync(new Fotos { Id = foto1Id.Value });
                        var foto2 = await fotosBL.ObtenerPorIdAsync(new Fotos { Id = foto2Id.Value });
                        var foto3 = await fotosBL.ObtenerPorIdAsync(new Fotos { Id = foto3Id.Value });

                        if (foto1 != null && foto2 != null && foto3 != null)
                        {
                            foto1.EmpleadosId = empleados.Id;
                            foto2.EmpleadosId = empleados.Id;
                            foto3.EmpleadosId = empleados.Id;

                            await fotosBL.ModificarAsyn(foto1);
                            await fotosBL.ModificarAsyn(foto2);
                            await fotosBL.ModificarAsyn(foto3);
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
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
        // Métodos para capturar fotos y realizar reconocimiento facial

        public IActionResult Capturar()
        {
            var modelo = new Empleados();
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarFotos(string image1, string image2, string image3)
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

                var nuevaFoto1 = new Fotos { Foto = foto1Bytes };
                var nuevaFoto2 = new Fotos { Foto = foto2Bytes };
                var nuevaFoto3 = new Fotos { Foto = foto3Bytes };

                await fotosBL.CrearAsync(nuevaFoto1);
                await fotosBL.CrearAsync(nuevaFoto2);
                await fotosBL.CrearAsync(nuevaFoto3);

                return RedirectToAction("Crear", new { foto1Id = nuevaFoto1.Id, foto2Id = nuevaFoto2.Id, foto3Id = nuevaFoto3.Id });
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error: " + ex.Message;
                return View("Error");
            }
        }
        private byte[] ConvertBase64ToByteArray(string base64Image)
        {
            try
            {
                var base64Data = base64Image.Split(',')[1];
                return Convert.FromBase64String(base64Data);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al convertir la imagen Base64 a byte array", ex);
            }
        }
        // Reconocimiento facial
        private async Task<Empleados> RealizarReconocimientoFacial(byte[] fotoCapturada)
        {
            var recognizer = new Emgu.CV.Face.LBPHFaceRecognizer();
            var empleados = await empleadosBL.ObtenerTodosAsync();

            foreach (var empleado in empleados)
            {
                var fotoEmpleado = empleado.Fotos.FirstOrDefault();
                if (fotoEmpleado != null)
                {
                    using (var msEmpleado = new MemoryStream(fotoEmpleado.Foto))
                    {
                        using (var bitmapEmpleado = new Bitmap(msEmpleado))
                        {
                            var fotoBase = BitmapToMat(bitmapEmpleado);
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
                bitmap.PixelFormat);

            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] buffer = new byte[bytes]; 
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, buffer, 0, bytes);

            bitmap.UnlockBits(bitmapData);

            CvInvoke.Imdecode(buffer, ImreadModes.Color, mat);

            return mat;
        }

    }
}