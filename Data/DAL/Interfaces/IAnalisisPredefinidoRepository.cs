using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.MapasTematicos;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IAnalisisPredefinidoRepository
    {
        IEnumerable<Distritos> GetDistritos();

        void InsertReclamos(CargasTecnicas reclamoTecnico);

        AnalisisTecnicos GetAnalisisTecnicoById(long idAnalisisTecnico);
    }
}
