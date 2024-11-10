using Face.EntidadesDeNegocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.AccesoADatos
{
    public class RendimientoEmpleadosDAL
    {
        public static async Task<int> CrearAsync(RendimientoEmpleados rendimiento)
        {
            using (var context = new BDContexto())
            {
                context.RendimientoEmpleados.Add(rendimiento);
                return await context.SaveChangesAsync();
            }
        }

        public static async Task<List<RendimientoEmpleados>> ObtenerRendimientoPorEmpleadoAsync(int empleadoId)
        {
            using (var context = new BDContexto())
            {
                return await context.RendimientoEmpleados
                    .Where(r => r.EmpleadosId == empleadoId)
                    .ToListAsync();
            }
        }
    }
}
