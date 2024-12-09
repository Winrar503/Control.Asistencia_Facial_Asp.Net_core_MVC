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
                    var total = (await _empleadosBL.ObtenerTodosAsync()).Count;
                    return $"Actualmente hay {total} empleados registrados.";
                }
                else if (pregunta.Contains("activos", StringComparison.OrdinalIgnoreCase))
                {
                    var activos = (await _empleadosBL.ObtenerTodosAsync()).Count(e => e.Estado);
                    return $"Actualmente hay {activos} empleados activos.";
                }
                else if (pregunta.Contains("asistieron hoy", StringComparison.OrdinalIgnoreCase))
                {
                    var hoy = DateTime.Today;
                    var asistenciasHoy = (await _asistenciasBL.ObtenerTodosAsync())
                        .Count(a => a.Fecha.Date == hoy);
                    return $"Hoy asistieron {asistenciasHoy} empleados.";
                }
                else
                {
                    return "Lo siento, no entiendo tu pregunta. Intenta algo como '¿Cuántos empleados hay?' o '¿Cuántos están activos?'.";
                }
            }
            catch (Exception ex)
            {
                return $"Ocurrió un error al procesar tu pregunta: {ex.Message}";
            }
        }
    }
}

