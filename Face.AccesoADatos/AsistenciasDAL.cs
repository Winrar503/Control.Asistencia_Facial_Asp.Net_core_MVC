using Face.EntidadesDeNegocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.AccesoADatos
{
    public class AsistenciasDAL
    {
        public static async Task<int> CrearAsync(Asistencias pAsistencias)
        {
            try
            {
                using (var bdContext = new BDContexto())
                {
                    bdContext.Add(pAsistencias);
                    return await bdContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el Asistencia", ex);
            }
        }
        public static async Task<int> ModificarAsync(Asistencias pAsistencia)
        {
            int result = 0;
            using (var bdContexto = new BDContexto())
            {
                var asistencias = await bdContexto.Asistencias.FirstOrDefaultAsync(a => a.Id == pAsistencia.Id);
                if (asistencias != null) return 0;

                asistencias.Comentarios = pAsistencia.Comentarios;
                asistencias.Fecha = pAsistencia.Fecha;
                asistencias.Tipo = pAsistencia.Tipo;
                asistencias.EstadoReconocimiento = pAsistencia.EstadoReconocimiento;

                result = await bdContexto.SaveChangesAsync();

            }
            return result;
        }
        public static async Task<int> EliminarAsync(Asistencias pAsistencia)
        {
            using (var bdContexto = new BDContexto())
            {
                var asistencia = await bdContexto.Asistencias.FirstOrDefaultAsync(s => s.Id == pAsistencia.Id);
                if (asistencia == null) return 0;

                bdContexto.Asistencias.Remove(asistencia);
                return await bdContexto.SaveChangesAsync();
            }
        }
        public static async Task<Asistencias> ObtenerPorIdAsync(Asistencias pAsistencia)
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Asistencias.FirstOrDefaultAsync(s => s.Id == pAsistencia.Id);
            }

        }
        public static async Task<List<Asistencias>> ObtenerTodosAsync()
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Asistencias
                     .Include(a => a.Empleados)
                     .ToListAsync();
            }
        }
        internal static IQueryable<Asistencias> QuerySelect(IQueryable<Asistencias> pQuery, Asistencias pAsistencias)
        {
            if (pAsistencias.Id > 0)
                pQuery = pQuery.Where(s => s.Id == pAsistencias.Id);

            if (!string.IsNullOrWhiteSpace(pAsistencias.Tipo))
                pQuery = pQuery.Where(s => s.Tipo.Contains(pAsistencias.Tipo));


            pQuery = pQuery.OrderBy(s => s.Id);

            if (pAsistencias.Top_Aux > 0)
                pQuery = pQuery.Take(pAsistencias.Top_Aux).AsQueryable();

            return pQuery;
        }
        public static async Task<List<Asistencias>> BuscarAsync(Asistencias pAsistencias)
        {
            using (var bdContexto = new BDContexto())
            {
                var query = bdContexto.Asistencias
           .Include(a => a.Empleados) // Incluimos la relación
           .AsQueryable();

                query = QuerySelect(query, pAsistencias);
                return await query.ToListAsync();
            }
        }
        public static async Task<Asistencias> ObtenerPorIdConRelacionesAsync(int asistenciaId)
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Asistencias
                    .Include(e => e.Empleados)  // Incluye las Asistencias
                    .FirstOrDefaultAsync(e => e.Id == asistenciaId);
            }
        }
        public static async Task<List<Asistencias>> ObtenerTodosConRelacionesAsync()
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Asistencias
                    .Include(a => a.Empleados) // Asegúrate de cargar la relación
                    .ToListAsync();
            }
        }



    }
}
