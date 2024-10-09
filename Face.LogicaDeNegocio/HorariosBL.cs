using Face.AccesoADatos;
using Face.EntidadesDeNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.LogicaDeNegocio
{
    public class HorariosBL
    {
        public async Task<int> CrearAsync(Horarios pHorarios)
        {
            return await HorariosDAL.CrearAsync(pHorarios);
        }
        public async Task<int> ModificarAsync(Horarios pHorarios)
        {
            return await HorariosDAL.ModificarAsync(pHorarios);
        }

        public async Task<int> EliminarAsync(Horarios pHorarios)
        {
            return await HorariosDAL.EliminarAsync(pHorarios);
        }

        public async Task<Horarios> ObtenerPorIdAsync(Horarios pHorarios)
        {
            return await HorariosDAL.ObtenerPorIdAsync(pHorarios);
        }

        public async Task<List<Horarios>> ObtenerTodosAsync()
        {
            return await HorariosDAL.ObtenerTodosAsync();
        }

        public async Task<List<Horarios>> BuscarAsync(Horarios pHorarios)
        {
            return await HorariosDAL.BuscarAsync(pHorarios);
        }
    }
}
