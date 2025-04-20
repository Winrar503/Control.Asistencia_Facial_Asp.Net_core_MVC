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
        public static async Task<int> CrearAsync(Cargo cargo)
        {
            try
            {
                using (var dbContext = new BDContexto())
                {
                    dbContext.Cargos.Add(cargo);
                    return await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el cargo", ex);
            }
        }

        public static async Task<int> ModificarAsync(Cargo cargo)
        {
            using (var dbContext = new BDContexto())
            {
                var cargoDB = await dbContext.Cargos.FindAsync(cargo.Id);
                if (cargoDB != null)
                {
                    cargoDB.Nombre = cargo.Nombre;
                    return await dbContext.SaveChangesAsync();
                }
                return 0;
            }
        }
        public static async Task<int> EliminarAsync(Cargo cargo)
        {
            using (var dbContext = new BDContexto())
            {
                var cargoDB = await dbContext.Cargos.FindAsync(cargo.Id);
                if (cargoDB != null)
                {
                    dbContext.Cargos.Remove(cargoDB);
                    return await dbContext.SaveChangesAsync();
                }
                return 0;
            }
        }

        public static async Task<Cargo> ObtenerPorIdAsync(int id)
        {
            using (var dbContext = new BDContexto())
            {
                return await dbContext.Cargos.FindAsync(id);
            }
        }

        public static async Task<List<Cargo>> ObtenerTodosAsync()
        {
            using (var dbContext = new BDContexto())
            {
                return await dbContext.Cargos.ToListAsync();
            }
        }

        public static async Task<int> ObtenerCantidadAsync()
        {
            using (var dbContext = new BDContexto())
            {
                return await dbContext.Cargos.CountAsync();
            }
        }


        public static async Task<List<Cargo>> ObtenerTodosConRelacionesAsync()
        {
            using (var dbContext = new BDContexto())
            {
                return await dbContext.Cargos
                    .Include(c => c.Empleados)
                    .ToListAsync();
            }
        }
        

    }
}
