using Face.EntidadesDeNegocio;
using Face.LogicaDeNegocio;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Face.UserInterface.Controllers
{
    [Route("Chat")]
    public class ChatController : Controller
    {
        private readonly ChatService _chatService;

        public ChatController()
        {
            _chatService = new ChatService();
        }

        [HttpPost("ProcesarPregunta")]
        public async Task<IActionResult> ProcesarPregunta([FromBody] PreguntaRequest request)
        {
            // Log para depuración
            Console.WriteLine("Solicitud recibida en ProcesarPregunta.");

            // Validación del modelo
            if (request == null || string.IsNullOrWhiteSpace(request.Pregunta))
            {
                Console.WriteLine("Error: Pregunta nula o vacía.");
                return BadRequest(new { Respuesta = "Por favor escribe una pregunta válida." });
            }

            try
            {
                Console.WriteLine($"Pregunta recibida: {request.Pregunta}");
                var respuesta = await _chatService.ObtenerRespuestaAsync(request.Pregunta);
                Console.WriteLine($"Respuesta generada: {respuesta}");
                return Json(new { Respuesta = respuesta });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ProcesarPregunta: {ex.Message}");
                return StatusCode(500, new { Respuesta = "Hubo un error procesando tu solicitud." });
            }
        }
    }
}
