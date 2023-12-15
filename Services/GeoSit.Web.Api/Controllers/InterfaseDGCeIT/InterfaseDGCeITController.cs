using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Web.Api.Controllers.InterfaseRentas;

namespace GeoSit.Web.Api.Controllers.InterfaseDGCeIT
{
    public class InterfaseDGCeITController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly InterfaseRentasHelper _interfaseRentasHelper;

        public InterfaseDGCeITController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _interfaseRentasHelper = new InterfaseRentasHelper(unitOfWork);
        }

        public IHttpActionResult CopiarParcela(long featId, char ctype, long userId)
        {
            const int ORIGEN_DEFAULT = 3;
            const int ESTADO_DEFAULT = 3;
            const int CLASE_DEFAULT = 1;
            const int DIM = 2;

            bool datosCopiados = false;
            bool copyAlpha = (ctype == 'A' || ctype == 'T');
            bool copyGeometry = (ctype == 'G' || ctype == 'T');

            DGCeITServices.Parcela parcelaDGC;
            using (var cbtSitService = new DGCeITServices.DGCeITService())
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    var param = db.ParametrosGenerales.FirstOrDefault(x => x.Clave == "IDGC_EXT_SERVICE_URL");
                    if (param != null && !string.IsNullOrEmpty(param.Valor))
                    {
                        cbtSitService.Url = param.Valor;
                    }
                }
                parcelaDGC = cbtSitService.GetParcela(featId, DIM, ctype);
            }
            // Copia de datos
            if (parcelaDGC != null)
            {
                bool isNew = false;
                var fechaOperacion = DateTime.Now;
                Dominio[] dominios = null;
                string nomenclaturaGeoSIT = GetNomenclaturaGeoSIT(parcelaDGC);

                copyGeometry &= ValidateGeometry(parcelaDGC.GeometryInfo);

                // Paso 1. Se busca la parcela municipal en base al FeatId de la parcela provincial
                var parcela = _unitOfWork.ParcelaRepository.GetParcelaByFeatIdDGC(featId);

                // Paso 2. Si no hay ninguna parcela municipal asociada al FeatId provincial, se busca por nomenclatura
                if (parcela == null)
                {
                    var nomenclatura = _unitOfWork.NomenclaturaRepository.GetNomenclatura(nomenclaturaGeoSIT);
                    if (nomenclatura != null)
                    {
                        parcela = _unitOfWork.ParcelaRepository.GetParcelaById(nomenclatura.ParcelaID);
                        if (parcela != null) parcela.FeatIdDGC = featId;
                    }
                }
                // Paso 3. Si tampoco se encuentra buscando por nomenclatura, se crea una parcela municipal nueva a partir de la provincial
                if (parcela == null)
                {
                    isNew = true;
                    parcela = new Parcela
                    {
                        UsuarioAltaID = userId,
                        FechaAlta = fechaOperacion
                    };
                }

                if (copyAlpha)
                {
                    var tiposParcela = _unitOfWork.TipoParcelaRepository.GetTipoParcelas();
                    if (tiposParcela == null || !tiposParcela.Any(x => x.TipoParcelaID == (long)parcelaDGC.IdTipoParcela))
                    {
                        return Content(HttpStatusCode.BadRequest, string.Format("Tipo de Parcela no encontrado: {0}.", parcelaDGC.IdTipoParcela));
                    }
                    var estadosParcela = _unitOfWork.EstadosParcelaRepository.GetEstadosParcela();
                    if (estadosParcela == null || !estadosParcela.Any(x => x.EstadoParcelaID == (long)parcelaDGC.IdEstado))
                    {
                        parcelaDGC.IdEstado = ESTADO_DEFAULT;
                    }
                    var clasesParcela = _unitOfWork.ClaseParcelaRepository.GetClasesParcelas();
                    if (clasesParcela == null || !clasesParcela.Any(x => x.ClaseParcelaID == (long)parcelaDGC.IdClaseParcela))
                    {
                        parcelaDGC.IdClaseParcela = CLASE_DEFAULT;
                    }

                    // Parcela
                    parcela.FeatIdDGC = featId;
                    parcela.OrigenParcelaID = ORIGEN_DEFAULT; //parcelaDGC.IdFuente;
                    parcela.TipoParcelaID = parcelaDGC.IdTipoParcela;
                    parcela.EstadoParcelaID = parcelaDGC.IdEstado;
                    parcela.ExpedienteAlta = parcelaDGC.ExpCreacion;
                    parcela.ClaseParcelaID = parcelaDGC.IdClaseParcela;
                    parcela.Superficie = Convert.ToDecimal(parcelaDGC.SupGrafico);
                    parcela.UsuarioModificacionID = userId;
                    parcela.FechaModificacion = fechaOperacion;

                    using (var sw = new StringWriter())
                    {
                        using (var xw = new XmlTextWriter(sw))
                        {
                            xw.Formatting = Formatting.Indented;
                            xw.Indentation = 4;

                            xw.WriteStartDocument();
                            xw.WriteStartElement("Datos");

                            xw.WriteStartElement("SuperfecieMensura");
                            xw.WriteValue(parcelaDGC.SupMensura);
                            xw.WriteEndElement();

                            xw.WriteStartElement("SuperfecieTitulo");
                            xw.WriteValue(parcelaDGC.SupTitulo);
                            xw.WriteEndElement();

                            xw.WriteEndElement();
                            xw.WriteEndDocument();
                        }
                        parcela.Atributos = sw.ToString();
                    }

                    // Nomenclatura
                    if (parcela.Nomenclaturas == null)
                    {
                        parcela.Nomenclaturas = new List<Nomenclatura>();
                    }
                    var nomenclatura = parcela.Nomenclaturas.FirstOrDefault(x => x.Nombre == nomenclaturaGeoSIT);
                    if (nomenclatura == null)
                    {
                        nomenclatura = new Nomenclatura
                        {
                            UsuarioAltaID = userId,
                            FechaAlta = fechaOperacion
                        };
                        parcela.Nomenclaturas.Add(nomenclatura);
                    }
                    nomenclatura.TipoNomenclaturaID = parcelaDGC.IdTipoParcela;
                    nomenclatura.FechaModificacion = fechaOperacion;
                    nomenclatura.UsuarioModificacionID = userId;
                    nomenclatura.Nombre = nomenclaturaGeoSIT;

                    // Unidades Tributarias
                    var partidasNuevas = new HashSet<string>();
                    if (parcelaDGC.partidas != null)
                    {
                        if (parcela.UnidadesTributarias == null)
                        {
                            parcela.UnidadesTributarias = new List<UnidadTributaria>();
                        }
                        using (var db = GeoSITMContext.CreateContext())
                        {
                            foreach (var partidaDGC in parcelaDGC.partidas)
                            {
                                string nroPartida = Convert.ToString(partidaDGC.Numero);
                                var unidadTributaria = parcela.UnidadesTributarias.FirstOrDefault(x => x.CodigoMunicipal == nroPartida);
                                if (unidadTributaria == null)
                                {
                                    unidadTributaria = parcela.UnidadesTributarias.FirstOrDefault(x => x.CodigoProvincial == nroPartida);
                                }
                                if (unidadTributaria == null)
                                {
                                    unidadTributaria = new UnidadTributaria
                                    {
                                        UsuarioAltaID = userId,
                                        FechaAlta = fechaOperacion,
                                        Dominios = new List<Dominio>()
                                    };
                                    parcela.UnidadesTributarias.Add(unidadTributaria);
                                    partidasNuevas.Add(unidadTributaria.CodigoMunicipal);
                                }
                                unidadTributaria.CodigoMunicipal = nroPartida;
                                unidadTributaria.CodigoProvincial = nroPartida;
                                unidadTributaria.UsuarioModificacionID = userId;
                                unidadTributaria.FechaModificacion = fechaOperacion;
                                unidadTributaria.FechaVigenciaDesde = fechaOperacion;
                                unidadTributaria.UnidadFuncional = Convert.ToString(partidaDGC.UnidadFuncional);
                                unidadTributaria.PorcentajeCopropiedad = Convert.ToDecimal(partidaDGC.PorcCopropiedad);

                                // Dominios
                                if (!string.IsNullOrEmpty(partidaDGC.NroInscripcion))
                                {
                                    if (unidadTributaria.Dominios == null)
                                    {
                                        unidadTributaria.Dominios = db.Dominios.Where(x => x.UnidadTributariaID == unidadTributaria.UnidadTributariaId).ToList();
                                    }
                                    var dominio = unidadTributaria.Dominios.FirstOrDefault(x => x.Inscripcion == partidaDGC.NroInscripcion);
                                    if (dominio == null)
                                    {
                                        dominio = new Dominio
                                        {
                                            IdUsuarioAlta = userId,
                                            FechaAlta = fechaOperacion,
                                        };

                                        unidadTributaria.Dominios.Add(dominio);
                                    }
                                    dominio.UnidadTributaria = unidadTributaria;
                                    dominio.Inscripcion = partidaDGC.NroInscripcion;
                                    dominio.TipoInscripcionID = partidaDGC.TipoInscripcion;
                                    dominio.FechaModif = fechaOperacion;
                                    dominio.IdUsuarioModif = userId;
                                }
                            }
                        }
                        dominios = parcela.UnidadesTributarias.Where(x => x.Dominios != null).SelectMany(x => x.Dominios).ToArray();
                    }
                    if (isNew)
                    {
                        _unitOfWork.ParcelaRepository.InsertParcela(parcela);
                    }
                    else
                    {
                        _unitOfWork.ParcelaRepository.UpdateParcela(parcela);
                    }
                    if (dominios != null)
                    {
                        foreach (var dominio in dominios)
                        {
                            if (dominio.DominioID > 0)
                            {
                                _unitOfWork.DominioRepository.UpdateDominio(dominio);
                            }
                            else
                            {
                                _unitOfWork.DominioRepository.InsertDominio(dominio);
                            }
                        }
                    }
                    _unitOfWork.Save();
                    datosCopiados = true;

                    // Interfase Rentas
                    foreach (var unidadTributaria in parcela.UnidadesTributarias)
                    {
                        if (partidasNuevas.Contains(unidadTributaria.CodigoMunicipal))
                        {
                            _interfaseRentasHelper.Alta(parcela, unidadTributaria.CodigoMunicipal);
                        }
                        else
                        {
                            _interfaseRentasHelper.Modificacion(parcela, unidadTributaria.CodigoMunicipal);
                        }
                    }
                }
                if (copyGeometry)
                {
                    isNew = false;
                    var parcelaGrafica = _unitOfWork.ParcelaGraficaRepository.GetParcelaGraficaByIdParcela(parcela.ParcelaID);
                    if (parcelaGrafica == null)
                    {
                        isNew = true;
                        parcelaGrafica = new ParcelaGrafica
                        {
                            UsuarioAltaID = userId,
                            FechaAlta = fechaOperacion
                        };
                        if (parcela.ParcelaID > 0)
                        {
                            parcelaGrafica.ParcelaID = parcela.ParcelaID;
                        }
                    }
                    parcelaGrafica.UsuarioModificacionID = userId;
                    parcelaGrafica.FechaModificacion = fechaOperacion;

                    if (isNew)
                    {
                        _unitOfWork.ParcelaGraficaRepository.InsertParcelaGrafica(parcelaGrafica);
                    }
                    else
                    {
                        _unitOfWork.ParcelaGraficaRepository.UpdateParcelaGrafica(parcelaGrafica);
                    }
                    _unitOfWork.Save();

                    var geometryInfo = parcelaDGC.GeometryInfo;
                    _unitOfWork.ParcelaGraficaRepository.UpdateGeometry(parcelaGrafica.FeatID, geometryInfo.GType, geometryInfo.Srid,
                        geometryInfo.Point != null ? new[] { geometryInfo.Point.X, geometryInfo.Point.Y, geometryInfo.Point.Z } : null,
                        geometryInfo.ElemInfo, geometryInfo.Ordinates);

                    _unitOfWork.Save();
                    datosCopiados = true;

                }
                if (datosCopiados)
                {
                    return Ok("Los datos se copiaron correctamente.");
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, "No hay datos para copiar.");
                }
            }
            return Content(HttpStatusCode.NotFound, string.Format("No se encontró la parcela {0}.", featId));
        }

        private bool ValidateGeometry(DGCeITServices.GeometryInfo geometryInfo)
        {
            if (geometryInfo != null)
            {
                return geometryInfo.GType > 0 && geometryInfo.Srid > 0 && (geometryInfo.Point != null || (geometryInfo.ElemInfo != null && geometryInfo.Ordinates != null));
            }
            return false;
        }

        private string GetNomenclaturaGeoSIT(DGCeITServices.Parcela parcelaDGC)
        {
            string nomenclatura;
            using (var db = GeoSITMContext.CreateContext())
            {
                int chacra, parcela, macizo, qmf = 0;
                int.TryParse(parcelaDGC.Chacra, out chacra);
                int.TryParse(parcelaDGC.NcaParcela, out parcela);
                int.TryParse(parcelaDGC.Macizo, out macizo);

                if (!string.IsNullOrEmpty(parcelaDGC.Quinta))
                {
                    int.TryParse(parcelaDGC.Quinta, out qmf);
                }
                else if (!string.IsNullOrEmpty(parcelaDGC.Manzana))
                {
                    int.TryParse(parcelaDGC.Manzana, out qmf);
                }
                else if (!string.IsNullOrEmpty(parcelaDGC.FraccionUrbana))
                {
                    int.TryParse(parcelaDGC.FraccionUrbana, out qmf);
                }
                else if (!string.IsNullOrEmpty(parcelaDGC.FraccionRural))
                {
                    int.TryParse(parcelaDGC.FraccionRural, out qmf);
                }
                string sql = "SELECT FN_NOMENCLATURA_GEOSIT(:circunscripcion, :sector, :chacra, :qmf, :parcela, :macizo) FROM DUAL";
                nomenclatura = db.Database.SqlQuery<string>(sql, parcelaDGC.CirDescriptor, parcelaDGC.SctDescriptor, chacra, qmf, parcela, macizo).FirstOrDefault();
            }
            return nomenclatura ?? string.Empty;

        }
    }
}
