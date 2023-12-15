using GeoSit.Data.DAL.Tramites.Interfaces;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Tramites.Invalid
{
    internal class TramiteInvalido : IProcessor
    {
        public void Configure() => throw new NotImplementedException();

        public void Configure(IEnumerable<IAction> actions) => throw new NotImplementedException();

        public bool IsTipoTramite() => false;

        public virtual void Process() => throw new InvalidOperationException();
    }
}
