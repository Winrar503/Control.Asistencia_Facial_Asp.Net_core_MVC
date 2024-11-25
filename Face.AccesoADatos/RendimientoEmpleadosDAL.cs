using Face.EntidadesDeNegocio;
using Microsoft.Data.SqlClient;
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
        public static async Task CalcularRendimientoAsync(int empleadoId, DateTime fechaInicio, DateTime fechaFin)
        {
            using (var bdContext = new BDContexto())
            {
                var sql = "EXEC SP_CalcularRendimientoEmpleados @EmpleadoId, @FechaInicio, @FechaFin";
                var parameters = new[]
                {
                    new SqlParameter("@EmpleadoId", empleadoId),
                    new SqlParameter("@FechaInicio", fechaInicio),
                    new SqlParameter("@FechaFin", fechaFin)
                };

                await bdContext.Database.ExecuteSqlRawAsync(sql, parameters);
            }
        }

        // Obtener registros de rendimiento para un empleado y rango de fechas
        public static async Task<List<RendimientoEmpleados>> ObtenerPorEmpleadoYRangoAsync(int empleadoId, DateTime fechaInicio, DateTime fechaFin)
        {
            using (var bdContext = new BDContexto())
            {
                return await bdContext.RendimientoEmpleados
                    .Where(r => r.EmpleadosId == empleadoId &&
                                r.FechaInicio >= fechaInicio &&
                                r.FechaFin <= fechaFin)
                    .ToListAsync();
            }
        }
    }
}
