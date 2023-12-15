using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using System;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Tramites.Actions
{
    class EliminarParcelasOrigen : AccionEntrada
    {
        private readonly long[] _clasesParcelasDestinoEliminanOrigen;
        public EliminarParcelasOrigen(METramite tramite, GeoSITMContext contexto)
            : base(Convert.ToInt32(Entradas.Parcela), tramite, contexto)
        {
            //clases de parcelas destino que provocan la baja de una parcela origen (REGISTRADA, MUNICIPAL, PROPIEDAD HORIZONTAL, CONJUNTO INMOBILIARIO
            _clasesParcelasDestinoEliminanOrigen = new[]
            {
                Convert.ToInt64(ClasesParcelas.Registrada),
                Convert.ToInt64(ClasesParcelas.Municipal),
                Convert.ToInt64(ClasesParcelas.PropiedadHorizontal),
                Convert.ToInt64(ClasesParcelas.ConjuntoInmobiliario)
            };
        }

        protected override void ExecuteEntrada(METramiteEntrada entrada)
        {
            #region Baja de Parcela
            var parOrigen = Contexto.ParcelasTemporal.Find(entrada.IdObjeto.GetValueOrDefault(), entrada.IdTramite);
            parOrigen.ExpedienteBaja = Tramite.Numero;
            parOrigen.FechaBajaExpediente = Tramite.FechaModif.Date;

            parOrigen.UsuarioModificacionID = Tramite.UsuarioModif;
            parOrigen.UsuarioBajaID = parOrigen.UsuarioModificacionID;
            parOrigen.FechaModificacion = Tramite.FechaModif;
            parOrigen.FechaBaja = parOrigen.FechaModificacion;

            var cmpParcela = Contexto.Componente
                                     .Include(x => x.Atributos)
                                     .Single(x => x.ComponenteId == entrada.IdComponente);

            using (var qbuilder = Contexto.CreateSQLQueryBuilder())
            {
                qbuilder.AddTable("TEMPORAL", cmpParcela.TablaGrafica ?? cmpParcela.Tabla, null)
                        .AddFilter(new Atributo() { Campo = cmpParcela.Atributos.GetAtributoClave().Campo }, entrada.IdObjeto, Common.Enums.SQLOperators.EqualsTo)
                        .AddFilter(new Atributo() { Campo = "ID_TRAMITE" }, entrada.IdTramite, Common.Enums.SQLOperators.EqualsTo, Common.Enums.SQLConnectors.And)
                        .AddFieldsToUpdate(new[]
                        {
                            new KeyValuePair<Atributo, object>(new Atributo { Campo = "ID_USU_MODIF" }, Tramite.UsuarioModif),
                            new KeyValuePair<Atributo, object>(new Atributo { Campo = "ID_USU_BAJA" }, Tramite.UsuarioModif),
                            new KeyValuePair<Atributo, object>(new Atributo { Campo = "FECHA_MODIF", TipoDatoId = 666 }, null),
                            new KeyValuePair<Atributo, object>(new Atributo { Campo = "FECHA_BAJA", TipoDatoId = 666 }, null)
                        })
                        .ExecuteUpdate();
            }
            #endregion

            #region Baja de UTs de Parcela
            var queryUTs = from ut in Contexto.UnidadesTributariasTemporal
                           where ut.ParcelaID == parOrigen.ParcelaID && ut.IdTramite == parOrigen.IdTramite
                           select ut;

            foreach (var ut in queryUTs)
            {
                ut.UsuarioBajaID = ut.UsuarioModificacionID = parOrigen.UsuarioModificacionID;
                ut.FechaBaja = ut.FechaModificacion = parOrigen.FechaModificacion;
            }
            #endregion
        }

        protected override IQueryable<METramiteEntrada> GetEntradas(int idEntrada)
        { //recupero todas las entradas de tipo parcela, que sean Origen (no existen como hijas de ninguna otra entrada objeto)
          //y que tengan al menos una parcela hija que sea de tipo 

            return from entrada in base.GetEntradas(idEntrada)
                   where !Contexto.TramitesEntradasRelacion.Any(rel => rel.IdTramiteEntrada == entrada.IdTramiteEntrada) &&
                         (from relacion in Contexto.TramitesEntradasRelacion
                          join mte in Contexto.TramitesEntradas on relacion.IdTramiteEntrada equals mte.IdTramiteEntrada
                          join moe in Contexto.ObjetosEntrada on mte.IdObjetoEntrada equals moe.IdObjetoEntrada
                          join parcelaHija in Contexto.ParcelasTemporal on mte.IdObjeto equals parcelaHija.ParcelaID
                          where moe.IdEntrada == idEntrada && mte.IdTramite == entrada.IdTramite && _clasesParcelasDestinoEliminanOrigen.Contains(parcelaHija.ClaseParcelaID)
                          select relacion).Any(rel => rel.IdTramiteEntradaPadre == entrada.IdTramiteEntrada)
                   select entrada;

        }
    }
}
