using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class TipoSeccionRepository : ITipoSeccionRepository
    {
        private readonly GeoSITMContext _context;
        public TipoSeccionRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<TramiteTipoSeccion> GetTipoSeccions()
        {
            return _context.Tipo_Seccion.Where(n => n.Id_Usu_Baja == null).ToList();
        }

        public TramiteTipoSeccion GetTipoSeccionById(long pIdTipoSeccion)
        {
            return _context.Tipo_Seccion.FirstOrDefault(n => n.Id_Usu_Baja == null && n.Id_Tipo_Seccion == pIdTipoSeccion);
        }

        public void InsertTipoSeccion(TramiteTipoSeccion mTipoSeccion)
        {
            _context.Tipo_Seccion.Add(mTipoSeccion);
        }

        public void UpdateTipoSeccion(TramiteTipoSeccion mTipoSeccion)
        {
            _context.Entry(mTipoSeccion).State = EntityState.Modified;
        }

        public void DeleteTipoSeccion(long pIdTipoSeccion)
        {
            var mTipoSeccion = _context.Tipo_Seccion.Find(pIdTipoSeccion);
            mTipoSeccion.Id_Usu_Baja = 1;
            mTipoSeccion.Fecha_Baja = DateTime.Now;
            _context.Entry(mTipoSeccion).State = EntityState.Modified;
        }

        public void DeleteTipoSeccion(TramiteTipoSeccion mTipoSeccion)
        {
            mTipoSeccion.Id_Usu_Baja = 1;
            mTipoSeccion.Fecha_Baja = DateTime.Now;
            _context.Entry(mTipoSeccion).State = EntityState.Modified;
        }
    }
}
