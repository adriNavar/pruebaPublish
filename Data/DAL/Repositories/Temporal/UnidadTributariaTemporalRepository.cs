using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories.Temporal
{
    public class UnidadTributariaTemporalRepository : IUnidadTributariaTemporalRepository
    {
        private readonly GeoSITMContext _contexto;
        public UnidadTributariaTemporalRepository(GeoSITMContext contexto)
        {
            _contexto = contexto;
        }
        public UnidadTributariaTemporal GetById(long id, int idTramite)
        {
            return _contexto.UnidadesTributariasTemporal.Find(id, idTramite);
        }

        public IEnumerable<UnidadTributariaTemporal> GetEntradasByIdTramite(int idTramite)
        {
            int tipoEntradaUT = Convert.ToInt32(Entradas.UnidadTributaria);

            var query = from entradaTramite in _contexto.TramitesEntradas
                        join objetoEntrada in _contexto.ObjetosEntrada on entradaTramite.IdObjetoEntrada equals objetoEntrada.IdObjetoEntrada
                        join ut in _contexto.UnidadesTributariasTemporal on new { idUT = entradaTramite.IdObjeto.Value, entradaTramite.IdTramite } equals new { idUT = ut.UnidadTributariaId, ut.IdTramite }
                        where objetoEntrada.IdEntrada == tipoEntradaUT && idTramite == entradaTramite.IdTramite
                        select ut;
            
            return query.ToList();
        }
    }
}
