using Face.AccesoADatos;
using Face.EntidadesDeNegocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.LogicaDeNegocio
{
    public class AsistenciasBL
    {
        public async Task<int> CrearAsync(Asistencias pAsistencias)
        {
            return await AsistenciasDAL.CrearAsync(pAsistencias); 
        }
        public async Task<int> ModificarAsync(Asistencias pAsistencias)
        {
            return await AsistenciasDAL.ModificarAsync(pAsistencias);
        }
        public async Task<int> EliminarAsync(Asistencias pAsistencias)
        {
            return await  AsistenciasDAL.EliminarAsync(pAsistencias);
        }
        public async Task<Asistencias> ObtenerPorIdAsync(Asistencias pAsistencias)
        {
            return await AsistenciasDAL.ObtenerPorIdAsync(pAsistencias);
        }
        public async Task<Asistencias> ObtenerPorIdConRelacionesAsync(int asistenciaId)
        {
            return await AsistenciasDAL.ObtenerPorIdConRelacionesAsync(asistenciaId);
        }

        public async Task<List<Asistencias>> ObtenerTodosAsync()
        {
            return await AsistenciasDAL.ObtenerTodosAsync();
        }

        public async Task<List<Asistencias>> BuscarAsync(Asistencias pAsistencias)
        {
            return await AsistenciasDAL.BuscarAsync(pAsistencias);
        }
    }
}
