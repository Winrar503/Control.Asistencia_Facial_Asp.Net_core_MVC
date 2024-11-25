using Face.EntidadesDeNegocio;
using Face.AccesoADatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Face.LogicaDeNegocio
{
    public class EmpleadosBL
    {
        public async Task<int> CrearAsync(Empleados pEmpleados)
        {
            return await EmpleadosDAL.CrearAsync(pEmpleados);
        }
        public async Task<int> ModificarAsync(Empleados pEmpleados)
        {
            return await EmpleadosDAL.ModificarAsync(pEmpleados);
        }

        public async Task<int> EliminarAsync(Empleados pEmpleados)
        {
            return await EmpleadosDAL.EliminarAsync(pEmpleados);
        }

        public async Task<Empleados> ObtenerPorIdAsync(Empleados pEmpleados)
        {
            return await EmpleadosDAL.ObtenerPorIdAsync(pEmpleados);
        }

        //para detalles
        public async Task<List<Empleados>> ObtenerTodosAsync()
        {
            return await EmpleadosDAL.ObtenerTodosAsync();
        }
        public async Task<Empleados> ObtenerPorIdConRelacionesAsync(int empleadoId)
        {
            return await EmpleadosDAL.ObtenerPorIdConRelacionesAsync(empleadoId);
        }


        public async Task<List<Empleados>> BuscarAsync(Empleados pEmpleados)
        {
            return await EmpleadosDAL.BuscarAsync(pEmpleados);
        }

        public async Task<Empleados> ObtenerPorNombreAsync(string nombre)
        {
            return await EmpleadosDAL.ObtenerPorNombreAsync(nombre);
        }

        public async Task<List<Empleados>> ObtenerTodosConRelacionesAsync()
        {
            return await EmpleadosDAL.ObtenerTodosConRelacionesAsync(); // Llamar directamente al método estático
        }


    }
}
