using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Face.EntidadesDeNegocio;

namespace Face.LogicaDeNegocio
{
    public class ReconocimientoService
    {
        private readonly EigenFaceRecognizer _recognizer;
        private readonly Dictionary<int, string> _empleadoLabels;

        public ReconocimientoService()
        {
            _recognizer = new EigenFaceRecognizer();
            _empleadoLabels = new Dictionary<int, string>();
        }

        public void EntrenarModelo(List<Empleados> empleadosConFotos)
        {
            var images = new List<Mat>();
            var labels = new List<int>();

            foreach (var empleado in empleadosConFotos)
            {
                foreach (var foto in empleado.Fotos)
                {
                    using (var ms = new MemoryStream(foto.Foto))
                    {
                        using (var bitmap = new Bitmap(ms))
                        {
                            Mat matImage = bitmap.ToMat();
                            Mat grayImage = new Mat();
                            CvInvoke.CvtColor(matImage, grayImage, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                            images.Add(grayImage);
                            labels.Add(empleado.Id);
                            _empleadoLabels[empleado.Id] = empleado.Nombre;
                        }
                    }
                }
            }

            if (images.Count > 0)
            {
                _recognizer.Train(images.ToArray(), labels.ToArray());
            }
        }

        public string IdentificarEmpleado(byte[] fotoCapturada)
        {
            using (var ms = new MemoryStream(fotoCapturada))
            {
                using (var bitmap = new Bitmap(ms))
                {
                    Mat matImage = bitmap.ToMat();
                    Mat grayImage = new Mat();
                    CvInvoke.CvtColor(matImage, grayImage, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                    var result = _recognizer.Predict(grayImage);

                    if (result.Label != -1 && result.Distance < 3000)
                    {
                        return _empleadoLabels[result.Label];
                    }
                }
            }

            return "Desconocido";
        }
    }
}
