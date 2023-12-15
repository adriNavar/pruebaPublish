using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IHojaRepository
    {
        IEnumerable<Hoja> GetHojas();

        Hoja GetHojaById(int idHoja);

        void InsertHoja(Hoja hoja);

        void UpdateHoja(Hoja hoja);

        void DeleteHoja(int id);
    }
}
