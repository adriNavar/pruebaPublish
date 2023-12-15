using System;
using System.Linq;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.DAL.Common;
using System.Collections.Generic;
using GeoSit.Web.Api.Solr;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Web.Api.Common;
using Newtonsoft.Json.Linq;

namespace GeoSit.Web.Api.Controllers
{
    public class AlfanumericoParcelaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public AlfanumericoParcelaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Post([FromBody] JObject modelo)
        {
            try
            {
                string ip = modelo["ip"].ToString();
                string machineName = modelo["machineName"].ToString();

                var unidadAlfanumericoParcela = modelo["unidadAlfanumericoParcela"].ToObject<UnidadAlfanumericoParcela>();

                void auditarParcela(Parcela parcela, string evento, string operacion, string datosAdicionales = null)
                {
                    _unitOfWork.AuditoriaRepository.InsertAuditoria(new Auditoria()
                    {
                        Id_Objeto = parcela.ParcelaID,
                        Id_Evento = long.Parse(evento),
                        Id_Tipo_Operacion = long.Parse(operacion),
                        Fecha = parcela.FechaModificacion,
                        Datos_Adicionales = datosAdicionales,
                        Cantidad = 1,
                        Objeto = "Parcela",
                        Autorizado = Autorizado.Si,
                        Machine_Name = machineName,
                        Ip = ip,
                        Id_Usuario = parcela.UsuarioModificacionID

                    });
                }
                void auditarUnidadTributaria(UnidadTributaria ut, string evento, string operacion)
                {
                    _unitOfWork.AuditoriaRepository.InsertAuditoria(new Auditoria()
                    {
                        Id_Objeto = ut.UnidadTributariaId,
                        Id_Evento = long.Parse(evento),
                        Id_Tipo_Operacion = long.Parse(operacion),
                        Fecha = ut.FechaModificacion.Value,
                        Cantidad = 1,
                        Objeto = "UnidadTributaria",
                        Autorizado = Autorizado.Si,
                        Machine_Name = machineName,
                        Ip = ip,
                        Id_Usuario = ut.UsuarioModificacionID.Value
                    });
                }

                unidadAlfanumericoParcela.OperacionesParcelasOrigenes.AnalyzeOperations("ParcelaID");
                unidadAlfanumericoParcela.OperacionesParcelasDestinos.AnalyzeOperations("ParcelaID");

                DateTime fechaHora = DateTime.Now;
                var nuevasParcelas = new List<Parcela>();
                if (Convert.ToInt32(TiposParcelaOperacion.Creacion) == unidadAlfanumericoParcela.Operacion)
                {
                    foreach (var operacionParcelaDestino in unidadAlfanumericoParcela
                                                                        .OperacionesParcelasDestinos
                                                                        .Where(operacionParcelaDestino => operacionParcelaDestino.Operation == Operation.Add))
                    {
                        operacionParcelaDestino.Item.FechaModificacion = fechaHora;
                        operacionParcelaDestino.Item.UsuarioModificacionID = unidadAlfanumericoParcela.IdUsuario;

                        var parcela = CreateParcelaDestino(operacionParcelaDestino, unidadAlfanumericoParcela.Expediente, unidadAlfanumericoParcela.Fecha, unidadAlfanumericoParcela.IdJurisdiccion, unidadAlfanumericoParcela.Vigencia, new Dominio[0]);
                        _unitOfWork.ParcelaOperacionRepository.InsertParcelaOperacion(new ParcelaOperacion
                        {
                            ParcelaOrigen = parcela,
                            TipoOperacionID = unidadAlfanumericoParcela.Operacion,
                            ParcelaDestino = parcela,
                            FechaOperacion = fechaHora,
                            UsuarioAltaID = unidadAlfanumericoParcela.IdUsuario,
                            FechaAlta = fechaHora,
                            UsuarioModificacionID = unidadAlfanumericoParcela.IdUsuario,
                            FechaModificacion = fechaHora
                        });
                        nuevasParcelas.Add(parcela);
                    }

                }
                else
                {
                    var dominios = new List<Dominio>();
                    if (!new[] { Convert.ToInt32(TiposParcelaOperacion.DerechoRealSuperficie), Convert.ToInt32(TiposParcelaOperacion.PrescripcionAdquisitiva) }.Contains(unidadAlfanumericoParcela.Operacion))
                    {
                        bool copiarDominios = new[] { Convert.ToInt32(TiposParcelaOperacion.Subdivision), Convert.ToInt32(TiposParcelaOperacion.Unificacion), Convert.ToInt32(TiposParcelaOperacion.Redistribucion) }.Contains(unidadAlfanumericoParcela.Operacion);
                        foreach (var parcelaOrigen in unidadAlfanumericoParcela.OperacionesParcelasOrigenes.Select(po => po.Item))
                        {
                            parcelaOrigen.UsuarioModificacionID = unidadAlfanumericoParcela.IdUsuario;
                            parcelaOrigen.ExpedienteBaja = unidadAlfanumericoParcela.Expediente;
                            parcelaOrigen.FechaBajaExpediente = unidadAlfanumericoParcela.Fecha;

                            _unitOfWork.ParcelaRepository.UpdateParcela(parcelaOrigen);

                            auditarParcela(parcelaOrigen, Eventos.BajaParcela, TiposOperacion.Baja);
                            try
                            {
                                var parcelaGrafica = _unitOfWork.ParcelaGraficaRepository.GetParcelaGraficaByIdParcela(parcelaOrigen.ParcelaID);

                                if (parcelaGrafica != null)
                                {
                                    parcelaGrafica.UsuarioBajaID = parcelaOrigen.UsuarioBajaID;
                                    parcelaGrafica.FechaBaja = parcelaOrigen.FechaBaja;
                                    parcelaGrafica.UsuarioModificacionID = parcelaOrigen.UsuarioBajaID;
                                    parcelaGrafica.FechaModificacion = parcelaOrigen.FechaBaja;
                                    auditarParcela(parcelaOrigen, Eventos.BajaParcelagrafica, TiposOperacion.Baja);
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                return Conflict();
                            }

                            var unidadesTributarias = _unitOfWork.UnidadTributariaRepository.GetUnidadesTributariasByParcela(parcelaOrigen.ParcelaID, copiarDominios);
                            if (copiarDominios)
                            {
                                dominios.AddRange(unidadesTributarias.SelectMany(ut => ut.Dominios));
                            }
                            foreach (var unidadTributaria in unidadesTributarias)
                            {
                                unidadTributaria.FechaVigenciaHasta = fechaHora;
                                unidadTributaria.UsuarioBajaID = parcelaOrigen.UsuarioBajaID;
                                unidadTributaria.FechaBaja = parcelaOrigen.FechaBaja;
                                unidadTributaria.UsuarioModificacionID = parcelaOrigen.UsuarioBajaID;
                                unidadTributaria.FechaModificacion = parcelaOrigen.FechaBaja;
                                auditarUnidadTributaria(unidadTributaria, Eventos.BajaUnidadesTributarias, TiposOperacion.Baja);
                                auditarParcela(parcelaOrigen, Eventos.BajaUnidadesTributarias, TiposOperacion.Baja, unidadTributaria.CodigoProvincial);
                            }
                        }
                    }
                    int idx = 0;
                    foreach (var operacionParcelaDestino in unidadAlfanumericoParcela
                                                                .OperacionesParcelasDestinos
                                                                .Where(operacionParcelaDestino => operacionParcelaDestino.Operation == Operation.Add))
                    {
                        operacionParcelaDestino.Item.FechaModificacion = fechaHora;
                        operacionParcelaDestino.Item.UsuarioModificacionID = unidadAlfanumericoParcela.IdUsuario;

                        nuevasParcelas.Add(CreateParcelaDestino(operacionParcelaDestino, unidadAlfanumericoParcela.Expediente, unidadAlfanumericoParcela.Fecha, unidadAlfanumericoParcela.IdJurisdiccion, unidadAlfanumericoParcela.Vigencia, dominios, idx++));
                    }

                    foreach (var nueva in nuevasParcelas)
                    {
                        foreach (var origen in unidadAlfanumericoParcela.OperacionesParcelasOrigenes)
                        {
                            _unitOfWork.ParcelaOperacionRepository.InsertParcelaOperacion(new ParcelaOperacion
                            {
                                TipoOperacionID = unidadAlfanumericoParcela.Operacion,
                                ParcelaOrigenID = origen.Item.ParcelaID,
                                ParcelaDestino = nueva,
                                FechaOperacion = fechaHora,
                                UsuarioAltaID = nueva.UsuarioModificacionID,
                                FechaAlta = fechaHora,
                                UsuarioModificacionID = nueva.UsuarioModificacionID,
                                FechaModificacion = fechaHora
                            });
                        }
                    }
                }
                _unitOfWork.Save();

                foreach (var parcela in nuevasParcelas)
                {
                    var ut = parcela.UnidadesTributarias.Single();
                    auditarParcela(parcela, Eventos.AltaParcela, TiposOperacion.Alta);
                    auditarParcela(parcela, Eventos.AltaUnidadesTributarias, TiposOperacion.Alta, ut.CodigoProvincial);
                    auditarUnidadTributaria(ut, Eventos.AltaUnidadesTributarias, TiposOperacion.Alta);
                }
                _unitOfWork.Save();

                //SolrUpdater.Instance.Enqueue(Entities.parcela);
                //SolrUpdater.Instance.Enqueue(Entities.parcelahistorica);
                //SolrUpdater.Instance.Enqueue(Entities.prescripcion);
                //SolrUpdater.Instance.Enqueue(Entities.parcelamunicipal);
                //SolrUpdater.Instance.Enqueue(Entities.parcelaproyecto);
                //SolrUpdater.Instance.Enqueue(Entities.unidadtributaria);
                //SolrUpdater.Instance.Enqueue(Entities.unidadtributariahistorica);
                try
                {
                    _unitOfWork.ParcelaRepository.RefreshVistaMaterializadaParcela();

                    return Ok();
                }
                catch (Exception ex)
                {
                    Global.GetLogger().LogError("AlfanumericoParcela.Post(UnidadAlfanumericoParcela)", ex);
                    return StatusCode(System.Net.HttpStatusCode.ExpectationFailed);
                }
                
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("AlfanumericoParcela.Post(UnidadAlfanumericoParcela)", ex);
                return InternalServerError(ex);
            }
        }

        private Parcela CreateParcelaDestino(OperationItem<Parcela> operacionParcelaDestino, string expediente, DateTime? fechaExpediente, long idJurisdiccion, DateTime? vigencia, IEnumerable<Dominio> dominios, int idx = 0)
        {
            long idUT = (++idx) * -1;
            long nextIdDominio(int i) => (i + 1) * -1;
            var parcela = new Parcela
            {
                UnidadesTributarias = new[]
                {
                    new UnidadTributaria()
                    {
                        UnidadTributariaId = idUT,
                        CodigoProvincial = operacionParcelaDestino.Item.UnidadesTributarias.First().CodigoProvincial.ToUpper(),
                        JurisdiccionID = idJurisdiccion,
                        TipoUnidadTributariaID = operacionParcelaDestino.Item.UnidadesTributarias.First().TipoUnidadTributariaID, //COMUN
                        FechaVigenciaDesde = operacionParcelaDestino.Item.FechaModificacion,
                        FechaAlta = operacionParcelaDestino.Item.FechaModificacion,
                        UsuarioAltaID = operacionParcelaDestino.Item.UsuarioModificacionID,
                        FechaModificacion = operacionParcelaDestino.Item.FechaModificacion,
                        UsuarioModificacionID = operacionParcelaDestino.Item.UsuarioModificacionID,
                        Vigencia = vigencia,
                        Superficie = operacionParcelaDestino.Item.UnidadesTributarias.First().Superficie,
                        Dominios = dominios.Where(d=>d.FechaBaja == null).Select((d,i)=>new Dominio
                        {
                            DominioID = nextIdDominio(i),
                            UnidadTributariaID = idUT,
                            Fecha = d.Fecha,
                            Inscripcion = d.Inscripcion,
                            TipoInscripcionID = d.TipoInscripcionID,
                            Titulares = (d.Titulares ?? new DominioTitular[0]).Where(t=>t.FechaBaja == null).Select(t=>new DominioTitular()
                            {
                                DominioID = nextIdDominio(i),
                                PersonaID = t.PersonaID,
                                TipoTitularidadID = t.TipoTitularidadID,
                                TipoPersonaID = t.TipoPersonaID,
                                PorcientoCopropiedad = t.PorcientoCopropiedad,
                                UsuarioAltaID = operacionParcelaDestino.Item.UsuarioModificacionID,
                                FechaAlta = operacionParcelaDestino.Item.FechaModificacion,
                                UsuarioModificacionID = operacionParcelaDestino.Item.UsuarioModificacionID,
                                FechaModificacion = operacionParcelaDestino.Item.FechaModificacion
                            }).ToList(),
                            IdUsuarioAlta = operacionParcelaDestino.Item.UsuarioModificacionID,
                            FechaAlta = operacionParcelaDestino.Item.FechaModificacion,
                            IdUsuarioModif = operacionParcelaDestino.Item.UsuarioModificacionID,
                            FechaModif = operacionParcelaDestino.Item.FechaModificacion
                        }).ToList()
                    }
                },
                //Nomenclaturas = new[]
                //{
                //    new Nomenclatura
                //    {
                //        Nombre = string.Empty,
                //        TipoNomenclaturaID = 0,
                //        UsuarioAltaID = operacionParcelaDestino.Item.UsuarioModificacionID,
                //        FechaAlta = operacionParcelaDestino.Item.FechaModificacion,
                //        UsuarioModificacionID = operacionParcelaDestino.Item.UsuarioModificacionID,
                //        FechaModificacion = operacionParcelaDestino.Item.FechaModificacion
                //    }
                //},
                TipoParcelaID = operacionParcelaDestino.Item.TipoParcelaID,
                ClaseParcelaID = operacionParcelaDestino.Item.ClaseParcelaID,
                EstadoParcelaID = operacionParcelaDestino.Item.EstadoParcelaID,
                OrigenParcelaID = operacionParcelaDestino.Item.OrigenParcelaID,
                ExpedienteAlta = expediente,
                FechaAltaExpediente = fechaExpediente,
                UsuarioAltaID = operacionParcelaDestino.Item.UsuarioModificacionID,
                FechaAlta = operacionParcelaDestino.Item.FechaModificacion,
                FechaModificacion = operacionParcelaDestino.Item.FechaModificacion,
                UsuarioModificacionID = operacionParcelaDestino.Item.UsuarioModificacionID,
                Atributos = operacionParcelaDestino.Item.Atributos,
                Superficie = operacionParcelaDestino.Item.Superficie
            };
            _unitOfWork.ParcelaRepository.InsertParcela(parcela);

            return parcela;
        }
    }
}
