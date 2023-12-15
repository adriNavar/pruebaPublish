using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IManzanaRepository
    {
        Manzana GetManzanaById(long id);
    }
}
