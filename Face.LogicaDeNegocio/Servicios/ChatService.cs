using System;
using System.Linq;
using System.Threading.Tasks;

namespace Face.LogicaDeNegocio
{
    public class ChatService
    {
        private readonly EmpleadosBL _empleadosBL = new EmpleadosBL();
        private readonly AsistenciasBL _asistenciasBL = new AsistenciasBL();

        private readonly Dictionary<string, string> respuestasConversacionales = new()
        {
            { "hola", "¡Hola! ¿En qué puedo ayudarte?" },
            { "buenos días", "¡Buenos días! ¿Qué necesitas saber?" },
            { "buenas tardes", "¡Buenas tardes! ¿Cómo puedo ayudarte?" },
            { "adiós", "¡Hasta luego! Espero que tengas un buen día." },
            { "gracias", "¡De nada! Estoy aquí para ayudarte." },
            { "cómo estás", "¡Estoy bien, gracias por preguntar! ¿Y tú?" }
        };

        public async Task<string> ObtenerRespuestaAsync(string pregunta)
        {
            try
            {
                Console.WriteLine($"Procesando la pregunta: {pregunta}");

                // Respuestas conversacionales
                foreach (var entrada in respuestasConversacionales)
                {
                    if (pregunta.ToLower().Contains(entrada.Key))
                    {
                        Console.WriteLine($"Respuesta encontrada en respuestas conversacionales: {entrada.Value}");
                        return entrada.Value;
                    }
                }

                // Preguntas relacionadas al sistema
                if (ContienePalabraClave(pregunta, new[] { "empleados", "trabajadores" }))
                {
                    var empleados = await _empleadosBL.ObtenerTodosAsync();
                    var total = empleados?.Count ?? 0;
                    return total == 1
                        ? "Actualmente hay 1 empleado registrado."
                        : $"Actualmente hay {total} empleados registrados.";
                }

                if (ContienePalabraClave(pregunta, new[] { "activos", "disponibles" }))
                {
                    var empleados = await _empleadosBL.ObtenerTodosAsync();
                    var activos = empleados?.Count(e => e.Estado) ?? 0;
                    return activos == 1
                        ? "Actualmente hay 1 empleado activo."
                        : $"Actualmente hay {activos} empleados activos.";
                }

                if (ContienePalabraClave(pregunta, new[] { "asistencias", "asistieron", "hoy" }))
                {
                    var hoy = DateTime.Today;
                    var asistencias = await _asistenciasBL.ObtenerTodosAsync();
                    var asistenciasHoy = asistencias?.Count(a => a.Fecha.Date == hoy) ?? 0;
                    return asistenciasHoy == 1
                        ? "Hoy asistió 1 empleado."
                        : $"Hoy asistieron {asistenciasHoy} empleados.";
                }

                // Respuesta por defecto
                return "Lo siento, no entiendo tu pregunta. Intenta algo como '¿Cuántos empleados hay?' o '¿Cuántos asistieron hoy?'.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ChatService: {ex.Message}");
                return $"Error procesando tu pregunta: {ex.Message}";
            }
        }

        private bool ContienePalabraClave(string pregunta, string[] palabrasClave)
        {
            foreach (var palabra in palabrasClave)
            {
                if (pregunta.Contains(palabra, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
