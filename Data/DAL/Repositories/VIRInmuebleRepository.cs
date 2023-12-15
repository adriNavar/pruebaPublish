using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System;
using System.Linq;


namespace GeoSit.Data.DAL.Repositories
{
    public class VIRInmuebleRepository : IVIRInmuebleRepository
    {
        private readonly GeoSITMContext _context;

        public VIRInmuebleRepository(GeoSITMContext context)
        {
            this._context = context;
        }

        public VIRInmueble GetVIRInmuebleByIdInmueble(long idInmueble)
        {
            return _context.VirInmuebles.Where(x => x.InmuebleId == idInmueble).FirstOrDefault();
        }

        public VIRInmueble SaveVIRInmueble(VIRInmueble inmueble)
        {
            try
            {
                var inmOrig = _context.VirInmuebles.Where(x => x.InmuebleId == inmueble.InmuebleId).FirstOrDefault();
                if (inmOrig == null)
                {
                    throw (new MissingMemberException("No existe el inmueble VIR"));
                }
                inmOrig.FechaVigenciaDesde = inmueble.FechaVigenciaDesde;
                inmOrig.MejoraSupCubierta = inmueble.MejoraSupCubierta;
                inmOrig.MejoraSupSemicubierta = inmueble.MejoraSupSemicubierta;
                inmOrig.MejoraTipoVIR = inmueble.MejoraTipoVIR;
                inmOrig.MejoraUsoVIR = inmueble.MejoraUsoVIR;
                inmOrig.MejoraEstado = inmueble.MejoraEstado;

                return inmOrig;
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError($"VIRInmuebleRepository->SaveVIRInmueble({inmueble?.Partida})", ex);
                throw;
            }

        }
    }
}
