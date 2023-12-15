using System.Linq;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.BusinessEntities.Temporal;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System;

namespace GeoSit.Data.DAL.Repositories.Temporal
{
    public class DominioTemporalRepository : IDominioTemporalRepository
    {
        private readonly GeoSITMContext _context;

        public DominioTemporalRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public DominioTemporal GetDominio(int idDominio, int tramite)
        {
            return _context.DominiosTemporal.FirstOrDefault(x => x.DominioID == idDominio && x.IdTramite == tramite);
        }

        public IEnumerable<DominioTemporal> GetDominiosByTramite(int tramite)
        {
            int tipoEntradaTitulo = Convert.ToInt32(Entradas.Titulo);

            var query = from entradaTramite in _context.TramitesEntradas
                        join objetoEntrada in _context.ObjetosEntrada on entradaTramite.IdObjetoEntrada equals objetoEntrada.IdObjetoEntrada
                        join dominio in _context.DominiosTemporal on new { id = entradaTramite.IdObjeto.Value, entradaTramite.IdTramite } equals new { id = dominio.DominioID, dominio.IdTramite }
                        where objetoEntrada.IdEntrada == tipoEntradaTitulo && tramite == entradaTramite.IdTramite
                        select dominio;

            return query.ToList();
        }

        public DominioTitularTemporal GetDominioTitular(int idPersona, int tramite)
        {
            return _context.DominiosTitularesTemporal.FirstOrDefault(x => x.PersonaID == idPersona && x.IdTramite == tramite);
        }

    }
}
