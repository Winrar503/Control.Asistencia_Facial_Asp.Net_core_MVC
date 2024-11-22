using Face.EntidadesDeNegocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.AccesoADatos
{
    public class CargosDAL
    {
        public static async Task<int> CrearAsync(Cargo pCargo)
        {
            using (var bdContexto = new BDContexto())
            {
                bdContexto.Add(pCargo);
                return await bdContexto.SaveChangesAsync();
            }
        }

        public static async Task<List<Cargo>> ObtenerTodosAsync()
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Cargos.ToListAsync();
            }
        }
    }
}
