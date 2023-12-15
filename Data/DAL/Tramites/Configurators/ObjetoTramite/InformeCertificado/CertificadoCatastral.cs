using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions;
using GeoSit.Data.DAL.Tramites.Configurators.ObjetoTramite.Abstract;
using GeoSit.Data.DAL.Tramites.Interfaces;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System;

namespace GeoSit.Data.DAL.Tramites.Configurators.ObjetoTramite.InformeCertificado
{
    class CertificadoCatastral : ObjetoTramiteConfigurator
    {
        public CertificadoCatastral(int tipoTramiteConfigurable, METramite tramite, GeoSITMContext contexto)
            : base(tramite, tipoTramiteConfigurable, Convert.ToInt32(ObjetosTramites.CertificadoCatastral),
                  new IAction[] { new GenerarCertificadoCatastral(tramite, contexto) }) { }
    }
}
