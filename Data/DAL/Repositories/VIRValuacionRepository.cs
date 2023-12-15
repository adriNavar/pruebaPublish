using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class VIRValuacionRepository : IVIRValuacionRepository
    {
        private readonly GeoSITMContext _context;

        public VIRValuacionRepository(GeoSITMContext context)
        {
            this._context = context;
        }
        public List<VIRValuacion> GetVIRValuacionesByIdInmueble(long idInmueble)
        {
            var lista = (from valVir in _context.VirValuaciones
                         where valVir.InmuebleId == idInmueble
                         orderby valVir.VigenciaDesde descending
                         select valVir).ToList();

            var vigente = lista.FirstOrDefault(v => v.VigenciaDesde == lista.Max(p => p.VigenciaDesde));
            if(vigente != null)
            {
                vigente.Vigente = true;
            }
            return lista;
        }

        public VIRValuacion GetValuacionVigente(long idInmueble)
        {
            return (from valVir in _context.VirValuaciones
                    where valVir.InmuebleId == idInmueble
                    orderby valVir.VigenciaDesde descending
                    select valVir).FirstOrDefault();
        }
    }
}
