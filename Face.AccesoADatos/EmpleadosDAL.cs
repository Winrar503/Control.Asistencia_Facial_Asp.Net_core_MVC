using Face.EntidadesDeNegocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.AccesoADatos
{
    public class EmpleadosDAL
    {
        public static async Task<int> CrearAsync(Empleados pEmpleados)
        {
            try
            {
                using (var bdContexto = new BDContexto())
                {
                    bdContexto.Add(pEmpleados);
                    return await bdContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el empleado", ex);
            }
        }

        public static async Task<int> ModificarAsync(Empleados pEmpleados)
        {
            using (var bdContexto = new BDContexto())
            {
                var empleadosBD = await bdContexto.Empleados.FirstOrDefaultAsync(l => l.Id == pEmpleados.Id);
                if (empleadosBD != null)
                {
                    empleadosBD.Nombre = pEmpleados.Nombre;
                    empleadosBD.Edad = pEmpleados.Edad;
                    empleadosBD.Email = pEmpleados.Email;
                    empleadosBD.Cargo = pEmpleados.Cargo;
                    empleadosBD.Telefono = pEmpleados.Telefono;
                    empleadosBD.Estado = pEmpleados.Estado;
                    empleadosBD.FechaRegistro = pEmpleados.FechaRegistro;

                    bdContexto.Entry(empleadosBD).State = EntityState.Modified;

                    return await bdContexto.SaveChangesAsync();
                }
                return 0;
            }
        }
        public static async Task<int> EliminarAsync(Empleados pEmpleados)
        {
            using (var bdContexto = new BDContexto())
            {
                var fotosEmpleado = await bdContexto.Fotos.Where(f => f.EmpleadosId == pEmpleados.Id).ToListAsync();
                bdContexto.Fotos.RemoveRange(fotosEmpleado);

                var empleados = await bdContexto.Empleados.FirstOrDefaultAsync(s => s.Id == pEmpleados.Id);
                if (empleados == null) return 0;

                bdContexto.Empleados.Remove(empleados);

                return await bdContexto.SaveChangesAsync();
            }
        }

        //public static async Task<int> EliminarAsync(Empleados pEmpleados)
        //{
        //    using (var bdContexto = new BDContexto())
        //    {
        //        var empleados = await bdContexto.Empleados.FirstOrDefaultAsync(s => s.Id == pEmpleados.Id);
        //        if (empleados == null) return 0;

        //        bdContexto.Empleados.Remove(empleados);
        //        return await bdContexto.SaveChangesAsync();
        //    }
        //}

        public static async Task<Empleados> ObtenerPorIdAsync(Empleados pEmpleados)
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Empleados.FirstOrDefaultAsync(s => s.Id == pEmpleados.Id);
            }
            
        }

        public static async Task<List<Empleados>> ObtenerTodosAsync()
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Empleados.ToListAsync();
            }
        }

        internal static IQueryable<Empleados> QuerySelect(IQueryable<Empleados> pQuery, Empleados pEmpleados)
        {
            if (pEmpleados.Id > 0)
                pQuery = pQuery.Where(s => s.Id == pEmpleados.Id);

            if (!string.IsNullOrWhiteSpace(pEmpleados.Nombre))
                pQuery = pQuery.Where(s => s.Nombre.Contains(pEmpleados.Nombre));


            pQuery = pQuery.OrderBy(s => s.Id);


            if (pEmpleados.Top_Aux > 0)
                pQuery = pQuery.Take(pEmpleados.Top_Aux).AsQueryable();

            return pQuery;
        }
        public static async Task<Empleados> ObtenerPorIdConRelacionesAsync(int empleadoId)
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Empleados
                    .Include(e => e.Asistencias)  // Incluye las Asistencias
                    .Include(e => e.Horarios)     // Incluye los Horarios
                    .Include(e => e.Reportes)     // Incluye los Reportes
                    .Include(e => e.Fotos)
                    .FirstOrDefaultAsync(e => e.Id == empleadoId);
            }
        }
        public static async Task<List<Empleados>> BuscarAsync(Empleados pEmpleados)
        {
          using (var bdContexto = new BDContexto())
            {
                var select = bdContexto.Empleados.AsQueryable();
                select = QuerySelect(select, pEmpleados);
                return await select.ToListAsync();
            }
        }
    }
}
