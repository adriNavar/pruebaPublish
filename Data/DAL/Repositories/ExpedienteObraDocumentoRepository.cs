using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class ExpedienteObraDocumentoRepository : IExpedienteObraDocumentoRepository
    {
        private readonly GeoSITMContext _context;

        public ExpedienteObraDocumentoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<DocumentoExpedienteObra> GetExpedienteObraDocumentos(long idExpedienteObra)
        {
            var expedienteObraDocumentos = _context.ExpedienteObraDocumentos;
            var documentos = _context.Documento;
            var tipoDocumentos = _context.TipoDocumento;

            var query = from eod in expedienteObraDocumentos
                        join documento in documentos
                        on eod.DocumentoId equals documento.id_documento
                        join tipoDocumento in tipoDocumentos
                        on documento.id_tipo_documento equals tipoDocumento.TipoDocumentoId
                        where eod.ExpedienteObraId == idExpedienteObra && eod.FechaBaja == null
                        select new DocumentoExpedienteObra
                        {
                            DocumentoId = documento.id_documento,
                            TipoDocumento = tipoDocumento.Descripcion,
                            Descripcion = documento.descripcion,
                            FechaDateTime = documento.fecha,
                            Observaciones = documento.observaciones
                        };

            return query;
        }

        public ExpedienteObraDocumento GetExpedienteObraDocumentoById(long idExpedienteObra, long idDocumento)
        {
            return _context.ExpedienteObraDocumentos.Find(idExpedienteObra, idDocumento);
        }

        public void InsertExpedienteObraDocumento(ExpedienteObraDocumento expedienteObraDocumento, ExpedienteObra expedienteObra)
        {
            expedienteObraDocumento.ExpedienteObra = expedienteObra;
            _context.ExpedienteObraDocumentos.Add(expedienteObraDocumento);
        }

        public void InsertExpedienteObraDocumento(ExpedienteObraDocumento expedienteObraDocumento)
        {
            _context.ExpedienteObraDocumentos.Add(expedienteObraDocumento);
        }

        public void UpdateExpedienteObraDocumento(ExpedienteObraDocumento expedienteObraDocumento)
        {
            _context.Entry(expedienteObraDocumento).State = EntityState.Modified;
        }

        public void DeleteExpedienteObraDocumento(long idExpedienteObra)
        {
            var expedienteObraDocumentos = _context.ExpedienteObraDocumentos.Where(x => x.ExpedienteObraId == idExpedienteObra);
            foreach (var expedienteObraDocumento in expedienteObraDocumentos)
            {
                _context.Entry(expedienteObraDocumento).State = EntityState.Deleted;
            }
        }

        public void DeleteExpedienteObraDocumento(long idExpedienteObra, long idDocumento)
        {
            DeleteExpedienteObraDocumento(GetExpedienteObraDocumentoById(idExpedienteObra, idDocumento));
        }

        public void DeleteExpedienteObraDocumento(ExpedienteObraDocumento expedienteObraDocumento)
        {
            if (expedienteObraDocumento != null)
                _context.Entry(expedienteObraDocumento).State = EntityState.Deleted;
        }
    }
}
