using Face.EntidadesDeNegocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.AccesoADatos
{
    public class ReporteDAL
    {
        public static async Task<int> CrearAsync(Reportes pReporte)
        {
            try
            {
                using (var bdContext = new BDContexto())
                {
                    bdContext.Add(pReporte);
                    return await bdContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el reporte", ex);
            }

        }

        public static async Task<int> ModificarAsync(Reportes pReporte)
        {
            int result = 0;
            using (var bdContexto = new BDContexto())
            {
                var reportes = await bdContexto.Reportes.FirstOrDefaultAsync(r => r.Id == pReporte.Id);
                if (reportes != null) return 0;
                reportes.FechaInicio = pReporte.FechaInicio;
                reportes.FechaFin = pReporte.FechaFin;
                result = await bdContexto.SaveChangesAsync();

            }
            return result;
        }

        public static async Task<int> EliminarAsync(Reportes pReporte)
        {
            using (var bdContexto = new BDContexto())
            {
                var reportes = await bdContexto.Horarios.FirstOrDefaultAsync(r => r.Id == pReporte.Id);
                if (reportes == null) return 0;
                bdContexto.Horarios.Remove(reportes);
                return await bdContexto.SaveChangesAsync();
            }
        }

        public static async Task<Reportes> ObtenerPorIdAsync(Reportes pReporte)
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Reportes.FirstOrDefaultAsync(r => r.Id == pReporte.Id);
            }
        }

        public static async Task<List<Reportes>> ObtenerTodosAsync()
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Reportes.ToListAsync();
            }
        }
        internal static IQueryable<Reportes> QuerySelect(IQueryable<Reportes> pQuery, Reportes pReporte)
        {
            if (pReporte.Id > 0)
                pQuery = pQuery.Where(s => s.Id == pReporte.Id);
            if (!string.IsNullOrWhiteSpace(pReporte.Resumen))
                pQuery = pQuery.Where(s => s.Resumen.Contains(pReporte.Resumen));
            pQuery = pQuery.OrderBy(s => s.Id);
            if (pReporte.Top_Aux > 0)
                pQuery = pQuery.Take(pReporte.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<Reportes>> BuscarAsync(Reportes pHorario)
        {
            using (var bdContexto = new BDContexto())
            {
                var select = bdContexto.Reportes.AsQueryable();
                select = QuerySelect(select, pHorario);
                return await select.ToListAsync();
            }
        }
    }
}
