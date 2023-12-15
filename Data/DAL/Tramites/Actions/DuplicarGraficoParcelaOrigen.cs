using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.Actions
{
    class DuplicarGraficoParcelaOrigen : AccionEntrada
    {
        const string PARAM_ID_COMPONENTE_PARCELA = "ID_COMPONENTE_PARCELA";
        readonly string tabla;
        readonly string campoId = "ID_PARCELA";
        readonly string campoFeatId = "FEATID";

        List<KeyValuePair<Atributo, object>> camposTabla;
        public DuplicarGraficoParcelaOrigen(METramite tramite, GeoSITMContext contexto)
            : base(Convert.ToInt32(Entradas.Parcela), tramite, contexto)
        {
            long idComponente = Convert.ToInt64(Contexto.ParametrosGenerales.Single(pg => pg.Clave == PARAM_ID_COMPONENTE_PARCELA).Valor);
            var componente = Contexto.Componente.Find(idComponente);

            tabla = componente.TablaGrafica ?? componente.TablaTemporal;
            camposTabla = Contexto.CreateSQLQueryBuilder()
                                  .GetTableFields(ConfigurationManager.AppSettings["DATABASE"], tabla)
                                  .Select(c => new KeyValuePair<Atributo, object>(new Atributo() { Campo = c }, c))
                                  .ToList();
        }
        protected override void ExecuteEntrada(METramiteEntrada entrada)
        {
            long idParcelaOrigen = (from relHija in Contexto.TramitesEntradasRelacion
                                    join entradaPadre in Contexto.TramitesEntradas on relHija.IdTramiteEntradaPadre equals entradaPadre.IdTramiteEntrada
                                    where relHija.IdTramiteEntrada == entrada.IdTramiteEntrada
                                    select entradaPadre.IdObjeto).SingleOrDefault() ?? 0;

            var filtros = new Dictionary<string, object>() { { campoId, idParcelaOrigen } };
            //CopyToTemporal(tabla, filtros, camposTabla);

            var camposFinales = camposTabla
                                    //elimino el campo featid del insert para que me genere uno nuevo y el id_parcela para reemplazarlo con el de la parcela destino
                                    .Where(kvp => string.Compare(kvp.Key.Campo, campoFeatId, true) != 0 && string.Compare(kvp.Key.Campo, campoId, true) != 0);

            camposFinales = camposFinales.Append(new KeyValuePair<Atributo, object>(new Atributo() { Campo = campoId }, entrada.IdObjeto));

            CopyToTemporal(tabla, filtros, camposFinales.ToList());
        }
        protected override IQueryable<METramiteEntrada> GetEntradas(int idEntrada)
        { //recupero todas las entradas de tipo parcela, que sean Destino (existen como hijas de ninguna otra entrada objeto)
            return from entrada in base.GetEntradas(idEntrada)
                   join relacion in Contexto.TramitesEntradasRelacion on entrada.IdTramiteEntrada equals relacion.IdTramiteEntrada
                   group entrada by entrada.IdObjeto into grp
                   select grp.FirstOrDefault();
        }
    }
}
