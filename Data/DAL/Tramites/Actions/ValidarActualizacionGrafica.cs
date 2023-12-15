using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Implementaciones;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Repositories;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Tramites.Actions
{
    class ValidarActualizacionGrafica : Accion
    {
        public ValidarActualizacionGrafica(METramite tramite, GeoSITMContext contexto)
            : base(tramite, contexto) { }

        public override bool Execute()
        {
            Errores = new List<string>();
            Resultado = new ValidacionDBRepository(Contexto)
                .Validar(new ObjetoValidable()
                {
                    IdTramite = Tramite.IdTramite,
                    TipoObjeto = TipoObjetoValidable.ParcelaGrafica,
                    Funcion = FuncionValidable.Todas,
                    IdObjeto = Tramite.IdObjetoTramite
                }, out List<string> _errores);

            Errores.AddRange(_errores);

            return Resultado == ResultadoValidacion.Ok;
        }
    }
}
