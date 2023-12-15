using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.DAL.Repositories
{
    public class UnidadTributariaDocumentoRepository: IUnidadTributariaDocumentoRepository
    {
        private readonly GeoSITMContext _context;

        public UnidadTributariaDocumentoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public UnidadTributariaDocumento GetUnidadTributariaDocumentoByID(long unidadTributariaDocumentoId)
        {
            return _context.UnidadesTributariasDocumento.FirstOrDefault(utd => utd.UnidadTributariaDocID == unidadTributariaDocumentoId);
        }

        public void InsertUnidadTributariaDocumento(UnidadTributariaDocumento unidadTributariaDocumento)
        {
            unidadTributariaDocumento.UsuarioAltaID = unidadTributariaDocumento.UsuarioModificacionID;
            unidadTributariaDocumento.FechaModificacion = DateTime.Now;
            unidadTributariaDocumento.FechaAlta = unidadTributariaDocumento.FechaModificacion;
            _context.UnidadesTributariasDocumento.Add(unidadTributariaDocumento);
        }

        public void RemoveUnidadTributariaDocumento(UnidadTributariaDocumento unidadTributariaDocumento)
        {
            unidadTributariaDocumento.UsuarioBajaID = unidadTributariaDocumento.UsuarioModificacionID;
            unidadTributariaDocumento.FechaModificacion = DateTime.Now;
            unidadTributariaDocumento.FechaBaja = unidadTributariaDocumento.FechaModificacion;
            _context.Entry(unidadTributariaDocumento).State = EntityState.Modified;
            _context.Entry(unidadTributariaDocumento).Property(p => p.FechaAlta).IsModified = false;
            _context.Entry(unidadTributariaDocumento).Property(p => p.UsuarioAltaID).IsModified = false;
        }
    }
}
