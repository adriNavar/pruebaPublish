using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.Processors
{
    internal class TramiteMensuraCI : TramiteMensuraPH
    {
        public TramiteMensuraCI(METramite tramite, GeoSITMContext contexto)
            : base(tramite, contexto) { }

        public override bool IsTipoTramite()
        {
            /*por ahora filtro en este lugar los objetos de tramite posibles de procesar*/
            return new[]
            {
                Convert.ToInt32(ObjetosTramites.DivisionRegimenConjuntoInmobiliarioMensuraRegistrada),
                Convert.ToInt32(ObjetosTramites.MensuraYDivisionRegimenConjuntoInmobiliario),
            }.Contains(_tramite.IdObjetoTramite);
        }
    }
}
