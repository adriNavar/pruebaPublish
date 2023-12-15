using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IImagenSatelitalRepository
    {
        IEnumerable<ImagenSatelital> GetAllImagenSatelital();

        ImagenSatelital GetImagenSatelitalById(int idImagenSatelital);
    }
}
