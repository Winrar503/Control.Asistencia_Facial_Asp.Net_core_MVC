using Face.EntidadesDeNegocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.AccesoADatos
{
    public class HorariosDAL
    {
        public static async Task<int> CrearAsync(Horarios pHorario)
        {
            try
            {
                using (var bdContext = new BDContexto())
                {
                    bdContext.Add(pHorario);
                    return await bdContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            { 
                throw new Exception("Error al crear el Horario", ex); 
            }
        }
        public static async Task<int> ModificarAsync(Horarios pHorario)
        {
            using (var bdContexto = new BDContexto())
            {
                var horarios = await bdContexto.Horarios.FirstOrDefaultAsync(h => h.Id == pHorario.Id);
                if (horarios == null) return 0; // Si no se encuentra, retorna 0

                // Modifica los campos necesarios
                horarios.HoraEntrada = pHorario.HoraEntrada;
                horarios.HoraSalida = pHorario.HoraSalida;
                horarios.EmpleadosId = pHorario.EmpleadosId; // Asegúrate de actualizar el empleado si es necesario

                // Guarda los cambios en la base de datos
                return await bdContexto.SaveChangesAsync();
            }
        }
        public static async Task<int> EliminarAsync(Horarios pHorario)
        {
            using (var bdContexto = new BDContexto())
            {
                var horarios = await bdContexto.Horarios.FirstOrDefaultAsync(h => h.Id == pHorario.Id);
                if (horarios == null) return 0;

                bdContexto.Horarios.Remove(horarios);
                return await bdContexto.SaveChangesAsync();
            }
        }

        public static async Task<Horarios> ObtenerPorIdAsync(Horarios pHorario)
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Horarios.FirstOrDefaultAsync(h => h.Id == pHorario.Id);
            }
        }

        public static async Task<List<Horarios>> ObtenerTodosAsync()
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Horarios.ToListAsync();
            }
        }
        public static async Task<Horarios> ObtenerPorIdConRelacionesAsync(int horariosId)
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Horarios
                    .Include(e => e.Empleados) // Incluye las relaciones con empleados
                    .FirstOrDefaultAsync(h => h.Id == horariosId); // Filtra por el Id de horarios
            }
        }

        internal static IQueryable<Horarios> QuerySelect(IQueryable<Horarios> pQuery, Horarios pHorario)
        {
            if (pHorario.Id > 0)
                pQuery = pQuery.Where(s => s.Id == pHorario.Id);

            if (pHorario.EmpleadosId > 0)
                pQuery = pQuery.Where(s => s.EmpleadosId == pHorario.EmpleadosId);  // Filtra por empleado

            if (pHorario.HoraEntrada != TimeSpan.MinValue) // Si hay un filtro de fecha de entrada
                pQuery = pQuery.Where(s => s.HoraEntrada >= pHorario.HoraEntrada);

            if (pHorario.HoraSalida != TimeSpan.MinValue) // Si hay un filtro de fecha de salida
                pQuery = pQuery.Where(s => s.HoraSalida <= pHorario.HoraSalida);

            // Ordena por Id de horario
            pQuery = pQuery.OrderBy(s => s.Id);

            // Limita los resultados si Top_Aux está definido
            if (pHorario.Top_Aux > 0)
                pQuery = pQuery.Take(pHorario.Top_Aux).AsQueryable();

            return pQuery;
        }

        public static async Task<List<Horarios>> BuscarAsync(Horarios pHorario)
        {
            using (var bdContexto = new BDContexto())
            {
                var select = bdContexto.Horarios.AsQueryable();

                // Filtra según sea necesario
                if (pHorario.Id > 0)
                    select = select.Where(s => s.Id == pHorario.Id);

                select = select.OrderBy(s => s.Id);

                if (pHorario.Top_Aux > 0)
                    select = select.Take(pHorario.Top_Aux);

                return await select.ToListAsync();
            }
        }

    }
}
