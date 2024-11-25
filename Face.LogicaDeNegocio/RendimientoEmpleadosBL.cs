using Face.AccesoADatos;
using Face.EntidadesDeNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.LogicaDeNegocio
{
    public class RendimientoEmpleadosBL
    {
        public async Task<int> CrearAsync(RendimientoEmpleados rendimiento)
        {
            return await RendimientoEmpleadosDAL.CrearAsync(rendimiento);
        }

        public async Task<List<RendimientoEmpleados>> ObtenerRendimientoPorEmpleadoAsync(int empleadoId)
        {
            return await RendimientoEmpleadosDAL.ObtenerRendimientoPorEmpleadoAsync(empleadoId);
        }
        public async Task CalcularRendimientoAsync(int empleadoId, DateTime fechaInicio, DateTime fechaFin)
        {
            await RendimientoEmpleadosDAL.CalcularRendimientoAsync(empleadoId, fechaInicio, fechaFin);
        }

        public async Task<List<RendimientoEmpleados>> ObtenerPorEmpleadoYRangoAsync(int empleadoId, DateTime fechaInicio, DateTime fechaFin)
        {
            return await RendimientoEmpleadosDAL.ObtenerPorEmpleadoYRangoAsync(empleadoId, fechaInicio, fechaFin);
        }
    }
}
