using System.Data;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Common.Enums;

namespace GeoSit.Data.DAL.Repositories
{
    public class ExpansionPlotRepository : IExpansionPlotRepository
    {
        private readonly GeoSITMContext _context;

        public ExpansionPlotRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public ExpansionPlot[] GetExpansionPlotByObjetoBase(Componente componenteBase, string esquema, string tabla, string campoGeometry, string campoId, string campoNombre, int filtroGeografico, string idObjetoBase, long? filtroIdAtributo, string anio)
        {
            if (string.IsNullOrEmpty(idObjetoBase) || filtroGeografico != 1)
            {
                return null;
            }
            try
            {
                string esquemaBase = componenteBase.Esquema;
                string tablaBase = componenteBase.Tabla;
                Atributo campoGeometryBase = null;
                Atributo campoFeatIdBase = null;
                Atributo geomAttr = new Atributo() { Campo = campoGeometry };
                Atributo labelAttr = string.IsNullOrEmpty(campoNombre) ? null : new Atributo() { Campo = campoNombre };
                Atributo featId = new Atributo() { Campo = campoId };
                try
                {
                    campoFeatIdBase = componenteBase.Atributos.GetAtributoClave();
                    campoGeometryBase = componenteBase.Atributos.GetAtributoGeometry();
                }
                catch (ApplicationException appEx)
                {
                    _context.GetLogger().LogError("Componente (id: " + componenteBase.ComponenteId + ") mal configurado.", appEx);
                    return null;
                }

                using (var builder = this._context.CreateSQLQueryBuilder())
                {
                    SQLConnectors connector = SQLConnectors.None;
                    builder.AddTable(tabla, "t1")
                           .AddFields(featId, labelAttr)
                           .AddGeometryField(builder.CreateGeometryFieldBuilder(geomAttr, "t1").ChangeToSRID(SRID.App).ToWKT(), "geom");

                    if (!string.IsNullOrEmpty(anio) && filtroIdAtributo != null)
                    {
                        Atributo atributoFiltro = _context.Atributo.Find(filtroIdAtributo);
                        if (atributoFiltro != null)
                        {
                            builder.AddFilter(atributoFiltro, anio, SQLOperators.EqualsTo);
                            connector = SQLConnectors.And;
                        }
                    }

                    if (string.Compare(tabla, componenteBase.Tabla, true) != 0)
                    {
                        featId = campoFeatIdBase;
                        builder.AddTable(componenteBase, "t2")
                               .AddFilter(builder.CreateGeometryFieldBuilder(geomAttr, "t1"), builder.CreateGeometryFieldBuilder(campoGeometryBase, "t2"), SQLSpatialRelationships.AnyInteract, connector);

                        var g1 = builder.CreateGeometryFieldBuilder(geomAttr, "t1");
                        var g2 = builder.CreateGeometryFieldBuilder(campoGeometryBase, "t2").OverlappingArea(g1);

                        builder.AddRawFilter(string.Format("({0} / {1})", g2, g1.ToPolygon().Area()), 0.9, SQLOperators.GreaterThan, SQLConnectors.And);
                        connector = SQLConnectors.And;
                    }
                    return builder.AddFilter(featId, idObjetoBase, SQLOperators.EqualsTo, connector)
                                  .ExecuteQuery((IDataReader reader) =>
                                  {
                                      return new ExpansionPlot()
                                      {
                                          IdReferencia = reader.GetStringOrEmpty(0),
                                          Nombre = labelAttr == null ? string.Empty : reader.GetStringOrEmpty(1),
                                          Geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"))
                                      };
                                  })
                                  .ToArray();
                }

                //if (!string.IsNullOrEmpty(idObjetoBase) && filtroGeografico == 1)
                //{
                //    string sSql = string.Empty;
                //    string filtro = string.Empty;
                //    if (!string.IsNullOrEmpty(anio))
                //    {
                //        if (filtroIdAtributo != null)
                //        {
                //            Atributo atributoFiltro = _context.Atributo.Find(filtroIdAtributo);
                //            if (atributoFiltro != null)
                //            {
                //                filtro = "p." + atributoFiltro.Campo + "=" + anio;
                //            }
                //        }
                //    }
                //    if (tabla != tablaBase)
                //    {
                //        sSql = "SELECT p." + campoId + " " +
                //                      (campoNombre != string.Empty ? ", p." + campoNombre + " " : " ") +
                //                      ",SDO_CS.TRANSFORM(p." + campoGeometry + ", 2000000001, 22195).Get_WKT() as WKTCBlob " + " " +
                //                " FROM " + esquema + "." + tabla + " p, " + esquemaBase + "." + tablaBase + " m " +
                //                " WHERE m." + campoFeatIdBase + " = " + idObjetoBase + " " +
                //                  " AND MDSYS.SDO_RELATE(m." + campoGeometryBase + ",p." + campoGeometry + ",'MASK=ANYINTERACT') = 'TRUE' " +
                //                  " AND (SDO_GEOM.SDO_AREA(SDO_GEOM.SDO_INTERSECTION(p." + campoGeometry + ", m." + campoGeometryBase + ", 0.1),0.1) / " +
                //                       " SDO_GEOM.SDO_AREA(p." + campoGeometry + ",0.1) ) > 0.9 ";
                //    }
                //    else
                //    {
                //        sSql = "SELECT p." + campoId + " " +
                //                      (campoNombre != string.Empty ? ", p." + campoNombre + " " : " ") +
                //                      ",SDO_CS.TRANSFORM(p." + campoGeometry + ", 2000000001, 22195).Get_WKT() as WKTCBlob " + " " +
                //                " FROM " + esquema + "." + tabla + " p " +
                //                " WHERE p." + campoId + " = " + idObjetoBase + " ";
                //    }
                //    if (filtro != string.Empty)
                //    {
                //        sSql += " AND " + filtro;
                //    }
                //    sSql += " ORDER BY ORDEN ";
                //    IDbCommand objComm = _context.Database.Connection.CreateCommand();
                //    _context.Database.Connection.Open();
                //    objComm.CommandText = sSql;
                //    IDataReader data = objComm.ExecuteReader();
                //    ExpansionPlot expansionPlotRead;
                //    while (data.Read())
                //    {
                //        expansionPlotRead = new ExpansionPlot() { IdReferencia = GetStringValue(data, 0) };

                //        //expansionPlotRead.Id = GetLongValue(data, 0);
                //        int idx = 1;
                //        if (campoNombre != string.Empty)
                //        {
                //            expansionPlotRead.Nombre = GetStringValue(data, idx);
                //            idx++;
                //        }
                //        //expansionPlotRead.IdReferencia = prefijoId + expansionPlotRead.Id.ToString().PadLeft(4, '0');

                //        DbGeometry geom = data.GetGeometryFromField(idx);

                //        if (geom != null)
                //        {
                //            expansionPlotRead.Geom = geom;
                //            lstExpansionPlot.Add(expansionPlotRead);
                //        }
                //    }
                //    _context.Database.Connection.Close();
                //}
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetExpansionPlotByObjetoBase", ex);
                return null;
            }
        }
    }
}
