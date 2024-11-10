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
    }
}
