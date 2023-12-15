using System;
using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.BusinessEntities.GlobalResources;

namespace GeoSit.Data.DAL.Repositories.Temporal
{
    public class EspacioPublicoTemporalRepository : IEspacioPublicoTemporalRepository
    {
        private readonly GeoSITMContext _context;

        public EspacioPublicoTemporalRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public EspacioPublicoTemporal GetEspacioPublicoById(long id, int tramite)
        {
            return _context.EspaciosPublicosTemporal.Find(id , tramite);
        }

        public IEnumerable<EspacioPublicoTemporal> GetEspaciosPublicosByTramite(int tramite)
        {
            int tipoEntradaEspacioPublico = Convert.ToInt32(Entradas.EspacioPublico);

            var query = from entradaTramite in _context.TramitesEntradas
                        join objetoEntrada in _context.ObjetosEntrada on entradaTramite.IdObjetoEntrada equals objetoEntrada.IdObjetoEntrada
                        join espacioPublico in _context.EspaciosPublicosTemporal on new { id = entradaTramite.IdObjeto.Value, entradaTramite.IdTramite } equals new { id = espacioPublico.EspacioPublicoID, espacioPublico.IdTramite }
                        where objetoEntrada.IdEntrada == tipoEntradaEspacioPublico && tramite == entradaTramite.IdTramite
                        select espacioPublico;

            return query.ToList();
        }
    }
}
