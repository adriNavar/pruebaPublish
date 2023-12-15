using GeoSit.Data.BusinessEntities.Certificados;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogRPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IRegistroPropiedadInmuebleRepository
    {
        object GetCertificadoCatastral(string numCertificadoCatastral);

        object GetCertificadoCatastralByNumero(string numCertificadoCatastral);

        void RegistrarLogRespuesta(RPILogRespuestas logRespuesta);

        List<object> GetPlanosMensuraByNumero(string numMensura, string letraMensura);

        List<object> GetPlanosMensuraByNomenclatura(string nomenclatura);

        List<object> GetPlanosMensuraByNumeroPartida(string numeroPartida);

        object GetPlanoMensuraByIdMensura(long idMensura);

        object VerificarPermisoDeAcceso(long idUsuario);

        void RegistrarLogConsulta(RPILogConsultas logConsulta);
    }
}
