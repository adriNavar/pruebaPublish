using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Geosit.Data.DAL.DDJJyValuaciones.Enums;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.Valuaciones;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Common.ExtensionMethods.DecimalDegreesToDMS;
using System.Xml;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common;
using System.Linq.Expressions;
using System.Configuration;
using GeoSit.Data.BusinessEntities.GlobalResources;

namespace GeoSit.Data.DAL.Repositories
{
    public class ParcelaRepository : IParcelaRepository
    {
        private readonly GeoSITMContext _context;
        private const int IdZonaTributaria = 42;

        public ParcelaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public string GetNextPartida(long idTipo, long idJurisdiccion)
        {
            var juri = Convert.ToInt64(_context.ParametrosGenerales.Single(x => x.Clave == "ID_TIPO_OBJETO_JURISDICCION").Valor);
            var query = from ut in _context.UnidadesTributarias
                        join par in _context.Parcelas on ut.ParcelaID equals par.ParcelaID
                        join jur in _context.Objetos on ut.JurisdiccionID equals jur.FeatId
                        where jur.TipoObjetoId == juri && par.TipoParcelaID == idTipo && ut.JurisdiccionID == idJurisdiccion
                        select ut.CodigoProvincial.Substring(2, 6);

            int valor = Convert.ToInt32(query.Max()) + 1;
            return valor.ToString().PadLeft(6, '0');
        }

        public Parcela GetParcelaById(long idParcela, bool completa = true, bool utsHistoricas = false) => GetParcela(x => x.ParcelaID == idParcela, completa, utsHistoricas);
        public Parcela GetParcelaByFeatIdDGC(long featId) => GetParcela(x => x.FeatIdDGC.HasValue && x.FeatIdDGC.Value == featId, true, false);

        private Parcela GetParcela(Expression<Func<Parcela, bool>> predicado, bool completa, bool utsHistoricas)
        {
            var parcela = _context.Parcelas.SingleOrDefault(predicado);
            if (parcela != null && completa)
            {
                load(parcela, utsHistoricas);
            }
            return parcela;
        }
        public Zonificacion GetZonificacion(long idParcela, bool esHistorico = false )
        {
            try
            {
                using (var qbuilder = _context.CreateSQLQueryBuilder())
                {
                    long idComponenteParcela = long.Parse(_context.ParametrosGenerales.Single(pg => pg.Clave == "ID_COMPONENTE_PARCELA").Valor);
                    var cmpParcela = _context.Componente.Include(c => c.Atributos).Single(c => c.ComponenteId == idComponenteParcela);
                    cmpParcela.Tabla = cmpParcela.TablaGrafica ?? cmpParcela.Tabla;
                    var cmpObjeto = new Componente() { ComponenteId = 1, Tabla = "oa_objeto", Esquema = ConfigurationManager.AppSettings["DATABASE"] };
                    var foreignKeyIdTipo = new Atributo() { ComponenteId = cmpObjeto.ComponenteId, Campo = "id_tipo_objeto" };
                    var cmpTipo = new Componente() { ComponenteId = 2, Tabla = "oa_tipo_objeto", Esquema = ConfigurationManager.AppSettings["DATABASE"] };
                    var campoGeomParcela = qbuilder.CreateGeometryFieldBuilder(cmpParcela.Atributos.GetAtributoGeometry(), "par");

                    long? featidZPLN = null;
                    Zonificacion zonificacion = null;
                    qbuilder.AddTable(cmpObjeto, "obj")
                            .AddJoin(cmpTipo, "tipo", new Atributo() { ComponenteId = cmpTipo.ComponenteId, Campo = "id_tipo_objeto" }, foreignKeyIdTipo)
                            .AddTable(cmpParcela, "par")
                            .AddFilter(foreignKeyIdTipo, 15, Common.Enums.SQLOperators.EqualsTo)
                            .AddFilter(campoGeomParcela, qbuilder.CreateGeometryFieldBuilder(new Atributo() { ComponenteId = cmpObjeto.ComponenteId, Campo = "geometry" }, "obj"), Common.Enums.SQLSpatialRelationships.Inside | Common.Enums.SQLSpatialRelationships.CoveredBy, Common.Enums.SQLConnectors.And)
                            .AddFilter(cmpParcela.Atributos.GetAtributoClave(), idParcela, Common.Enums.SQLOperators.EqualsTo, Common.Enums.SQLConnectors.And)
                            .AddFields(new Atributo() { ComponenteId = cmpObjeto.ComponenteId, Campo = "featid" },
                                       new Atributo() { ComponenteId = cmpObjeto.ComponenteId, Campo = "codigo" },
                                       new Atributo() { ComponenteId = cmpObjeto.ComponenteId, Campo = "nombre" })
                            .ExecuteQuery((IDataReader reader, ReaderStatus status) =>
                            {
                                featidZPLN = reader.GetInt64(reader.GetOrdinal("featid"));
                                zonificacion = new Zonificacion()
                                {
                                    CodigoZona = reader.GetStringOrEmpty(reader.GetOrdinal("codigo")),
                                    NombreZona = reader.GetStringOrEmpty(reader.GetOrdinal("nombre")),
                                };
                                status.Break();
                            });
                    if (featidZPLN.HasValue)
                    {
                        zonificacion.AtributosZonificacion = _context.ZonaAtributo
                                                                     .Include(a => a.Atributo)
                                                                     .Where(a => a.FeatId_Objeto == featidZPLN && (esHistorico || !a.Fecha_Baja.HasValue))
                                                                     .ToList()
                                                                     .Select(a => new AtributosZonificacion() { Descripcion = a.Atributo.Descripcion, UnidadMedida = a.U_Medida, Valor = a.Valor })
                                                                     .OrderBy(a => a.Descripcion)
                                                                     .ToList();
                    }
                    return zonificacion;
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError($"ParcelaRepository.GetZonificacion({idParcela})", ex);
                return null;
            }
        }

        public List<AtributosZonificacion> GetAtributosZonificacion(long idParcela)
        {
            throw new NotSupportedException("Ya no se recupera datos usando este método.");
        }

        private void load(Parcela parcela, bool utsHistoricas)
        {
            this._context.Entry(parcela).Reference(p => p.Clase).Load();
            this._context.Entry(parcela).Reference(p => p.Estado).Load();
            this._context.Entry(parcela).Reference(p => p.Origen).Load();
            this._context.Entry(parcela).Reference(p => p.Tipo).Load();

            this._context.Entry(parcela).Collection(c => c.UnidadesTributarias).Query().Where(ut => utsHistoricas || ut.FechaBaja == null).Load();

            this._context.Entry(parcela).Collection(c => c.ParcelaMensuras).Query().Where(m => m.FechaBaja == null).Load();
            if (parcela.ParcelaMensuras != null)
            {
                foreach (var parcelamensura in parcela.ParcelaMensuras)
                {
                    this._context.Entry(parcelamensura).Reference(pm => pm.Mensura).Load();
                    this._context.Entry(parcelamensura.Mensura).Reference(pm => pm.TipoMensura).Load();
                    this._context.Entry(parcelamensura.Mensura).Reference(pm => pm.EstadoMensura).Load();
                }
            }
            foreach (var ut in parcela.UnidadesTributarias ?? new List<UnidadTributaria>())
            {
                this._context.Entry(ut).Collection(u => u.UTDomicilios).Query().Where(utd => utd.FechaBaja == null).Load();
                this._context.Entry(ut).Reference(u => u.TipoUnidadTributaria).Load();
                if (ut.UTDomicilios != null)
                {
                    foreach (var utDom in ut.UTDomicilios)
                    {
                        this._context.Entry(utDom).Reference(utd => utd.Domicilio).Load();
                        this._context.Entry(utDom.Domicilio).Reference(d => d.TipoDomicilio).Load();
                    }
                }
                this._context.Entry(ut).Collection(u => u.UTDocumentos).Query().Where(utd => utd.FechaBaja == null).Load();
                if (ut.UTDocumentos != null)
                {
                    foreach (var utDoc in ut.UTDocumentos)
                    {
                        this._context.Entry(utDoc).Reference(d => d.Documento).Load();
                        utDoc.Documento.contenido = null;
                        this._context.Entry(utDoc.Documento).Reference(d => d.Tipo).Load();
                    }
                }
                this._context.Entry(ut).Collection(u => u.Valuaciones).Load();
            }
            this._context.Entry(parcela).Collection(c => c.Nomenclaturas).Load();
            foreach (var nomenc in parcela.Nomenclaturas)
            {
                this._context.Entry(nomenc).Reference(n => n.Tipo).Load();
            }

            this._context.Entry(parcela).Collection(p => p.ParcelaDocumentos).Query().Where(pd => pd.FechaBaja == null).Load();
            if (parcela.ParcelaDocumentos?.Any() ?? false)
            {
                var documentosMensuraByParcela = (from parcelaMensura in this._context.ParcelaMensura
                                                  join mensura in this._context.Mensura on parcelaMensura.IdMensura equals mensura.IdMensura
                                                  join mensuraDocumento in this._context.MensuraDocumento on mensura.IdMensura equals mensuraDocumento.IdMensura
                                                  where mensura.IdEstadoMensura == 3 && parcelaMensura.FechaBaja == null
                                                        && mensura.FechaBaja == null && mensuraDocumento.FechaBaja == null
                                                        && parcelaMensura.IdParcela == parcela.ParcelaID
                                                  select mensuraDocumento.IdDocumento).ToList();

                foreach (var pd in parcela.ParcelaDocumentos.Where(x => !documentosMensuraByParcela.Contains(x.DocumentoID)))
                {
                    this._context.Entry(pd).Reference(d => d.Documento).Load();
                    pd.Documento.contenido = null;
                    this._context.Entry(pd.Documento).Reference(d => d.Tipo).Load();
                }

                parcela.ParcelaDocumentos = parcela.ParcelaDocumentos.Where(x => x.Documento != null).ToList();
            }

            try
            {
                using (var qbuilder = _context.CreateSQLQueryBuilder())
                {
                    if (!long.TryParse(_context.ParametrosGenerales.SingleOrDefault(p => p.Clave == "ID_COMPONENTE_PARCELA")?.Valor, out long idCompParcela))
                    {
                        throw new Exception("No se encuentra el parámetro ID_COMPONENTE_PARCELA");
                    }
                    var cmp = _context.Componente.Include(c => c.Atributos).SingleOrDefault(c => c.ComponenteId == idCompParcela);
                    Atributo attrCampoClave;
                    Atributo attrCampoGeometry;
                    try
                    {
                        attrCampoClave = cmp.Atributos.GetAtributoClave();
                        attrCampoGeometry = cmp.Atributos.GetAtributoGeometry();
                    }
                    catch (ApplicationException appEx)
                    {
                        _context.GetLogger().LogError("Componente (id: " + cmp.ComponenteId + ") mal configurado.", appEx);
                        throw;
                    }
                    qbuilder
                        .AddTable(cmp.TablaGrafica ?? cmp.Tabla, "par")
                        .AddFilter(attrCampoClave.Campo, attrCampoClave.GetFormattedValue(parcela.ParcelaID), Common.Enums.SQLOperators.EqualsTo)
                        .AddGeometryField(qbuilder.CreateGeometryFieldBuilder(attrCampoGeometry, "par").Centroid().ChangeToSRID(Common.Enums.SRID.LL84).ToWKT(), "geom")
                        .ExecuteQuery((reader, status) =>
                        {
                            var geometry = reader.GetGeometryFromField(0, Common.Enums.SRID.LL84);
                            if (geometry != null)
                            {
                                parcela.Coordenadas = $"{geometry.YCoordinate.GetValueOrDefault().ConvertToDMS(/*LatLon.Lat*/)},{geometry.XCoordinate.GetValueOrDefault().ConvertToDMS(/*LatLon.Lon*/)}";// geometry
                            }
                        });

                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("ParcelaRepository - load ubicación", ex);
            }
        }

        public VALValuacion GetValuacionParcela(long idParcela, bool esHistorico)
        {
            try
            {
                return new DeclaracionJuradaRepository(_context).GetValuacionVigenteConsolidada(idParcela, esHistorico);
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError($"GetValuacionParcela(IdParcela: {idParcela})", ex);
                throw;
            }
        }

        public ICollection<Mejora> GetMejorasByIdParcela(long idParcela)
        {
            List<Mejora> mejoras = _context.Mejora.Where(m => m.ParcelaID == idParcela).ToList();
            return mejoras;
        }

        public void UpdateParcela(Parcela parcela)
        {
            parcela.FechaModificacion = DateTime.Now;
            if (parcela.FechaBajaExpediente.HasValue)
            {
                parcela.FechaBaja = parcela.FechaModificacion;
                parcela.UsuarioBajaID = parcela.UsuarioModificacionID;
            }
            _context.Entry(parcela).State = EntityState.Modified;
            _context.Entry(parcela).Property(x => x.UsuarioAltaID).IsModified = false;
            _context.Entry(parcela).Property(x => x.FechaAlta).IsModified = false;
        }

        public IEnumerable<Objeto> GetParcelaValuacionZonas()
        {
            var idTipoObjeto = Int32.Parse(_context.ParametrosGenerales.Find(IdZonaTributaria).Valor);
            var zonas = _context.Objetos.Where(o => o.TipoObjetoId == idTipoObjeto && !o.FechaBaja.HasValue).Take(200);
            return zonas.ToList();
        }

        public Objeto GetParcelaValuacionZona(long idAtributoZona)
        {
            var objetos = _context.Objetos;

            var idTipoObjeto = Int32.Parse(_context.ParametrosGenerales.Find(IdZonaTributaria).Valor);

            var query = from o in objetos
                        where o.TipoObjetoId == idTipoObjeto && o.FeatId == idAtributoZona
                        select o;

            return query.FirstOrDefault();
        }

        //private TipoValorBasicoTierra GetTipoValorBasicoTierra(long idParcela)
        //{
        //    TipoValorBasicoTierra tvbt = null;
        //    var valPadronDetalle = _context.ValuacionPadronDetalle.FirstOrDefault(pd => pd.IdParcela == idParcela && pd.Fecha_Baja == null);
        //    if (valPadronDetalle != null)
        //    {
        //        _context.Entry(valPadronDetalle).Reference(pd => pd.Cabecera).Load();
        //        _context.Entry(valPadronDetalle.Cabecera).Reference(pdc => pdc.TipoValorBasicoTierra).Load();
        //        tvbt = valPadronDetalle.Cabecera.TipoValorBasicoTierra;
        //    }
        //    return tvbt;
        //}

        public void InsertParcela(Parcela parcela)
        {
            _context.Parcelas.Add(parcela);
        }

        public List<string> GetPartidabyId(long idParcela)
        {
            return (from t1 in _context.UnidadesTributarias
                    where t1.ParcelaID == idParcela
                    select t1.CodigoMunicipal).ToList();
        }

        public ParcelaSuperficies GetSuperficiesByIdParcela(long id, bool esHistorico = false)
        {
            try
            {
                var superficies = new ParcelaSuperficies();

                var uTributaria = _context.UnidadesTributarias
                                          .Include(p => p.Parcela)
                                          .Where(ut => ut.ParcelaID == id && (esHistorico || ut.FechaBaja == null) &&
                                                        ((TipoUnidadTributariaEnum)ut.TipoUnidadTributariaID == TipoUnidadTributariaEnum.Comun ||
                                                         (TipoUnidadTributariaEnum)ut.TipoUnidadTributariaID == TipoUnidadTributariaEnum.PropiedaHorizontal))
                                          .OrderBy(ut => ut.TipoUnidadTributariaID)
                                          .Single();

                #region Superficies Parcelas
                try
                {

                    if (!string.IsNullOrEmpty(uTributaria.Parcela.Atributos))
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(uTributaria.Parcela.Atributos);
                        decimal readXmlAttribute(string campo)
                        {
                            var node = doc.SelectSingleNode($"//datos/{campo}/text()");
                            decimal.TryParse(node?.Value, out decimal valor);
                            return valor;
                        }
                        superficies.AtributosParcela.Catastro = readXmlAttribute("SuperficieCatastro");
                        superficies.AtributosParcela.Titulo = readXmlAttribute("SuperficieTitulo");
                        superficies.AtributosParcela.Mensura = readXmlAttribute("SuperficieMensura");
                        superficies.AtributosParcela.Estimada = readXmlAttribute("SuperficieEstimada");
                    }
                }
                catch (Exception ex)
                {
                    _context.GetLogger().LogError($"GetSuperficiesByIdParcela({id}) - Superficies Parcelas", ex);
                    return null;
                }
                #endregion

                #region Superficies Mejoras DGC
                long[] caracteristicas = { 24, 28, 32, 33, 34, 36, 37, 38 };
                try
                {

                    var query = from ut in _context.UnidadesTributarias
                                join valddjj in _context.DDJJ on ut.UnidadTributariaId equals valddjj.IdUnidadTributaria
                                join mejora in _context.INMMejora on valddjj.IdDeclaracionJurada equals mejora.IdDeclaracionJurada
                                join mejoraOtraCar in _context.INMMejoraOtraCar on mejora.IdMejora equals mejoraOtraCar.IdMejora
                                where caracteristicas.Contains(mejoraOtraCar.IdOtraCar) && ut.ParcelaID == id && valddjj.FechaBaja == null
                                group new { mejoraOtraCar } by mejoraOtraCar.IdOtraCar into grupo
                                select new { caracteristica = grupo.Key, total = grupo.Sum(g => g.mejoraOtraCar.Valor ?? 0) };

                    foreach (var grupo in query)
                    {
                        if ((OtrasCaracteristicasV1)grupo.caracteristica == OtrasCaracteristicasV1.PiletasNatacion)
                        {
                            superficies.DGCMejorasOtras.Piscina = grupo.total;
                        }
                        else if ((OtrasCaracteristicasV2)grupo.caracteristica == OtrasCaracteristicasV2.Pavimento)
                        {
                            superficies.DGCMejorasOtras.Pavimento = grupo.total;
                        }
                        else if ((OtrasCaracteristicasV1)grupo.caracteristica == OtrasCaracteristicasV1.SuperficieCubierta || (OtrasCaracteristicasV2)grupo.caracteristica == OtrasCaracteristicasV2.SuperficieCubierta)
                        {
                            superficies.DGCMejorasConstrucciones.Cubierta += grupo.total;
                        }
                        else if ((OtrasCaracteristicasV1)grupo.caracteristica == OtrasCaracteristicasV1.SuperficieNegocio || (OtrasCaracteristicasV2)grupo.caracteristica == OtrasCaracteristicasV2.SuperficieNegocio)
                        {
                            superficies.DGCMejorasConstrucciones.Negocio += grupo.total;
                        }
                        else if ((OtrasCaracteristicasV1)grupo.caracteristica == OtrasCaracteristicasV1.SuperficieSemiCubierta || (OtrasCaracteristicasV2)grupo.caracteristica == OtrasCaracteristicasV2.SuperficieSemiCubierta)
                        {
                            superficies.DGCMejorasConstrucciones.Semicubierta += grupo.total;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _context.GetLogger().LogError($"GetSuperficiesByIdParcela({id}) - Superficies Mejoras DGC", ex);
                    return null;
                }

                #endregion

                #region Superficies Relevamiento
                using (var builder = _context.CreateSQLQueryBuilder())
                {
                    try
                    {

                        string[] fields =
                            {
                                "superficie_tierra_graf", "superficie_cubierta", "superficie_galpon",
                                "superficie_semicubierta", "superficie_piscina", "superficie_deportiva",
                                "superficie_en_const", "superficie_precaria"
                            };
                        builder.AddTable("res_parcela_grafica", "t1")
                               .AddFilter("partida", $"'{uTributaria.CodigoProvincial}'", Common.Enums.SQLOperators.EqualsTo)
                               .AddFields(fields)
                               .ExecuteQuery((IDataReader reader, Common.ReaderStatus readerStatus) =>
                               {
                                   superficies.RelevamientoParcela.Relevada = reader.GetNullableDecimal(reader.GetOrdinal("superficie_tierra_graf")).GetValueOrDefault();
                                   superficies.RelevamientoMejorasConstrucciones.Cubierta = reader.GetNullableDecimal(reader.GetOrdinal("superficie_cubierta")).GetValueOrDefault();
                                   superficies.RelevamientoMejorasConstrucciones.Galpon = reader.GetNullableDecimal(reader.GetOrdinal("superficie_galpon")).GetValueOrDefault();
                                   superficies.RelevamientoMejorasConstrucciones.Semicubierta = reader.GetNullableDecimal(reader.GetOrdinal("superficie_semicubierta")).GetValueOrDefault();
                                   superficies.RelevamientoMejorasOtras.Piscina = reader.GetNullableDecimal(reader.GetOrdinal("superficie_piscina")).GetValueOrDefault();
                                   superficies.RelevamientoMejorasOtras.Deportiva = reader.GetNullableDecimal(reader.GetOrdinal("superficie_deportiva")).GetValueOrDefault();
                                   superficies.RelevamientoMejorasOtras.Construccion = reader.GetNullableDecimal(reader.GetOrdinal("superficie_en_const")).GetValueOrDefault();
                                   superficies.RelevamientoMejorasOtras.Precaria = reader.GetNullableDecimal(reader.GetOrdinal("superficie_precaria")).GetValueOrDefault();
                                   readerStatus.Break();
                               });
                    }
                    catch (Exception ex)
                    {
                        _context.GetLogger().LogError($"GetSuperficiesByIdParcela({id}) - Superficies Relevamiento", ex);
                        return null;
                    }
                }
                #endregion
                return superficies;
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError($"GetSuperficiesByIdParcela({id}) - General", ex);
                return null;
            }
        }
        public void RefreshVistaMaterializadaParcela()
        {
            _context.CreateSQLQueryBuilder().RefreshMaterializedView("VW_PARCELAS_GRAF_ALFA");
        }

        public Dictionary<long, List<Objeto>> GetJurisdiccionesByDepartamentoParcela(long id)
        {
            long objetoJurisdiccionTipo = long.Parse(TiposObjetoAdministrativo.JURISDICCION);
            long objetoDepartamentoTipo = long.Parse(TiposObjetoAdministrativo.DEPARTAMENTO);
            var jurisdicciones = (from jurByParcela in _context.Objetos
                                  join deptoParcela in _context.Objetos on jurByParcela.ObjetoPadreId equals deptoParcela.FeatId
                                  join jurByDeptoParcela in _context.Objetos on deptoParcela.FeatId equals jurByDeptoParcela.ObjetoPadreId
                                  join ut in _context.UnidadesTributarias on jurByParcela.FeatId equals ut.JurisdiccionID
                                  where ut.ParcelaID == id && ut.FechaBaja == null &&
                                        jurByDeptoParcela.TipoObjetoId == objetoJurisdiccionTipo && jurByDeptoParcela.FechaBaja == null &&
                                        deptoParcela.TipoObjetoId == objetoDepartamentoTipo
                                  group jurByDeptoParcela by jurByParcela into gp
                                  select gp);

            return jurisdicciones.ToList().ToDictionary(j => j.Key.FeatId, j => j.Distinct().ToList());
        }

        public bool EsVigente(long id) => GetParcela(x => x.ParcelaID == id && x.FechaBaja == null, false, false) != null;

        public Parcela GetParcelaByUt(long idUnidadTributaria)
        {
            var query = (from ut in _context.UnidadesTributarias
                         join par in _context.Parcelas on ut.ParcelaID equals par.ParcelaID
                         where ut.UnidadTributariaId == idUnidadTributaria
                         select par).FirstOrDefault();

            return query;
        }


    }
}
