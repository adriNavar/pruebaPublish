using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Interfaces;
using System;
using System.Linq;
using System.Data.Entity;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    class UnidadTributariaDomicilioRepository : IUnidadTributariaDomicilioRepository
    {
        private readonly GeoSITMContext _context;
        public UnidadTributariaDomicilioRepository(GeoSITMContext context)
        {
            this._context = context;
        }

        public UnidadTributariaDomicilio GetUnidadTributariaDomicilioRepositoryById(long idUnidadTributaria, long idDomicilio)
        {
            return _context.UnidadesTributariasDomicilio.FirstOrDefault(utd => utd.UnidadTributariaID == idUnidadTributaria && utd.DomicilioID == idDomicilio);
        }
        public void InsertUnidadTributariaDomicilioRepository(UnidadTributariaDomicilio unidadTributariaDomicilio)
        {
            unidadTributariaDomicilio.Domicilio.TipoDomicilio = null;
            unidadTributariaDomicilio.UsuarioAltaID = unidadTributariaDomicilio.UsuarioModificacionID;
            unidadTributariaDomicilio.FechaModificacion = DateTime.Now;
            unidadTributariaDomicilio.FechaAlta = unidadTributariaDomicilio.FechaModificacion;
            _context.UnidadesTributariasDomicilio.Add(unidadTributariaDomicilio);
        }

        public void DeleteUnidadTributariaDomicilio(UnidadTributariaDomicilio unidadTributariaDomicilio)
        {
            unidadTributariaDomicilio.Domicilio.TipoDomicilio = null;
            unidadTributariaDomicilio.FechaModificacion = DateTime.Now;
            unidadTributariaDomicilio.FechaBaja = unidadTributariaDomicilio.FechaModificacion;
            unidadTributariaDomicilio.UsuarioBajaID = unidadTributariaDomicilio.UsuarioModificacionID;
            _context.Entry(unidadTributariaDomicilio).State = EntityState.Modified;
            _context.Entry(unidadTributariaDomicilio).Property(p => p.UsuarioAltaID).IsModified = false;
            _context.Entry(unidadTributariaDomicilio).Property(p => p.FechaAlta).IsModified = false;
        }
        public IQueryable<UnidadTributariaDomicilio> GetUnidadTributuriaDombyId(long idUnidadTributaria)
        {
            return _context.UnidadesTributariasDomicilio.Where(x => x.UnidadTributariaID == idUnidadTributaria && x.TipoDomicilioID == 1 && x.FechaBaja == null);
        }
        public UnidadTributariaDomicilio GetUnidadTributuriaDombyIdDom(long idDom)
        {
            return _context.UnidadesTributariasDomicilio.Where(x => x.DomicilioID == idDom && x.TipoDomicilioID == 1 && x.FechaBaja == null).FirstOrDefault();
        }
    }
}
