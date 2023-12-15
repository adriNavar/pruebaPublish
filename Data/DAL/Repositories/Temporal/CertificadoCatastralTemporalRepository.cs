using System;
using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.BusinessEntities.GlobalResources;

namespace GeoSit.Data.DAL.Repositories.Temporal
{
    public class CertificadoCatastralTemporalRepository : ICertificadoCatastralTemporalRepository
    {
        private readonly GeoSITMContext _context;

        public CertificadoCatastralTemporalRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public INMCertificadoCatastralTemporal GetCertificado(int idCertificado, int tramite)
        {
            return _context.INMCertificadosCatastralesTemporal.Find(idCertificado, tramite);
        }

        public IEnumerable<INMCertificadoCatastralTemporal> GetCertificadosByTramite(int tramite)
        {
            int tipoEntradaDescInmueble = Convert.ToInt32(Entradas.DescripcionInmueble);

            var query = from entradaTramite in _context.TramitesEntradas
                        join objetoEntrada in _context.ObjetosEntrada on entradaTramite.IdObjetoEntrada equals objetoEntrada.IdObjetoEntrada
                        join certificado in _context.INMCertificadosCatastralesTemporal on new { id = entradaTramite.IdObjeto.Value, entradaTramite.IdTramite } equals new { id = certificado.CertificadoCatastralId, certificado.IdTramite }
                        where objetoEntrada.IdEntrada == tipoEntradaDescInmueble && tramite == entradaTramite.IdTramite
                        select certificado;

            return query.ToList();

        }
    }
}

