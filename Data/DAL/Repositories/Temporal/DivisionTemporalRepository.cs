using System.Linq;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.BusinessEntities.Temporal;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System;

namespace GeoSit.Data.DAL.Repositories.Temporal
{
    public class DivisionTemporalRepository : IDivisionTemporalRepository
    {
        private readonly GeoSITMContext _context;

        public DivisionTemporalRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<DivisionTemporal> GetByTramite(int tramite)
        {
            int tipoEntradaMza = Convert.ToInt32(Entradas.Manzana);

            var query = from entradaTramite in _context.TramitesEntradas
                        join objetoEntrada in _context.ObjetosEntrada on entradaTramite.IdObjetoEntrada equals objetoEntrada.IdObjetoEntrada
                        join manzana in _context.DivisionesTemporal on new { id = entradaTramite.IdObjeto.Value, entradaTramite.IdTramite } equals new { id = manzana.FeatId, manzana.IdTramite }
                        where objetoEntrada.IdEntrada == tipoEntradaMza && tramite == entradaTramite.IdTramite
                        select manzana;

            return query.ToList();
        }

        public DivisionTemporal GetManzana(int idManzana, int tramite)
        {
            return _context.DivisionesTemporal.Find(idManzana,tramite);
        }
    }
}
