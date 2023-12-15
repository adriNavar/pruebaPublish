using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.BusinessEntities.GlobalResources;

namespace GeoSit.Data.DAL.Repositories.Temporal
{
    public class ParcelaTemporalRepository : IParcelaTemporalRepository
    {
        private readonly GeoSITMContext _context;

        public ParcelaTemporalRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public ParcelaTemporal GetParcelaById(long idParcela, int tramite)
        {
            return _context.ParcelasTemporal.Find(idParcela, tramite);
        }

        public List<ParcelaTemporal> GetEntradasByIdTramite(int idTramite)
        {
            int tipoEntradaParcela = Convert.ToInt32(Entradas.Parcela);

            var query = from entradaTramite in _context.TramitesEntradas
                        join objetoEntrada in _context.ObjetosEntrada on entradaTramite.IdObjetoEntrada equals objetoEntrada.IdObjetoEntrada
                        join par in _context.ParcelasTemporal on new { id = entradaTramite.IdObjeto.Value, entradaTramite.IdTramite } equals new { id = par.ParcelaID, par.IdTramite }
                        where objetoEntrada.IdEntrada == tipoEntradaParcela && idTramite == entradaTramite.IdTramite
                        select par;

            return query.ToList();
        }
    }
}
