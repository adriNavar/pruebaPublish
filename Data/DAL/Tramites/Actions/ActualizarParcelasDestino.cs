using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System;
using System.Linq;
using GeoSit.Data.DAL.Repositories;
using GeoSit.Data.BusinessEntities.MapasTematicos;

namespace GeoSit.Data.DAL.Tramites.Actions
{
    class ActualizarParcelasDestino : AccionEntrada
    {
        private MensuraTemporal _nuevaMensura;
        public ActualizarParcelasDestino(METramite tramite, GeoSITMContext contexto)
            : base(Convert.ToInt32(Entradas.Parcela), tramite, contexto) { }

        public override bool Execute()
        {
            _nuevaMensura = Contexto.ChangeTracker.Entries<MensuraTemporal>().Single().Entity;
            return base.Execute();
        }
        protected override void ExecuteEntrada(METramiteEntrada entrada)
        {
            int idEntradaParcela = Convert.ToInt32(Entradas.Parcela);
            var relaciones = (from relHija in Contexto.TramitesEntradasRelacion
                              join entradaPadre in Contexto.TramitesEntradas on relHija.IdTramiteEntradaPadre equals entradaPadre.IdTramiteEntrada
                              join moePadre in Contexto.ObjetosEntrada on entradaPadre.IdObjetoEntrada equals moePadre.IdObjetoEntrada
                              join relAbuelo in Contexto.TramitesEntradasRelacion on relHija.IdTramiteEntradaPadre equals relAbuelo.IdTramiteEntrada into ljRelAbuelo
                              from relAbuelo in ljRelAbuelo.DefaultIfEmpty()
                              join entradaAbuelo in Contexto.TramitesEntradas on relAbuelo.IdTramiteEntradaPadre equals entradaAbuelo.IdTramiteEntrada into ljEntradaAbuelo
                              from entradaAbuelo in ljEntradaAbuelo.DefaultIfEmpty()
                              where relHija.IdTramiteEntrada == entrada.IdTramiteEntrada
                              select new
                              {
                                  esHijaParcela = moePadre.IdEntrada == idEntradaParcela,
                                  entradaPadre,
                                  entradaAbuelo
                              }).ToArray();

            long? idDivision;
            if (relaciones.First().esHijaParcela)
            {
                var parcelaOrigen = Contexto.ParcelasTemporal.Find(relaciones.First().entradaPadre.IdObjeto.Value, entrada.IdTramite);
                idDivision = parcelaOrigen.FeatIdDivision;
            }
            else
            { // la parcela destino depende de una manzana, nueva o no, 
              // por lo que la relacion directa es la manzana y no la parcela origen.
                var parcelaOrigen = Contexto.ParcelasTemporal.Find(relaciones.First().entradaAbuelo.IdObjeto.Value, entrada.IdTramite);
                idDivision = relaciones.First().entradaPadre.IdObjeto;
            }

            var parcela = Contexto.ParcelasTemporal.Find(entrada.IdObjeto.Value, entrada.IdTramite);
            parcela.FeatIdDivision = idDivision;
            parcela.ExpedienteAlta = Tramite.Numero;
            parcela.FechaAltaExpediente = Tramite.FechaModif.Date;

            parcela.AtributosCrear("0", "0", string.Empty, parcela.ClaseParcelaID == Convert.ToInt64(ClasesParcelas.PropiedadHorizontal), string.Empty);

            Contexto.ParcelaMensurasTemporal.Add(new ParcelaMensuraTemporal()
            {
                IdParcela = parcela.ParcelaID,
                Mensura = _nuevaMensura,

                FechaAlta = _nuevaMensura.FechaAlta,
                FechaModif = _nuevaMensura.FechaAlta,
                IdUsuarioAlta = _nuevaMensura.IdUsuarioAlta,
                IdUsuarioModif = _nuevaMensura.IdUsuarioAlta
            });

            /*no es prescripcion y no es proyecto y no es subparcela de cementerios privados*/
            if (parcela.ClaseParcelaID != Convert.ToInt64(ClasesParcelas.Prescripcion) &&
                parcela.ClaseParcelaID != Convert.ToInt64(ClasesParcelas.Proyecto) &&
                (parcela.ClaseParcelaID != Convert.ToInt64(ClasesParcelas.SubParcela) ||
                  !(Tramite.IdObjetoTramite == Convert.ToInt32(ObjetosTramites.MensuraYDivisionRegimenConjuntoInmobiliarioCementerioPrivado) ||
                    Tramite.IdObjetoTramite == Convert.ToInt32(ObjetosTramites.DivisionRegimenConjuntoInmobiliarioCementerioPrivadoMensuraRegistrada))))
            {
                using (var builder = Contexto.CreateSQLQueryBuilder())
                {
                    string wkt = builder.AddTable("temporal", "inm_parcela_grafica", "pg")
                                        .AddFilter("id_parcela", entrada.IdObjeto.Value, Common.Enums.SQLOperators.EqualsTo)
                                        .AddFilter("id_tramite", entrada.IdTramite, Common.Enums.SQLOperators.EqualsTo, Common.Enums.SQLConnectors.And)
                                        .AddFilter("fecha_baja", null, Common.Enums.SQLOperators.IsNull, Common.Enums.SQLConnectors.And)
                                        .AddGeometryField(builder.CreateGeometryFieldBuilder(new Atributo() { Campo = "geometry" }, "pg").ToWKT(), "geomwkt")
                                        .MaxResults(1)
                                        .ExecuteQuery((reader, status) =>
                                        {
                                            return reader.GetString(reader.GetOrdinal("geomwkt"));
                                        })
                                        .SingleOrDefault();

                    Contexto.NomenclaturasTemporal.Add(new NomenclaturaTemporal()
                    {
                        FechaAlta = Tramite.FechaModif,
                        FechaModificacion = Tramite.FechaModif,
                        UsuarioAltaID = Tramite.UsuarioModif,
                        UsuarioModificacionID = Tramite.UsuarioModif,

                        Nombre = new MesaEntradasRepository(Contexto).GenerarNomenclaturaParcela(wkt),
                        TipoNomenclaturaID = 3, //CATASTRO
                        ParcelaID = entrada.IdObjeto.Value,
                        IdTramite = entrada.IdTramite
                    });
                }
            }
            foreach (var relacion in relaciones)
            {
                Contexto.ParcelasOperacionesTemporal.Add(new ParcelaOperacionTemporal()
                {
                    ParcelaOrigenID = (relacion.esHijaParcela ? relacion.entradaPadre : relacion.entradaAbuelo).IdObjeto.Value,
                    ParcelaDestinoID = parcela.ParcelaID,
                    TipoOperacionID = ObtenerTipoParcelaOperacion(),
                    FechaOperacion = Tramite.FechaModif,
                    IdTramite = Tramite.IdTramite,

                    FechaAlta = Tramite.FechaModif,
                    FechaModificacion = Tramite.FechaModif,
                    UsuarioAltaID = Tramite.UsuarioModif,
                    UsuarioModificacionID = Tramite.UsuarioModif
                });
            }
            return;
        }

        private long ObtenerTipoParcelaOperacion()
        {
            if (new[] { Convert.ToInt64(TiposMensuras.MensuraYDivision),
                        Convert.ToInt64(TiposMensuras.MensuraDivisionMensuraRegistrada)
                      }.Contains(_nuevaMensura.IdTipoMensura))
            {
                return Convert.ToInt64(TiposParcelaOperacion.Subdivision);
            }
            else if (new[] { Convert.ToInt64(TiposMensuras.MensuraYUnificacion),
                             Convert.ToInt64(TiposMensuras.MensuraUnificacionMensuraRegistrada)
                           }.Contains(_nuevaMensura.IdTipoMensura))
            {
                return Convert.ToInt64(TiposParcelaOperacion.Unificacion);
            }
            else if (new[] { Convert.ToInt64(TiposMensuras.MensuraYDeslinde),
                             Convert.ToInt64(TiposMensuras.MensuraUnificacionYRedistribucionParcelaria),
                             Convert.ToInt64(TiposMensuras.MensuraUnificacionYRedistribucionParcelariaMensuraRegistrada)
                           }.Contains(_nuevaMensura.IdTipoMensura))
            {
                return Convert.ToInt64(TiposParcelaOperacion.Redistribucion);
            }
            else if (new[] { Convert.ToInt64(TiposMensuras.Mensura),
                             Convert.ToInt64(TiposMensuras.MensuraParcelaPlanoRegistrado)
                           }.Contains(_nuevaMensura.IdTipoMensura))
            {
                return Convert.ToInt64(TiposParcelaOperacion.Creacion);
            }
            else if (new[] { Convert.ToInt64(TiposMensuras.MensuraPrescripcionAdquisitiva),
                             Convert.ToInt64(TiposMensuras.MensuraPrescripcionAdquisitivaYDivision),
                             Convert.ToInt64(TiposMensuras.MensuraDivisionPrescripcionAdquisitivaMensuraRegistrada)
                           }.Contains(_nuevaMensura.IdTipoMensura))
            {
                return Convert.ToInt64(TiposParcelaOperacion.PrescripcionAdquisitiva);
            }
            else if (new[] { Convert.ToInt64(TiposMensuras.MensuraYProyectoUnificacion),
                             Convert.ToInt64(TiposMensuras.MensuraProyectoUnificacionMensuraRegistrada),
                             Convert.ToInt64(TiposMensuras.MensuraProyectoUnificacionYRedistribucionParcelaria),
                             Convert.ToInt64(TiposMensuras.MensuraProyectoUnificacionYRedistribucionParcelariaMensuraRegistrada)
                           }.Contains(_nuevaMensura.IdTipoMensura))
            {
                return Convert.ToInt64(TiposParcelaOperacion.Proyecto);
            }
            else if (new[] { Convert.ToInt64(TiposMensuras.MensuraDivisionEdificacionExistenteRegimenPHMensuraRegistrada),
                             Convert.ToInt64(TiposMensuras.MensuraModificacionEdificacionExistenteRegimenPHMensuraRegistrada),
                             Convert.ToInt64(TiposMensuras.MensuraDivisionEdificacionExistenteAConstruirRegimenPHMensuraRegistrada),
                             Convert.ToInt64(TiposMensuras.MensuraDivisionEdificacionAConstruirRegimenPreHorizontalidadMensuraRegistrada)
                           }.Contains(_nuevaMensura.IdTipoMensura))
            {
                return Convert.ToInt64(TiposParcelaOperacion.PropiedadHorizontal);
            }
            else if (new[] { Convert.ToInt64(TiposMensuras.MensuraYDivisionRegimenConjuntoInmobiliario),
                             Convert.ToInt64(TiposMensuras.MensuraDivisionConjuntosInmobiliariosMensuraRegistrada),
                             Convert.ToInt64(TiposMensuras.MensuraYDivisionRegimenConjuntoInmobiliarioCementerioPrivado),
                             Convert.ToInt64(TiposMensuras.MensuraDivisionConjuntoInmobiliarioCementerioPrivadoMensuraRegistrada)
                           }.Contains(_nuevaMensura.IdTipoMensura))
            {
                return Convert.ToInt64(TiposParcelaOperacion.ConjuntoInmobiliario);
            }
            else if (Convert.ToInt64(TiposMensuras.MensuraFraccionExtensionMayorRegimenDerechoRealSuperficie) == _nuevaMensura.IdTipoMensura)
            {
                return Convert.ToInt64(TiposParcelaOperacion.DerechoRealSuperficie);
            }
            else
            {
                return Convert.ToInt64(TiposParcelaOperacion.Otros);
            }
        }

        protected override IQueryable<METramiteEntrada> GetEntradas(int idEntrada)
        {
            return from entrada in base.GetEntradas(idEntrada)
                   join relacion in Contexto.TramitesEntradasRelacion on entrada.IdTramiteEntrada equals relacion.IdTramiteEntrada
                   group entrada by entrada.IdObjeto into grp
                   select grp.FirstOrDefault();
        }
    }
}
