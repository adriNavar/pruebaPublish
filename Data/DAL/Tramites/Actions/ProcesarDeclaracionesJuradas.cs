using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System;
using System.Data.Entity;
using System.Linq;
using Geosit.Data.DAL.DDJJyValuaciones.Enums;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Linq.Expressions;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;

namespace GeoSit.Data.DAL.Tramites.Actions
{
    class ProcesarDeclaracionesJuradas : AccionEntrada
    {
        private MensuraTemporal _nuevaMensura;
        private Dictionary<long, List<DDJJTemporal>> _ddjjMejorasUTValuables;
        private Dictionary<UnidadTributariaTemporal, DDJJTemporal> _utValuables;
        public ProcesarDeclaracionesJuradas(METramite tramite, GeoSITMContext contexto)
            : base(Convert.ToInt32(Entradas.UnidadTributaria), tramite, contexto)
        {
            _ddjjMejorasUTValuables = new Dictionary<long, List<DDJJTemporal>>();
            _utValuables = new Dictionary<UnidadTributariaTemporal, DDJJTemporal>();
        }

        public override bool Execute()
        {
            _nuevaMensura = Contexto.ChangeTracker.Entries<MensuraTemporal>().Single().Entity;
            if (!base.Execute()) return false;
            try
            {
                foreach (var kvp in _utValuables)
                {
                    var ut = kvp.Key;
                    var parcela = Contexto.ChangeTracker
                                      .Entries<ParcelaTemporal>()
                                      .SingleOrDefault(e => e.Entity.ParcelaID == ut.ParcelaID)
                                      ?.Entity ?? Contexto.ParcelasTemporal.Single(pt => pt.ParcelaID == ut.ParcelaID && pt.IdTramite == ut.IdTramite);

                    GenerarValuacion(kvp.Value, ut, parcela.ClaseParcelaID);

                    bool esAfectacionPH = parcela.ClaseParcelaID == Convert.ToInt64(ClasesParcelas.PropiedadHorizontal);

                    #region Actualización de Estado Parcela
                    if (_ddjjMejorasUTValuables[ut.UnidadTributariaId].Any() || ut.TipoUnidadTributariaID == Convert.ToInt64(TipoUnidadTributariaEnum.PropiedaHorizontal))
                    {
                        parcela.EstadoParcelaID = Convert.ToInt64(EstadosParcelas.Edificado);
                    }
                    #endregion
                }
                Contexto.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Resultado = ResultadoValidacion.Error;
                Errores = new List<string>() { { ex.Message } };
                return false;
            }
        }
        protected override void ExecuteEntrada(METramiteEntrada entrada)
        {
            Expression<Func<DDJJTemporal, bool>> ddjjFilter = d => d.IdUnidadTributaria == entrada.IdObjeto && d.IdTramite == entrada.IdTramite;

            var ut = Contexto.ChangeTracker
                             .Entries<UnidadTributariaTemporal>()
                             .SingleOrDefault(e => e.Entity.UnidadTributariaId == entrada.IdObjeto)
                             ?.Entity ?? Contexto.UnidadesTributariasTemporal.Single(utt => utt.UnidadTributariaId == entrada.IdObjeto && utt.IdTramite == entrada.IdTramite);

            var ddjjsUTTramite = Contexto.DeclaracionesJuradasTemporal.Where(ddjjFilter);

            bool esUTExistente = Contexto.UnidadesTributarias.Any(x => x.UnidadTributariaId == ut.UnidadTributariaId);
            LoadDDJJMejoras(ut, ddjjsUTTramite, esUTExistente);
            if (Convert.ToInt64(TipoUnidadTributariaEnum.UnidadFuncionalPH) != ut.TipoUnidadTributariaID)
            {
                DDJJTemporal ddjjTierra;
                try
                {
                    ddjjTierra = LoadDDJJTierra(ut, ddjjsUTTramite, !esUTExistente);
                }
                catch (InvalidOperationException ex)
                {
                    Contexto.GetLogger().LogError($"ProcesarDeclaracionesJuradas-DDJJTierra (ut: {ut.UnidadTributariaId}", ex);
                    throw;
                }
                if (esUTExistente)
                {/*Copio valuación vigente para setearle vigencia_hasta*/
                    var valuacionVigente = Contexto.VALValuacion
                                                   .OrderByDescending(x => x.FechaAlta)
                                                   .FirstOrDefault(x => x.IdUnidadTributaria == ut.UnidadTributariaId && x.FechaBaja == null && x.FechaHasta == null);
                    CopyToTemporal("VAL_VALUACION", new Dictionary<string, object>() { { "ID_VALUACION", valuacionVigente.IdValuacion } });
                }
                #region Designaciones
                #region Baja de Designaciones Actuales
                var queryDesignaciones = from designacion in Contexto.DesignacionesTemporal
                                         where designacion.IdParcela == ut.ParcelaID && designacion.IdTramite == Tramite.IdTramite
                                         select designacion;

                foreach (var designacion in queryDesignaciones)
                {
                    designacion.IdUsuarioBaja = designacion.IdUsuarioModif = Tramite.UsuarioModif;
                    designacion.FechaBaja = designacion.FechaModif = Tramite.FechaModif;
                }
                #endregion

                #region Alta de Designaciones
                Contexto.Entry(ddjjTierra).Reference(x => x.Designacion).Load();
                if (ddjjTierra.Designacion != null)
                {
                    Contexto.DesignacionesTemporal.Add(new DesignacionTemporal()
                    {
                        Barrio = ddjjTierra.Designacion.Barrio,
                        Calle = ddjjTierra.Designacion.Calle,
                        Chacra = ddjjTierra.Designacion.Chacra,
                        CodigoPostal = ddjjTierra.Designacion.CodigoPostal,
                        Departamento = ddjjTierra.Designacion.Departamento,
                        Fraccion = ddjjTierra.Designacion.Fraccion,
                        IdBarrio = ddjjTierra.Designacion.IdBarrio,
                        IdCalle = ddjjTierra.Designacion.IdCalle,
                        IdDepartamento = ddjjTierra.Designacion.IdDepartamento,
                        IdLocalidad = ddjjTierra.Designacion.IdLocalidad,
                        IdManzana = ddjjTierra.Designacion.IdManzana,
                        IdParaje = ddjjTierra.Designacion.IdParaje,
                        IdParcela = ut.ParcelaID.Value,
                        IdSeccion = ddjjTierra.Designacion.IdSeccion,
                        IdTipoDesignador = (short)ddjjTierra.Designacion.IdTipoDesignador,
                        Localidad = ddjjTierra.Designacion.Localidad,
                        Lote = ddjjTierra.Designacion.Lote,
                        Manzana = ddjjTierra.Designacion.Manzana,
                        Numero = ddjjTierra.Designacion.Numero,
                        Paraje = ddjjTierra.Designacion.Paraje,
                        Quinta = ddjjTierra.Designacion.Quinta,
                        Seccion = ddjjTierra.Designacion.Seccion,

                        IdTramite = Tramite.IdTramite,
                        FechaAlta = Tramite.FechaModif,
                        FechaModif = Tramite.FechaModif,
                        IdUsuarioAlta = Tramite.UsuarioModif,
                        IdUsuarioModif = Tramite.UsuarioModif
                    });
                }
                #endregion
                #endregion

                #region Dominios
                #region Baja de Dominios y Titulares Actuales
                var queryDominios = Contexto.DominiosTemporal
                                                    .Include(d => d.Titulares)
                                                    .Where(d => d.IdTramite == Tramite.IdTramite && d.UnidadTributariaID == ut.UnidadTributariaId);

                foreach (var dominio in queryDominios)
                {
                    foreach (var titular in dominio.Titulares)
                    {
                        titular.UsuarioBajaID = titular.UsuarioModificacionID = Tramite.UsuarioModif;
                        titular.FechaBaja = titular.FechaModificacion = Tramite.FechaModif;
                    }
                    dominio.IdUsuarioBaja = dominio.IdUsuarioModif = Tramite.UsuarioModif;
                    dominio.FechaBaja = dominio.FechaModif = Tramite.FechaModif;
                }
                #endregion

                #region Alta de Dominios y Titulares
                Contexto.Entry(ddjjTierra).Collection(x => x.Dominios).Query().Include(d => d.Titulares).Load();
                foreach (var ddjjDominio in ddjjTierra.Dominios ?? new DDJJDominioTemporal[0])
                {
                    Contexto.DominiosTemporal.Add(new DominioTemporal()
                    {
                        IdTramite = Tramite.IdTramite,
                        Fecha = ddjjDominio.Fecha,
                        Inscripcion = ddjjDominio.Inscripcion,
                        TipoInscripcionID = ddjjDominio.IdTipoInscripcion,
                        UnidadTributariaID = ut.UnidadTributariaId,
                        Titulares = ddjjDominio.Titulares.Select(t => new DominioTitularTemporal()
                        {
                            PersonaID = t.IdPersona,
                            PorcientoCopropiedad = t.PorcientoCopropiedad,
                            TipoTitularidadID = t.IdTipoTitularidad,

                            FechaAlta = Tramite.FechaModif,
                            FechaModificacion = Tramite.FechaModif,
                            UsuarioAltaID = Tramite.UsuarioModif,
                            UsuarioModificacionID = Tramite.UsuarioModif
                        }).ToList(),

                        FechaAlta = Tramite.FechaModif,
                        FechaModif = Tramite.FechaModif,
                        IdUsuarioAlta = Tramite.UsuarioModif,
                        IdUsuarioModif = Tramite.UsuarioModif
                    });
                }
                #endregion
                #endregion

                #region Agrego UT si valúa
                if (!_utValuables.ContainsKey(ut))
                {
                    _utValuables.Add(ut, ddjjTierra);
                }
                #endregion
            }
            return;
        }

        private void LoadDDJJMejoras(UnidadTributariaTemporal ut, IQueryable<DDJJTemporal> ddjjTemporales, bool includeCurrent)
        {
            var _versionesMejora = new long[] { Convert.ToInt64(VersionesDDJJ.E1), Convert.ToInt64(VersionesDDJJ.E2) };

            var ddjjMejoras = ddjjTemporales.Include(x => x.Mejora.MejorasCar.Select(m => m.Caracteristica))
                                            .Include(x => x.Mejora.OtrasCar.Select(m => m.OtraCar))
                                            .Where(d => _versionesMejora.Contains(d.IdVersion));
            if (includeCurrent)
            {
                var ddjjMejorasActuales = Contexto.DDJJ.AsNoTracking()
                                      .Where(d => d.IdUnidadTributaria == ut.UnidadTributariaId &&
                                                 _versionesMejora.Contains(d.IdVersion) &&
                                                 d.FechaBaja == null)
                                      .Include(d => d.Mejora.Select(m => m.MejorasCar.Select(mc => mc.Caracteristica)))
                                      .Include(d => d.Mejora.Select(m => m.OtrasCar.Select(mc => mc.OtraCar)))
                                      .OrderByDescending(d => d.FechaAlta)
                                      .ToList();

                ddjjMejoras = ddjjMejorasActuales.Select(MapFromMejoraVigente).Concat(ddjjMejoras).AsQueryable();
            }
            _ddjjMejorasUTValuables.Add(ut.UnidadTributariaId, ddjjMejoras.ToList());
        }

        private DDJJTemporal LoadDDJJTierra(UnidadTributariaTemporal ut, IQueryable<DDJJTemporal> ddjjTemporales, bool fallbackCurrent)
        {
            var _versionesTierra = new long[] { Convert.ToInt64(VersionesDDJJ.U), Convert.ToInt64(VersionesDDJJ.SoR) };

            var ddjj = ddjjTemporales.SingleOrDefault(d => _versionesTierra.Contains(d.IdVersion));
            if (ddjj == null && !fallbackCurrent)
            {
                throw new InvalidOperationException();
            }
            if (ddjj == null)
            {
                var vigentes = from vigente in (from aux in Contexto.DDJJ
                                                where aux.IdUnidadTributaria == ut.UnidadTributariaId &&
                                                      aux.FechaBaja == null &&
                                                      _versionesTierra.Contains(aux.IdVersion)
                                                orderby aux.FechaVigencia descending
                                                select aux)
                               select vigente;
                ddjj = MapFromTierraVigente(vigentes.First());
            }
            else if (Convert.ToInt64(VersionesDDJJ.U) == ddjj.IdVersion)
            {
                Contexto.Entry(ddjj).Reference(x => x.U).Query()
                                    .Include(u => u.Fracciones.Select(f => f.MedidasLineales.Select(ml => ml.ClaseParcelaMedidaLineal)))
                                    .Load();
                ddjj.U.Mensura = _nuevaMensura;
            }
            else
            {
                Contexto.Entry(ddjj).Reference(x => x.Sor).Query()
                                    .Include(sor => sor.SoRCars.Select(s => s.AptCar.Aptitud))
                                    .Include(sor => sor.SoRCars.Select(s => s.AptCar.SorCaracteristica))
                                    .Include(sor => sor.Superficies.Select(s => s.Aptitud))
                                    .Load();

                ddjj.Sor.Mensura = _nuevaMensura;
            }
            return ddjj;
        }

        protected override IQueryable<METramiteEntrada> GetEntradas(int idEntrada)
        {
            return from entrada in base.GetEntradas(idEntrada)
                   join ddjj in Contexto.DeclaracionesJuradasTemporal on entrada.IdObjeto equals ddjj.IdUnidadTributaria
                   join ut in Contexto.UnidadesTributariasTemporal on ddjj.IdUnidadTributaria equals ut.UnidadTributariaId
                   orderby ut.ParcelaID, ut.TipoUnidadTributariaID
                   where ddjj.IdTramite == entrada.IdTramite
                   group entrada by entrada.IdObjeto into grp
                   select grp.FirstOrDefault();
        }

        protected bool GenerarValuacion(DDJJTemporal ddjj, UnidadTributariaTemporal ut, long idClaseParcela)
        {
            try
            {
                var fechaVigencia = ddjj.FechaVigencia.GetValueOrDefault().Date.AddDays(1).AddMilliseconds(-1);

                var decretos = Contexto.ValDecretos
                                       .Include(x => x.Jurisdiccion)
                                       .Include(x => x.Zona)
                                       .Where(x => !x.IdUsuarioBaja.HasValue && x.FechaInicio <= fechaVigencia && ((!x.FechaFin.HasValue) || (x.FechaFin >= fechaVigencia)) &&
                                                    x.Zona.Any(y => y.IdTipoParcela == ut.Parcela.TipoParcelaID && !y.IdUsuarioBaja.HasValue) &&
                                                    x.Jurisdiccion.Any(y => y.IdJurisdiccion == ut.JurisdiccionID && !y.IdUsuarioBaja.HasValue))
                                       .ToList();

                #region Genero valuación para unidad ph de parcela de tipo conjunto inmobiliario
                if ((TipoUnidadTributariaEnum)ut.TipoUnidadTributariaID == TipoUnidadTributariaEnum.PropiedaHorizontal && long.TryParse(ClasesParcelas.ConjuntoInmobiliario, out long idClaseConjuntoInmobiliario) && idClaseConjuntoInmobiliario == idClaseParcela)
                {
                    foreach (var valuacion in ObtenerValuacionConjuntoInmobiliario(ddjj, ut, decretos))
                    {
                        TerminarValuacionesVigentes(valuacion);
                    }
                }
                #endregion
                #region Genero valuación para unidades tributarias tipo 1 o 2 de parcelas
                else if ((TipoUnidadTributariaEnum)ut.TipoUnidadTributariaID == TipoUnidadTributariaEnum.PropiedaHorizontal && long.TryParse(ClasesParcelas.PropiedadHorizontal, out long idClasePropiedadHorizontal) && idClasePropiedadHorizontal == idClaseParcela)
                {
                    foreach (var valuacion in ObtenerValuacionPropiedadHorizontal(ddjj, ut, decretos))
                    {
                        TerminarValuacionesVigentes(valuacion);
                    }
                }
                else if ((TipoUnidadTributariaEnum)ut.TipoUnidadTributariaID != TipoUnidadTributariaEnum.UnidadFuncionalPH)
                {
                    TerminarValuacionesVigentes(ObtenerValuacion(ddjj, ut, decretos));
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                Contexto.GetLogger().LogError($"GenerarValuacion({ddjj.IdDeclaracionJurada})", ex);
                throw;
            }
        }

        private DDJJTemporal MapFromTierraVigente(DDJJ ddjjTierra)
        {
            return new DDJJTemporal()
            {
                IdDeclaracionJurada = ddjjTierra.IdDeclaracionJurada,
                IdUnidadTributaria = ddjjTierra.IdUnidadTributaria,
                FechaModif = ddjjTierra.FechaModif,
                IdVersion = ddjjTierra.IdVersion,
                U = LoadU(ddjjTierra),
                Sor = LoadSoR(ddjjTierra)
            };
        }

        private DDJJTemporal MapFromMejoraVigente(DDJJ ddjjMejora)
        {
            return new DDJJTemporal()
            {
                IdDeclaracionJurada = ddjjMejora.IdDeclaracionJurada,
                IdUnidadTributaria = ddjjMejora.IdUnidadTributaria,
                FechaModif = ddjjMejora.FechaModif,
                IdVersion = ddjjMejora.IdVersion,
                Mejora = ddjjMejora.Mejora.Select(m =>
                {
                    return new INMMejoraTemporal()
                    {
                        IdDestinoMejora = m.IdDestinoMejora,
                        IdEstadoConservacion = m.IdEstadoConservacion,
                        IdMejora = m.IdMejora,
                        FechaAlta = m.FechaAlta,
                        FechaModif = m.FechaModif,
                        MejorasCar = m.MejorasCar.Select(mc =>
                        {
                            return new INMMejoraCaracteristicaTemporal()
                            {
                                IdCaracteristica = mc.IdCaracteristica,
                                Caracteristica = mc.Caracteristica
                            };
                        }).ToList(),
                        OtrasCar = m.OtrasCar.Select(oc =>
                        {
                            return new INMMejoraOtraCarTemporal()
                            {
                                IdOtraCar = oc.IdOtraCar,
                                Valor = oc.Valor,
                                OtraCar = oc.OtraCar
                            };
                        }).ToList()
                    };
                }).Single()
            };
        }

        private void TerminarValuacionesVigentes(VALValuacionTemporal valuacion)
        {
            var valuacionesVigentes = Contexto.ValuacionesTemporal
                                              .Where(x => x.IdUnidadTributaria == valuacion.IdUnidadTributaria &&
                                                          x.IdTramite == valuacion.IdTramite &&
                                                         !x.FechaBaja.HasValue && (!x.FechaHasta.HasValue || (x.FechaDesde <= valuacion.FechaDesde && x.FechaHasta.Value > valuacion.FechaDesde)));
            foreach (var v in valuacionesVigentes)
            {
                v.FechaHasta = valuacion.FechaDesde;
                v.FechaModif = valuacion.FechaAlta;
                v.IdUsuarioModif = valuacion.IdUsuarioAlta;
            }
        }

        private DDJJSorTemporal LoadSoR(DDJJ ddjjTierra)
        {
            if (ddjjTierra.IdVersion != Convert.ToInt64(VersionesDDJJ.U)) return null;

            var sor = Contexto.DDJJSor
                              .Include(x => x.SorCar.Select(sc => sc.AptCar.Aptitud))
                              .Include(x => x.SorCar.Select(sc => sc.AptCar.SorCaracteristica))
                              .Include(x => x.Superficies.Select(s => s.Aptitud))
                              .First(x => x.IdDeclaracionJurada == ddjjTierra.IdDeclaracionJurada);

            return MapFromSoRVigente(sor);
        }

        private DDJJUTemporal LoadU(DDJJ ddjjTierra)
        {
            if (ddjjTierra.IdVersion != Convert.ToInt64(VersionesDDJJ.U)) return null;

            var u = Contexto.DDJJU
                            .Include(x => x.Fracciones.Select(z => z.MedidasLineales.Select(ml => ml.ClaseParcelaMedidaLineal)))
                            .First(x => x.IdDeclaracionJurada == ddjjTierra.IdDeclaracionJurada);

            return MapFromUVigente(u);
        }

        private DDJJSorTemporal MapFromSoRVigente(DDJJSor vigente)
        {
            if (vigente == null) return null;

            return new DDJJSorTemporal()
            {
                DistanciaCamino = vigente.DistanciaCamino,
                DistanciaEmbarque = vigente.DistanciaEmbarque,
                DistanciaLocalidad = vigente.DistanciaLocalidad,
                IdCamino = vigente.IdCamino,
                IdLocalidad = vigente.IdLocalidad,
                NumeroHabitantes = vigente.NumeroHabitantes,
                SoRCars = vigente.SorCar
                                 .Select(sc => new DDJJSorCarTemporal()
                                 {
                                     IdAptCar = sc.IdAptCar,
                                     AptCar = sc.AptCar
                                 }).ToList(),
                Superficies = vigente.Superficies
                                     .Select(s => new VALSuperficieTemporal()
                                     {
                                         IdAptitud = s.IdAptitud,
                                         Superficie = s.Superficie,
                                         Aptitud = s.Aptitud
                                     }).ToList()
            };
        }

        private DDJJUTemporal MapFromUVigente(DDJJU vigente)
        {
            if (vigente == null) return null;

            return new DDJJUTemporal()
            {
                AguaCorriente = vigente.AguaCorriente,
                Cloaca = vigente.Cloaca,
                NumeroHabitantes = vigente.NumeroHabitantes,
                SuperficiePlano = vigente.SuperficiePlano,
                SuperficieTitulo = vigente.SuperficieTitulo,
                Fracciones = vigente.Fracciones
                                    .Select(f => new DDJJUFraccionTemporal()
                                    {
                                        IdFraccion = f.IdFraccion,
                                        IdU = f.IdU,
                                        NumeroFraccion = f.NumeroFraccion,
                                        MedidasLineales = f.MedidasLineales
                                                           .Select(ml => new DDJJUMedidaLinealTemporal()
                                                           {
                                                               AlturaCalle = ml.AlturaCalle,
                                                               Calle = ml.Calle,
                                                               IdClaseParcelaMedidaLineal = ml.IdClaseParcelaMedidaLineal,
                                                               IdTramoVia = ml.IdTramoVia,
                                                               IdUFraccion = ml.IdUFraccion.Value,
                                                               IdVia = ml.IdVia,
                                                               NumeroParametro = ml.NumeroParametro,
                                                               ValorAforo = ml.ValorAforo,
                                                               ValorMetros = ml.ValorMetros,
                                                               ClaseParcelaMedidaLineal = ml.ClaseParcelaMedidaLineal
                                                           }).ToList()
                                    }).ToList()
            };
        }

        private VALValuacionTemporal ObtenerValuacion(DDJJTemporal ddjj, UnidadTributariaTemporal ut, IEnumerable<VALDecreto> decretos)
        {
            decimal valorTierra = 0;
            decimal valorMejoras = 0;

            var valuacionTemporal = new VALValuacionTemporal()
            {
                IdTramite = Tramite.IdTramite,
                IdUnidadTributaria = ut.UnidadTributariaId,
                FechaDesde = ddjj.FechaVigencia.Value,

                FechaAlta = Tramite.FechaModif,
                FechaModif = Tramite.FechaModif,
                IdUsuarioModif = Tramite.UsuarioModif,
                IdUsuarioAlta = Tramite.UsuarioModif,
            };

            valorTierra = ObtenerValorTierra(ddjj, (TipoParcelaEnum)ut.Parcela.TipoParcelaID);

            #region Calculo el valor de mejoras
            valorMejoras = ObtenerValorMejoras(ut);
            #endregion

            #region Determino la superficie de la valuacion (superficie de tierra)
            if (ddjj.U != null)
            {
                valuacionTemporal.Superficie = Convert.ToDouble(ddjj.U.SuperficiePlano ?? ddjj.U.SuperficieTitulo ?? 0);
            }
            else
            {
                valuacionTemporal.Superficie = ddjj.Sor.Superficies.Sum(x => x.Superficie) ?? 0;
            }
            #endregion

            #region Aplico coeficientes de decretos de ser necesario
            valuacionTemporal.Decretos = valuacionTemporal.Decretos ?? new List<VALValuacionDecretoTemporal>();
            foreach (var decretoAplicado in ObtenerDecretos(decretos))
            {
                valorTierra *= Convert.ToDecimal(decretoAplicado.Item1.Coeficiente ?? 1);
                valuacionTemporal.Decretos.Add(decretoAplicado.Item2);
            }
            #endregion

            valuacionTemporal.ValorTierra = valorTierra;
            valuacionTemporal.ValorMejoras = valorMejoras;
            valuacionTemporal.ValorTotal = valuacionTemporal.ValorTierra + valuacionTemporal.ValorMejoras.Value;
            ddjj.Valuaciones = new List<VALValuacionTemporal>() { { valuacionTemporal } };

            return valuacionTemporal;
        }

        private IEnumerable<VALValuacionTemporal> ObtenerValuacionConjuntoInmobiliario(DDJJTemporal ddjj, UnidadTributariaTemporal ph, IEnumerable<VALDecreto> decretos)
        {
            var unidadesFuncionales = Contexto.ChangeTracker
                                              .Entries<UnidadTributariaTemporal>()
                                              .Where(e => e.Entity.ParcelaID == ph.ParcelaID && Convert.ToInt64(TipoUnidadTributariaEnum.UnidadFuncionalPH) == e.Entity.TipoUnidadTributariaID)
                                              .Select(e => e.Entity);

            /* la superficie que usa para valuarse la tierra es la sumatoria de las superficies de las UT de tipo UF.
             * la superficie de la PH no se tiene en cuenta para ésto.
             */
            decimal superficieValuablePH = unidadesFuncionales.Sum(x => (decimal)(x.Superficie ?? 0));

            decimal valorTierraCI, valorMejorasCI = 0;

            valorTierraCI = ObtenerValorTierra(ddjj, (TipoParcelaEnum)ph.Parcela.TipoParcelaID);
            valorMejorasCI = ObtenerValorMejoras(ph);

            var valuaciones = new List<VALValuacionTemporal>();
            var valuacionPH = new VALValuacionTemporal()
            {
                IdUnidadTributaria = ph.UnidadTributariaId,
                Superficie = ph.Superficie,
                ValorMejorasPropio = valorMejorasCI,
                ValorMejoras = valorMejorasCI,
                FechaDesde = ddjj.FechaVigencia.Value.Date,
                Decretos = new List<VALValuacionDecretoTemporal>(),

                IdTramite = Tramite.IdTramite,
                FechaAlta = Tramite.FechaModif,
                FechaModif = Tramite.FechaModif,
                IdUsuarioAlta = Tramite.UsuarioModif,
                IdUsuarioModif = Tramite.UsuarioModif,
            };

            foreach (var decretoAplicado in ObtenerDecretos(decretos))
            {
                valorTierraCI *= Convert.ToDecimal(decretoAplicado.Item1.Coeficiente ?? 1);
                valuacionPH.Decretos.Add(decretoAplicado.Item2);
            }
            valuacionPH.ValorTierra = valorTierraCI;
            valuacionPH.ValorTotal = valorTierraCI + valorMejorasCI;

            ddjj.Valuaciones = (ddjj.Valuaciones ?? new List<VALValuacionTemporal>()).Append(valuacionPH).ToList();

            yield return valuacionPH;

            foreach (var uf in unidadesFuncionales)
            {
                /* el porcentaje de incidencia se calcula teniendo en cuenta la superficie de la UF en relación 
                 * con la superficieValuablePH. 
                 * Este porcentaje se utiliza para obtener el valor de la tierra y el valor de las mejoras comunes
                 */
                decimal porcentajeIncidenciaUF = Math.Round((decimal)(uf.Superficie ?? 0) / superficieValuablePH, 4);
                decimal valorTierraUF = Math.Round(valorTierraCI * porcentajeIncidenciaUF, 4);
                decimal valorMejorasPropias = Math.Round(ObtenerValorMejoras(uf), 4);
                decimal valorMejorasTotalUF = Math.Round(valorMejorasCI * porcentajeIncidenciaUF, 4) + valorMejorasPropias;

                //uf.PorcentajeCopropiedad = Math.Round((decimal)(uf.Superficie ?? 0) / (decimal)(ph.Superficie ?? 0), 4) * 100;
                uf.PorcentajeCopropiedad = porcentajeIncidenciaUF * 100;

                var ddjjUF = Contexto.ChangeTracker
                                     .Entries<DDJJTemporal>()
                                     .Where(e => e.Entity.IdUnidadTributaria == uf.UnidadTributariaId)
                                     .Select(e => e.Entity)
                                     .OrderByDescending(a => a.FechaAlta)
                                     .FirstOrDefault() ?? ddjj;

                var valuacionUF = new VALValuacionTemporal()
                {
                    IdUnidadTributaria = uf.UnidadTributariaId,
                    CoefProrrateo = (double)porcentajeIncidenciaUF,
                    ValorTierra = valorTierraUF,
                    ValorMejorasPropio = valorMejorasPropias,
                    ValorMejoras = valorMejorasTotalUF,
                    ValorTotal = valorTierraUF + valorMejorasTotalUF,
                    Superficie = uf.Superficie,
                    FechaDesde = ddjj.FechaVigencia.Value.Date,
                    Decretos = ObtenerDecretos(decretos).Select(d => d.Item2).ToList(),

                    IdTramite = Tramite.IdTramite,
                    FechaAlta = Tramite.FechaModif,
                    FechaModif = Tramite.FechaModif,
                    IdUsuarioAlta = Tramite.UsuarioModif,
                    IdUsuarioModif = Tramite.UsuarioModif,
                };
                ddjjUF.Valuaciones = (ddjjUF.Valuaciones ?? new List<VALValuacionTemporal>()).Append(valuacionUF).ToList();
                yield return valuacionUF;
            }
        }

        private IEnumerable<VALValuacionTemporal> ObtenerValuacionPropiedadHorizontal(DDJJTemporal ddjj, UnidadTributariaTemporal ph, IEnumerable<VALDecreto> decretos)
        {
            var unidadesFuncionales = Contexto.ChangeTracker
                                              .Entries<UnidadTributariaTemporal>()
                                              .Where(e => e.Entity.ParcelaID == ph.ParcelaID && Convert.ToInt64(TipoUnidadTributariaEnum.UnidadFuncionalPH) == e.Entity.TipoUnidadTributariaID)
                                              .Select(e => e.Entity);


            var prorrateos = new Dictionary<long, VALCoeficientesProrrateo>();
            double sup_prorrateada_uf_total = 0;
            foreach (var uf in unidadesFuncionales)
            {
                int.TryParse(uf.Piso, out int piso);
                if (!prorrateos.TryGetValue(piso, out VALCoeficientesProrrateo coeficienteProrrateo))
                {
                    coeficienteProrrateo = Contexto.VALCoeficientesProrrateo.FirstOrDefault(x => x.Piso == piso && !x.IdUsuarioBaja.HasValue);
                    if (coeficienteProrrateo?.Coeficiente != null)
                    {
                        prorrateos.Add(piso, coeficienteProrrateo);
                    }
                }
                sup_prorrateada_uf_total += (coeficienteProrrateo?.Coeficiente ?? 1) * (uf.Superficie ?? 0);
            }

            decimal valorMejorasPH = valorMejorasPH = ObtenerValorMejoras(ph);
            decimal valorTierraPH = ObtenerValorTierra(ddjj, (TipoParcelaEnum)ph.Parcela.TipoParcelaID);

            var valuaciones = new List<VALValuacionTemporal>();
            var valuacionPH = new VALValuacionTemporal()
            {
                IdUnidadTributaria = ph.UnidadTributariaId,
                Superficie = ph.Superficie,
                ValorMejorasPropio = valorMejorasPH,
                FechaDesde = ddjj.FechaVigencia.Value.Date,
                Decretos = new List<VALValuacionDecretoTemporal>(),

                IdTramite = Tramite.IdTramite,
                FechaAlta = Tramite.FechaModif,
                FechaModif = Tramite.FechaModif,
                IdUsuarioAlta = Tramite.UsuarioModif,
                IdUsuarioModif = Tramite.UsuarioModif,
            };

            foreach (var decretoAplicado in ObtenerDecretos(decretos))
            {
                valorTierraPH *= Convert.ToDecimal(decretoAplicado.Item1.Coeficiente ?? 1);
                valuacionPH.Decretos.Add(decretoAplicado.Item2);
            }
            valuacionPH.ValorTierra = valorTierraPH;

            ddjj.Valuaciones = (ddjj.Valuaciones ?? new List<VALValuacionTemporal>()).Append(valuacionPH).ToList();

            var valoresMejorasPropiosUF = new Dictionary<UnidadTributariaTemporal, decimal>();
            var valoresTierraUF = new Dictionary<UnidadTributariaTemporal, decimal>();
            var valoresPropiosUF = new Dictionary<UnidadTributariaTemporal, decimal>();
            var valoresTotalesMejoraUF = new Dictionary<UnidadTributariaTemporal, decimal>();

            foreach (var uf in unidadesFuncionales)
            {
                //calculo el valor de la tierra que le corresponde a la UF según porcentaje de prorrateo de tierra
                //y se lo sumo al valor de la mejora propia para obtener el valor propio de la UF (sin mejoras comunes)
                int.TryParse(uf.Piso, out int piso);
                double coefProrrateo = prorrateos[piso]?.Coeficiente ?? 0;

                decimal valor_tierra_uf = Math.Round(valorTierraPH / (decimal)sup_prorrateada_uf_total * Convert.ToDecimal(coefProrrateo) * (decimal)(uf.Superficie ?? 0), 4);
                valoresTierraUF.Add(uf, valor_tierra_uf);

                decimal valorMejorasPropias = ObtenerValorMejoras(uf);
                valoresMejorasPropiosUF.Add(uf, valorMejorasPropias);
                valoresPropiosUF.Add(uf, valor_tierra_uf + valorMejorasPropias);
            }

            decimal valorTotalPropiosUF = valoresPropiosUF.Sum(e => e.Value);
            foreach (var uf in unidadesFuncionales)
            {
                decimal coeficienteUF = Math.Round(valoresPropiosUF[uf] / valorTotalPropiosUF, 4);
                uf.PorcentajeCopropiedad = coeficienteUF * 100;
                valoresTotalesMejoraUF.Add(uf, valoresMejorasPropiosUF[uf] + Math.Round(valorMejorasPH * coeficienteUF, 2));

                var ddjjUF = Contexto.ChangeTracker
                                     .Entries<DDJJTemporal>()
                                     .Where(e => e.Entity.IdUnidadTributaria == uf.UnidadTributariaId)
                                     .Select(e => e.Entity)
                                     .OrderByDescending(a => a.FechaAlta)
                                     .FirstOrDefault() ?? ddjj;

                var valuacionUF = new VALValuacionTemporal()
                {
                    IdUnidadTributaria = uf.UnidadTributariaId,
                    CoefProrrateo = (double)coeficienteUF,
                    ValorTierra = valoresTierraUF[uf],
                    ValorMejorasPropio = valoresMejorasPropiosUF[uf],
                    ValorMejoras = valoresTotalesMejoraUF[uf],
                    ValorTotal = valoresTierraUF[uf] + valoresTotalesMejoraUF[uf],
                    Superficie = uf.Superficie,
                    FechaDesde = ddjj.FechaVigencia.Value.Date,
                    Decretos = ObtenerDecretos(decretos).Select(d => d.Item2).ToList(),

                    IdTramite = Tramite.IdTramite,
                    FechaAlta = Tramite.FechaModif,
                    FechaModif = Tramite.FechaModif,
                    IdUsuarioAlta = Tramite.UsuarioModif,
                    IdUsuarioModif = Tramite.UsuarioModif,
                };
                ddjjUF.Valuaciones = (ddjjUF.Valuaciones ?? new List<VALValuacionTemporal>()).Append(valuacionUF).ToList();
                yield return valuacionUF;
            }
            valuacionPH.ValorMejoras = (valuacionPH.ValorMejorasPropio ?? 0) + valoresMejorasPropiosUF.Sum(e => e.Value);
            valuacionPH.ValorTotal = valuacionPH.ValorTierra + (valuacionPH.ValorMejoras ?? 0);
            yield return valuacionPH;
        }

        private decimal ObtenerValorTierra(DDJJTemporal ddjj, TipoParcelaEnum tipoParcela)
        {
            decimal valorTierra = 0;
            if (ddjj.U != null)
            {
                double valorTierraUrbana = 0;
                var ddjjU = ddjj.U;
                int idLocalidad = ddjj.Designacion.IdLocalidad ?? 0;

                double superficieParcela = (double?)ddjjU.SuperficiePlano ?? (double?)ddjjU.SuperficieTitulo ?? 0d;
                foreach (var fraccion in ddjjU.Fracciones)
                {
                    switch ((ClasesEnum)fraccion.MedidasLineales.First().ClaseParcelaMedidaLineal.IdClaseParcela)
                    {
                        case ClasesEnum.PARCELA_RECTANGULAR_NO_EN_ESQUINA_HASTA_2000M2: // 1
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                if (AllNotNullAndWithValue(new[] { frente, fondo }, fraccion))
                                {
                                    valorTierraUrbana += ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo.ValorMetros.Value, ObtenerAforo(frente, idLocalidad), superficieParcela);
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_INTERNA_CON_ACCESO_A_PASILLO_HASTA_2000M2: // 2
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);

                                if (AllNotNullAndWithValue(new[] { frente, fondo1, fondo2 }, fraccion))
                                {
                                    double valorAforo = ObtenerAforo(frente, idLocalidad);

                                    double superficie = frente.ValorMetros.Value * fondo1.ValorMetros.Value;
                                    double valor1 = ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo, superficie);

                                    superficie = frente.ValorMetros.Value * fondo2.ValorMetros.Value;
                                    double valor2 = ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo, superficie);

                                    valorTierraUrbana += (valor1 - valor2) / (fondo1.ValorMetros.Value * frente.ValorMetros.Value - fondo2.ValorMetros.Value * frente.ValorMetros.Value) * superficieParcela;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_A_DOS_CALLES_NO_OPUESTAS_HASTA_2000M2: // 3
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var fondoA1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOA1);
                                var fondoB1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOB1);
                                var fondoA2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOA2);
                                var fondoB2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOB2);

                                if (AllNotNullAndWithValue(new[] { frente, frente2, fondoA1, fondoB1, fondoA2, fondoB2 }, fraccion))
                                {
                                    double fondo = (fondoA1.ValorMetros.Value + fondoB1.ValorMetros.Value) / 2;
                                    double superficie = fondo * frente.ValorMetros.Value;
                                    double valor1 = ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo, ObtenerAforo(frente, idLocalidad), superficie);

                                    fondo = (fondoA2.ValorMetros.Value + fondoB2.ValorMetros.Value) / 2;
                                    superficie = fondo * frente2.ValorMetros.Value;
                                    double valor2 = ObtenerValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo, ObtenerAforo(frente2, idLocalidad), superficie);

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_MARTILLO_AL_FRENTE_HASTA_2000M2: // 4
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);
                                var contrafrente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.CONTRAFRENTE);

                                if (AllNotNullAndWithValue(new[] { frente, fondo1, fondo2, contrafrente }, fraccion))
                                {
                                    double valorAforo = ObtenerAforo(frente, idLocalidad);

                                    if (Math.Abs(frente.ValorMetros.Value - contrafrente.ValorMetros.Value) > 4)
                                    {

                                        double superficie = fondo1.ValorMetros.Value * contrafrente.ValorMetros.Value;
                                        double valor1 = ObtenerValuacionUTipoParcela1(contrafrente.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo, superficie);

                                        superficie = fondo2.ValorMetros.Value * frente.ValorMetros.Value;
                                        double valor2 = ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo, superficie);

                                        superficie = fondo2.ValorMetros.Value * contrafrente.ValorMetros.Value;
                                        double valor3 = ObtenerValuacionUTipoParcela1(contrafrente.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo, superficie);

                                        valorTierraUrbana += valor1 + valor2 - valor3;
                                    }
                                    else
                                    {
                                        double superficie = fondo1.ValorMetros.Value * frente.ValorMetros.Value;
                                        valorTierraUrbana += ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo, superficie);
                                    }
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_MARTILLO_AL_FONDO_HASTA_2000M2: // 5
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);
                                var contrafrente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.CONTRAFRENTE);

                                if (AllNotNullAndWithValue(new[] { frente, fondo1, fondo2, contrafrente }, fraccion))
                                {
                                    double superficie = fondo1.ValorMetros.Value * frente.ValorMetros.Value;

                                    double valorAforo = ObtenerAforo(frente, idLocalidad);
                                    double valor1 = ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo, superficie);

                                    superficie = (contrafrente.ValorMetros.Value - frente.ValorMetros.Value) * fondo1.ValorMetros.Value;
                                    double mts = contrafrente.ValorMetros.Value - frente.ValorMetros.Value;
                                    double valor2 = ObtenerValuacionUTipoParcela1(mts, fondo1.ValorMetros.Value, valorAforo, superficie);

                                    superficie = (contrafrente.ValorMetros.Value - frente.ValorMetros.Value) * fondo2.ValorMetros.Value;
                                    mts = contrafrente.ValorMetros.Value - frente.ValorMetros.Value;
                                    double valor3 = ObtenerValuacionUTipoParcela1(mts, fondo2.ValorMetros.Value, valorAforo, superficie);

                                    valorTierraUrbana += (valor1 + valor2 - valor3);
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_ROMBOIDAL_HASTA_2000M2: // 6
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                valorTierraUrbana = ObtenerValuacionUTipoParcela6(frente, fondo, ObtenerAforo(frente, idLocalidad), superficieParcela, fraccion);
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_EN_FALSA_ESCUADRA_HASTA_2000M2: // 7
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);

                                if (AllNotNullAndWithValue(new[] { frente, fondo, fondo2 }, fraccion))
                                {
                                    double fondoX = (fondo.ValorMetros.Value + fondo2.ValorMetros.Value) / 2;
                                    double valorAforo = ObtenerAforo(frente, idLocalidad);

                                    valorTierraUrbana += ObtenerValuacionUTipoParcela6(frente, new DDJJUMedidaLinealTemporal() { ValorMetros = fondoX }, valorAforo, superficieParcela, fraccion);
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_CONTRAFRENTE_EN_FALSA_ESCUADRA_HASTA_2000M2: // 8
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);

                                if (AllNotNullAndWithValue(new[] { frente, fondo, fondo2 }, fraccion))
                                {
                                    double fondoX = (fondo.ValorMetros.Value + fondo2.ValorMetros.Value) / 2;
                                    double valorAforo = ObtenerAforo(frente, idLocalidad);

                                    valorTierraUrbana += ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondoX, valorAforo, superficieParcela);
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_A_CALLES_OPUESTAS_HASTA_2000M2: // 9
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                valorTierraUrbana = ObtenerValuacionUTipoParcela9(frente1, frente2, fondo, idLocalidad, fraccion);
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_A_TRES_CALLES_HASTA_2000M2: // 10
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);
                                var fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);


                                if (AllNotNullAndWithValue(new[] { frente1, frente2, frente3, fondo1, fondo2 }, fraccion))
                                {
                                    double valor1 = ObtenerValuacionUTipoParcela9(frente1, frente3, fondo1, idLocalidad, fraccion);
                                    double superficie = fondo2.ValorMetros.Value * frente2.ValorMetros.Value;
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad);
                                    double valor2 = ObtenerValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo2, superficie);
                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_EN_ESQUINA_CON_FRENTE_A_DOS_CALLES_HASTA_900M2: // 11
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                double aforoFrente1 = ObtenerAforo(frente1, idLocalidad);
                                double aforoFrente2 = ObtenerAforo(frente2, idLocalidad);

                                valorTierraUrbana = ObtenerValuacionUTipoParcela11(frente1, frente2, aforoFrente1, aforoFrente2, superficieParcela, fraccion);
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_A_DOS_CALLES_OPUESTAS_MAYOR_A_2000M2: // 12
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                valorTierraUrbana = ObtenerValuacionUTipoParcela12(frente1, frente2, fondo, superficieParcela, idLocalidad, fraccion);
                            }
                            break;
                        case ClasesEnum.PARCELA_NO_EN_ESQUINA_CON_SUPERFICIE_ENTRE_2000M2_Y_15000M2: // 13
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                double valorAforo = ObtenerAforo(frente, idLocalidad);

                                valorTierraUrbana = ObtenerValuacionUTipoParcela13(frente, fondo, valorAforo, superficieParcela, fraccion);
                            }
                            break;
                        case ClasesEnum.PARCELA_EN_ESQUINA_DE_2000M2_Y_15000M2: // 14
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);

                                double valorAforo = ObtenerAforo(frente, idLocalidad);
                                double valorAforo1 = ObtenerAforo(frente1, idLocalidad);

                                double valorAforoMayor;

                                if (AllNotNullAndWithValue(new[] { frente, frente1 }, fraccion))
                                {
                                    DDJJUMedidaLinealTemporal frenteMenorAforo;

                                    if (valorAforo > valorAforo1)
                                    {
                                        frenteMenorAforo = frente1;
                                        valorAforoMayor = valorAforo;
                                    }
                                    else
                                    {
                                        frenteMenorAforo = frente;
                                        valorAforoMayor = valorAforo1;
                                    }

                                    var coeficiente = Contexto.VALCoef2a15k
                                                              .FirstOrDefault(x => x.FondoMinimo <= frenteMenorAforo.ValorMetros.Value && x.FondoMaximo >= frenteMenorAforo.ValorMetros.Value &&
                                                                                   x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);
                                    valorTierraUrbana += valorAforoMayor * ((coeficiente?.Coeficiente ?? 1) + 0.1) * superficieParcela;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_SUPERFICIE_MAYOR_A_15000M2: // 15
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);

                                if (AllNotNullAndWithValue(new[] { frente }, fraccion))
                                {
                                    var coeficiente = Contexto.VALCoefMayor15k
                                                              .FirstOrDefault(x => x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);
                                    valorTierraUrbana += (coeficiente?.Coeficiente ?? 1) * superficieParcela * ObtenerAforo(frente, idLocalidad);
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_TRIANGULAR_CON_FRENTE_A_UNA_CALLE_HASTA_2000M2: // 16
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                double valorAforo = ObtenerAforo(frente, idLocalidad);

                                valorTierraUrbana = ObtenerValuacionUTipoParcela16(frente, fondo, valorAforo, superficieParcela, fraccion);
                            }
                            break;
                        case ClasesEnum.PARCELA_TRIANGULAR_CON_VERTICE_A_UNA_CALLE_HASTA_2000M2: // 17
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                valorTierraUrbana = ObtenerValuacionUTipoParcela17(frente, fondo, ObtenerAforo(frente, idLocalidad), superficieParcela, fraccion);
                            }
                            break;
                        case ClasesEnum.PARCELA_TRAPEZOIDAL_CON_FRENTE_MAYOR_A_UNA_CALLE_HASTA_2000M2: // 18
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var contrafrente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.CONTRAFRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                if (AllNotNullAndWithValue(new[] { frente, contrafrente, fondo }, fraccion))
                                {
                                    double valorAforo = ObtenerAforo(frente, idLocalidad);
                                    double superficie1 = fondo.ValorMetros.Value * contrafrente.ValorMetros.Value;
                                    double valor1 = ObtenerValuacionUTipoParcela1(contrafrente.ValorMetros.Value, fondo.ValorMetros.Value, valorAforo, superficie1);
                                    double superficie2 = ((frente.ValorMetros.Value - contrafrente.ValorMetros.Value) * fondo.ValorMetros.Value) / 2;
                                    var frenteX = new DDJJUMedidaLinealTemporal() { ValorMetros = frente.ValorMetros.Value - contrafrente.ValorMetros.Value };
                                    double valor2 = ObtenerValuacionUTipoParcela16(frenteX, fondo, valorAforo, superficie2, fraccion);

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_TRAPEZOIDAL_CON_FRENTE_MENOR_A_UNA_CALLE_HASTA_2000M2: // 19
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var contrafrente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.CONTRAFRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                if (AllNotNullAndWithValue(new[] { frente, contrafrente, fondo }, fraccion))
                                {
                                    double valorAforo = ObtenerAforo(frente, idLocalidad);
                                    double superficie1 = fondo.ValorMetros.Value * frente.ValorMetros.Value;
                                    double valor1 = ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo.ValorMetros.Value, valorAforo, superficie1);

                                    double superficie2 = ((contrafrente.ValorMetros.Value - frente.ValorMetros.Value) * fondo.ValorMetros.Value) / 2;
                                    var frenteX = new DDJJUMedidaLinealTemporal()
                                    {
                                        ValorMetros = contrafrente.ValorMetros.Value - frente.ValorMetros.Value,
                                    };
                                    double valor2 = ObtenerValuacionUTipoParcela17(frenteX, fondo, valorAforo, superficie2, fraccion);

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_TRIANGULAR_CON_FRENTE_A_TRES_CALLES_HASTA_2000M2: // 20
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2, frente3 }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad);
                                    double valorAforo3 = ObtenerAforo(frente3, idLocalidad);

                                    double aforo = (valorAforo1 + valorAforo2 + valorAforo3) / 3;

                                    valorTierraUrbana += aforo * superficieParcela;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_EN_ESQUINA_CON_SUP_ENTRE_900M2_Y_2000M2: // 21
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2 }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad);
                                    valorTierraUrbana += ObtenerValuacionUTipoParcela21(frente1, frente2, valorAforo1, valorAforo2, superficieParcela);
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_EN_ESQUINA_CON_FRENTE_A_TRES_CALLES_Y_SUP_ENTRE_900M2_Y_2000M2: // 22
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2, frente3 }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad);
                                    double valorAforo3 = ObtenerAforo(frente3, idLocalidad);

                                    double superficie1 = (frente2.ValorMetros.Value / 2) * frente1.ValorMetros.Value;
                                    double superficie2 = (frente2.ValorMetros.Value / 2) * frente3.ValorMetros.Value;

                                    double valor1, valor2;
                                    var frente1X = new DDJJUMedidaLinealTemporal() { ValorMetros = frente2.ValorMetros.Value / 2 };
                                    double superficie = (frente2.ValorMetros.Value / 2) * frente3.ValorMetros.Value;
                                    if (superficie1 > 900)
                                    {
                                        valor1 = ObtenerValuacionUTipoParcela21(frente1X, frente1, valorAforo2, valorAforo1, superficie1);
                                        frente1X.ValorMetros = frente2.ValorMetros.Value / 2;
                                        valor2 = ObtenerValuacionUTipoParcela21(frente1X, frente3, valorAforo2, valorAforo3, superficie);
                                    }
                                    else
                                    {
                                        valor1 = ObtenerValuacionUTipoParcela11(frente1X, frente1, valorAforo2, valorAforo1, superficie1, fraccion);
                                        valor2 = ObtenerValuacionUTipoParcela11(frente1X, frente3, valorAforo2, valorAforo3, superficie, fraccion);
                                    }

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_ESPECIAL: // 23
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);

                                if (AllNotNullAndWithValue(new[] { frente }, fraccion))
                                {
                                    double valorAforo = ObtenerAforo(frente, idLocalidad);

                                    valorTierraUrbana += superficieParcela * valorAforo;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_EN_ESQUINA_CON_FRENTE_A_TRES_CALLES_HASTA_900M2: // 24
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2, frente3 }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad);
                                    double valorAforo3 = ObtenerAforo(frente3, idLocalidad);

                                    double superficie1 = (frente2.ValorMetros.Value / 2) * frente1.ValorMetros.Value;
                                    var frente1X = new DDJJUMedidaLinealTemporal() { ValorMetros = frente2.ValorMetros.Value / 2 };
                                    double valor1 = ObtenerValuacionUTipoParcela11(frente1X, frente1, valorAforo2, valorAforo1, superficie1, fraccion);
                                    double superficie2 = (frente2.ValorMetros.Value / 2) * frente3.ValorMetros.Value;
                                    double valor2 = ObtenerValuacionUTipoParcela11(frente1X, frente3, valorAforo2, valorAforo3, superficie2, fraccion);

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_A_DOS_CALLES_NO_OPUESTAS_MAYOR_A_2000M2: // 25
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var fondoA1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOA1);
                                var fondoA2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOA2);
                                var fondoB1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOB1);
                                var fondoB2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOB2);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2, fondoA1, fondoA2, fondoB1, fondoB2 }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad);

                                    double fondo1 = (fondoA1.ValorMetros.Value + fondoB1.ValorMetros.Value) / 2;
                                    double fondo2 = (fondoA2.ValorMetros.Value + fondoB2.ValorMetros.Value) / 2;

                                    double superficie1 = fondo1 * frente1.ValorMetros.Value;
                                    double valor1 = ObtenerValuacionUTipoParcela13(frente1, new DDJJUMedidaLinealTemporal() { ValorMetros = fondo1 }, valorAforo1, superficie1, fraccion);

                                    double superficie2 = fondo2 * frente2.ValorMetros.Value;
                                    double valor2 = ObtenerValuacionUTipoParcela13(frente2, new DDJJUMedidaLinealTemporal() { ValorMetros = fondo2 }, valorAforo2, superficie2, fraccion);

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_SALIENTE_LATERAL_HASTA_2000M2: // 26
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);
                                var fondoSaliente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOSALIENTE);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2, fondo1, fondo2, fondoSaliente }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad);

                                    double superficie1 = fondo1.ValorMetros.Value * frente1.ValorMetros.Value;
                                    double valor1 = ObtenerValuacionUTipoParcela1(frente1.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo1, superficie1);

                                    double superficie2 = (fondo2.ValorMetros.Value + fondoSaliente.ValorMetros.Value) * frente2.ValorMetros.Value;
                                    double valor2 = ObtenerValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2.ValorMetros.Value + fondoSaliente.ValorMetros.Value, valorAforo1, superficie2);

                                    double superficie3 = frente2.ValorMetros.Value * fondo2.ValorMetros.Value;
                                    double valor3 = ObtenerValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo1, superficie3);

                                    valorTierraUrbana += valor1 + valor2 + valor3;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_EN_TODA_LA_MANZANA_Y_SUP_ENTRE_2000M2_Y_15000M2: // 27
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);

                                if (AllNotNullAndWithValue(new[] { frente, frente1, frente2, frente3 }, fraccion))
                                {
                                    double valorAforo = ObtenerAforo(frente, idLocalidad);
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad);
                                    double valorAforo3 = ObtenerAforo(frente3, idLocalidad);

                                    double[] aforoArray = { valorAforo, valorAforo1, valorAforo2, valorAforo3 };
                                    int indexMaxAforo = Array.IndexOf(aforoArray, aforoArray.Max());

                                    double fondo = new[] { frente, frente1, frente2, frente3 }[indexMaxAforo].ValorMetros.Value;

                                    var coeficiente = Contexto.VALCoef2a15k.FirstOrDefault(x => x.FondoMinimo <= fondo && x.FondoMaximo >= fondo &&
                                                                                                x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);

                                    double aforoTotal = (valorAforo + valorAforo1 + valorAforo2 + valorAforo3) / 4;
                                    valorTierraUrbana += ((coeficiente?.Coeficiente ?? 1) + 0.1) * superficieParcela * aforoTotal;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_A_TRES_CALLES_MAYOR_A_2000M2: // 28
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);
                                var fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2, frente3, fondo1, fondo2 }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad);
                                    double valorAforo3 = ObtenerAforo(frente3, idLocalidad);

                                    double valor1;
                                    double superficie1 = frente1.ValorMetros.Value * fondo1.ValorMetros.Value;
                                    if (superficie1 < 2000)
                                    {
                                        valor1 = ObtenerValuacionUTipoParcela9(frente1, frente3, fondo1, idLocalidad, fraccion);
                                    }
                                    else
                                    {
                                        valor1 = ObtenerValuacionUTipoParcela12(frente1, frente3, fondo1, superficie1, idLocalidad, fraccion);
                                    }

                                    double valor2;
                                    double superficie2 = frente2.ValorMetros.Value * fondo2.ValorMetros.Value;
                                    if (superficie2 < 2000)
                                    {
                                        valor2 = ObtenerValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo2, superficie2);
                                    }
                                    else
                                    {
                                        valor2 = ObtenerValuacionUTipoParcela13(frente2, fondo2, valorAforo2, superficie2, fraccion);
                                    }

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                    }
                }
                valorTierra = (decimal)valorTierraUrbana;
            }
            else if (ddjj.Sor != null)
            {
                var ddjjSor = ddjj.Sor;

                double puntajeEmplazamiento = 0;
                if (ddjjSor.IdCamino.HasValue && ddjjSor.DistanciaCamino.HasValue)
                {
                    var puntajeCamino = Contexto.VALPuntajesCaminos.FirstOrDefault(x => x.IdCamino == ddjjSor.IdCamino && x.DistanciaMinima <= ddjjSor.DistanciaCamino && x.DistanciaMaxima >= ddjjSor.DistanciaCamino && !x.IdUsuarioBaja.HasValue);
                    puntajeEmplazamiento = puntajeCamino?.Puntaje ?? 0;
                }

                if (ddjjSor.DistanciaEmbarque.HasValue)
                {
                    var puntajeEmbarque = Contexto.VALPuntajesEmbarques.FirstOrDefault(x => x.DistanciaMinima <= ddjjSor.DistanciaEmbarque && x.DistanciaMaxima >= ddjjSor.DistanciaEmbarque && !x.IdUsuarioBaja.HasValue);
                    puntajeEmplazamiento += puntajeEmbarque?.Puntaje ?? 0;
                }

                if (ddjjSor.IdLocalidad.HasValue && ddjjSor.DistanciaLocalidad.HasValue)
                {
                    var puntajeLocalidad = Contexto.VALPuntajesLocalidades.FirstOrDefault(x => x.IdLocalidad == ddjjSor.IdLocalidad && x.DistanciaMinima <= ddjjSor.DistanciaLocalidad && x.DistanciaMaxima >= ddjjSor.DistanciaLocalidad && !x.IdUsuarioBaja.HasValue);
                    puntajeEmplazamiento += puntajeLocalidad?.Puntaje ?? 0;
                }

                var designacion = ddjj.Designacion;

                double valores_parciales = 0;
                foreach (var sup in ddjjSor.Superficies)
                {
                    long subtotal_puntaje = ddjjSor.SoRCars.Where(x => x.AptCar.IdAptitud == sup.IdAptitud).Sum(x => x.AptCar.Puntaje);
                    valores_parciales += (sup.Superficie ?? 0) * ((puntajeEmplazamiento + subtotal_puntaje) / 100);
                }

                double? valor_optimo = null;
                if (TipoParcelaEnum.Suburbana == tipoParcela)
                {
                    double superficieTotal = ddjjSor.Superficies.Sum(x => x.Superficie) ?? 0;
                    var valor = Contexto.VALValoresOptSuburbanos.FirstOrDefault(x => x.IdLocalidad == designacion.IdLocalidad && x.SuperficieMinima <= superficieTotal && x.SuperficieMaxima >= superficieTotal && !x.IdUsuarioBaja.HasValue);
                    valor_optimo = valor?.Valor;
                }
                else if (TipoParcelaEnum.Rural == tipoParcela)
                {
                    var valor = Contexto.VALValoresOptRurales.FirstOrDefault(x => x.IdDepartamento == designacion.IdDepartamento && !x.IdUsuarioBaja.HasValue);
                    valor_optimo = valor?.Valor;
                }

                valorTierra = (decimal)(valores_parciales * (valor_optimo ?? 1));
            }
            else
            {
                throw new InvalidOperationException($"La unidad tributaria {ddjj.IdUnidadTributaria}, perteneciente al trámite {ddjj.IdTramite}, no tiene tiene DDJJ de tierra definida.");
            }
            return valorTierra;
        }

        private decimal ObtenerValorMejoras(UnidadTributariaTemporal ut)
        {
            // como ya cargué el diccionario con las ddjj pertinentes, aunque puede no tener,
            // no necesito hacer una nueva carga desde la DB
            if (!_ddjjMejorasUTValuables.ContainsKey(ut.UnidadTributariaId)) return 0m;

            var ddjjMejoras = from ddjj in _ddjjMejorasUTValuables[ut.UnidadTributariaId]
                              orderby ddjj.FechaModif, ddjj.IdDeclaracionJurada
                              select ddjj;

            decimal valorMejorasE1E2 = 0;
            foreach (var ddjjMejora in ddjjMejoras)
            {
                var mejora = ddjjMejora.Mejora;
                var incisos = GetInmIncisos(ddjjMejora.IdVersion);

                if (mejora.IdDestinoMejora == 99)
                {
                    var coeficiente = Contexto.VALCoeficientesIncisos.FirstOrDefault(x => x.IdJurisdiccion == ut.JurisdiccionID && x.IdInciso == 10 && !x.IdUsuarioBaja.HasValue);
                    valorMejorasE1E2 += (decimal)coeficiente.Coeficiente.Value;
                }

                var caracteristicas = mejora.MejorasCar.Where(mct => mct.FechaBaja == null).ToList();

                var grupoCaracteristicas = from caracteristica in caracteristicas
                                           group caracteristica by caracteristica.Caracteristica.IdInciso into gp
                                           orderby incisos.FindIndex(i => i.IdInciso == gp.Key)
                                           select new { idInciso = gp.Key, caracteristicas = gp, cantidad = gp.Count() };


                var tipo_mejora = grupoCaracteristicas.First(x => x.cantidad == grupoCaracteristicas.Max(w => w.cantidad));

                decimal valor_total = 0;
                foreach (var grp in grupoCaracteristicas)
                {
                    var coeficiente = Contexto.VALCoeficientesIncisos.FirstOrDefault(x => x.IdJurisdiccion == ut.JurisdiccionID && x.IdInciso == grp.idInciso && !x.IdUsuarioBaja.HasValue);

                    if (coeficiente?.Coeficiente != null)
                    {
                        valor_total += grp.cantidad * (decimal)coeficiente.Coeficiente.Value;
                    }
                }

                decimal valor_unitario = valor_total / caracteristicas.Count();

                long edad = 0;
                bool matchCaracteristica(INMOtraCaracteristica otraCar, OtrasCaracteristicasV1 v1, OtrasCaracteristicasV2 v2)
                {
                    return otraCar.IdVersion == 1 && (OtrasCaracteristicasV1)otraCar.IdOtraCar == v1 ||
                           otraCar.IdVersion == 2 && (OtrasCaracteristicasV2)otraCar.IdOtraCar == v2;
                }

                var otrasCaracteristicas = mejora.OtrasCar.Where(moct => moct.FechaBaja == null).ToList();

                var anio = otrasCaracteristicas.SingleOrDefault(x => matchCaracteristica(x.OtraCar, OtrasCaracteristicasV1.AnioConstruccion, OtrasCaracteristicasV2.AnioConstruccion));
                if (anio != null)
                {
                    edad = DateTime.Now.Year - Convert.ToInt64(anio.Valor ?? 0m);
                }

                var depreciacion = Contexto.VALCoefDepreciacion.FirstOrDefault(x => x.IdEstadoConservacion == mejora.IdEstadoConservacion && x.IdInciso == tipo_mejora.idInciso && x.EdadEdificacion == edad && !x.IdUsuarioBaja.HasValue);
                decimal coeficienteDepreciacion = (decimal?)depreciacion?.Coeficiente ?? 1m;

                decimal superficie_cubierta = 0m;
                var mejoraOtraCarSuperficieCubierta = otrasCaracteristicas.SingleOrDefault(x => matchCaracteristica(x.OtraCar, OtrasCaracteristicasV1.SuperficieCubierta, OtrasCaracteristicasV2.SuperficieCubierta));
                if (mejoraOtraCarSuperficieCubierta?.Valor != null)
                {
                    superficie_cubierta = mejoraOtraCarSuperficieCubierta.Valor.Value;
                }

                decimal superficie_semi = 0m;
                var mejoraOtraCarSuperficieSemi = otrasCaracteristicas.SingleOrDefault(x => matchCaracteristica(x.OtraCar, OtrasCaracteristicasV1.SuperficieSemiCubierta, OtrasCaracteristicasV2.SuperficieSemiCubierta));
                if (mejoraOtraCarSuperficieSemi?.Valor != null)
                {
                    superficie_semi = mejoraOtraCarSuperficieSemi.Valor.Value;
                }

                decimal superficie_negocio = 0m;
                var mejoraOtraCarSuperficieNegocio = otrasCaracteristicas.SingleOrDefault(x => matchCaracteristica(x.OtraCar, OtrasCaracteristicasV1.SuperficieNegocio, OtrasCaracteristicasV2.SuperficieNegocio));
                if (mejoraOtraCarSuperficieNegocio?.Valor != null)
                {
                    superficie_negocio = mejoraOtraCarSuperficieNegocio.Valor.Value;
                }

                decimal valor_sup_cubierta = coeficienteDepreciacion * valor_unitario * superficie_cubierta;

                decimal valor_unitario_sup_semi = 0;
                if (incisos.Where(x => x.Descripcion == "A" || x.Descripcion == "B" || x.Descripcion == "C").Select(x => x.IdInciso).ToList().IndexOf(tipo_mejora.idInciso) >= 0)
                {
                    valor_unitario_sup_semi = valor_unitario * 0.5m;
                }
                else
                {
                    valor_unitario_sup_semi = valor_unitario * 0.3m;
                }

                decimal valor_sup_semi = coeficienteDepreciacion * valor_unitario_sup_semi * superficie_semi;

                decimal valor_total_mejora_vivienda = valor_sup_cubierta + valor_sup_semi;

                decimal valor_unitario_sup_negocio = valor_unitario;
                if (superficie_negocio > 100)
                {
                    valor_unitario_sup_negocio = valor_unitario * 0.7m;
                }

                decimal valor_total_mejora_negocio = coeficienteDepreciacion * valor_unitario_sup_negocio * superficie_negocio;

                decimal valor_total_obra_accesoria = 0;
                foreach (var otraCar in otrasCaracteristicas)
                {
                    var coeficiente = Contexto.VALCoeficientesOtrasCar
                                              .FirstOrDefault(x => x.IdOtraCar == otraCar.IdOtraCar &&
                                                                   x.ValorMinimo <= otraCar.Valor && x.ValorMaximo >= otraCar.Valor &&
                                                                   x.IdDestinoMejora == mejora.IdDestinoMejora && x.IdInciso == tipo_mejora.idInciso &&
                                                                   !x.IdUsuarioBaja.HasValue);

                    if (coeficiente != null)
                    {
                        valor_total_obra_accesoria += ((decimal?)coeficiente.Valor ?? 1m) * (otraCar.Valor ?? 0m) * coeficienteDepreciacion;
                    }
                }

                valorMejorasE1E2 += (valor_total_mejora_vivienda + valor_total_mejora_negocio + valor_total_obra_accesoria);
            }
            return valorMejorasE1E2;
        }

        private IEnumerable<Tuple<VALDecreto, VALValuacionDecretoTemporal>> ObtenerDecretos(IEnumerable<VALDecreto> decretos)
        {
            foreach (var decreto in decretos ?? new VALDecreto[0])
            {
                yield return
                    new Tuple<VALDecreto, VALValuacionDecretoTemporal>(
                        decreto,
                        new VALValuacionDecretoTemporal()
                        {
                            IdTramite = Tramite.IdTramite,
                            IdDecreto = decreto.IdDecreto,
                            IdUsuarioModif = Tramite.UsuarioModif,
                            IdUsuarioAlta = Tramite.UsuarioModif,
                            FechaAlta = Tramite.FechaModif,
                            FechaModif = Tramite.FechaModif
                        });
            }
        }

        private bool AllNotNullAndWithValue(DDJJUMedidaLinealTemporal[] lista, DDJJUFraccionTemporal fraccion)
        {
            if (!lista.Any(x => x != null && x.ValorMetros.HasValue))
            {
                Contexto.GetLogger().LogInfo($"DeclaracionJuradaRepository - GetValuacion - Medidas incompletas.{Environment.NewLine}DeclaracionJurada U: {fraccion.IdU}{Environment.NewLine}IdFraccion: {fraccion.IdFraccion}");
                return false;
            }
            return true;
        }

        private double ObtenerAforo(DDJJUMedidaLinealTemporal medlin, int idLocalidad)
        {
            try
            {
                return medlin.ValorAforo ?? 0;
                //if (tipoValuacion == TipoValuacionEnum.Revaluacion)
                //{
                //    return 0; //  ObtenerAforo(medlin, idLocalidad); REVISAR POR TEMA DE TRAMO / POSIBLE TRAMO TEMPORAL
                //}
                //else
                //{
                //    return medlin.ValorAforo ?? 0;
                //}
            }
            catch (Exception ex)
            {
                Contexto.GetLogger().LogError($"DeclaracionJuradaRepository - ObtenerAforo - IdUMedidaLineal {medlin.IdUMedidaLineal}", ex);
                return 1;
            }
        }

        private double ObtenerValuacionUTipoParcela1(double frente, double fondo, double valorAforo, double? superficie)
        {
            var coeficiente = Contexto.VALCoefMenor2k
                                      .FirstOrDefault(x => x.FrenteMinimo <= frente && x.FrenteMaximo >= frente && x.FondoMinimo <= fondo && x.FondoMaximo >= fondo);
            return (coeficiente?.Coeficiente ?? 1) * (superficie ?? (frente * fondo)) * valorAforo;
        }

        private double ObtenerValuacionUTipoParcela6(DDJJUMedidaLinealTemporal frente, DDJJUMedidaLinealTemporal fondo, double valorAforo, double superficie, DDJJUFraccionTemporal fraccion)
        {
            if (AllNotNullAndWithValue(new[] { frente, fondo }, fraccion))
            {
                var coeficiente = Contexto.VALCoefMenor2k
                                          .FirstOrDefault(x => x.FrenteMinimo <= frente.ValorMetros.Value && x.FrenteMaximo >= frente.ValorMetros.Value &&
                                                               x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value);
                double valorCoeficiente = ((coeficiente?.Coeficiente ?? 1) * 0.9);
                return superficie * valorCoeficiente * valorAforo;
            }

            return 0;
        }

        private double ObtenerValuacionUTipoParcela9(DDJJUMedidaLinealTemporal frente1, DDJJUMedidaLinealTemporal frente2, DDJJUMedidaLinealTemporal fondo, int idLocalidad, DDJJUFraccionTemporal fraccion)
        {
            if (AllNotNullAndWithValue(new[] { frente1, frente2, fondo }, fraccion))
            {
                double valorAforo1 = ObtenerAforo(frente1, idLocalidad);
                double valorAforo2 = ObtenerAforo(frente2, idLocalidad);

                double fondo1 = (fondo.ValorMetros.Value * valorAforo1) / (valorAforo1 + valorAforo2);
                double fondo2 = (fondo.ValorMetros.Value * valorAforo2) / (valorAforo1 + valorAforo2);

                double superficie1 = fondo1 * frente1.ValorMetros.Value;
                double superficie2 = fondo2 * frente2.ValorMetros.Value;

                double valor1 = ObtenerValuacionUTipoParcela1(frente1.ValorMetros.Value, fondo1, valorAforo1, superficie1);
                double valor2 = ObtenerValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2, valorAforo2, superficie2);

                return valor1 + valor2;
            }

            return 0;
        }

        private double ObtenerValuacionUTipoParcela11(DDJJUMedidaLinealTemporal frente1, DDJJUMedidaLinealTemporal frente2, double aforoFrente1, double aforoFrente2, double superficieParcela, DDJJUFraccionTemporal fraccion)
        {
            if (AllNotNullAndWithValue(new[] { frente1, frente2 }, fraccion))
            {
                DDJJUMedidaLinealTemporal frenteMayorAforo, frenteMenorAforo;
                double aforoMayor, aforoMenor;
                if (aforoFrente1 > aforoFrente2)

                {
                    frenteMayorAforo = frente1;
                    frenteMenorAforo = frente2;
                    aforoMayor = aforoFrente1;
                    aforoMenor = aforoFrente2;
                }
                else
                {
                    frenteMayorAforo = frente2;
                    frenteMenorAforo = frente1;
                    aforoMayor = aforoFrente2;
                    aforoMenor = aforoFrente1;
                }

                double relacion_aforos = aforoMayor / aforoMenor;
                double relacion_frentes = frenteMayorAforo.ValorMetros.Value / frenteMenorAforo.ValorMetros.Value;

                var coeficiente = Contexto.VALCoefEsquinaMenor900
                                          .FirstOrDefault(x => x.RelValoresMinima <= relacion_aforos && x.RelValoresMaxima >= relacion_aforos &&
                                                               x.RelFrenteMinima <= relacion_frentes && x.RelFrenteMaxima >= relacion_frentes &&
                                                               x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);

                return aforoMayor * (coeficiente?.Coeficiente ?? 1) * superficieParcela;
            }

            return 0;
        }

        private double ObtenerValuacionUTipoParcela12(DDJJUMedidaLinealTemporal frente1, DDJJUMedidaLinealTemporal frente2, DDJJUMedidaLinealTemporal fondo, double superficieParcela, int idLocalidad, DDJJUFraccionTemporal fraccion)
        {
            if (AllNotNullAndWithValue(new[] { frente1, frente2, fondo }, fraccion))
            {
                double valorAforo1 = ObtenerAforo(frente1, idLocalidad);
                double valorAforo2 = ObtenerAforo(frente2, idLocalidad);
                double fondo1 = (fondo.ValorMetros.Value * valorAforo1) / (valorAforo1 + valorAforo2);
                double fondo2 = (fondo.ValorMetros.Value * valorAforo2) / (valorAforo1 + valorAforo2);
                double superficie1 = fondo1 * frente1.ValorMetros.Value;
                double superficie2 = fondo2 * frente1.ValorMetros.Value;

                var coeficiente1 = Contexto.VALCoef2a15k
                                           .FirstOrDefault(x => x.FondoMinimo <= fondo1 && x.FondoMaximo >= fondo1 && x.SuperficieMinima <= superficieParcela &&
                                                                x.SuperficieMaxima >= superficieParcela);

                var coeficiente2 = Contexto.VALCoef2a15k
                                           .FirstOrDefault(x => x.FondoMinimo <= fondo2 && x.FondoMaximo >= fondo2 && x.SuperficieMinima <= superficieParcela &&
                                                                x.SuperficieMaxima >= superficieParcela);

                double valor1 = superficie1 * (coeficiente1?.Coeficiente ?? 1) * valorAforo1;
                double valor2 = superficie2 * (coeficiente2?.Coeficiente ?? 1) * valorAforo2;

                return valor1 + valor2;
            }

            return 0;
        }

        private double ObtenerValuacionUTipoParcela13(DDJJUMedidaLinealTemporal frente, DDJJUMedidaLinealTemporal fondo, double valorAforo, double superficieParcela, DDJJUFraccionTemporal fraccion)
        {
            if (AllNotNullAndWithValue(new[] { frente, fondo }, fraccion))
            {
                var coeficiente = Contexto.VALCoef2a15k
                                          .FirstOrDefault(x => x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value &&
                                                               x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);
                return frente.ValorMetros.Value * fondo.ValorMetros.Value * valorAforo * (coeficiente?.Coeficiente ?? 1);
            }

            return 0;
        }

        private double ObtenerValuacionUTipoParcela16(DDJJUMedidaLinealTemporal frente, DDJJUMedidaLinealTemporal fondo, double valorAforo, double superficieParcela, DDJJUFraccionTemporal fraccion)
        {
            if (AllNotNullAndWithValue(new[] { frente, fondo }, fraccion))
            {
                var coeficiente = Contexto.VALCoefTriangFrente
                                          .FirstOrDefault(x => x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value &&
                                                               x.FrenteMinimo <= frente.ValorMetros.Value && x.FrenteMaximo >= frente.ValorMetros.Value);
                return (coeficiente?.Coeficiente ?? 1) * superficieParcela * valorAforo;
            }

            return 0;
        }

        private double ObtenerValuacionUTipoParcela17(DDJJUMedidaLinealTemporal frente, DDJJUMedidaLinealTemporal fondo, double valorAforo, double superficieParcela, DDJJUFraccionTemporal fraccion)
        {
            if (AllNotNullAndWithValue(new[] { frente, fondo }, fraccion))
            {
                var coeficiente = Contexto.VALCoefTriangVertice
                                          .FirstOrDefault(x => x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value &&
                                                               x.ContrafrenteMinimo <= frente.ValorMetros.Value && x.ContrafrenteMaximo >= frente.ValorMetros.Value);

                return (coeficiente?.Coeficiente ?? 1) * superficieParcela * valorAforo;
            }
            return 0;
        }

        private double ObtenerValuacionUTipoParcela21(DDJJUMedidaLinealTemporal frente1, DDJJUMedidaLinealTemporal frente2, double valorAforo1, double valorAforo2, double superficieParcela)
        {
            DDJJUMedidaLinealTemporal frente, fondo, frenteMayorAforo, frenteMenorAforo;
            double aforo_frente, aforo_fondo;

            if (valorAforo1 > valorAforo2)
            {
                frente = frente1;
                aforo_frente = valorAforo1;
                fondo = frente2;
                aforo_fondo = valorAforo2;
            }
            else
            {
                frente = frente2;
                aforo_frente = valorAforo2;
                fondo = frente1;
                aforo_fondo = valorAforo1;
            }

            double frente_esquina;
            if (superficieParcela / 2 > 900)
            {
                frente_esquina = 900 / fondo.ValorMetros.Value;
            }
            else
            {
                frente_esquina = frente.ValorMetros.Value / 2;
            }

            if (valorAforo1 > valorAforo2)
            {
                frenteMayorAforo = frente1;
                frenteMenorAforo = frente2;
            }
            else
            {
                frenteMayorAforo = frente2;
                frenteMenorAforo = frente1;
            }

            double relacion_aforos = aforo_frente / aforo_fondo;
            double relacion_frente = frente.ValorMetros.Value / fondo.ValorMetros.Value;

            double superficie_900;
            if (superficieParcela / 2 < 900 && (frente_esquina * fondo.ValorMetros.Value) < 900)
            {
                superficie_900 = frente_esquina * fondo.ValorMetros.Value;
            }
            else
            {
                superficie_900 = 899;
            }

            double superficie_redondeada = Math.Round(superficie_900, 2);
            var coeficiente_esquina = Contexto.VALCoefEsquinaMenor900
                                              .FirstOrDefault(x => x.SuperficieMinima <= superficie_redondeada && x.SuperficieMaxima >= superficie_redondeada &&
                                                                   x.RelFrenteMinima <= relacion_frente && x.RelFrenteMaxima >= relacion_frente &&
                                                                   x.RelValoresMinima <= relacion_aforos && x.RelValoresMaxima >= relacion_aforos);

            double valorCoeficienteEsquina = coeficiente_esquina?.Coeficiente ?? 1;
            double frente_coeficiente = frente.ValorMetros.Value - frente_esquina;

            var coeficiente_no_esquina = Contexto.VALCoefMenor2k
                                                 .FirstOrDefault(x => x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value &&
                                                                      x.FrenteMinimo <= frente_coeficiente && x.FrenteMaximo >= frente_coeficiente);

            double valorCoeficienteNoEsquina = coeficiente_no_esquina?.Coeficiente ?? 1;
            double coeficiente = Math.Truncate(Math.Round(100 * ((valorCoeficienteEsquina + valorCoeficienteNoEsquina) / 2), 3)) / 100;

            return aforo_frente * coeficiente * superficieParcela;
        }

        private List<INMInciso> GetInmIncisos(long idVersion)
        {
            return Contexto.INMIncisos
                           .Where(x => x.IdVersion == idVersion && !x.FechaBaja.HasValue)
                           .OrderBy(x => x.Descripcion)
                           .ToList();
        }
    }
}
