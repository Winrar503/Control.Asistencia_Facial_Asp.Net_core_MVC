//using System;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Face.LogicaDeNegocio
//{
//    public class ChatService
//    {
//        private readonly EmpleadosBL _empleadosBL = new EmpleadosBL();
//        private readonly AsistenciasBL _asistenciasBL = new AsistenciasBL();

//        public async Task<string> ObtenerRespuestaAsync(string pregunta)
//        {
//            try
//            {
//                if (pregunta.Contains("empleados", StringComparison.OrdinalIgnoreCase))
//                {
//                    var empleados = await _empleadosBL.ObtenerTodosAsync();
//                    var total = empleados?.Count ?? 0; // Evita errores si la lista es null
//                    return $"Actualmente hay {total} empleados registrados.";
//                }
//                else if (pregunta.Contains("activos", StringComparison.OrdinalIgnoreCase))
//                {
//                    var empleados = await _empleadosBL.ObtenerTodosAsync();
//                    var activos = empleados?.Count(e => e.Estado) ?? 0;
//                    return $"Actualmente hay {activos} empleados activos.";
//                }
//                else if (pregunta.Contains("asistieron hoy", StringComparison.OrdinalIgnoreCase))
//                {
//                    var hoy = DateTime.Today;
//                    var asistencias = await _asistenciasBL.ObtenerTodosAsync();
//                    var asistenciasHoy = asistencias?.Count(a => a.Fecha.Date == hoy) ?? 0;
//                    return $"Hoy asistieron {asistenciasHoy} empleados.";
//                }
//                else
//                {
//                    return "Lo siento, no entiendo tu pregunta. Intenta algo como '¿Cuántos empleados hay?' o '¿Cuántos están activos?'.";
//                }
//            }
//            catch (Exception ex)
//            {
//                return $"Error procesando tu pregunta: {ex.Message}";
//            }
//        }
//    }
//}

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Face.LogicaDeNegocio
{
    public class ChatService
    {
        private readonly EmpleadosBL _empleadosBL = new EmpleadosBL();
        private readonly AsistenciasBL _asistenciasBL = new AsistenciasBL();

        public async Task<string> ObtenerRespuestaAsync(string pregunta)
        {
            try
            {
                if (pregunta.Contains("empleados", StringComparison.OrdinalIgnoreCase))
                {
                    var empleados = await _empleadosBL.ObtenerTodosAsync();
                    var total = empleados?.Count ?? 0;
                    return total == 1
                        ? $"Actualmente hay 1 empleado registrado."
                        : $"Actualmente hay {total} empleados registrados.";
                }
                else if (pregunta.Contains("activos", StringComparison.OrdinalIgnoreCase))
                {
                    var empleados = await _empleadosBL.ObtenerTodosAsync();
                    var activos = empleados?.Count(e => e.Estado) ?? 0;
                    return activos == 1
                        ? $"Actualmente hay 1 empleado activo."
                        : $"Actualmente hay {activos} empleados activos.";
                }
                else if (pregunta.Contains("asistieron hoy", StringComparison.OrdinalIgnoreCase))
                {
                    var hoy = DateTime.Today;
                    var asistencias = await _asistenciasBL.ObtenerTodosAsync();
                    var asistenciasHoy = asistencias?.Count(a => a.Fecha.Date == hoy) ?? 0;
                    return asistenciasHoy == 1
                        ? $"Hoy asistió 1 empleado."
                        : $"Hoy asistieron {asistenciasHoy} empleados.";
                }
                else
                {
                    return "Lo siento, no entiendo tu pregunta. Intenta algo como '¿Cuántos empleados hay?' o '¿Cuántos están activos?'.";
                }
            }
            catch (Exception ex)
            {
                return $"Error procesando tu pregunta: {ex.Message}";
            }
        }
    }
}
