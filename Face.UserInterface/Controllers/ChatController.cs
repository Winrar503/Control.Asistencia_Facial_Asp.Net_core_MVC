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
        public async Task<IActionResult> ProcesarPregunta([FromBody] string pregunta)
        {
            if (string.IsNullOrEmpty(pregunta))
            {
                return BadRequest(new { Respuesta = "La pregunta no puede estar vacía." });
            }

            var respuesta = await _chatService.ObtenerRespuestaAsync(pregunta);
            return Json(new { Respuesta = respuesta });
        }
    }
}
