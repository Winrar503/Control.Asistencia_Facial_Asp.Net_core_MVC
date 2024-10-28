using Face.AccesoADatos;
using Face.EntidadesDeNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.LogicaDeNegocio
{
    public class FotosBL
    {
        public async Task<int> CrearAsync(Fotos pFotos)
        {
            return await FotosDAL.CrearAsync(pFotos);
        }
        public async Task<int> ModificarAsyn(Fotos pFotos)
        {
            return await FotosDAL.ModificarAsync(pFotos);
        }
        public async Task<int> EliminarAsync(Fotos pFotos)
        {
            return await FotosDAL.EliminarAsync(pFotos);
        }
        public async Task<Fotos> ObtenerPorIdAsync(Fotos pFotos)
        {
            return await FotosDAL.ObtenerPorIdAsync(pFotos);
        }

        public async Task<List<Fotos>> ObtenerTodosAsync()
        {
            return await FotosDAL.ObtenerTodosAsync();
        }

        public async Task<List<Fotos>> BuscarAsync(Fotos pFotos)
        {
            return await FotosDAL.BuscarAsync(pFotos);
        }
        public async Task<List<Fotos>> ObtenerPorEmpleadoIdAsync(int empleadoId)
        {
            return await FotosDAL.ObtenerPorEmpleadoIdAsync(empleadoId);
        }
    }
}
