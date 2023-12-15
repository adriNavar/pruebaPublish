using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories
{
    public class ComprobantePagoRepository : IComprobantePagoRepository
    {
        readonly GeoSITMContext _context;
        public ComprobantePagoRepository(GeoSITMContext context)
        {
            _context = context;
        }
        public MEComprobantePago GetById(long id)
        {
            return _context.ComprobantePago.Find(id);
        }

        public IEnumerable<MEComprobantePago> GetByTramite(int tramite)
        {
            int tipoEntradaComprobantePago = Convert.ToInt32(Entradas.ComprobantePago);

            var query = from entradaTramite in _context.TramitesEntradas
                        join objetoEntrada in _context.ObjetosEntrada on entradaTramite.IdObjetoEntrada equals objetoEntrada.IdObjetoEntrada
                        join comprobantePago in _context.ComprobantePago on entradaTramite.IdObjeto.Value equals comprobantePago.IdComprobantePago
                        where objetoEntrada.IdEntrada == tipoEntradaComprobantePago && tramite == entradaTramite.IdTramite
                        select comprobantePago;

            return query.ToList();

        }
    }
}
