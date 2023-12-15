using System.Linq;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Repositories.Temporal
{
    public class LibreDeDeudaTemporalRepository : ILibreDeDeudaTemporalRepository
    {
        private readonly GeoSITMContext _context;

        public LibreDeDeudaTemporalRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public INMLibreDeDeudaTemporal GetLibreDeDeuda(int idLibreDeuda, int tramite)
        {
            return _context.INMLibresDeDeudasTemporal.Find(idLibreDeuda, tramite);
        }

        public IEnumerable<INMLibreDeDeudaTemporal> GetLibresDeDeudaByTramite(int tramite)
        {
            int tipoEntradaLD = Convert.ToInt32(Entradas.LibreDeuda);

            var query = from entradaTramite in _context.TramitesEntradas
                        join objetoEntrada in _context.ObjetosEntrada on entradaTramite.IdObjetoEntrada equals objetoEntrada.IdObjetoEntrada
                        join libreDeuda in _context.INMLibresDeDeudasTemporal on new { id = entradaTramite.IdObjeto.Value, entradaTramite.IdTramite } equals new { id = libreDeuda.IdLibreDeuda, libreDeuda.IdTramite }
                        where objetoEntrada.IdEntrada == tipoEntradaLD && tramite == entradaTramite.IdTramite
                        select libreDeuda;

            return query.ToList();
        }
    }
}
