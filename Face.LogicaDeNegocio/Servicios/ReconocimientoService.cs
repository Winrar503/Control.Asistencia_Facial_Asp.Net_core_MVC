using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Face.EntidadesDeNegocio;
using Emgu.CV.CvEnum;

namespace Face.LogicaDeNegocio
{
    public class ReconocimientoService
    {
        private readonly EigenFaceRecognizer _recognizer;
        private readonly Dictionary<int, string> _empleadoLabels;
        private const int PredictionThreshold = 2500;
        private const int ImageSize = 100;

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
                            Mat matImage = PreprocessImage(bitmap);
                            images.Add(matImage);
                            labels.Add(empleado.Id);

                            
                            var augmentedImages = AugmentData(matImage);
                            images.AddRange(augmentedImages);

                            labels.AddRange(new int[augmentedImages.Count].Select(_ => empleado.Id));

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


        //public void EntrenarModelo(List<Empleados> empleadosConFotos)
        //{
        //    var images = new List<Mat>();
        //    var labels = new List<int>();

        //    foreach (var empleado in empleadosConFotos)
        //    {
        //        foreach (var foto in empleado.Fotos)
        //        {
        //            using (var ms = new MemoryStream(foto.Foto))
        //            {
        //                using (var bitmap = new Bitmap(ms))
        //                {
        //                    Mat matImage = PreprocessImage(bitmap);
        //                    images.Add(matImage);
        //                    labels.Add(empleado.Id);


        //                    images.AddRange(AugmentData(matImage));
        //                    labels.AddRange(new int[] { empleado.Id, empleado.Id, empleado.Id });

        //                    _empleadoLabels[empleado.Id] = empleado.Nombre;
        //                }
        //            }
        //        }
        //    }

        //    if (images.Count > 0)
        //    {
        //        _recognizer.Train(images.ToArray(), labels.ToArray());
        //    }
        //}

        public string IdentificarEmpleado(byte[] fotoCapturada)
        {
            using (var ms = new MemoryStream(fotoCapturada))
            {
                using (var bitmap = new Bitmap(ms))
                {
                    Mat grayImage = PreprocessImage(bitmap);
                    var result = _recognizer.Predict(grayImage);

                    if (result.Label != -1 && result.Distance < PredictionThreshold)
                    {
                        return _empleadoLabels.ContainsKey(result.Label) ? _empleadoLabels[result.Label] : "Desconocido";
                    }
                }
            }

            return "Desconocido";
        }

        private Mat PreprocessImage(Bitmap bitmap)
        {
            Mat matImage = bitmap.ToMat();
            Mat grayImage = new Mat();

            CvInvoke.CvtColor(matImage, grayImage, ColorConversion.Bgr2Gray);
            CvInvoke.Resize(grayImage, grayImage, new Size(ImageSize, ImageSize));

            CvInvoke.EqualizeHist(grayImage, grayImage);

            return grayImage;
        }
        private List<Mat> AugmentData(Mat originalImage)
        {
            var augmentedImages = new List<Mat>();

            Mat rotatedImage = new Mat();
            CvInvoke.Transpose(originalImage, rotatedImage);
            CvInvoke.Flip(rotatedImage, rotatedImage, FlipType.Horizontal);
            augmentedImages.Add(rotatedImage);

            rotatedImage = new Mat();
            CvInvoke.Transpose(originalImage, rotatedImage);
            CvInvoke.Flip(rotatedImage, rotatedImage, FlipType.Vertical);
            augmentedImages.Add(rotatedImage);

            rotatedImage = new Mat();
            CvInvoke.Flip(originalImage, rotatedImage, FlipType.Horizontal);
            CvInvoke.Flip(rotatedImage, rotatedImage, FlipType.Vertical);
            augmentedImages.Add(rotatedImage);

            Mat brighterImage = new Mat();
            originalImage.ConvertTo(brighterImage, DepthType.Cv8U, 1.2, 20);
            augmentedImages.Add(brighterImage);

            Mat blurredImage = new Mat();
            CvInvoke.GaussianBlur(originalImage, blurredImage, new Size(5, 5), 0);
            augmentedImages.Add(blurredImage);

            return augmentedImages;
        }


        //private List<Mat> AugmentData(Mat originalImage)
        //{
        //    var augmentedImages = new List<Mat>();

        //    Mat rotatedImage = new Mat();
        //    CvInvoke.Transpose(originalImage, rotatedImage);
        //    CvInvoke.Flip(rotatedImage, rotatedImage, FlipType.Horizontal);
        //    augmentedImages.Add(rotatedImage);

        //    rotatedImage = new Mat();
        //    CvInvoke.Transpose(originalImage, rotatedImage);
        //    CvInvoke.Flip(rotatedImage, rotatedImage, FlipType.Vertical);
        //    augmentedImages.Add(rotatedImage);

        //    rotatedImage = new Mat();
        //    CvInvoke.Flip(originalImage, rotatedImage, FlipType.Horizontal);
        //    CvInvoke.Flip(rotatedImage, rotatedImage, FlipType.Vertical);
        //    augmentedImages.Add(rotatedImage);

        //    return augmentedImages;
        //}

    }
}