using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class DomicilioInmuebleRepository : IDomicilioInmuebleRepository
    {
        private readonly GeoSITMContext _context;

        public DomicilioInmuebleRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Domicilio> GetDomicilioInmuebles(string nombreVia)
        {
            if (!string.IsNullOrEmpty(nombreVia))
            {
                nombreVia = nombreVia.ToLower();
            }
            return _context.Domicilios.Where(x => x.ViaNombre.ToLower().Contains(nombreVia));
        }

        public Domicilio GetDomicilioInmuebleById(long idDomicilio)
        {
            return _context.Domicilios.Find(idDomicilio);
        }

        public void InsertDomicilio(Domicilio domicilio)
        {
            domicilio.TipoDomicilio = null;
            domicilio.UsuarioAltaId = domicilio.UsuarioModifId;
            domicilio.FechaModif = DateTime.Now;
            domicilio.FechaAlta = domicilio.FechaModif;
            _context.Domicilios.Add(domicilio);
        }

        public void UpdateDomicilio(Domicilio domicilio)
        {
            domicilio.TipoDomicilio = null;
            domicilio.FechaModif = DateTime.Now;
            _context.Entry(domicilio).State = EntityState.Modified;
            _context.Entry(domicilio).Property(x => x.UsuarioAltaId).IsModified = false;
            _context.Entry(domicilio).Property(x => x.FechaAlta).IsModified = false;
        }

        public void DeleteDomicilio(Domicilio domicilio)
        {
            domicilio.TipoDomicilio = null;
            domicilio.UsuarioBajaId = domicilio.UsuarioModifId;
            domicilio.FechaModif = DateTime.Now;
            domicilio.FechaBaja = domicilio.FechaModif;
            _context.Entry(domicilio).State = EntityState.Modified;
            _context.Entry(domicilio).Property(d => d.UsuarioAltaId).IsModified = false;
            _context.Entry(domicilio).Property(d => d.FechaAlta).IsModified = false;
        }

        public Domicilio GetDomicilioByUnidadTributariaId(long idUnidadTributaria)
        {
            return (from utd in _context.UnidadesTributariasDomicilio
                    join dom in _context.Domicilios on utd.DomicilioID equals dom.DomicilioId
                    where utd.UnidadTributariaID == idUnidadTributaria && !dom.FechaBaja.HasValue
                    orderby dom.DomicilioId
                    select dom).FirstOrDefault();
        }

        public IEnumerable<Domicilio> GetDomiciliosByUnidadTributariaId(long idUnidadTributaria)
        {
            return (from utd in _context.UnidadesTributariasDomicilio
                    join dom in _context.Domicilios on utd.DomicilioID equals dom.DomicilioId
                    where utd.UnidadTributariaID == idUnidadTributaria && !dom.FechaBaja.HasValue
                    orderby dom.DomicilioId
                    select dom)
                .Include(x => x.TipoDomicilio)
                .ToList();
        }

    }
}
