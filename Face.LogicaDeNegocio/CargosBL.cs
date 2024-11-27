using Face.AccesoADatos;
using Face.EntidadesDeNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.LogicaDeNegocio
{
    public class CargosBL
    {
        public async Task<int> CrearAsync(Cargo cargo)
        {
            return await CargosDAL.CrearAsync(cargo);
        }

        public async Task<int> ModificarAsync(Cargo cargo)
        {
            return await CargosDAL.ModificarAsync(cargo);
        }

        public async Task<int> EliminarAsync(Cargo cargo)
        {
            return await CargosDAL.EliminarAsync(cargo);
        }

        public async Task<Cargo> ObtenerPorIdAsync(int id)
        {
            return await CargosDAL.ObtenerPorIdAsync(id);
        }

        public async Task<List<Cargo>> ObtenerTodosAsync()
        {
            return await CargosDAL.ObtenerTodosAsync();
        }

        public async Task<int> ObtenerCantidadAsync()
        {
            return await CargosDAL.ObtenerCantidadAsync();
        }


        public async Task<List<Cargo>> ObtenerTodosConRelacionesAsync()
        {
            return await CargosDAL.ObtenerTodosConRelacionesAsync();
        }
    }
}
