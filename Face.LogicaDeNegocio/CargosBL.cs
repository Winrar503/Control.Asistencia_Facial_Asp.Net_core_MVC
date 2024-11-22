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
        public async Task<int> CrearAsync(Cargo pCargo)
        {
            return await CargosDAL.CrearAsync(pCargo);
        }

        public async Task<List<Cargo>> ObtenerTodosAsync()
        {
            return await CargosDAL.ObtenerTodosAsync();
        }
    }
}
