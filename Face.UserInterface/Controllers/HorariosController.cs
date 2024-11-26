using Face.AccesoADatos;
using Face.EntidadesDeNegocio;
using Face.LogicaDeNegocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Face.UserInterface.Controllers
{

    public class HorariosController : Controller
    {
        EmpleadosBL empleadosBL = new EmpleadosBL();
        HorariosBL horariosBL = new HorariosBL();

        public async Task<IActionResult> Index(Horarios horarios = null)
        {
            if (horarios == null)
                horarios = new Horarios();
            if (horarios.Top_Aux == 0)
                horarios.Top_Aux = 0;
            else if (horarios.Top_Aux == -1)
                horarios.Top_Aux = 0;

            var lisHorarios = await horariosBL.BuscarAsync(horarios);
            var empleados = await empleadosBL.ObtenerTodosAsync();

            ViewBag.Empleados = empleados;
            ViewBag.Top = horarios.Top_Aux;
            return View(lisHorarios);
        }
        public async Task<IActionResult> Details(int id)
        {
            var horario = await horariosBL.ObtenerPorIdConRelacionesAsync(id);
            if (horario == null)
            {
                return NotFound();
            }
            return View(horario);
        }
        public async Task<IActionResult> Create()
        {
            var empleados = await empleadosBL.ObtenerTodosAsync();
            ViewBag.Empleados = empleados;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Horarios horarios)
        {
            try
            {

                int result = await horariosBL.CrearAsync(horarios);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                var horario = await empleadosBL.ObtenerTodosAsync();
                ViewBag.Empleados = horario;
                return View(horarios);
            }
        }

       

        public async Task<IActionResult> Edit(int id)
        {
            var horario = await horariosBL.ObtenerPorIdAsync(new Horarios { Id = id });
            if (horario == null)
            {
                return NotFound();
            }

            return View(horario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Horarios pHorarios)
        {
            if (id != pHorarios.Id)
            {
                return NotFound(); 
            }
            try
            {
                await horariosBL.ModificarAsync(pHorarios);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
            }

            return View(pHorarios);
        }


        public async Task<ActionResult> Delete(int id)
        {
            var horarios = await horariosBL.ObtenerPorIdAsync(new Horarios { Id = id });
            return View(horarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Horarios horarios)
        {
            try
            {
                var horario = await horariosBL.ObtenerPorIdAsync(new Horarios { Id = id });
                if (horario == null)
                {
                    return NotFound();
                }
                int result = await horariosBL.EliminarAsync(horario);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al eliminar el empleado: " + ex.Message;
                var horariosBd = await horariosBL.ObtenerPorIdAsync(new Horarios { Id = id });
                return View(horariosBd);
            }
        }
    }
}
