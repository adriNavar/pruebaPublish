using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.Certificados;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using Oracle.ManagedDataAccess.Client;

namespace GeoSit.Data.DAL.Repositories
{
    public class ExpedienteObraRepository : IExpedienteObraRepository
    {
        private readonly GeoSITMContext _context;

        public ExpedienteObraRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<ExpedienteObra> GetExpedienteObras()
        {
            return _context.ExpedientesObra;
        }

        public List<ExpedienteObra> GetExpedienteObras(long unidadTributariaId, string numeroLegajoIni, string numeroLegajoFin,
            string numeroExpedienteIni, string numeroExpedienteFin, string fechaLegajoIni, string fechaLegajoFin,
            string fechaExpedienteIni, string fechaExpedienteFin, long personaId, long estadoId)
        {
            bool filtraLegajoIni = !string.IsNullOrEmpty(numeroLegajoIni);
            bool filtraLegajoFin = !string.IsNullOrEmpty(numeroLegajoFin);
            bool filtraExpedienteIni = !string.IsNullOrEmpty(numeroExpedienteIni);
            bool filtraExpedienteFin = !string.IsNullOrEmpty(numeroExpedienteFin);
            bool filtraFechaExpedienteIni = DateTime.TryParse(fechaExpedienteIni, out DateTime fechaExpIni);
            bool filtraFechaExpedienteFin = DateTime.TryParse(fechaExpedienteFin, out DateTime fechaExpFin);
            bool filtraFechaLegajoIni = DateTime.TryParse(fechaLegajoIni, out DateTime fechaLgjIni);
            bool filtraFechaLegajoFin = DateTime.TryParse(fechaLegajoFin, out DateTime fechaLgjFin);

            var query = from expediente in _context.ExpedientesObra
                        join per in _context.PersonasExpedienteObra on expediente.ExpedienteObraId equals per.ExpedienteObraId into personas
                        join estado in _context.EstadosExpedienteObra on expediente.ExpedienteObraId equals estado.ExpedienteObraId into estados
                        join ut in _context.UnidadesTributariasExpedienteObra on expediente.ExpedienteObraId equals ut.ExpedienteObraId into unidadesTributarias
                        where (!filtraLegajoIni || numeroLegajoIni.CompareTo(expediente.NumeroLegajo) != 1) &&
                              (!filtraLegajoFin || numeroLegajoFin.CompareTo(expediente.NumeroLegajo) != -1) &&
                              (!filtraExpedienteIni || numeroExpedienteIni.CompareTo(expediente.NumeroExpediente) != 1) &&
                              (!filtraExpedienteFin || numeroExpedienteFin.CompareTo(expediente.NumeroExpediente) != -1) &&
                              (!filtraFechaExpedienteIni || expediente.FechaExpediente >= fechaExpIni) &&
                              (!filtraFechaExpedienteFin || expediente.FechaExpediente <= fechaExpFin) &&
                              (!filtraFechaLegajoIni || expediente.FechaLegajo >= fechaLgjIni) &&
                              (!filtraFechaLegajoFin || expediente.FechaLegajo <= fechaLgjFin) &&
                              (personaId == 0 || personas.Any(p => p.FechaBaja == null && p.PersonaInmuebleId == personaId)) &&
                              (estadoId == 0 || estados.Any(e => e.FechaBaja == null && e.FechaAlta == estados.Where(m => m.FechaBaja == null).Max(m => m.FechaAlta) && e.EstadoExpedienteId == estadoId)) &&
                              (unidadTributariaId == 0 || unidadesTributarias.Any(ut => ut.FechaBaja == null && ut.UnidadTributariaId == unidadTributariaId)) &&
                              (expediente.FechaBaja == null)
                        select expediente;

            return query.ToList();
        }

        /*public string GetNumeroLegajoSiguiente()
        {
            var pResult = new OracleParameter("result", OracleDbType.RefCursor, ParameterDirection.Output);
            var numeroLegajo = _context.Database.SqlQuery<NumeroLegajo>("BEGIN READ_NUMERO_LEGAJO_SEQ(:result); END;",
                pResult).Single();
            return numeroLegajo.NumeroPrefact.ToString().PadLeft(8, '0');
        }*/

        public string GetNumeroLegajoSiguiente()
        {
            var nLegajo = _context.ExpedientesObra.Where(n => n.FechaBaja == null).OrderByDescending(t => t.FechaAlta).Select(x=>x.NumeroLegajo).FirstOrDefault();
            return nLegajo + 1;
        }

        public string GetNumeroExpedienteSiguiente()
        {
            var nExpediente = _context.ExpedientesObra.Where(n=>n.FechaBaja == null).OrderByDescending(t => t.FechaAlta).Select(x => x.NumeroExpediente).FirstOrDefault();

            return nExpediente;
        }

        public ExpedienteObra GetExpedienteObraByNumeroLegajo(string numeroLegajo)
        {
            if (numeroLegajo == null) return null;
            numeroLegajo = numeroLegajo.ToLower();

            return _context.ExpedientesObra
                           .Include("EstadoExpedienteObras.EstadoExpediente")
                           .Include("TipoExpedienteObras.TipoExpediente")
                           .Include("ObservacionExpedientes")
                           .Include("UnidadTributariaExpedienteObras.UnidadTributaria")
                           .Include("ControlTecnicos")
                           .FirstOrDefault(x => x.NumeroLegajo.ToLower() == numeroLegajo && x.FechaBaja == null);
        }

        public ExpedienteObra GetExpedienteObraByNumeroExpediente(string numeroExpediente)
        {
            if (numeroExpediente == null) return null;
            numeroExpediente = numeroExpediente.ToLower();

            return _context.ExpedientesObra
                           .Include("EstadoExpedienteObras.EstadoExpediente")
                           .Include("TipoExpedienteObras.TipoExpediente")
                           .Include("ObservacionExpedientes")
                           .Include("UnidadTributariaExpedienteObras.UnidadTributaria")
                           .Include("ControlTecnicos")
                           .FirstOrDefault(x => x.NumeroExpediente.ToLower() == numeroExpediente && x.FechaBaja == null);
        }

        public ExpedienteObra GetExpedienteObraById(long idExpedienteObra)
        {
            return _context.ExpedientesObra.Find(idExpedienteObra);
        }

        public void InsertExpedienteObra(ExpedienteObra expedienteObra)
        {
            _context.ExpedientesObra.Add(expedienteObra);
        }

        public void UpdateExpedienteObra(ExpedienteObra expedienteObra)
        {
            _context.Entry(expedienteObra).State = EntityState.Modified;

            //exclude fields from update
            _context.Entry(expedienteObra).Property(x => x.UsuarioAltaId).IsModified = false;
            _context.Entry(expedienteObra).Property(x => x.FechaAlta).IsModified = false;
        }

        public void DeleteExpedienteObra(long idExpedienteObra, long idUsuarioBaja, DateTime fechaBaja)
        {
            var expedienteObra = _context.ExpedientesObra.Find(idExpedienteObra);

            expedienteObra.UsuarioBajaId = idUsuarioBaja;
            expedienteObra.FechaBaja = fechaBaja;
            expedienteObra.UsuarioModificacionId = idUsuarioBaja;
            expedienteObra.FechaModificacion = fechaBaja;

            DeleteExpedienteObra(expedienteObra);
        }

        public void DeleteExpedienteObra(ExpedienteObra expedienteObra)
        {
            //Es básicamente un update pero semánticamente es un delete
            if (expedienteObra == null) return;

            _context.Entry(expedienteObra).State = EntityState.Modified;

            //exclude fields from update
            _context.Entry(expedienteObra).Property(x => x.UsuarioAltaId).IsModified = false;
            _context.Entry(expedienteObra).Property(x => x.FechaAlta).IsModified = false;
        }

        public INMCertificadoCatastral GetCertificadoCatastral(long idCertificadoCatastral)
        {
            return _context.INMCertificadosCatastrales
                        .Include("UnidadTributaria")
                        .Include("UnidadTributaria.Parcela")
                        .Include("UnidadTributaria.Parcela.Nomenclaturas")
                        .Include("UnidadTributaria.UTDomicilios")
                        .Include("UnidadTributaria.UTDomicilios.Domicilio")
                        .Include("UnidadTributaria.Valuaciones")
                        .Include("Solicitante")
                        .Include("Solicitante.PersonaDomicilios")
                        .Include("Solicitante.PersonaDomicilios.Domicilio")
                        .Include("Solicitante.TipoDocumentoIdentidad")
                        .Include("Solicitante.TipoPersona")
                        .FirstOrDefault(x => x.CertificadoCatastralId == idCertificadoCatastral && x.FechaBaja == null);
        }

        public List<INMCertificadoCatastral> GetCertificadosCatastral()
        {
            return _context.INMCertificadosCatastrales.ToList();
        }

        public void InsertCertificadoCatastral(INMCertificadoCatastral certificadoCatastral)
        {
            _context.INMCertificadosCatastrales.Add(certificadoCatastral);
        }
    }
}
