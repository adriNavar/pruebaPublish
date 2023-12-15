using System.Collections.Generic;
using System.Data;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System;
using System.Linq;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Data.DAL.Repositories
{
    public class ParcelaPlotRepository : IParcelaPlotRepository
    {
        private readonly GeoSITMContext _context;

        public ParcelaPlotRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<KeyValuePair<long, long>> GetManzanas(List<string> idsParcela, bool esApic)
        {
            try
            {
                Componente componenteParcela = null;
                Componente componenteManzana = null;
                Atributo campoIdParcela = null;
                Atributo campoIdManzana = null;
                Atributo campoGeomParcela = null;
                Atributo campoGeomManzana = null;
                bool filtrarManzanasFechaBaja = true;
                var parametros = this._context.ParametrosGenerales.Where(p => p.Clave == "ID_COMPONENTE_PARCELA" || p.Clave == "ID_COMPONENTE_MANZANA");
                long idComponente = 0;
                try
                {
                    idComponente = long.Parse(parametros.First(p => p.Clave == "ID_COMPONENTE_PARCELA").Valor);
                    componenteParcela = this._context.Componente.Include("Atributos").Single(c => c.ComponenteId == idComponente);
                    campoIdParcela = componenteParcela.Atributos.GetAtributoClave();
                    campoGeomParcela = componenteParcela.Atributos.GetAtributoGeometry();
                }
                catch (ApplicationException appEx)
                {
                    _context.GetLogger().LogError("Componente (id: " + componenteParcela.ComponenteId + ") mal configurado.", appEx);
                    return null;
                }
                try
                {
                    idComponente = long.Parse(parametros.First(p => p.Clave == "ID_COMPONENTE_MANZANA").Valor);
                    componenteManzana = this._context.Componente.Include("Atributos").Single(c => c.ComponenteId == idComponente);
                    campoIdManzana = componenteManzana.Atributos.GetAtributoClave();
                    campoGeomManzana = componenteManzana.Atributos.GetAtributoGeometry();
                    filtrarManzanasFechaBaja = componenteManzana.Atributos.Any(attr => string.Compare(attr.Campo, "fecha_baja", true) == 0);
                }
                catch (ApplicationException appEx)
                {
                    _context.GetLogger().LogError("Componente (id: " + componenteManzana.ComponenteId + ") mal configurado.", appEx);
                    return null;
                }

                var resultado = new List<KeyValuePair<long, long>>();
                int MAX_CANT_ID = 900;

                int idx = 0;
                while (idx < idsParcela.Count())
                {
                    var objetos = idsParcela.Skip(idx).Take(Math.Min(MAX_CANT_ID, idsParcela.Count() - idx));
                    idx += MAX_CANT_ID;

                    try
                    {
                        using (var builder = this._context.CreateSQLQueryBuilder())
                        {
                            builder.AddTable(componenteParcela, "par")
                                   .AddFields(campoIdParcela)
                                   .AddTable(componenteManzana, "mza")
                                   .AddFields(campoIdManzana)
                                   .AddFilter(builder.CreateGeometryFieldBuilder(campoGeomManzana, "mza"),
                                              builder.CreateGeometryFieldBuilder(campoGeomParcela, "par"),
                                              SQLSpatialRelationships.Contains | SQLSpatialRelationships.Covers | SQLSpatialRelationships.Equal)
                                   .AddFilter(campoIdParcela, $"({string.Join(",", objetos)})", SQLOperators.In, SQLConnectors.And);

                            if (filtrarManzanasFechaBaja)
                            {
                                builder.AddFilter(new Atributo { ComponenteId = componenteManzana.ComponenteId, Campo = "fecha_baja" }, null, SQLOperators.IsNull, SQLConnectors.And);
                            }
                            resultado.AddRange(builder.ExecuteQuery((IDataReader reader) =>
                                                      {
                                                          return new KeyValuePair<long, long>(reader.GetNullableInt64(reader.GetOrdinal(campoIdParcela.Campo)).Value,
                                                                                              reader.GetNullableInt64(reader.GetOrdinal(campoIdManzana.Campo)).Value);
                                                      }));
                        }
                    }
                    catch (Exception ex)
                    {
                        _context.GetLogger().LogError(string.Format("GetManzanas - Loop(step ids: {0})", string.Join(",", objetos)), ex);
                        return null;
                    }
                }
                return resultado;
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError(string.Format("GetManzanas - General(ids: {0})", string.Join(",", idsParcela)), ex);
                return null;
            }
        }

        public ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, int filtroGeografico, double x1, double y1, double x2, double y2)
        {
            List<string> lstCoordsGeometry = new List<string>();
            lstCoordsGeometry.Add(x1.ToString().Replace(",", ".") + ", " + y1.ToString().Replace(",", "."));
            lstCoordsGeometry.Add(x2.ToString().Replace(",", ".") + ", " + y2.ToString().Replace(",", "."));
            string campoExpediente = string.Empty;
            string campoNomCatastral = string.Empty;
            string campoIdClienteTipo = string.Empty;
            return GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, lstCoordsGeometry);
        }
        public ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, string campoExpediente, string campoNomCatastral, string campoIdClienteTipo, int filtroGeografico, double x1, double y1, double x2, double y2)
        {
            List<string> lstCoordsGeometry = new List<string>();
            lstCoordsGeometry.Add(x1.ToString().Replace(",", ".") + ", " + y1.ToString().Replace(",", "."));
            lstCoordsGeometry.Add(x2.ToString().Replace(",", ".") + ", " + y2.ToString().Replace(",", "."));
            return GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, lstCoordsGeometry);
        }
        public ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, int filtroGeografico, double x1, double y1, double x2, double y2)
        {
            List<string> lstCoordsGeometry = new List<string>();
            lstCoordsGeometry.Add(x1.ToString().Replace(",", ".") + ", " + y1.ToString().Replace(",", "."));
            lstCoordsGeometry.Add(x2.ToString().Replace(",", ".") + ", " + y2.ToString().Replace(",", "."));
            string campoFeatId = string.Empty;
            string campoGeometry = string.Empty;
            string campoNroPuerta = string.Empty;
            string campoIdCuadra = string.Empty;
            string campoExpediente = string.Empty;
            string campoNomCatastral = string.Empty;
            string campoIdClienteTipo = string.Empty;
            return GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, lstCoordsGeometry);
        }
        public ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, int filtroGeografico, List<string> lstCoordsGeometry)
        {
            string campoExpediente = string.Empty;
            string campoNomCatastral = string.Empty;
            string campoIdClienteTipo = string.Empty;
            return GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, lstCoordsGeometry);
        }
        public ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, string campoExpediente, string campoNomCatastral, string campoIdClienteTipo, int filtroGeografico, List<string> lstCoordsGeometry)
        {
            string tablaBarrioCarenciado = string.Empty;
            return GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, lstCoordsGeometry, tablaBarrioCarenciado);
        }
        public ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, string campoExpediente, string campoNomCatastral, string campoIdClienteTipo, int filtroGeografico, List<string> lstCoordsGeometry, string tablaBarrioCarenciado)
        {
            try
            {
                if (campoFeatId == string.Empty)
                {
                    campoFeatId = "ID_PARCELA";
                }
                if (campoGeometry == string.Empty)
                {
                    campoGeometry = "GEOMETRY";
                }
                if (campoNroPuerta == string.Empty)
                {
                    campoNroPuerta = "NRO_PUERTA";
                }
                if (campoIdCuadra == string.Empty)
                {
                    campoIdCuadra = "ID_CUADRA";
                }
                if (campoExpediente == string.Empty)
                {
                    campoExpediente = "NUMERO";
                }
                if (campoNomCatastral == string.Empty)
                {
                    campoNomCatastral = "NOM_CATASTRAL";
                }
                if (campoIdClienteTipo == string.Empty)
                {
                    campoIdClienteTipo = "ID_CLIENTE_TIPO";
                }
                if (tablaBarrioCarenciado == string.Empty)
                {
                    tablaBarrioCarenciado = "CT_VILLA";
                }

                if (lstCoordsGeometry.Count == 2) //es extent/bbox
                {
                    var c1 = lstCoordsGeometry[0].Split(',').Select(c => Convert.ToDouble(c.Trim()));
                    var c2 = lstCoordsGeometry[1].Split(',').Select(c => Convert.ToDouble(c.Trim()));
                    lstCoordsGeometry.Insert(1, string.Format("{0}, {1}", c1.ElementAt(0), c2.ElementAt(1)));
                    lstCoordsGeometry.AddRange(new[] { string.Format("{0}, {1}", c1.ElementAt(1), c2.ElementAt(0)), lstCoordsGeometry[0] });
                }


                using (var builder = this._context.CreateSQLQueryBuilder())
                {
                    SQLSpatialRelationships relations = filtroGeografico == 1 ? SQLSpatialRelationships.Inside | SQLSpatialRelationships.CoveredBy | SQLSpatialRelationships.Equal : SQLSpatialRelationships.AnyInteract;
                    string aliasGraf = "t1";
                    var geometry = new Atributo { Campo = campoGeometry };

                    builder
                        .AddTable(esquema, tabla, aliasGraf)
                        .AddFields(campoFeatId, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo)
                        .AddGeometryField(builder.CreateGeometryFieldBuilder(geometry, aliasGraf).Area(), "area")
                        .AddGeometryField(builder.CreateGeometryFieldBuilder(geometry, aliasGraf).ChangeToSRID(SRID.App).ToWKT(), campoGeometry);

                    var transientGeom = builder
                                            .CreateGeometryFieldBuilder(this._context.CreateParameter(":wkt", string.Format("POLYGON(({0}))", string.Join(",", lstCoordsGeometry.Select(c => string.Join(" ", c.Split(',').Select(p => p.Trim())))))), 3857)
                                            .ChangeToSRID(SRID.DB);
                    builder
                        .AddFilter(builder.CreateGeometryFieldBuilder(geometry, aliasGraf), transientGeom, relations, SQLConnectors.None);

                    if (filtroGeografico == 1)
                    {
                        var paramGral = _context.ParametrosGenerales.FirstOrDefault(pg => pg.Clave == "ToleranciaFiltroPrincipalPloteo");
                        transientGeom.AddBuffer(paramGral == null ? 1.0 : Convert.ToDouble(paramGral.Valor));
                    }
                    else if (filtroGeografico == 2)
                    {
                        using (var innerBuilder = this._context.CreateSQLQueryBuilder())
                        {
                            innerBuilder
                                .AddTable(esquema, tabla, "inner")
                                .AddFilter(innerBuilder.CreateGeometryFieldBuilder(geometry, "inner"), transientGeom, SQLSpatialRelationships.Inside, SQLConnectors.None)
                                .AddFields(campoFeatId);

                            builder.AddFilter(campoFeatId, string.Format("({0})", innerBuilder), SQLOperators.NotIn, SQLConnectors.And);
                        }
                    }
                    try
                    {
                        int contadorNumerales = 1;
                        return builder
                                .ExecuteQuery((IDataReader reader) =>
                                {
                                    var par = new ParcelaPlot()
                                    {
                                        FeatId = reader.GetNullableInt64(reader.GetOrdinal(campoFeatId)).Value,
                                        NroPuerta = reader.GetStringOrEmpty(reader.GetOrdinal(campoNroPuerta)),
                                        IdCuadra = reader.GetNullableInt64(reader.GetOrdinal(campoIdCuadra)).Value,
                                        Expediente = reader.GetStringOrEmpty(reader.GetOrdinal(campoExpediente)),
                                        NomCatastral = reader.GetStringOrEmpty(reader.GetOrdinal(campoNomCatastral)),
                                        IdClienteTipo = reader.GetNullableInt32(reader.GetOrdinal(campoIdClienteTipo)).GetValueOrDefault(),
                                        Superficie = reader.GetNullableDouble(reader.GetOrdinal("area")).Value
                                    };
                                    par.Expediente = par.Expediente == "#" ? string.Format("#{0:00}", contadorNumerales++) : par.Expediente;
                                    try
                                    {
                                        par.Geom = reader.GetGeometryFromField(reader.GetOrdinal(campoGeometry), SRID.App);
                                        return par;
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        _context.GetLogger().LogInfo(string.Format("Error en Geometry(featid: {0}): {1}", par.FeatId, ex.Message));
                                        return null;
                                    }
                                })
                                .ToArray();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //    if (geom != null)
                //    {
                //        double xParc = 0, yParc = 0;
                //        double.TryParse(geom.Centroid.XCoordinate.ToString(), out xParc);
                //        double.TryParse(geom.Centroid.YCoordinate.ToString(), out yParc);
                //        //bool esEspacioVerde = GetSuperposicionOCObjetoByIdParcela(esquema, tabla, parcelaPlotRead.FeatId, 9, 90);
                //        if (tablaBarrioCarenciado != string.Empty)
                //        {
                //            //tablaBarrioCarenciado = "CT_VILLA";
                //            bool esBarrioCarenciado = GetExisteVillaByCoords(esquema, tablaBarrioCarenciado, xParc, yParc);
                //            if (esBarrioCarenciado)
                //            {
                //                parcelaPlotRead.EsBarrioCarenciado = true;
                //            }
                //        }
                //        lstParcelaPlot.Add(parcelaPlotRead);
                //    }
                //}
                //_context.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetParcelaPlotByCoords", ex);
                return null;
            }
        }
        public ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, double x, double y)
        {
            try
            {
                using (var builder = this._context.CreateSQLQueryBuilder())
                {
                    var campoGeomentry = new Atributo { Campo = "GEOMETRY" };
                    return builder.AddTable(esquema, tabla, "t1")
                                  .AddFields("ID_PARCELA", "NRO_PUERTA")
                                  .AddGeometryField(builder.CreateGeometryFieldBuilder(campoGeomentry, "t1").ChangeToSRID(SRID.App).ToWKT(), "geom")
                                  .AddFilter(builder.CreateGeometryFieldBuilder(campoGeomentry, "t1"), builder.CreateGeometryFieldBuilder($"POINT({x} {y})", SRID.DB),
                                             SQLSpatialRelationships.AnyInteract)
                                  .ExecuteQuery((IDataReader reader) =>
                                  {
                                      var geom = reader.GetGeometryFromField(2);

                                      return geom == null ?
                                               null :
                                               new ParcelaPlot()
                                               {
                                                   Nombre = string.Empty,
                                                   FeatId = reader.GetNullableInt64(0).Value,
                                                   NroPuerta = reader.GetStringOrEmpty(1),
                                                   Geom = geom
                                               };
                                  })
                                  .ToArray();
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetParcelaPlotByCoords", ex);
                return null;
            }
            //ParcelaPlot[] aParcelaPlot = null;
            //List<ParcelaPlot> lstParcelaPlot = new List<ParcelaPlot>();
            //try
            //{
            //    string mask = "ANYINTERACT";
            //    string campoFeatId = string.Empty;
            //    string campoLabel = string.Empty;
            //    string campoGeometry = string.Empty;
            //    string campoId = string.Empty;

            //    campoFeatId = "ID_PARCELA";
            //    campoGeometry = "GEOMETRY";

            //    string coordsGeometry = x.ToString().Replace(",", ".") + ", " + y.ToString().Replace(",", ".");
            //    string sSql = "SELECT p." + campoFeatId + ", p.NRO_PUERTA " +
            //                               ",SDO_CS.TRANSFORM(p." + campoGeometry + ", 2000000001, 22195).Get_WKT() as WKTCBlob " + " " +
            //                  " FROM " + esquema + "." + tabla + " p " +
            //                  " WHERE MDSYS.SDO_RELATE(p." + campoGeometry + "," +
            //                                    "SDO_CS.TRANSFORM(" +
            //                                    "MDSYS.SDO_GEOMETRY (2001, 22195, MDSYS.SDO_POINT_TYPE(" + coordsGeometry + ", NULL), " +
            //                                    "NULL, " +
            //                                    "NULL) " +
            //                                    ",2000000001) " +
            //                                    ",'MASK=" + mask + "') = 'TRUE'";
            //    IDbCommand objComm = _context.Database.Connection.CreateCommand();
            //    _context.Database.Connection.Open();
            //    objComm.CommandText = sSql;
            //    IDataReader data = objComm.ExecuteReader();
            //    ParcelaPlot parcelaPlotRead;
            //    while (data.Read())
            //    {
            //        parcelaPlotRead = new ParcelaPlot();

            //        parcelaPlotRead.Nombre = string.Empty;
            //        parcelaPlotRead.FeatId = GetLongValue(data, 0);
            //        parcelaPlotRead.NroPuerta = GetIntValue(data, 1).ToString();

            //        DbGeometry geom = data.GetGeometryFromField(2);

            //        if (geom != null)
            //        {
            //            parcelaPlotRead.Geom = geom;
            //            lstParcelaPlot.Add(parcelaPlotRead);
            //        }
            //    }
            //    _context.Database.Connection.Close();
            //}
            //catch (Exception e)
            //{
            //    _context.GetLogger().LogError("GetParcelaPlotByCoords2", e);
            //}
            //if (lstParcelaPlot != null && lstParcelaPlot.Count > 0)
            //{
            //    aParcelaPlot = lstParcelaPlot.ToArray();
            //}
            //return aParcelaPlot;
        }
        public ParcelaPlot GetParcelaPlotByIdObjGraf(string IdObjGraf)
        {
            throw new NotImplementedException();
        }
        public ParcelaPlot[] GetParcelaPlotByObjetoBase(Componente componenteBase, string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, string campoExpediente, string campoNomCatastral, string campoIdClienteTipo, int filtroGeografico, string idObjetoBase)
        {
            string tablaBarrioCarenciado = string.Empty;
            return GetParcelaPlotByObjetoBase(componenteBase, esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, idObjetoBase, tablaBarrioCarenciado);
        }
        public ParcelaPlot[] GetParcelaPlotByObjetoBase(Componente componenteBase, string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, int filtroGeografico, string idObjetoBase)
        {
            string campoExpediente = string.Empty;
            string campoNomCatastral = string.Empty;
            string campoIdClienteTipo = string.Empty;
            string tablaBarrioCarenciado = string.Empty;
            return GetParcelaPlotByObjetoBase(componenteBase, esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, idObjetoBase, tablaBarrioCarenciado);
        }
        public ParcelaPlot[] GetParcelaPlotByObjetoBase(Componente componenteBase, string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, string campoExpediente, string campoNomCatastral, string campoIdClienteTipo, int filtroGeografico, string idObjetoBase, string tablaBarrioCarenciado)
        {
            if (string.IsNullOrEmpty(idObjetoBase)) return null;

            try
            {
                Atributo campoClaveBase = null;
                Atributo campoGeomBase = null;
                var parametros = this._context.ParametrosGenerales.Where(p => p.Clave == "ID_COMPONENTE_PARCELA" || p.Clave == "ID_COMPONENTE_MANZANA").OrderBy(p => p.Clave);
                try
                {
                    campoClaveBase = componenteBase.Atributos.GetAtributoClave();
                    campoGeomBase = componenteBase.Atributos.GetAtributoGeometry();
                }
                catch (ApplicationException appEx)
                {
                    _context.GetLogger().LogError("Componente (id: " + componenteBase.ComponenteId + ") mal configurado.", appEx);
                    return null;
                }
                using (var builder = this._context.CreateSQLQueryBuilder())
                {
                    var auxComponente = string.Compare(tabla, componenteBase.Tabla, true) != 0 ? new Componente { Tabla = tabla, Esquema = esquema } : componenteBase;

                    SQLSpatialRelationships relations = filtroGeografico == 1 ? SQLSpatialRelationships.Contains | SQLSpatialRelationships.Covers | SQLSpatialRelationships.Equal : SQLSpatialRelationships.AnyInteract;
                    string aliasT1 = "tbase";
                    string aliasT2 = aliasT1;
                    var geometry = new Atributo { Campo = campoGeometry };

                    builder
                        .AddTable(esquema, tabla, aliasT1)
                        .AddFilter(campoClaveBase, idObjetoBase, SQLOperators.EqualsTo);

                    if (auxComponente != componenteBase)
                    {
                        aliasT2 = "otra";
                        builder.AddTable(auxComponente, aliasT2);
                    }

                    builder.AddFields(new[] { campoFeatId, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo }.Select(a => new Atributo { Campo = a, ComponenteId = auxComponente.ComponenteId }).ToArray())
                           .AddGeometryField(builder.CreateGeometryFieldBuilder(geometry, aliasT2).Area(), "area")
                           .AddGeometryField(builder.CreateGeometryFieldBuilder(geometry, aliasT2).ChangeToSRID(SRID.App).ToWKT(), campoGeometry);

                    if (filtroGeografico == 1)
                    {
                        var paramGral = _context.ParametrosGenerales.FirstOrDefault(pg => pg.Clave == "ToleranciaFiltroPrincipalPloteo");
                        builder.AddFilter(builder.CreateGeometryFieldBuilder(geometry, aliasT2),
                                          builder.CreateGeometryFieldBuilder(geometry, aliasT1).AddBuffer(paramGral == null ? 1.0 : Convert.ToDouble(paramGral.Valor)),
                                          relations, SQLConnectors.And);
                    }
                    try
                    {
                        int contadorNumerales = 1;
                        return builder
                                .ExecuteQuery<ParcelaPlot>((IDataReader reader) =>
                                {
                                    var par = new ParcelaPlot()
                                    {
                                        FeatId = reader.GetNullableInt64(reader.GetOrdinal(campoFeatId)).Value,
                                        NroPuerta = reader.GetStringOrEmpty(reader.GetOrdinal(campoNroPuerta)),
                                        IdCuadra = reader.GetNullableInt64(reader.GetOrdinal(campoIdCuadra)).Value,
                                        Expediente = reader.GetStringOrEmpty(reader.GetOrdinal(campoExpediente)),
                                        NomCatastral = reader.GetStringOrEmpty(reader.GetOrdinal(campoNomCatastral)),
                                        IdClienteTipo = reader.GetNullableInt32(reader.GetOrdinal(campoIdClienteTipo)).GetValueOrDefault(),
                                        Superficie = reader.GetNullableDouble(reader.GetOrdinal("area")).Value
                                    };
                                    par.Expediente = par.Expediente == "#" ? string.Format("#{0:00}", contadorNumerales++) : par.Expediente;
                                    try
                                    {
                                        par.Geom = reader.GetGeometryFromField(reader.GetOrdinal(campoGeometry), SRID.App);
                                        return par;
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        _context.GetLogger().LogInfo(string.Format("Error en Geometry(featid: {0}): {1}", par.FeatId, ex.Message));
                                        return null;
                                    }
                                })
                                .ToArray();
                    }
                    catch (DataException ex)
                    {
                        _context.GetLogger().LogInfo(string.Format("sql query: {0}", ex.Message));
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetParcelaPlotByObjetoBase", ex);
                return null;
            }
            //            if (geom != null)
            //            {
            //                double xParc = 0, yParc = 0;
            //                double.TryParse(geom.Centroid.XCoordinate.ToString(), out xParc);
            //                double.TryParse(geom.Centroid.YCoordinate.ToString(), out yParc);
            //                //bool esEspacioVerde = GetSuperposicionOCObjetoByIdParcela(esquema, tabla, parcelaPlotRead.FeatId, 9, 90);
            //                if (tablaBarrioCarenciado != string.Empty)
            //                {
            //                    //tablaBarrioCarenciado = "CT_VILLA";
            //                    bool esBarrioCarenciado = GetExisteVillaByCoords(esquema, tablaBarrioCarenciado, xParc, yParc);
            //                    if (esBarrioCarenciado)
            //                    {
            //                        parcelaPlotRead.EsBarrioCarenciado = true;
            //                    }
            //                }
            //                lstParcelaPlot.Add(parcelaPlotRead);
            //            }
            //        }
            //        _context.Database.Connection.Close();
            //    }
            //catch (Exception e)
            //{
            //    _context.GetLogger().LogError("GetParcelaPlotByObjetoBase", e);
            //    try
            //    {
            //        _context.Database.Connection.Close();
            //    }
            //    catch { }
            //}
            //if (lstParcelaPlot != null && lstParcelaPlot.Count > 0)
            //{
            //    aParcelaPlot = lstParcelaPlot.ToArray();
            //}
            //return aParcelaPlot;
        }

        public bool GetSuperposicionOCObjetoByIdParcela(string esquema, string tabla, long idParcela, int idOCTipo, int porcentajeAreaOverlap)
        {
            try
            {
                using (var builder = this._context.CreateSQLQueryBuilder())
                {
                    builder.AddTable(esquema, tabla, "main")
                           .AddFormattedField("count(1)");
                    using (var innerBuilder = this._context.CreateSQLQueryBuilder())
                    {
                        string aliasTPar = "par", aliasTObj = "obj";

                        var componenteParcela = this._context.Componente.Include("Atributos").Single(c => c.ComponenteId == 10);
                        var campoFeatId = componenteParcela.Atributos.GetAtributoClave();

                        var campoGeometry = componenteParcela.Atributos.GetAtributoGeometry();
                        var geomParcela = builder.CreateGeometryFieldBuilder(campoGeometry, aliasTPar);

                        innerBuilder.AddTable(componenteParcela, aliasTPar)
                                    .AddFields(campoFeatId)
                                    .AddFilter(campoFeatId, idParcela, SQLOperators.EqualsTo)
                                    .AddTable("ct_oc_objeto", aliasTObj)
                                    .AddFilter("id_oc_tipo", idOCTipo, SQLOperators.EqualsTo, SQLConnectors.And)
                                    .AddRawFilter(string.Format("({0} / {1})",
                                                        builder.CreateGeometryFieldBuilder(new Atributo { Campo = "geometry" }, aliasTObj).OverlappingArea(geomParcela),
                                                        geomParcela.Area()),
                                                  porcentajeAreaOverlap / 100M, SQLOperators.GreaterThan, SQLConnectors.And);
                        builder.AddFilter(campoFeatId.Campo, $"({innerBuilder})", SQLOperators.In);
                    }

                    return builder.ExecuteQuery((IDataReader reader, ReaderStatus status) =>
                    {
                        status.Break();
                        return reader.GetNullableInt32(0).GetValueOrDefault();
                    }).Single() != 0;
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetSuperposicionOCObjetoByIdParcela", ex);
                return false;
            }
        }

        public long? GetIdManzanaByParcela(long idParcela)
        {
            var manzanas = GetManzanas(new List<string>() { idParcela.ToString() }, false);

            long? Manzana = null;
            if (manzanas.Any())
            {
                Manzana = manzanas.FirstOrDefault().Value;
            }

            return Manzana;
        }
    }
}
