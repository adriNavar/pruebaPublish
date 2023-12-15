using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Tramites.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.Configurators.ObjetoTramite.Abstract
{
    abstract class ObjetoTramiteConfigurator : IConfigurator
    {
        private readonly int[] _objetosTramite;
        private readonly int _tipoTramiteConfigurable;
        protected readonly IEnumerable<IAction> _actions;
        protected readonly METramite _tramite;

        public ObjetoTramiteConfigurator()
            : this(null, 0, 0, new IAction[0]) { }

        protected ObjetoTramiteConfigurator(METramite tramite, int tipoTramiteConfigurable, int objetoTramite, IEnumerable<IAction> actions)
            : this(tramite, tipoTramiteConfigurable, new[] { objetoTramite }, actions) { }
        protected ObjetoTramiteConfigurator(METramite tramite, int tipoTramiteConfigurable, IEnumerable<int> objetosTramite, IEnumerable<IAction> actions)
        {
            _tramite = tramite;
            _tipoTramiteConfigurable = tipoTramiteConfigurable;
            _objetosTramite = objetosTramite.ToArray();
            _actions = actions;
        }
        public void Configure(IProcessor processor)
        {
            processor.Configure(_actions);
        }
        public bool IsConfiguradorTipoTramite()
        {
            return _tramite != null && _tipoTramiteConfigurable == _tramite.IdTipoTramite;
        }
        public virtual bool IsConfiguradorTramiteDefault()
        {
            return false;
        }
        public virtual bool IsObjetoTramite()
        {
            return _tramite != null && _objetosTramite.Contains(_tramite.IdObjetoTramite);
        }
    }
}
