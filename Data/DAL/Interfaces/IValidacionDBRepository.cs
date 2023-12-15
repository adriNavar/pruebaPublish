using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Implementaciones;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IValidacionDBRepository
    {
        ResultadoValidacion ValidarFuncionGrupo(ObjetoValidable obj, out List<string> errores);
        ResultadoValidacion Validar(ObjetoValidable obj, out List<string> errores);
    }
}
