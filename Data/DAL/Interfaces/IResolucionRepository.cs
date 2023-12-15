using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IResolucionRepository
    {
        Resolucion GetResolucionById(int idResolucion);
    }
}
