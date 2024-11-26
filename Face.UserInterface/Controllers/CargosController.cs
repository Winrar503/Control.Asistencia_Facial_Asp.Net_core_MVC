using Face.AccesoADatos;
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
            var cargos = await _cargosBL.ObtenerTodosConRelacionesAsync();
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

        public async Task<IActionResult> Edit(int id)
        {
            var cargo = await _cargosBL.ObtenerPorIdAsync(id);
            if (cargo == null)
            {
                return NotFound();
            }
            return View(cargo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Cargo cargo)
        {
            if (ModelState.IsValid)
            {
                await _cargosBL.ModificarAsync(cargo);
                return RedirectToAction(nameof(Index));
            }
            return View(cargo);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cargo = await _cargosBL.ObtenerPorIdAsync(id);
            if (cargo == null)
            {
                return NotFound();
            }
            return View(cargo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cargo = new Cargo { Id = id };
            await _cargosBL.EliminarAsync(cargo);
            return RedirectToAction(nameof(Index));
        }


    }
}

