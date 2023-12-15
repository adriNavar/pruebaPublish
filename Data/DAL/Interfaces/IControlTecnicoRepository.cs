using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IControlTecnicoRepository
    {
        IEnumerable<ControlTecnico> GetControlTecnicos(long idExpedienteObra);

        ControlTecnico GetControlTecnicoById(long idControlTecnico);

        void InsertControlTecnico(ControlTecnico controlTecnico, ExpedienteObra expedienteObra);

        void InsertControlTecnico(ControlTecnico controlTecnico);

        void UpdateControlTecnico(ControlTecnico controlTecnico);

        void DeleteControlTecnicoesByExpedienteObraId(long idExpedienteObra);

        void DeleteControlTecnicoById(long idControlTecnico);

        void DeleteControlTecnico(ControlTecnico controlTecnico);
    }
}
