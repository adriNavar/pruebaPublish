using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Repositories
{
    public interface INorteRepository
    {
        List<Norte> GetNortes();
        
        Norte GetNorteById(int idNorte);

        void InsertNorte(Norte norte);

        void UpdateNorte(Norte norte);

        void DeleteNorte(int idNorte);
    }
}
