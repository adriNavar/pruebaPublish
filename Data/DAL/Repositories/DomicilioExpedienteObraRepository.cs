using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    class DomicilioExpedienteObraRepository: IDomicilioExpedienteObraRepository
    {
        private readonly GeoSITMContext _context;

        public DomicilioExpedienteObraRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<UbicacionExpedienteObra> GetUbicacionExpedienteObras(long idExpedienteObra)
        {
            var domicilioInmuebles = _context.Domicilios;
            var domicilioInmuebleExpedienteObras = _context.DomiciliosExpedienteObra;
            var expedienteObras = _context.ExpedientesObra;

            var query = from d in domicilioInmuebles
                        join dieo in domicilioInmuebleExpedienteObras
                        on d.DomicilioId equals dieo.DomicilioInmuebleId
                        join eo in expedienteObras on dieo.ExpedienteObraId equals eo.ExpedienteObraId
                        where dieo.ExpedienteObraId == idExpedienteObra
                        select new UbicacionExpedienteObra
                        {
                            DomicilioInmuebleId = d.DomicilioId,
                            NombreVia = d.ViaNombre,
                            NumeroPuerta = d.numero_puerta,
                            Barrio = d.barrio,
                            CodigoPostal = d.codigo_postal,
                            DomicilioPrimario = dieo.Primario
                        };

            return query;
        }

        public IEnumerable<DomicilioExpedienteObra> GetDomicilioExpedienteObras(long idDomicilio, long idExpedienteObra)
        {
            return _context.DomiciliosExpedienteObra
                .Where(x => x.DomicilioInmuebleId == idDomicilio && x.ExpedienteObraId == idExpedienteObra);
        }

        public DomicilioExpedienteObra GetDomicilioExpedienteObraById(long idDomicilio, long idExpedienteObra)
        {
            return _context.DomiciliosExpedienteObra.Find(idDomicilio, idExpedienteObra);
        }

        public long GetDomicilioExpedienteObraIdByUnidadTributariaId(long idUnidadTributaria)
        {
            var unidadTributariaDomicilio = _context.UnidadesTributariasDomicilio
                .SingleOrDefault(x => x.UnidadTributariaID == idUnidadTributaria);
            return unidadTributariaDomicilio != null ? unidadTributariaDomicilio.DomicilioID : 0;
        }

        public long GetUnidadTributariaExpedienteObraIdByDomicilioId(long idDomicilio)
        {
            var unidadTributariaDomicilio = _context.UnidadesTributariasDomicilio
                .SingleOrDefault(x => x.DomicilioID == idDomicilio);
            return unidadTributariaDomicilio != null ? unidadTributariaDomicilio.UnidadTributariaID : 0;
        }

        public void InsertDomicilioExpedienteObra(DomicilioExpedienteObra domicilioExpedienteObra, ExpedienteObra expedienteObra)
        {
            domicilioExpedienteObra.ExpedienteObra = expedienteObra;
            _context.DomiciliosExpedienteObra.Add(domicilioExpedienteObra);
        }

        public void InsertDomicilioExpedienteObra(DomicilioExpedienteObra domicilioExpedienteObra)
        {
            _context.DomiciliosExpedienteObra.Add(domicilioExpedienteObra);
        }

        public void UpdateDomicilioExpedienteObra(DomicilioExpedienteObra domicilioExpedienteObra)
        {
            _context.Entry(domicilioExpedienteObra).State = EntityState.Modified;
        }

        public void DeleteDomicilioExpedienteObra(long idDomicilio, long idExpedienteObra)
        {
            var dieo = GetDomicilioExpedienteObraById(idDomicilio, idExpedienteObra);
            DeleteDomicilioExpedienteObra(dieo);
        }

        public void DeleteDomiciliosByExpedienteObraId(long idExpedienteObra)
        {
            var domicilios = _context.DomiciliosExpedienteObra.Where(x => x.ExpedienteObraId == idExpedienteObra);
            foreach (var domicilio in domicilios)
            {
                _context.Entry(domicilio).State = EntityState.Deleted;
            }
        }

        public void DeleteDomicilioExpedienteObra(DomicilioExpedienteObra domicilioExpedienteObra)
        {
            if (domicilioExpedienteObra != null)
                _context.Entry(domicilioExpedienteObra).State =EntityState.Deleted;
        }

        public IEnumerable<DomicilioExpedienteObra> GetDomicilioExpedienteObras(long idExpedienteObra)
        {
            return _context.DomiciliosExpedienteObra.Where(x => x.ExpedienteObraId == idExpedienteObra);
        }
    }
}
