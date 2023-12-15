using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class TramiteSeccionRepository : ITramiteSeccionRepository
    {
        private readonly GeoSITMContext _context;
        public TramiteSeccionRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<TramiteSeccion> GetTramitesSecciones(long pIdTramiteSeccion)
        {
            return _context.Tramite_Seccion.Where(n => n.Id_Usu_Baja == null && n.Id_Tramite_Seccion == pIdTramiteSeccion).ToList();
        }

        public TramiteSeccion GetTramiteSeccionById(long pIdTramiteSeccion)
        {
            return _context.Tramite_Seccion.FirstOrDefault(n => n.Id_Usu_Baja == null && n.Id_Tramite_Seccion == pIdTramiteSeccion);
        }

        public IEnumerable<TramiteSeccion> GetTramiteSeccionByTramite(long pIdTramite)
        {
            return _context.Tramite_Seccion
                           .Include(l => l.TipoSeccion)
                           .Where(n => n.Id_Usu_Baja == null && n.Id_Tramite == pIdTramite)
                           .ToList();
        }

        public void InsertTramiteSeccion(TramiteSeccion mTramiteSeccion)
        {
            mTramiteSeccion.Id_Usu_Alta = mTramiteSeccion.Id_Usu_Modif;
            mTramiteSeccion.Fecha_Alta = DateTime.Now;
            mTramiteSeccion.Fecha_Modif = mTramiteSeccion.Fecha_Alta;
            _context.Tramite_Seccion.Add(mTramiteSeccion);
        }

        public void UpdateTramiteSeccion(TramiteSeccion mTramiteSeccion)
        {
            mTramiteSeccion.Fecha_Modif = DateTime.Now;
            _context.Entry(mTramiteSeccion).State = EntityState.Modified;
        }
    }
}
