using Face.EntidadesDeNegocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.AccesoADatos
{
    public class FotosDAL
    {
        public static async Task<int> CrearAsync(Fotos pFotos)
        {
            try
            {
                using (var bdContext = new BDContexto())
                {
                    bdContext.Add(pFotos);
                    return await bdContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al Tomar las fotos", ex);
            }
        }
        public static async Task<int> ModificarAsync(Fotos pFotos)
        {
            using (var bdContexto = new BDContexto())
            {
                var fotosDB = await bdContexto.Fotos.FirstOrDefaultAsync(l => l.Id == pFotos.Id);
                if (fotosDB != null)
                {
                    fotosDB.Foto = pFotos.Foto;

                    bdContexto.Entry(fotosDB).State = EntityState.Modified;

                    return await bdContexto.SaveChangesAsync();
                }
                return 0;
            }
        }
        public static async Task<int> EliminarAsync(Fotos pFotos)
        {
            using (var bdContexto = new BDContexto())
            {
                var fotos = await bdContexto.Fotos.FirstOrDefaultAsync(h => h.Id == pFotos.Id);
                if (fotos == null) return 0;

                bdContexto.Fotos.Remove(fotos);
                return await bdContexto.SaveChangesAsync();
            }
        }

        public static async Task<Fotos> ObtenerPorIdAsync(Fotos pFotos)
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Fotos.FirstOrDefaultAsync(h => h.Id == pFotos.Id);
            }
        }

        public static async Task<List<Fotos>> ObtenerTodosAsync()
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Fotos.ToListAsync();
            }
        }
        public static async Task<Fotos> ObtenerPorIdConRelacionesAsync(int fotosId)
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Fotos
                    .Include(e => e.Empleados)
                    .FirstOrDefaultAsync(h => h.Id == fotosId);
            }
        }

        internal static IQueryable<Fotos> QuerySelect(IQueryable<Fotos> pQuery, Fotos pFotos)
        {
            if (pFotos.Id > 0)
                pQuery = pQuery.Where(s => s.Id == pFotos.Id);

            if (pFotos.EmpleadosId > 0)
                pQuery = pQuery.Where(s => s.EmpleadosId == pFotos.EmpleadosId);

            if (pFotos.Top_Aux > 0)
                pQuery = pQuery.Take(pFotos.Top_Aux).AsQueryable();

            return pQuery;
        }

        public static async Task<List<Fotos>> BuscarAsync(Fotos pFotos)
        {
            using (var bdContexto = new BDContexto())
            {
                var select = bdContexto.Fotos.AsQueryable();

                if (pFotos.Id > 0)
                    select = select.Where(s => s.Id == pFotos.Id);

                select = select.OrderBy(s => s.Id);

                if (pFotos.Top_Aux > 0)
                    select = select.Take(pFotos.Top_Aux);

                return await select.ToListAsync();
            }
        }
    }
}
