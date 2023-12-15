using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.ValidacionesDB
{
    public class ValidacionException : Exception
    {
        public ResultadoValidacion ErrorValidacion { get; private set; }
        public IEnumerable<string> Errores { get; private set; }

        public ValidacionException(ResultadoValidacion errorValidacion, IEnumerable<string> errores)
        {
            ErrorValidacion = errorValidacion;
            Errores = errores;
        }
    }

    public class ValidacionTramiteException : ValidacionException
    {
        public int IdTramite { get; private set; }
        public ValidacionTramiteException(int idTramite, ResultadoValidacion errorValidacion, IEnumerable<string> errores)
            : base(errorValidacion, errores)
        {
            IdTramite = idTramite;
        }
    }
}
