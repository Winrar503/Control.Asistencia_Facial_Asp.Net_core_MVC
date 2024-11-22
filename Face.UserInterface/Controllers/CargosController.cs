using Face.EntidadesDeNegocio;
using Face.LogicaDeNegocio;
using Microsoft.AspNetCore.Mvc;

namespace Face.UserInterface.Controllers
{
    public class CargosController : Controller
    {
        private readonly CargosBL _cargosBL = new CargosBL();

        public async Task<IActionResult> Index()
        {
            var cargos = await _cargosBL.ObtenerTodosAsync();
            return View(cargos);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Cargo cargo)
        {
            if (ModelState.IsValid)
            {
                await _cargosBL.CrearAsync(cargo);
                return RedirectToAction(nameof(Index));
            }
            return View(cargo);
        }
    }
}

