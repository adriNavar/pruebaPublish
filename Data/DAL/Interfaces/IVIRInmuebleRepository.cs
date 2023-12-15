using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IVIRInmuebleRepository
    {
        VIRInmueble GetVIRInmuebleByIdInmueble(long idInmueble);
        VIRInmueble SaveVIRInmueble(VIRInmueble inmueble);
    }
}
