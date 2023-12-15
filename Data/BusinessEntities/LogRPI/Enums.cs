using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.LogRPI
{
    public enum TipoDeOperacion: int
    {
        ConsultaCertificadoCatastral = 101,
        ObtenerCertificadoCatastral = 102,
        ObtenerListaDePlanosPorNumero = 103,
        ObtenerPlanoDeMensuraPorID = 106,
        ObtenerListaDePlanosPorPartida = 105,
        VerificarPermisosDeAccesoR = 107,
        ObtenerListaDePlanosPorNomenclatura = 104,
        ConsultarInscripcionPorPartida = 201,
        ConsultarInscripcionPorNomenclatura = 202,
        ConsultarInscripcionPorDocumentoTitular = 203,
        ConsultarInscripcionPorNombreTitular = 204,
        ConsultarDatosDeDominioPorInscripcion = 205,
        VerficarPermisosDeAccesoC = 206,

    }
}
