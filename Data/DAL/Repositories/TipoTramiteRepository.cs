using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class TipoTramiteRepository : ITipoTramiteRepository
    {
        private readonly GeoSITMContext _context;

        public TipoTramiteRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<TipoTramite> GetTiposTramites()
        {
            return _context.Tipo_Tramite.ToList();
        }

        public TipoTramite GetTipoTramiteById(long idTipoTramite)
        {
            return _context.Tipo_Tramite.FirstOrDefault(n => n.Id_Usu_Baja == null && n.Id_Tipo_Tramite == idTipoTramite);
        }

        public void InsertTipoTramite(TipoTramite mTrtTipoTramite)
        {
            _context.Tipo_Tramite.Add(mTrtTipoTramite);
        }

        public void InsertTipoTramite(TipoTramite mTrtTipoTramite, Hoja hoja)
        {
            throw new NotImplementedException();
        }

        public void UpdateTipoTramite(TipoTramite mTrtTipoTramite)
        {
            _context.Entry(mTrtTipoTramite).State = EntityState.Modified;
        }

        public void DeleteTipoTramite(long idTipoTramite)
        {
            var objeto = _context.Tipo_Tramite.Find(idTipoTramite);
            objeto.Id_Usu_Baja = 1;
            objeto.Fecha_Baja = DateTime.Now;
            _context.Entry(objeto).State = EntityState.Modified;
        }

        public void DeleteTipoTramite(TipoTramite mTrtTipoTramite)
        {
            mTrtTipoTramite.Id_Usu_Baja = 1;
            mTrtTipoTramite.Fecha_Baja = DateTime.Now;
            _context.Entry(mTrtTipoTramite).State = EntityState.Modified;
        }
    }
}
