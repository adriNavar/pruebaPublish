using FluentValidation;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ValidationRules.Tramites
{
    public class TramiteValidator : AbstractValidator<Tramite>
    {
        private readonly Tramite _tramite;

        public TramiteValidator(Tramite tramite)
        {
            _tramite = tramite;

            RuleFor(x => x.Cod_Tramite)
                .Must(UnicoCodigoTramite)
                .WithMessage("Certificado: El identificador ya existe");
        }

        private bool UnicoCodigoTramite(Tramite tramite, string arg)
        {
            return _tramite == null || tramite.Id_Tramite == _tramite.Id_Tramite;
        }
    }
}
