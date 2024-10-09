using Face.AccesoADatos;
using Face.EntidadesDeNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.LogicaDeNegocio
{
    public class ReportesBL
    {
        public async Task<int> CrearAsync(Reportes pReporte)
        {
            return await ReporteDAL.CrearAsync(pReporte);
        }
        public async Task<int> ModificarAsync(Reportes pReporte)
        {
            return await ReporteDAL.ModificarAsync(pReporte);
        }

        public async Task<int> EliminarAsync(Reportes pReporte)
        {
            return await ReporteDAL.EliminarAsync(pReporte);
        }

        public async Task<Reportes> ObtenerPorIdAsync(Reportes pReporte)
        {
            return await ReporteDAL.ObtenerPorIdAsync(pReporte);
        }

        public async Task<List<Reportes>> ObtenerTodosAsync()
        {
            return await ReporteDAL.ObtenerTodosAsync();
        }

        public async Task<List<Reportes>> BuscarAsync(Reportes pReporte)
        {
            return await ReporteDAL.BuscarAsync(pReporte);
        }
    }
}
