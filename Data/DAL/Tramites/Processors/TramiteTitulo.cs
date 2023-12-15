using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Processors.Abstract;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System;

namespace GeoSit.Data.DAL.Tramites.NoHabilitado.Processors
{
    internal class TramiteTitulo : TramiteProcessor
    {
        public TramiteTitulo(METramite tramite, GeoSITMContext context) 
            : base(tramite, context, Convert.ToInt32(TiposTramites.TramiteTitulo)) { }

        protected override void ConfigurePreProcessingActions()
        {
            return;
        }
        protected override void ConfigurePostProcessingActions()
        {
            return;
        }
    }
}
