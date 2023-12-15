using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Spatial;
using System.Linq;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.Redes;

namespace GeoSit.Data.DAL.Repositories
{
    public class LayerGrafRepository : ILayerGrafRepository
    {
        private readonly GeoSITMContext _context;
        private long COMPONENTE_ID_ARCO = 0;
        public LayerGrafRepository(GeoSITMContext context)
        {
            _context = context;
            COMPONENTE_ID_ARCO = Convert.ToInt64(context.ParametrosGenerales.ToArray().FirstOrDefault(p => p.Clave.ToUpper() == "ID_COMPONENTE_ARCO").Valor);
        }

        #region Se comenta porque no tiene uso. borrar mas adelante
        //public LayerGraf[] GetLayerGrafByCoords(string tableName, string campoFeatId, string campoNombre, string campoGeometry, double x1, double y1, double x2, double y2, string mask)
        //{
        //    try
        //    {
        //        if (mask == string.Empty)
        //        {
        //            mask = "ANYINTERACT";
        //        }
        //        string sSql = "SELECT p." + campoFeatId + (campoNombre != string.Empty ? ", p." + campoNombre : " ") + " " +
        //                             ", TO_CHAR(SDO_CS.TRANSFORM(SDO_CS.MAKE_2D(p." + campoGeometry + ", 8307), 22195).Get_WKT()) as WKT " +
        //                      " FROM " + tableName + " p " +
        //                      " WHERE MDSYS.SDO_RELATE(p.GEOMETRY," +
        //                                        "SDO_CS.MAKE_3D(SDO_CS.TRANSFORM(" +
        //                                        "MDSYS.SDO_GEOMETRY (2003, 22195, NULL, " +
        //                                        "MDSYS.SDO_ELEM_INFO_ARRAY (1,1003,3), " +
        //                                        "MDSYS.SDO_ORDINATE_ARRAY (" + x1.ToString().Replace(",", ".") + ", " + y1.ToString().Replace(",", ".") + ", " + x2.ToString().Replace(",", ".") + ", " + y2.ToString().Replace(",", ".") + ")) " +
        //                                        ",8307), 0,8307) " +
        //                                        ",'MASK=" + mask + " QUERYTYPE=WINDOW') = 'TRUE'";

        //        IDbCommand objComm = _context.Database.Connection.CreateCommand();
        //        _context.Database.Connection.Open();
        //        objComm.CommandText = sSql;
        //        using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //        {
        //            List<LayerGraf> lstLayerGrafs = new List<LayerGraf>();
        //            LayerGraf layerGrafRead;
        //            while (data.Read())
        //            {
        //                layerGrafRead = new LayerGraf();
        //                layerGrafRead.FeatId = data.GetInt64(0);
        //                string wkt = string.Empty;
        //                if (campoNombre != string.Empty)
        //                {
        //                    layerGrafRead.Nombre = data.GetString(1);
        //                    wkt = data.GetString(2);
        //                }
        //                else
        //                {
        //                    layerGrafRead.Nombre = string.Empty;
        //                    wkt = data.GetString(1);
        //                }
        //                if (wkt != null && wkt != string.Empty)
        //                {
        //                    DbGeometry geom = null;
        //                    if (wkt.Contains("LINE"))
        //                    {
        //                        if (wkt.Contains("MULTILINE"))
        //                        {
        //                            geom = DbGeometry.MultiLineFromText(wkt, 22195);
        //                        }
        //                        else
        //                        {
        //                            geom = DbGeometry.LineFromText(wkt, 22195);
        //                        }
        //                    }
        //                    else if (wkt.Contains("POLYGON"))
        //                    {
        //                        geom = DbGeometry.PolygonFromText(wkt, 22195);
        //                    }
        //                    else if (wkt.Contains("POINT"))
        //                    {
        //                        geom = DbGeometry.PointFromText(wkt, 22195);
        //                    }
        //                    layerGrafRead.Geom = geom;
        //                }
        //                lstLayerGrafs.Add(layerGrafRead);
        //            }
        //            var layerGrafs = lstLayerGrafs.ToArray();
        //            return layerGrafs;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
        //        _context.GetLogger().LogError("GetLayerGrafByCoords", e);
        //        return null;
        //    }
        //}
        //public LayerGraf[] GetLayerGrafByCoords(string layerGrafName, double x1, double y1, double x2, double y2, string mask)
        //{
        //    try
        //    {
        //        if (mask == string.Empty)
        //        {
        //            mask = "ANYINTERACT";
        //        }
        //        string tableName = string.Empty;
        //        string campoFeatId = string.Empty;
        //        string campoNombre = string.Empty;
        //        string campoGeometry = string.Empty;
        //        layerGrafName = layerGrafName.ToUpper();
        //        if (layerGrafName.Contains("CALLE"))
        //        {
        //            tableName = "V_EJE_CALLE";
        //            campoFeatId = "FEATID";
        //            campoNombre = "NOMBRE";
        //            campoGeometry = "GEOMETRY";
        //        }
        //        else if (layerGrafName.Contains("MEJORA"))
        //        {
        //            tableName = "CT_MEJORAS";
        //            campoFeatId = "FEATID";
        //            campoNombre = string.Empty;
        //            campoGeometry = "GEOMETRY";
        //        }
        //        string sSql = "SELECT p." + campoFeatId + (campoNombre != string.Empty ? ", p." + campoNombre : " ") + " " +
        //                             ", TO_CHAR(SDO_CS.TRANSFORM(SDO_CS.MAKE_2D(p." + campoGeometry + ", 8307), 22195).Get_WKT()) as WKT " +
        //                      " FROM " + tableName + " p " +
        //                      " WHERE MDSYS.SDO_RELATE(p.GEOMETRY," +
        //                                        "SDO_CS.MAKE_3D(SDO_CS.TRANSFORM(" +
        //                                        "MDSYS.SDO_GEOMETRY (2003, 22195, NULL, " +
        //                                        "MDSYS.SDO_ELEM_INFO_ARRAY (1,1003,3), " +
        //                                        "MDSYS.SDO_ORDINATE_ARRAY (" + x1.ToString().Replace(",", ".") + ", " + y1.ToString().Replace(",", ".") + ", " + x2.ToString().Replace(",", ".") + ", " + y2.ToString().Replace(",", ".") + ")) " +
        //                                        ",8307), 0,8307) " +
        //                                        ",'MASK=" + mask + " QUERYTYPE=WINDOW') = 'TRUE'";
        //        //SDO_CS.TRANSFORM(SDO_CS.MAKE_2D(p.geometry, 8307), 1000003)
        //        IDbCommand objComm = _context.Database.Connection.CreateCommand();
        //        _context.Database.Connection.Open();
        //        objComm.CommandText = sSql;
        //        using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //        {
        //            List<LayerGraf> lstLayerGrafs = new List<LayerGraf>();
        //            LayerGraf layerGrafRead;
        //            while (data.Read())
        //            {
        //                layerGrafRead = new LayerGraf();
        //                layerGrafRead.FeatId = data.GetInt64(0);
        //                string wkt = string.Empty;
        //                if (campoNombre != string.Empty)
        //                {
        //                    layerGrafRead.Nombre = data.GetString(1);
        //                    wkt = data.GetString(2);
        //                }
        //                else
        //                {
        //                    layerGrafRead.Nombre = string.Empty;
        //                    wkt = data.GetString(1);
        //                }
        //                if (wkt != null && wkt != string.Empty)
        //                {
        //                    DbGeometry geom = null;
        //                    if (wkt.Contains("LINE"))
        //                    {
        //                        if (wkt.Contains("MULTILINE"))
        //                        {
        //                            geom = DbGeometry.MultiLineFromText(wkt, 22195);
        //                        }
        //                        else
        //                        {
        //                            geom = DbGeometry.LineFromText(wkt, 22195);
        //                        }
        //                    }
        //                    else if (wkt.Contains("POLYGON"))
        //                    {
        //                        geom = DbGeometry.PolygonFromText(wkt, 22195);
        //                    }
        //                    else if (wkt.Contains("POINT"))
        //                    {
        //                        geom = DbGeometry.PointFromText(wkt, 22195);
        //                    }
        //                    layerGrafRead.Geom = geom;
        //                }
        //                lstLayerGrafs.Add(layerGrafRead);
        //            }
        //            var layerGrafs = lstLayerGrafs.ToArray();
        //            return layerGrafs;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
        //        _context.GetLogger().LogError("GetLayerGrafByCoords2", e);
        //        return null;
        //    }
        //}
        //public LayerGraf[] GetLayerGrafByBuffer(string tableName, string campoFeatId, string campoNombre, string campoGeometry, string tableNameMaster, string campoFeatIdMaster, string campoGeometryMaster, long featIdMaster, int distBuffer, string mask)
        //{
        //    try
        //    {
        //        if (mask == string.Empty)
        //        {
        //            mask = "anyinteract";
        //        }
        //        string sSql = "SELECT p." + campoFeatId + (campoNombre != string.Empty ? ", p." + campoNombre : " ") + " " +
        //                             ", TO_CHAR(SDO_CS.TRANSFORM(SDO_CS.MAKE_2D(p." + campoGeometry + ", 8307), 22195).Get_WKT()) as WKT " +
        //                      " FROM " + tableName + " p " + ", " + tableNameMaster + " m " +
        //                      " WHERE MDSYS.SDO_RELATE(p.GEOMETRY," +
        //                                        "SDO_GEOM.SDO_BUFFER(m." + campoGeometryMaster + ", " + distBuffer.ToString() + ", 0.05, 'unit=m arc_tolerance=0.05') " +
        //                                        ",'MASK=" + mask + " QUERYTYPE=WINDOW') = 'TRUE'" +
        //                      "   AND m." + campoFeatIdMaster + "=" + featIdMaster.ToString() + " ";
        //        //SDO_CS.TRANSFORM(SDO_CS.MAKE_2D(p.geometry, 8307), 1000003)
        //        IDbCommand objComm = _context.Database.Connection.CreateCommand();
        //        _context.Database.Connection.Open();
        //        objComm.CommandText = sSql;
        //        using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //        {
        //            List<LayerGraf> lstLayerGrafs = new List<LayerGraf>();
        //            LayerGraf layerGrafRead;
        //            while (data.Read())
        //            {
        //                layerGrafRead = new LayerGraf();
        //                layerGrafRead.FeatId = data.GetInt64(0);
        //                string wkt = string.Empty;
        //                if (campoNombre != string.Empty)
        //                {
        //                    layerGrafRead.Nombre = data.GetString(1);
        //                    wkt = data.GetString(2);
        //                }
        //                else
        //                {
        //                    layerGrafRead.Nombre = string.Empty;
        //                    wkt = data.GetString(1);
        //                }
        //                if (wkt != null && wkt != string.Empty)
        //                {
        //                    DbGeometry geom = null;
        //                    if (wkt.IndexOf("LINE") > -1)
        //                    {
        //                        //geom = DbGeometry.LineFromText(wkt, 22195);
        //                        if (wkt.Contains("MULTILINE"))
        //                        {
        //                            geom = DbGeometry.MultiLineFromText(wkt, 22195);
        //                        }
        //                        else
        //                        {
        //                            geom = DbGeometry.LineFromText(wkt, 22195);
        //                        }
        //                    }
        //                    else if (wkt.IndexOf("POLYGON") > -1)
        //                    {
        //                        //geom = DbGeometry.PolygonFromText(wkt, 22195);
        //                        if (wkt.Contains("MULTIPOLYGON"))
        //                        {
        //                            geom = DbGeometry.MultiPolygonFromText(wkt, 22195);
        //                        }
        //                        else
        //                        {
        //                            geom = DbGeometry.PolygonFromText(wkt, 22195);
        //                        }
        //                    }
        //                    layerGrafRead.Geom = geom;
        //                }
        //                lstLayerGrafs.Add(layerGrafRead);
        //            }
        //            var layerGrafs = lstLayerGrafs.ToArray();
        //            return layerGrafs;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
        //        _context.GetLogger().LogError("GetLayerGrafByBuffer", e);
        //        return null;
        //    }
        //}
        //public LayerGraf[] GetLayerGrafById(string tableName, string campoFeatId, string campoNombre, string campoGeometry, string campoId, string id, bool cambioCoords)
        //{
        //    try
        //    {
        //        string sSql = "SELECT p." + campoFeatId + (campoNombre != string.Empty ? ", p." + campoNombre : " ") + " " +
        //                                  (cambioCoords ? ", TO_CHAR(SDO_CS.TRANSFORM(SDO_CS.MAKE_2D(p." + campoGeometry + ", 8307), 22195).Get_WKT()) as WKT " : ",  TO_CHAR(SDO_CS.MAKE_2D(p." + campoGeometry + ", 22195).Get_WKT()) as WKT ") + " " +
        //                      " FROM " + tableName + " p " +
        //                      (id != string.Empty ? " WHERE " + campoId + " = " + id + " " : " ");
        //        //SDO_CS.TRANSFORM(SDO_CS.MAKE_2D(p.geometry, 8307), 1000003)
        //        IDbCommand objComm = _context.Database.Connection.CreateCommand();
        //        _context.Database.Connection.Open();
        //        objComm.CommandText = sSql;
        //        using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //        {
        //            List<LayerGraf> lstLayerGrafs = new List<LayerGraf>();
        //            LayerGraf layerGrafRead;
        //            while (data.Read())
        //            {
        //                layerGrafRead = new LayerGraf();
        //                layerGrafRead.FeatId = data.GetInt64(0);
        //                string wkt = string.Empty;
        //                if (campoNombre != string.Empty)
        //                {
        //                    layerGrafRead.Nombre = data.GetString(1);
        //                    wkt = data.GetString(2);
        //                }
        //                else
        //                {
        //                    layerGrafRead.Nombre = string.Empty;
        //                    wkt = data.GetString(1);
        //                }
        //                if (wkt != null && wkt != string.Empty)
        //                {
        //                    DbGeometry geom = null;
        //                    if (wkt.Contains("LINE"))
        //                    {
        //                        if (wkt.Contains("MULTILINE"))
        //                        {
        //                            geom = DbGeometry.MultiLineFromText(wkt, 22195);
        //                        }
        //                        else
        //                        {
        //                            geom = DbGeometry.LineFromText(wkt, 22195);
        //                        }
        //                    }
        //                    else if (wkt.Contains("POLYGON"))
        //                    {
        //                        //geom = DbGeometry.PolygonFromText(wkt, 22195);
        //                        if (wkt.Contains("MULTIPOLYGON"))
        //                        {
        //                            geom = DbGeometry.MultiPolygonFromText(wkt, 22195);
        //                        }
        //                        else
        //                        {
        //                            geom = DbGeometry.PolygonFromText(wkt, 22195);
        //                        }
        //                    }
        //                    else if (wkt.Contains("POINT"))
        //                    {
        //                        geom = DbGeometry.PointFromText(wkt, 22195);
        //                    }
        //                    layerGrafRead.Geom = geom;
        //                }
        //                lstLayerGrafs.Add(layerGrafRead);
        //            }
        //            var layerGrafs = lstLayerGrafs.ToArray();
        //            return layerGrafs;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
        //        _context.GetLogger().LogError("GetLayerGrafById", e);
        //        return null;
        //    }
        //}
        //public LayerGraf[] GetLayerGrafById(string layerGrafName, string id, bool cambioCoords)
        //{
        //    try
        //    {
        //        string tableName = string.Empty;
        //        string campoFeatId = string.Empty;
        //        string campoNombre = string.Empty;
        //        string campoGeometry = string.Empty;
        //        string campoId = string.Empty;
        //        layerGrafName = layerGrafName.ToUpper();
        //        if (layerGrafName.Contains("CALLE"))
        //        {
        //            tableName = "V_EJE_CALLE";
        //            campoFeatId = "FEATID";
        //            campoNombre = "NOMBRE";
        //            campoGeometry = "GEOMETRY";
        //        }
        //        else if (layerGrafName.Contains("MEJORA"))
        //        {
        //            tableName = "CT_MEJORAS";
        //            campoFeatId = "FEATID";
        //            campoNombre = string.Empty;
        //            campoGeometry = "GEOMETRY";
        //        }
        //        else if (layerGrafName.Contains("PLANTLINEA"))
        //        {
        //            tableName = "MP_PLANTLINEAS";
        //            campoFeatId = "FEATID";
        //            campoNombre = string.Empty;
        //            campoGeometry = "GEOMETRY";
        //            campoId = string.Empty;
        //        }
        //        else if (layerGrafName.Contains("PLANTTEXTO"))
        //        {
        //            tableName = "MP_PLANTTEXTOS";
        //            campoFeatId = "FEATID";
        //            campoNombre = "TEXTO";
        //            campoGeometry = "GEOMETRY";
        //            campoId = string.Empty;
        //        }
        //        string sSql = "SELECT p." + campoFeatId + (campoNombre != string.Empty ? ", p." + campoNombre : " ") + " " +
        //                                  (cambioCoords ? ", TO_CHAR(SDO_CS.TRANSFORM(SDO_CS.MAKE_2D(p." + campoGeometry + ", 8307), 22195).Get_WKT()) as WKT " : ",  TO_CHAR(SDO_CS.MAKE_2D(p." + campoGeometry + ", 22195).Get_WKT()) as WKT ") + " " +
        //                      " FROM " + tableName + " p " +
        //                      (id != string.Empty ? " WHERE " + campoId + " = " + id + " " : " ");
        //        //SDO_CS.TRANSFORM(SDO_CS.MAKE_2D(p.geometry, 8307), 1000003)
        //        IDbCommand objComm = _context.Database.Connection.CreateCommand();
        //        _context.Database.Connection.Open();
        //        objComm.CommandText = sSql;
        //        using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //        {
        //            List<LayerGraf> lstLayerGrafs = new List<LayerGraf>();
        //            LayerGraf layerGrafRead;
        //            while (data.Read())
        //            {
        //                layerGrafRead = new LayerGraf();
        //                layerGrafRead.FeatId = data.GetInt64(0);
        //                string wkt = string.Empty;
        //                if (campoNombre != string.Empty)
        //                {
        //                    layerGrafRead.Nombre = data.GetString(1);
        //                    wkt = data.GetString(2);
        //                }
        //                else
        //                {
        //                    layerGrafRead.Nombre = string.Empty;
        //                    wkt = data.GetString(1);
        //                }
        //                if (wkt != null && wkt != string.Empty)
        //                {
        //                    DbGeometry geom = null;
        //                    if (wkt.Contains("LINE"))
        //                    {
        //                        if (wkt.Contains("MULTILINE"))
        //                        {
        //                            geom = DbGeometry.MultiLineFromText(wkt, 22195);
        //                        }
        //                        else
        //                        {
        //                            geom = DbGeometry.LineFromText(wkt, 22195);
        //                        }
        //                    }
        //                    else if (wkt.Contains("POLYGON"))
        //                    {
        //                        if (wkt.Contains("MULTIPOLYGON"))
        //                        {
        //                            geom = DbGeometry.MultiPolygonFromText(wkt, 22195);
        //                        }
        //                        else
        //                        {
        //                            geom = DbGeometry.PolygonFromText(wkt, 22195);
        //                        }
        //                    }
        //                    else if (wkt.Contains("POINT"))
        //                    {
        //                        geom = DbGeometry.PointFromText(wkt, 22195);
        //                    }
        //                    layerGrafRead.Geom = geom;
        //                }
        //                lstLayerGrafs.Add(layerGrafRead);
        //            }
        //            var layerGrafs = lstLayerGrafs.ToArray();
        //            return layerGrafs;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
        //        _context.GetLogger().LogError(string.Format("GetLayerGrafById(id: {0}, cambioCoords: {1})", id, cambioCoords), e);
        //        return null;
        //    }
        //} 
        #endregion

        public LayerGraf[] GetLayerGrafByCoords(Layer layer, Componente componente, double x1, double y1, double x2, double y2)
        {
            string sanitize(double val) => val.ToString().Replace(",", ".");

            return GetLayerGrafByCoords(layer, componente, new[] { string.Format("{0},{1}", sanitize(x1), sanitize(y1)), string.Format("{0},{1}", sanitize(x2), sanitize(y2)) }.ToList());
        }
        public LayerGraf[] GetLayerGrafByCoords(Layer layer, Componente componente, List<string> lstCoordsGeometry)
        {
            if (lstCoordsGeometry != null && lstCoordsGeometry.Count > 0)
            {
                Atributo campoFeatId = null;
                Atributo campoLabel = null;
                Atributo campoGeometry = null;
                Atributo campoId = null;

                try
                {
                    campoFeatId = componente.Atributos.GetAtributoFeatId();
                    campoId = componente.Atributos.GetAtributoClave();

                    campoGeometry = componente.Atributos.GetAtributoGeometry();

                    if (layer.EtiquetaIdAtributo != null && layer.EtiquetaIdAtributo > 0)
                    {
                        try
                        {
                            campoLabel = componente.Atributos.Single(p => p.AtributoId == layer.EtiquetaIdAtributo);
                        }
                        catch
                        {
                            throw new ApplicationException(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el layer {1} de la plantilla {2}", layer.EtiquetaIdAtributo, layer.Nombre, layer.IdPlantilla));
                        }
                    }
                }
                catch (ApplicationException appEx)
                {
                    _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
                    return null;
                }

                try
                {

                    if (lstCoordsGeometry.Count == 2) //es extent/bbox
                    {
                        var c1 = lstCoordsGeometry[0].Split(',').Select(c => Convert.ToDouble(c.Trim()));
                        var c2 = lstCoordsGeometry[1].Split(',').Select(c => Convert.ToDouble(c.Trim()));
                        lstCoordsGeometry.Insert(1, string.Format("{0}, {1}", c1.ElementAt(0), c2.ElementAt(1)));
                        lstCoordsGeometry.AddRange(new[] { string.Format("{0}, {1}", c1.ElementAt(1), c2.ElementAt(0)), lstCoordsGeometry[0] });
                    }

                    var componenteGrafico = string.IsNullOrEmpty(componente.TablaGrafica) ?
                                            componente :
                                            new Componente { Esquema = componente.Esquema, Tabla = componente.TablaGrafica };

                    using (var qbuilder = this._context.CreateSQLQueryBuilder())
                    {
                        SQLSpatialRelationships relations = layer.FiltroGeografico == 1
                                                                ? SQLSpatialRelationships.Inside | SQLSpatialRelationships.CoveredBy | SQLSpatialRelationships.Equal
                                                                : SQLSpatialRelationships.AnyInteract;
                        string aliasGraf = "t1";

                        qbuilder
                            .AddTable(componente, aliasGraf)
                            .AddFields(campoFeatId, campoLabel)
                            .AddGeometryField(qbuilder.CreateGeometryFieldBuilder(campoGeometry, aliasGraf).ChangeToSRID(SRID.App).ToWKT(), campoGeometry.Campo);

                        if (componenteGrafico.ComponenteId != componente.ComponenteId)
                        {
                            aliasGraf = "graf";
                            qbuilder.AddJoin(componenteGrafico, aliasGraf, campoId, campoId, SQLJoin.Inner);
                        }

                        var transientGeom = qbuilder
                                                .CreateGeometryFieldBuilder(this._context.CreateParameter(":wkt", string.Format("POLYGON(({0}))", string.Join(",", lstCoordsGeometry.Select(c => string.Join(" ", c.Split(',').Select(p => p.Trim())))))), 3857)
                                                .ChangeToSRID(SRID.DB);
                        qbuilder
                            .AddFilter(qbuilder.CreateGeometryFieldBuilder(campoGeometry, aliasGraf), transientGeom, relations, SQLConnectors.None);

                        if (layer.FiltroGeografico == 1)
                        {
                            var paramGral = _context.ParametrosGenerales.FirstOrDefault(pg => pg.Descripcion.ToUpper() == "TOLERANCIAFILTROPRINCIPALPLOTEO");
                            transientGeom.AddBuffer(paramGral == null ? 1.0 : Convert.ToDouble(paramGral.Valor));
                        }
                        else if (layer.FiltroGeografico == 2)
                        {
                            using (var innerBuilder = this._context.CreateSQLQueryBuilder())
                            {
                                innerBuilder
                                    .AddTable(componenteGrafico, "inner")
                                    .AddFilter(innerBuilder.CreateGeometryFieldBuilder(campoGeometry, "inner"), transientGeom, SQLSpatialRelationships.Inside, SQLConnectors.None)
                                    .AddFields(campoFeatId);

                                qbuilder.AddFilter(campoFeatId, string.Format("({0})", innerBuilder), SQLOperators.NotIn, SQLConnectors.And);
                            }
                        }

                        try
                        {
                            return qbuilder
                                    .ExecuteQuery((IDataReader reader) =>
                                            new LayerGraf()
                                            {
                                                FeatId = reader.GetNullableInt64(reader.GetOrdinal(campoFeatId.Campo)).Value,
                                                Nombre = campoLabel == null ? string.Empty : reader.GetStringOrEmpty(reader.GetOrdinal(campoLabel.Campo)),
                                                Geom = reader.GetGeometryFromField(reader.GetOrdinal(campoGeometry.Campo), SRID.App)
                                            })
                                    .Where((obj, idx) => layer.ComponenteId != COMPONENTE_ID_ARCO || idx % 4 == 0)
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
                    _context.GetLogger().LogError("ExecuteQuery", ex);
                }
            }
            return null;
        }
        #region comento porque no se usa
        //public LayerGraf[] GetLayerGrafByObjetoBase(Layer layer, Componente componente, Componente componenteBase, string idObjetoBase)
        //{
        //    if (string.IsNullOrEmpty(idObjetoBase) || layer.FiltroGeografico != 1)
        //    {
        //        return null;
        //    }
        //    try
        //    {
        //        var campoId = componente.Atributos.GetAtributoClave();
        //        var campoFeatId = componente.Atributos.GetAtributoFeatId();
        //        var campoGeometry = componente.Atributos.GetAtributoGeometry();
        //        var campoLabel = componente.Atributos.FirstOrDefault(atr => layer.EtiquetaIdAtributo.GetValueOrDefault() != 0 && atr.AtributoId == layer.EtiquetaIdAtributo.Value);

        //        var campoIdBase = componenteBase.Atributos.GetAtributoClave();
        //        var campoGeometryBase = componenteBase.Atributos.GetAtributoGeometry();

        //        using (var builder = this._context.CreateSQLQueryBuilder())
        //        {
        //            SQLConnectors connector = SQLConnectors.None;
        //            builder.AddTable(componente, "t1")
        //                   .AddFields(campoId, campoLabel)
        //                   .AddGeometryField(builder.CreateGeometryFieldBuilder(campoGeometry, "t1").ChangeToSRID(22195).ToWKT(), "geom");
        //            if (componente.ComponenteId != componenteBase.ComponenteId)
        //            {
        //                connector = SQLConnectors.And;
        //                builder.AddTable(componenteBase, "t2")
        //                       .AddFilter(builder.CreateGeometryFieldBuilder(campoGeometry, "t2"), builder.CreateGeometryFieldBuilder(campoGeometryBase, "t1"), SQLSpatialRelation.AnyInteract);

        //                if (layer.Componente.ComponenteId == COMPONENTE_ID_ARCO)
        //                {
        //                    var g1 = builder.CreateGeometryFieldBuilder(campoGeometry, "t1");
        //                    var g2 = builder.CreateGeometryFieldBuilder(campoGeometry, "t2").OverlappingArea(g1);

        //                    builder.AddRawFilter(string.Format("({0} / {1})", g2, g1.ToPolygon().Area()), 0.9, ">", SQLConnectors.And);
        //                }
        //            }

        //            return builder.AddFilter(campoIdBase, idObjetoBase, "=", connector)
        //                          .ExecuteQuery((IDataReader reader) =>
        //                          {
        //                              return new LayerGraf()
        //                              {
        //                                  FeatId = reader.GetNullableInt64(0).Value,
        //                                  Nombre = campoLabel == null ? string.Empty : reader.GetStringOrEmpty(1),
        //                                  Geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"))
        //                              };
        //                          })
        //                          .Where((g, idx) => layer.ComponenteId != COMPONENTE_ID_ARCO || idx % 4 == 0)
        //                          .ToArray();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _context.GetLogger().LogError("GetLayerGrafByObjetoBase", ex);
        //        return null;
        //    }
        //}
        //public LayerGraf[] GetLayerGrafById(Layer layer, Componente componente, string id)
        //{
        //    try
        //    {
        //        Atributo campoFeatId = null;
        //        Atributo campoLabel = null;
        //        Atributo campoGeometry = null;
        //        Atributo campoId = null;

        //        try
        //        {
        //            campoFeatId = componente.Atributos.GetAtributoFeatId();
        //            campoId = componente.Atributos.GetAtributoClave();

        //            campoGeometry = componente.Atributos.GetAtributoGeometry();

        //            if (layer.EtiquetaIdAtributo != null && layer.EtiquetaIdAtributo > 0)
        //            {
        //                try
        //                {
        //                    campoLabel = componente.Atributos.Single(p => p.AtributoId == layer.EtiquetaIdAtributo);
        //                }
        //                catch
        //                {
        //                    throw new ApplicationException(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el layer {1} de la plantilla {2}", layer.EtiquetaIdAtributo, layer.Nombre, layer.IdPlantilla));
        //                }
        //            }
        //        }
        //        catch (ApplicationException appEx)
        //        {
        //            _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
        //            return null;
        //        }
        //        using (var builder = this._context.CreateSQLQueryBuilder())
        //        {
        //            builder.AddTable(componente, "t1")
        //                   .AddFields(campoFeatId, campoLabel)
        //                   .AddGeometryField(builder.CreateGeometryFieldBuilder(campoGeometry, "t1").ChangeToSRID(22195), "geom");

        //            if (!string.IsNullOrEmpty(id))
        //            {
        //                builder.AddFilter(campoId, id, "=");
        //            }

        //            return builder.ExecuteQuery((IDataReader reader) =>
        //            {
        //                return new LayerGraf()
        //                {
        //                    FeatId = reader.GetNullableInt64(0).Value,
        //                    Nombre = campoLabel == null ? string.Empty : reader.GetStringOrEmpty(1),
        //                    Geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"))
        //                };
        //            }).ToArray();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _context.GetLogger().LogError(string.Format("GetLayerGrafById(layer: {0}, componente: {1}, id: {2})", layer.IdLayer, componente.ComponenteId, id), ex);
        //        return null;
        //    }
        //} 
        #endregion
        public string GetLayerGrafTextById(Componente componente, string id, long idAtributo)
        {
            try
            {
                Atributo campoAtributo = null;
                Atributo campoId = null;
                try
                {
                    campoId = componente.Atributos.GetAtributoClave();

                    try
                    {
                        campoAtributo = componente.Atributos.Single(p => idAtributo > 0 && p.AtributoId == idAtributo);
                    }
                    catch
                    {
                        throw new ApplicationException(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el componente {1}", idAtributo, componente.Nombre));
                    }
                }
                catch (ApplicationException appEx)
                {
                    _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
                    return string.Empty;
                }
                using (var builder = _context.CreateSQLQueryBuilder())
                {
                    builder.AddTable(componente, "t1")
                           .AddFields(campoAtributo);

                    if (!string.IsNullOrEmpty(id))
                    {
                        builder.AddFilter(campoId, id, SQLOperators.EqualsTo);
                    }
                    return builder.ExecuteQuery((IDataReader reader, ReaderStatus status) =>
                    {
                        status.Break();
                        return reader.GetStringOrEmpty(0);
                    }).Single();
                }
            }
            catch (Exception e)
            {
                _context.GetLogger().LogError(string.Format("GetLayerGrafTextById(componente: {0}, id: {1}, atributo: {2})", componente.ComponenteId, id, idAtributo), e);
                return string.Empty;
            }
        }
        public string GetLayerGrafDistritoById(Componente componente, string id)
        {
            try
            {
                Atributo campoId = null;
                try
                {
                    campoId = componente.Atributos.GetAtributoClave();
                }
                catch (ApplicationException appEx)
                {
                    _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
                    return null;
                }
                using (var builder = _context.CreateSQLQueryBuilder())
                {
                    var campoIdDistrito = new Atributo { Campo = "id_distrito" };

                    builder.AddTable("ct_distrito", "t1")
                           .AddJoin(componente, "t2", new Atributo { Campo = campoIdDistrito.Campo, ComponenteId = componente.ComponenteId }, campoIdDistrito, SQLJoin.Inner)
                           .AddFormattedField("distinct ({0} || ' - ' || {1})", campoIdDistrito, new Atributo { Campo = "nombre" });

                    if (!string.IsNullOrEmpty(id))
                    {
                        builder.AddFilter(campoId, id, SQLOperators.EqualsTo);
                    }
                    return builder.ExecuteQuery((IDataReader reader, ReaderStatus status) =>
                    {
                        status.Break();
                        return reader.GetStringOrEmpty(0);
                    }).SingleOrDefault();
                }
            }
            catch (Exception e)
            {
                _context.GetLogger().LogError(string.Format("GetLayerGrafDistritoById(componente: {0}, id: {1})", componente.ComponenteId, id), e);
                return string.Empty;
            }
        }
        public string GetLayerGrafDistritoByCoords(double x, double y)
        {
            try
            {
                using (var builder = _context.CreateSQLQueryBuilder())
                {
                    builder.AddTable("ct_distrito", "t1")
                           .AddFilter(builder.CreateGeometryFieldBuilder(new Atributo { Campo = "geometry" }, "t1"), builder.CreateGeometryFieldBuilder(string.Format("POINT({0} {1})", x, y), SRID.App)
                                                                                                                            .ChangeToSRID(SRID.DB), SQLSpatialRelationships.AnyInteract)
                           .AddFields(new Atributo { Campo = "id_distrito" }, new Atributo { Campo = "nombre" });

                    return builder.ExecuteQuery((IDataReader reader, ReaderStatus status) =>
                    {
                        status.Break();
                        return string.Format("{0} - {1}", reader.GetStringOrEmpty(0), reader.GetStringOrEmpty(1));
                    }).SingleOrDefault();
                }
            }
            catch (Exception e)
            {
                _context.GetLogger().LogError(string.Format("GetLayerGrafDistritoByCoords(x: {0}, y: {1})", x, y), e);
                return string.Empty;
            }
        }

        public LayerGraf[] GetLayerGrafByMapaTematico(string guid)
        {
            try
            {
                using (var builder = _context.CreateSQLQueryBuilder())
                {
                    var campoGeometry = new Atributo { Campo = "geometry" };
                    try
                    {
                        return builder.AddTable("mt_objeto_resultado", "t1")
                                      .AddFields("featid", "rango", "descripcion")
                                      .AddGeometryField(builder.CreateGeometryFieldBuilder(campoGeometry, "t1").ChangeToSRID(SRID.App).ToWKT(), "geom")
                                      .AddFilter(new Atributo { Campo = "guid", TipoDatoId = 6 }, guid, SQLOperators.EqualsTo)
                                      .ExecuteQuery((IDataReader reader) =>
                                      {
                                          return new LayerGraf
                                          {
                                              FeatId = reader.GetNullableInt64(0).Value,
                                              Nombre = reader.GetStringOrEmpty(1),
                                              Descripcion = reader.GetStringOrEmpty(2),
                                              Geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"))
                                          };
                                      }).ToArray();

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
                _context.GetLogger().LogError(string.Format("GetLayerGrafByMapaTematico(guid: {0})", guid), ex);
                return null;
            }
        }

        public LayerGraf[] GetLayerGrafById(Layer layer, Componente componente, string id, List<string> lstCoordsGeometry)
        {
            Atributo campoFeatId = null;
            Atributo campoLabel = null;
            Atributo campoGeometry = null;
            Atributo campoId = null;

            try
            {
                campoId = componente.Atributos.GetAtributoClave();
                campoFeatId = componente.Atributos.GetAtributoFeatId();

                campoGeometry = componente.Atributos.GetAtributoGeometry();

                if (layer.EtiquetaIdAtributo != null && layer.EtiquetaIdAtributo > 0)
                {
                    try
                    {
                        campoLabel = componente.Atributos.First(p => p.AtributoId == layer.EtiquetaIdAtributo);
                    }
                    catch
                    {
                        throw new ApplicationException(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el layer {1} de la plantilla {2}", layer.EtiquetaIdAtributo, layer.Nombre, layer.IdPlantilla));
                    }
                }
            }
            catch (ApplicationException appEx)
            {
                _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
                return null;
            }
            try
            {
                using (var builder = _context.CreateSQLQueryBuilder())
                {
                    string esquema = componente.Esquema;
                    string tableName = componente.Tabla;
                    string tablaGrafica = componente.TablaGrafica ?? componente.Tabla;
                    string aliasTablaGrafica = "t1";
                    var campoOrientacion = layer.PuntoAtributoOrientacion ? new Atributo() { ComponenteId = componente.ComponenteId, Campo = "GEOMETRY_ORIENT" } : null;

                    builder.AddTable(componente, "t1")
                           .AddFilter(campoId, id, SQLOperators.EqualsTo);
                    if (!string.IsNullOrEmpty(layer.CapaFiltro))
                    {
                        builder.AddRawFilter(layer.CapaFiltro, SQLConnectors.And);
                    }
                    if (tableName != tablaGrafica)
                    {
                        aliasTablaGrafica = "graf";
                        builder.AddJoin(new Componente() { ComponenteId = -1, Esquema = esquema, Tabla = tablaGrafica },
                                        aliasTablaGrafica,
                                        new Atributo() { ComponenteId = -1, Campo = campoFeatId.Campo },
                                        campoFeatId);
                    }
                    var geom = builder.CreateGeometryFieldBuilder(campoGeometry, aliasTablaGrafica);
                    if (lstCoordsGeometry.Any())
                    {
                        geom.IntersectionWith(builder.CreateGeometryFieldBuilder("", SRID.DB));
                    }
                    builder.AddFields(campoFeatId, campoLabel, campoOrientacion)
                           .AddGeometryField(geom.ChangeToSRID(SRID.App).ToWKT(), "geom");

                    return builder.ExecuteQuery((IDataReader reader) =>
                    {
                        return new LayerGraf()
                        {
                            FeatId = reader.GetNullableInt64(0).Value,
                            Nombre = campoLabel == null ? null : reader.GetStringOrEmpty(1),
                            Geom = reader.GetGeometryFromField(reader.GetOrdinal("geom")),
                            Rotation = campoOrientacion == null ? null : reader.GetNullableInt32(reader.GetOrdinal("GEOMETRY_ORIENT"))
                        };
                    }).ToArray();
                }

                //var fields = new List<string>();
                //var tables = new List<string>();
                //var where = new List<string>();

                ////Por defecto es conectado por lineas rectas, sino rectangulo
                //string sdo_interpretation = lstCoordsGeometry.Count == 2 ? "3" : "1";

                //fields.Add(string.Format("{0}.{1}", aliasTablaGrafica, campoFeatId));

                //if (!string.IsNullOrEmpty(campoLabel))
                //{
                //    fields.Add(string.Format("TO_CHAR(t1.{0})", campoLabel));
                //}
                //if (lstCoordsGeometry.Count > 0)
                //    fields.Add(string.Format(" SDO_CS.TRANSFORM(SDO_GEOM.SDO_INTERSECTION({0}.{1}," +
                //            "SDO_CS.TRANSFORM(MDSYS.SDO_GEOMETRY (2003, 22195, NULL, MDSYS.SDO_ELEM_INFO_ARRAY (1,1003,{3}), MDSYS.SDO_ORDINATE_ARRAY ({2})),(SELECT SRID FROM ALL_SDO_GEOM_METADATA WHERE OWNER = '" + esquema + "' AND TABLE_NAME = '" + tablaGrafica + "' AND COLUMN_NAME = '" + campoGeometry + "')),0.05),8307,22195) " +
                //            ".Get_WKT()", aliasTablaGrafica, campoGeometry, string.Join(",", lstCoordsGeometry), sdo_interpretation));
                //else
                //    fields.Add(string.Format("SDO_CS.TRANSFORM({0}.{1}, 8307, 22195).Get_WKT()", aliasTablaGrafica, campoGeometry));
                //if (layer.PuntoAtributoOrientacion)
                //{
                //    fields.Add("GEOMETRY_ORIENT");
                //}
                //if (!String.IsNullOrEmpty(layer.CapaFiltro))
                //{
                //    where.Add(layer.CapaFiltro);
                //}

                //using (IDbCommand objComm = _context.Database.Connection.CreateCommand())
                //{
                //    if (_context.Database.Connection.State == ConnectionState.Open)
                //        _context.Database.Connection.Close();
                //    _context.Database.Connection.Open();
                //    objComm.CommandText = string.Format("select {0} from {1} where {2}", string.Join(",", fields), string.Join(",", tables), string.Join(" AND ", where));
                //    try
                //    {
                //        using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
                //        {
                //            List<LayerGraf> lstLayerGrafs = new List<LayerGraf>();
                //            LayerGraf layerGrafRead;
                //            while (data.Read())
                //            {
                //                #region colectar datos
                //                layerGrafRead = new LayerGraf();
                //                layerGrafRead.FeatId = data.GetNullableInt64(0).Value;
                //                int idx = 1;
                //                if (campoLabel != string.Empty)
                //                {
                //                    layerGrafRead.Nombre = data.GetStringOrEmpty(1);
                //                    idx = 2;
                //                }
                //                else
                //                {
                //                    layerGrafRead.Nombre = string.Empty;
                //                }
                //                layerGrafRead.Geom = data.GetGeometryFromField(idx);
                //                if (data.GetSchemaTable().Columns.Contains("GEOMETRY_ORIENT"))
                //                {
                //                    layerGrafRead.Rotation = data.GetNullableInt32(++idx);
                //                }
                //                lstLayerGrafs.Add(layerGrafRead);
                //                #endregion
                //            }
                //            return lstLayerGrafs.ToArray();
                //        }
                //    }
                //    catch (DataException)
                //    {
                //        if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
                //        _context.GetLogger().LogInfo(string.Format("sql query: {0}", objComm.CommandText));
                //        throw;
                //    }
                //}
            }
            catch (Exception e)
            {
                _context.GetLogger().LogError(string.Format("GetLayerGrafById(layer: {0}, componente: {1}, id: {2})", layer.IdLayer, componente.ComponenteId, id), e);
                return null;
            }
            finally
            {
                if (_context.Database.Connection.State == ConnectionState.Open)
                    _context.Database.Connection.Close();
            }
        }

        #region comento porque no se usa. se borra mas adelante
        //public double[] GetCoordsBoxByLstIdObjAndIdComp(List<int> idsObj, int idComp)
        //{
        //    throw new NotImplementedException();
        //    #region comento hasta estar seguro de poder borrar
        //    //List<double> lstCoord = new List<double>();

        //    //string queryPrin = "";
        //    //string querySec = "";

        //    //queryPrin = "SELECT min(o.x) as minx," +
        //    //"max(o.x) as maxx," +
        //    //"min(o.y) as miny," +
        //    //"max(o.y) as maxy " +
        //    //"FROM ( {0} ) t," +
        //    //"TABLE(sdo_util.GetVertices(sdo_geom.sdo_mbr(SDO_CS.TRANSFORM(t.geometry, 8307, 22195)))) o";

        //    //string campoFeatId = string.Empty;
        //    //string campoLabel = string.Empty;
        //    //string campoGeometry = string.Empty;

        //    //Componente componente = _context.Componente.Where(c => c.ComponenteId == idComp).FirstOrDefault();
        //    //componente.Atributos = _context.Atributo.Where(a => a.ComponenteId == componente.ComponenteId).ToList();
        //    //string campoId = componente.Atributos.GetAtributoClave().Campo;
        //    //campoGeometry = componente.Atributos.GetAtributoGeometry().Campo;

        //    //string esquema = componente.Esquema;
        //    //string tableName = componente.Tabla;

        //    //string lstIds = string.Join(",", idsObj);


        //    //try
        //    //{
        //    //    querySec = string.Format("select {0} from {1}.{2} t1 where {3} in({4})",
        //    //                                                            campoGeometry, esquema, tableName, campoId, lstIds);

        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    _context.GetLogger().LogError("GetCoordsBoxByLstIdObjAndIdComp", e);
        //    //    return null;
        //    //}


        //    //queryPrin = string.Format(queryPrin, querySec);
        //    //using (IDbCommand objComm = _context.Database.Connection.CreateCommand())
        //    //{
        //    //    try
        //    //    {
        //    //        _context.Database.Connection.Open();
        //    //        objComm.CommandText = queryPrin;
        //    //        using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //    //        {
        //    //            int cont = 0;
        //    //            while (data.Read())
        //    //            {
        //    //                #region colectar datos
        //    //                //vienen como xmin,xmax, ymin,ymax
        //    //                //Y tengo que formar xmin, ymin, xmax, ymax
        //    //                //Los 0 son para coords 3d
        //    //                lstCoord.Add(data.GetNullableDouble(0).Value);
        //    //                lstCoord.Add(data.GetNullableDouble(2).Value);
        //    //                lstCoord.Add(0);
        //    //                lstCoord.Add(data.GetNullableDouble(1).Value);
        //    //                lstCoord.Add(data.GetNullableDouble(3).Value);
        //    //                lstCoord.Add(0);
        //    //                cont++;
        //    //                #endregion
        //    //            }
        //    //        }
        //    //    }
        //    //    catch (DataException)
        //    //    {
        //    //        if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
        //    //        _context.GetLogger().LogInfo(string.Format("sql query: {0}", queryPrin));
        //    //        throw;
        //    //    }
        //    //}

        //    //return lstCoord.ToArray(); 
        //    #endregion
        //}
        //public LayerGraf[] GetLayerGrafByIds(Layer layer, Componente componente, string ids)
        //{
        //    try
        //    {
        //        Atributo campoFeatId = null;
        //        Atributo campoLabel = null;
        //        Atributo campoGeometry = null;
        //        Atributo campoId = null;

        //        try
        //        {
        //            campoFeatId = componente.Atributos.GetAtributoFeatId();
        //            campoId = componente.Atributos.GetAtributoClave();
        //            campoGeometry = componente.Atributos.GetAtributoGeometry();

        //            if (layer.EtiquetaIdAtributo != null && layer.EtiquetaIdAtributo > 0)
        //            {
        //                try
        //                {
        //                    campoLabel = componente.Atributos.Single(p => p.AtributoId == layer.EtiquetaIdAtributo);
        //                }
        //                catch
        //                {
        //                    throw new ApplicationException(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el layer {1} de la plantilla {2}", layer.EtiquetaIdAtributo, layer.Nombre, layer.IdPlantilla));
        //                }
        //            }
        //        }
        //        catch (ApplicationException appEx)
        //        {
        //            _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
        //            return null;
        //        }

        //        int MAX_CANT_ID = 900;
        //        int idx = 0;
        //        var lista = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
        //        var final = new List<LayerGraf>();

        //        while (idx < lista.Count)
        //        {
        //            var objetos = lista.GetRange(idx, Math.Min(MAX_CANT_ID, lista.Count - idx));
        //            idx += MAX_CANT_ID;

        //            using (var builder = _context.CreateSQLQueryBuilder())
        //            {
        //                try
        //                {

        //                    final.AddRange(builder.AddTable(componente, "t1")
        //                                          .AddFilter(campoId, string.Format("({0})", string.Join(",", objetos)), "in")
        //                                          .AddFields(campoFeatId, campoLabel)
        //                                          .AddGeometryField(builder.CreateGeometryFieldBuilder(campoGeometry, "t1").ChangeToSRID(22195).ToWKT(), "geom")
        //                                          .ExecuteQuery((IDataReader reader) =>
        //                                          {
        //                                              return new LayerGraf
        //                                              {
        //                                                  FeatId = reader.GetNullableInt64(0).Value,
        //                                                  Nombre = campoLabel == null ? string.Empty : reader.GetStringOrEmpty(1),
        //                                                  Geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"))
        //                                              };
        //                                          }));
        //                }
        //                catch (DataException ex)
        //                {
        //                    _context.GetLogger().LogInfo(string.Format("sql query: {0}", ex.Message));
        //                    throw;
        //                }
        //            }
        //        }
        //        return final.ToArray();
        //    }
        //    catch (Exception ex)
        //    {
        //        _context.GetLogger().LogError(string.Format("GetLayerGrafByIds(layer: {0}, componente: {1}, ids: {2})", layer.IdLayer, componente.ComponenteId, ids), ex);
        //        return null;
        //    }
        //}
        //public double[] GetCoordsByRelaciones(List<Relacion> pRelaciones)
        //{
        //    List<double> lstCoord = new List<double>();

        //    string queryPrin = "";
        //    string querySec = "";

        //    queryPrin = "SELECT min(o.x) as minx," +
        //    "max(o.x) as maxx," +
        //    "min(o.y) as miny," +
        //    "max(o.y) as maxy " +
        //    "FROM ( {0} " +
        //        /*"select geometry from redes_etp3.A_VALVUL where id_valvul = 5979" +
        //        "union all" +
        //        "select geometry from redes_etp3.A_CANERI where id_caneri = 35998" +
        //        "*/" ) t," +
        //    "TABLE(sdo_util.GetVertices(sdo_geom.sdo_mbr(SDO_CS.TRANSFORM(t.geometry, 8307, 22195)))) o";

        //    for (int i = 0; i < pRelaciones.Count; i++)
        //    {
        //        List<Relacion> relacionLayer = new List<Relacion>();

        //        string campoFeatId = string.Empty;
        //        string campoLabel = string.Empty;
        //        string campoGeometry = string.Empty;
        //        string campoId = string.Empty;
        //        Componente componente;

        //        try
        //        {
        //            //Obtengo todas las relaciones que sean del mismo componente y las borro de la lista principal
        //            relacionLayer = pRelaciones.Where(p => p.TablaHijo == pRelaciones[i].TablaHijo).ToList();
        //            pRelaciones = pRelaciones.Where(p => p.TablaHijo != pRelaciones[i].TablaHijo).ToList();
        //            i--;


        //            string tabl = relacionLayer[0].TablaHijo;
        //            componente = _context.Componente.Where(c => c.Tabla == tabl).FirstOrDefault();
        //            //campoFeatId = componente.Atributos.GetAtributoFeatId().Campo;
        //            componente.Atributos = _context.Atributo.Where(a => a.ComponenteId == componente.ComponenteId).ToList();
        //            campoId = componente.Atributos.GetAtributoClave().Campo;

        //            campoGeometry = componente.Atributos.GetAtributoGeometry().Campo;
        //        }
        //        catch (ApplicationException appEx)
        //        {
        //            _context.GetLogger().LogError("Componente (" + null != null ? null : pRelaciones[0].TablaHijo + ") mal configurado.", appEx);
        //            return null;
        //        }

        //        try
        //        {
        //            string esquema = componente.Esquema;
        //            string tableName = componente.Tabla;
        //            string lstIds;


        //            lstIds = string.Join(",", relacionLayer.Select(r => r.IdTablaHijo));

        //            string query = string.Format("select {0} from {1}.{2} t1 where {3} in({4})",
        //                                                                    campoGeometry, esquema, tableName, campoId, lstIds);


        //            if (querySec != null && string.Empty != querySec)
        //                querySec += " union all ";

        //            querySec += query;

        //            //"SDO_CS.TRANSFORM(MDSYS.SDO_GEOMETRY (2003, 22195, NULL, MDSYS.SDO_ELEM_INFO_ARRAY (1,1003,{0}), MDSYS.SDO_ORDINATE_ARRAY ({1})),2000000001)"
        //        }
        //        catch (Exception e)
        //        {
        //            _context.GetLogger().LogError("GetLayerGrafByCoords3", e);
        //            return null;
        //        }
        //        finally
        //        {
        //            if (_context.Database.Connection.State == ConnectionState.Open)
        //                _context.Database.Connection.Close();
        //        }
        //    }


        //    queryPrin = string.Format(queryPrin, querySec);
        //    using (IDbCommand objComm = _context.Database.Connection.CreateCommand())
        //    {
        //        try
        //        {
        //            _context.Database.Connection.Open();
        //            objComm.CommandText = queryPrin;
        //            using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //            {
        //                int cont = 0;
        //                while (data.Read())
        //                {
        //                    #region colectar datos
        //                    //vienen como xmin,xmax, ymin,ymax
        //                    //Y tengo que formar xmin, ymin, xmax, ymax
        //                    lstCoord.Add(data.GetNullableDouble(0).Value);
        //                    lstCoord.Add(data.GetNullableDouble(2).Value);
        //                    lstCoord.Add(0);
        //                    lstCoord.Add(data.GetNullableDouble(1).Value);
        //                    lstCoord.Add(data.GetNullableDouble(3).Value);
        //                    lstCoord.Add(0);
        //                    cont++;
        //                    #endregion
        //                }
        //            }
        //        }
        //        catch (DataException)
        //        {
        //            if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
        //            _context.GetLogger().LogInfo(string.Format("sql query: {0}", queryPrin));
        //            throw;
        //        }
        //    }

        //    return lstCoord.ToArray();
        //}

        //public LayerGraf GetPuerta(string pIdUbicacionPloteo)
        //{
        //    LayerGraf lstCoordsTransformadas = new LayerGraf();
        //    try
        //    {
        //        string sSql = string.Empty;
        //        sSql = "select id_parcela_puerta, SDO_CS.TRANSFORM(GEOMETRY, 8307, 22195).Get_WKT() as WKTCBlob, nro_puerta, angulo from ct_parcela_puerta c where MDSYS.SDO_RELATE(c.GEOMETRY,SDO_GEOM.SDO_BUFFER((select geometry from rc_ubicacion_ploteo up where id_ubicacion_ploteo = " + pIdUbicacionPloteo + " ), 5000, 1),'MASK=ANYINTERACT') = 'TRUE' and c.nro_puerta = (select altura from rc_ubicacion_ploteo up where id_ubicacion_ploteo = " + pIdUbicacionPloteo + ")";
        //        IDbCommand objComm = _context.Database.Connection.CreateCommand();
        //        _context.Database.Connection.Open();
        //        objComm.CommandText = sSql;
        //        try
        //        {
        //            using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //            {
        //                while (data.Read())
        //                {
        //                    lstCoordsTransformadas.FeatId = data.GetInt64(0);
        //                    lstCoordsTransformadas.Nombre = data.GetInt32(2).ToString();
        //                    lstCoordsTransformadas.Descripcion = data.GetInt32(3).ToString();
        //                    try
        //                    {
        //                        DbGeometry geom = data.GetGeometryFromField(1);
        //                        if (geom != null)
        //                        {
        //                            lstCoordsTransformadas.Geom = geom;

        //                        }
        //                        else
        //                        {
        //                            // La geometria nunca va a retornar nulo.
        //                            //_context.GetLogger().LogInfo(string.Format("el objeto {0}={1} del componente {2} devuelve una geometria inválida -> SDO_CS.TRANSFORM(p.{3}, 2000000001, 22195).Get_WKT()", campoId, layerGrafRead.FeatId, layer.ComponenteId, campoGeometry));
        //                        }
        //                    }
        //                    catch (ApplicationException ex)
        //                    {
        //                        _context.GetLogger().LogInfo(ex.Message);
        //                    }
        //                    catch (Exception)
        //                    {
        //                        // _context.GetLogger().LogInfo(string.Format("EX - el objeto {0}={1} del componente {2} devuelve una geometria inválida -> SDO_CS.TRANSFORM(p.{3}, 2000000001, 22195).Get_WKT(). WKT={4}", campoId, layerGrafRead.FeatId, layer.ComponenteId, campoGeometry, ex.Message));
        //                    }
        //                }
        //            }
        //        }
        //        catch (DataException)
        //        {
        //            if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
        //            //_context.GetLogger().LogInfo(string.Format("sql query: {0}", objComm.CommandText));
        //            throw;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _context.GetLogger().LogError("LayerGrafRepository - GetPuerta", e);
        //    }
        //    finally
        //    {
        //        if (_context.Database.Connection.State == ConnectionState.Open)
        //            _context.Database.Connection.Close();
        //    }
        //    return lstCoordsTransformadas;
        //}

        //public LayerGraf GetParcelaSAR(string pIdUbicacionPloteo)
        //{
        //    LayerGraf layerGrafRead = new LayerGraf();
        //    string sSql = "select ID_PARCELA,"
        //                 + "TO_CHAR(NUMERO),"
        //                 + "SDO_CS.TRANSFORM(GEOMETRY, 8307, 22195).Get_WKT() as WKTCBlob"
        //                 + " from ct_parcela  pa where pa.id_parcela = "
        //                 + "(select id_parcela "
        //                + "from ct_parcela_puerta c "
        //                + "where MDSYS.SDO_RELATE(c.GEOMETRY,"
        //                                        + "SDO_GEOM.SDO_BUFFER((select geometry from rc_ubicacion_ploteo up where id_ubicacion_ploteo = " + pIdUbicacionPloteo + " ), 5000, 1)"
        //                                        + ",'MASK=ANYINTERACT') = 'TRUE' and nro_puerta = "
        //                                                                                        + "(select altura from rc_ubicacion_ploteo where id_ubicacion_ploteo = " + pIdUbicacionPloteo + " ))";

        //    try
        //    {
        //        using (IDbCommand objComm = _context.Database.Connection.CreateCommand())
        //        {
        //            _context.Database.Connection.Open();
        //            objComm.CommandText = sSql;
        //            using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //            {
        //                int i = 0;
        //                while (data.Read())
        //                {
        //                    i++;
        //                    layerGrafRead = new LayerGraf();
        //                    layerGrafRead.FeatId = data.GetNullableInt64(0).Value;

        //                    layerGrafRead.Nombre = data.GetStringOrEmpty(1);
        //                    #region Geometry
        //                    try
        //                    {
        //                        DbGeometry geom = data.GetGeometryFromField(2);
        //                        if (geom != null)
        //                        {
        //                            layerGrafRead.Geom = geom;

        //                        }
        //                        else
        //                        {
        //                            // La geometria nunca va a retornar nulo.
        //                            //_context.GetLogger().LogInfo(string.Format("el objeto {0}={1} del componente {2} devuelve una geometria inválida -> SDO_CS.TRANSFORM(p.{3}, 2000000001, 22195).Get_WKT()", campoId, layerGrafRead.FeatId, layer.ComponenteId, campoGeometry));
        //                        }
        //                    }
        //                    catch (ApplicationException ex)
        //                    {
        //                        _context.GetLogger().LogInfo(ex.Message);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        // _context.GetLogger().LogInfo(string.Format("EX - el objeto {0}={1} del componente {2} devuelve una geometria inválida -> SDO_CS.TRANSFORM(p.{3}, 2000000001, 22195).Get_WKT(). WKT={4}", campoId, layerGrafRead.FeatId, layer.ComponenteId, campoGeometry, ex.Message));
        //                    }
        //                    #endregion
        //                }
        //                if (i > 1)
        //                {
        //                    throw new Exception("No se pudo identificar una parcela unívoca a esa direccion");
        //                }
        //                if (i < 1)
        //                {
        //                    throw new Exception("No se pudo identificar ninguna parcela con esa direccion");
        //                }
        //            }
        //        }
        //    }
        //    catch (DataException)
        //    {
        //        if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
        //        //_context.GetLogger().LogInfo(string.Format("sql query: {0}", objComm.CommandText));
        //        throw;
        //    }

        //    return layerGrafRead;
        //}

        //public int GetIdManzanaByIdObjetoGraf(int pIdObjetoGraf)
        //{
        //    int idManzana = 0;

        //    string sSql = "select id_manzana from ct_manzana "
        //                + "where apic_id = "
        //                                + "(select manzana from rc_ubicacion_ploteo where id_ubicacion_ploteo = " + pIdObjetoGraf + " ) "
        //                + "and fecha_baja is null";


        //    try
        //    {
        //        using (IDbCommand objComm = _context.Database.Connection.CreateCommand())
        //        {

        //            _context.Database.Connection.Open();
        //            objComm.CommandText = sSql;
        //            using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //            {
        //                int i = 0;
        //                while (data.Read())
        //                {
        //                    idManzana = data.GetNullableInt32(0).Value;
        //                }
        //            }
        //        }
        //    }
        //    catch (DataException)
        //    {
        //        if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
        //        //_context.GetLogger().LogInfo(string.Format("sql query: {0}", objComm.CommandText));
        //        throw;
        //    }
        //    return idManzana;
        //}
        #endregion

        #region comento porque no se usa
        //public string GetDireccionByIdObjGraf(string pIdObjetoGraf)
        //{
        //    string calle = "";
        //    string altura = "";

        //    //En capital no encuentra las calles xq estan como 990. 
        //    //Se soluciona buscando las calles con 990 en caso de no encontrarlas.
        //    string sSql = "select (select nombre from ct_calle where apic_id like ( LPAD(pu.distrito, 3, '0')|| LPAD(pu.calle, 6, '0')||'%')), altura from rc_ubicacion_ploteo pu where id_ubicacion_ploteo = " + pIdObjetoGraf;

        //    try
        //    {
        //        using (IDbCommand objComm = _context.Database.Connection.CreateCommand())
        //        {

        //            _context.Database.Connection.Open();
        //            objComm.CommandText = sSql;
        //            using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //            {
        //                while (data.Read())
        //                {
        //                    calle = data.GetStringOrEmpty(0);
        //                    altura = data.GetNullableInt32(1).ToString();
        //                }
        //            }
        //        }
        //    }
        //    catch (DataException)
        //    {
        //        if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
        //        //_context.GetLogger().LogInfo(string.Format("sql query: {0}", objComm.CommandText));
        //        throw;
        //    }

        //    try
        //    {
        //        if (calle == "")
        //        {
        //            sSql = "select (select nombre from ct_calle where apic_id like ( '990' || LPAD(pu.calle, 6, '0')||'%')), altura from rc_ubicacion_ploteo pu where id_ubicacion_ploteo = " + pIdObjetoGraf;
        //            using (IDbCommand objComm = _context.Database.Connection.CreateCommand())
        //            {
        //                _context.Database.Connection.Open();
        //                objComm.CommandText = sSql;
        //                using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //                {
        //                    while (data.Read())
        //                    {
        //                        calle = data.GetStringOrEmpty(0);
        //                        altura = data.GetNullableInt32(1).ToString();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (DataException)
        //    {
        //        if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
        //        //_context.GetLogger().LogInfo(string.Format("sql query: {0}", objComm.CommandText));
        //        throw;
        //    }

        //    return calle + " " + altura;
        //} 
        #endregion

        public LayerGraf[] GetLayerGrafByCoordsAndIds(Layer layer, Componente componente, List<string> lstCoordsGeometry, string anio, List<long> ids = null)
        {
            if ((lstCoordsGeometry ?? new List<string>()).Any())
            {
                Atributo campoFeatId = null;
                Atributo campoLabel = null;
                Atributo campoGeometry = null;
                Atributo campoId = null;

                try
                {
                    campoFeatId = componente.Atributos.GetAtributoFeatId();
                    campoId = componente.Atributos.GetAtributoClave();

                    campoGeometry = componente.Atributos.GetAtributoGeometry();

                    if (layer.EtiquetaIdAtributo.GetValueOrDefault() > 0)
                    {
                        try
                        {
                            campoLabel = componente.Atributos.Single(p => p.AtributoId == layer.EtiquetaIdAtributo);
                        }
                        catch
                        {
                            _context.GetLogger().LogError(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el layer {1} de la plantilla {2}", layer.EtiquetaIdAtributo, layer.Nombre, layer.IdPlantilla), new Exception());
                        }
                    }
                }
                catch (ApplicationException appEx)
                {
                    _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
                    return null;
                }
                try
                {
                    int MAX_CANT_ID = 900;
                    int idx = 0;
                    bool procesa = true;
                    var final = new List<LayerGraf>();
                    var aliasTablaGrafica = "t1";
                    var campoOrientacion = layer.PuntoAtributoOrientacion ? new Atributo() { Campo = "GEOMETRY_ORIENT", ComponenteId = componente.ComponenteId } : null;

                    while (procesa)
                    {
                        procesa = ids != null && ids[0] > 0 && idx < ids.Count;
                        using (var builder = _context.CreateSQLQueryBuilder())
                        {
                            try
                            {
                                SQLConnectors connector = SQLConnectors.None;
                                builder.AddTable(componente, "t1");

                                if (ids != null && ids[0] > 0)
                                {
                                    var objetos = ids.GetRange(idx, Math.Min(MAX_CANT_ID, ids.Count - idx));
                                    idx += MAX_CANT_ID;

                                    builder.AddFilter(campoId, string.Format("({0})", string.Join(",", objetos)), SQLOperators.In);
                                }
                                if (!string.IsNullOrEmpty(anio) && layer.FiltroIdAtributo != null)
                                {
                                    Atributo atributoFiltro = _context.Atributo.Find(layer.FiltroIdAtributo);
                                    if (atributoFiltro != null)
                                    {
                                        builder.AddFilter(atributoFiltro, anio, SQLOperators.EqualsTo, connector);
                                        connector = SQLConnectors.And;
                                    }
                                }
                                if (!string.IsNullOrEmpty(layer.CapaFiltro))
                                {
                                    builder.AddRawFilter(layer.CapaFiltro, connector);
                                    connector = SQLConnectors.And;
                                }

                                if ((componente.TablaGrafica ?? componente.Tabla) != componente.Tabla)
                                {
                                    aliasTablaGrafica = "graf";
                                    builder.AddJoin(new Componente() { ComponenteId = -1, Esquema = componente.Esquema, Tabla = componente.TablaGrafica },
                                                    aliasTablaGrafica,
                                                    new Atributo() { ComponenteId = -1, Campo = campoFeatId.Campo },
                                                    campoFeatId);
                                }

                                string wkt = $"POLYGON(({string.Join(",", lstCoordsGeometry.Select(elem => string.Join(" ", elem.Split(',').Select(c => c.Trim()))))}))";
                                var geomFiltro = builder.CreateGeometryFieldBuilder(wkt, SRID.App).ChangeToSRID(SRID.DB);
                                SQLSpatialRelationships relation = SQLSpatialRelationships.AnyInteract;
                                if (layer.FiltroGeografico == 1)
                                {
                                    relation = SQLSpatialRelationships.Inside | SQLSpatialRelationships.CoveredBy | SQLSpatialRelationships.Equal;
                                    double buffer = 1d;
                                    var paramGral = _context.ParametrosGenerales.FirstOrDefault(pg => pg.Descripcion.ToUpper() == "TOLERANCIAFILTROPRINCIPALPLOTEO");
                                    if (paramGral != null)
                                    {
                                        buffer = Convert.ToDouble(paramGral.Valor);
                                    }
                                    geomFiltro.AddBuffer(buffer);
                                }
                                else if (layer.FiltroGeografico == 2)
                                {
                                    using (var innerBuilder = this._context.CreateSQLQueryBuilder())
                                    {
                                        innerBuilder.AddTable(new Componente() { ComponenteId = campoFeatId.ComponenteId, Esquema = componente.Esquema, Tabla = componente.TablaGrafica ?? componente.Tabla }, "inner")
                                                    .AddFilter(innerBuilder.CreateGeometryFieldBuilder(campoGeometry, "inner"), geomFiltro, SQLSpatialRelationships.Inside)
                                                    .AddFields(campoFeatId);

                                        builder.AddFilter(campoFeatId, $"({innerBuilder})", SQLOperators.NotIn, connector);
                                        connector = SQLConnectors.And;
                                    }
                                }
                                builder.AddFilter(builder.CreateGeometryFieldBuilder(campoGeometry, aliasTablaGrafica), geomFiltro, relation, connector);

                                var geomSelect = builder.CreateGeometryFieldBuilder(campoGeometry, aliasTablaGrafica)
                                                        .ChangeToSRID(SRID.App)
                                                        .IntersectionWith(builder.CreateGeometryFieldBuilder(wkt, SRID.App))
                                                        .ToWKT();

                                builder.AddFields(campoFeatId, campoLabel, campoOrientacion)
                                       .AddGeometryField(geomSelect, "geom");

                                final.AddRange(builder.ExecuteQuery((IDataReader reader) =>
                                {
                                    var geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"));
                                    if (geom == null)
                                    {
                                        return null;
                                    }
                                    return new LayerGraf
                                    {
                                        FeatId = reader.GetNullableInt64(0).Value,
                                        Nombre = campoLabel == null ? string.Empty : reader.GetStringOrEmpty(reader.GetOrdinal(campoLabel.Campo)),
                                        Rotation = campoOrientacion == null ? null : reader.GetNullableInt32(reader.GetOrdinal(campoOrientacion.Campo)),
                                        Geom = geom
                                    };
                                }));
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                    return final.ToArray();
                }
                catch (Exception ex)
                {
                    _context.GetLogger().LogError($"GetLayerGrafById(layer: {layer.IdLayer}, componente: {componente.ComponenteId}, id in {ids})", ex);
                    throw;
                }
                //try
                //{
                //    string esquema = componente.Esquema;
                //    string tableName = componente.Tabla;
                //    string tablaGrafica = componente.TablaGrafica ?? tableName;
                //    string aliasTablaGrafica = "t1";

                //    string mask = layer.FiltroGeografico == 1 ? "INSIDE+COVEREDBY+EQUAL" : "ANYINTERACT";

                //    //Por defecto es conectado por lineas rectas, sino rectangulo
                //    string sdo_interpretation = lstCoordsGeometry.Count == 2 ? "3" : "1";

                //    var fields = new List<string>();
                //    var tables = new List<string>();
                //    var where = new List<string>();

                //    #region formar clausulas sql
                //    if (tableName != tablaGrafica)
                //    {
                //        aliasTablaGrafica = "graf";
                //        tables.Add(esquema + "." + tablaGrafica + " " + aliasTablaGrafica);
                //        where.Add(string.Format("t1.{0} = {1}.{0}", campoId, aliasTablaGrafica));
                //    }

                //    if (!string.IsNullOrEmpty(anio) && layer.FiltroIdAtributo != null)
                //    {
                //        Atributo atributoFiltro = _context.Atributo.Find(layer.FiltroIdAtributo);
                //        if (atributoFiltro != null)
                //        {
                //            where.Add(string.Format("t1.{0} = {1}", atributoFiltro.Campo, anio));
                //        }
                //    }

                //    string transientGeometry = string.Format("SDO_CS.TRANSFORM(MDSYS.SDO_GEOMETRY (2003, 22195, NULL, MDSYS.SDO_ELEM_INFO_ARRAY (1,1003,{0}), MDSYS.SDO_ORDINATE_ARRAY ({1})),8307)",
                //                                                sdo_interpretation, string.Join(",", lstCoordsGeometry));
                //    if (layer.FiltroGeografico == 1)
                //    {
                //        double distBufferObjetoBase = 1.0;
                //        var paramGral = _context.ParametrosGenerales.FirstOrDefault(pg => pg.Descripcion.ToUpper() == "TOLERANCIAFILTROPRINCIPALPLOTEO");
                //        if (paramGral != null)
                //        {
                //            distBufferObjetoBase = Convert.ToDouble(paramGral.Valor);
                //        }
                //        transientGeometry = string.Format("SDO_GEOM.SDO_BUFFER({0},{1},1)", transientGeometry, distBufferObjetoBase);
                //    }
                //    else if (layer.FiltroGeografico == 2)
                //    {
                //        where.Add(string.Format("{0}.{1} NOT IN (SELECT {1} FROM {2}.{3} WHERE MDSYS.SDO_RELATE({4},{5}, 'MASK=INSIDE') = 'TRUE')",
                //                                                aliasTablaGrafica, campoFeatId, esquema, tablaGrafica, campoGeometry, transientGeometry));
                //    }

                //    where.Add(string.Format("MDSYS.SDO_RELATE({0}.{1}, {2},'MASK={3}') = 'TRUE'", aliasTablaGrafica, campoGeometry, transientGeometry, mask));

                //    tables.Add(esquema + "." + tableName + " t1");

                //    fields.Add(string.Format("{0}.{1}", aliasTablaGrafica, campoFeatId));

                //    if (!string.IsNullOrEmpty(campoLabel))
                //    {
                //        fields.Add(string.Format("TO_CHAR(t1.{0})", campoLabel));
                //    }

                //    fields.Add(string.Format(" SDO_CS.TRANSFORM(SDO_GEOM.SDO_INTERSECTION({0}.{1}," +
                //                            "SDO_CS.TRANSFORM(MDSYS.SDO_GEOMETRY (2003, 22195, NULL, MDSYS.SDO_ELEM_INFO_ARRAY (1,1003,{3}), MDSYS.SDO_ORDINATE_ARRAY ({2})),(SELECT SRID FROM ALL_SDO_GEOM_METADATA WHERE OWNER = '" + esquema + "' AND TABLE_NAME = '" + tablaGrafica + "' AND COLUMN_NAME = '" + campoGeometry + "')),0.05),8307,22195) " +
                //                            ".Get_WKT()", aliasTablaGrafica, campoGeometry, string.Join(",", lstCoordsGeometry), sdo_interpretation));

                //    if (layer.PuntoAtributoOrientacion)
                //    {
                //        fields.Add("GEOMETRY_ORIENT");
                //    }
                //    if (!String.IsNullOrEmpty(layer.CapaFiltro))
                //    {
                //        where.Add(layer.CapaFiltro);
                //    }

                //    //Si tiene ids, se agrega a la consulta
                //    if (ids != null && ids[0] > 0)
                //    {
                //        where.Add(string.Format(" {0} {1} ", campoId, "in(" + string.Join(",", ids.Select(i => i)) + ") "));
                //    }

                //    #endregion
                //    //where.Add("rownum < 10");//Para Prueba
                //    using (IDbCommand objComm = _context.Database.Connection.CreateCommand())
                //    {
                //        string sSql = string.Format("SELECT {0} FROM {1} WHERE {2}", string.Join(",", fields), string.Join(",", tables), string.Join(" AND ", where));
                //        if (layer.ComponenteId == COMPONENTE_ID_ARCO)
                //        {
                //            sSql += " ORDER BY ID_CALLE ";
                //        }

                //        try
                //        {
                //            _context.Database.Connection.Open();
                //            objComm.CommandText = sSql;
                //            using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
                //            {
                //                List<LayerGraf> lstLayerGrafs = new List<LayerGraf>();
                //                LayerGraf layerGrafRead;
                //                int i = 0;
                //                while (data.Read())
                //                {
                //                    #region colectar datos
                //                    layerGrafRead = new LayerGraf();
                //                    layerGrafRead.FeatId = data.GetNullableInt64(0).Value;
                //                    int idx = 1;
                //                    if (campoLabel != string.Empty)
                //                    {
                //                        layerGrafRead.Nombre = data.GetStringOrEmpty(1);
                //                        idx = 2;
                //                    }
                //                    else
                //                    {
                //                        layerGrafRead.Nombre = string.Empty;
                //                    }

                //                    #region Geometry
                //                    try
                //                    {
                //                        DbGeometry geom = data.GetGeometryFromField(idx);
                //                        if (geom != null)
                //                        {
                //                            layerGrafRead.Geom = geom;
                //                            if (layer.ComponenteId == COMPONENTE_ID_ARCO)
                //                            {
                //                                //Para calles salteo cuatro para que no se empasten los nombres
                //                                if ((i % 4 == 0))
                //                                {
                //                                    lstLayerGrafs.Add(layerGrafRead);
                //                                }
                //                            }
                //                            else
                //                            {
                //                                lstLayerGrafs.Add(layerGrafRead);
                //                            }
                //                            i++;
                //                        }
                //                        else
                //                        {
                //                            // La geometria nunca va a retornar nulo.
                //                            _context.GetLogger().LogInfo(string.Format("el objeto {0}={1} del componente {2} devuelve una geometria inválida -> SDO_CS.TRANSFORM(p.{3}, 8307, 22195).Get_WKT()", campoId, layerGrafRead.FeatId, layer.ComponenteId, campoGeometry));
                //                        }
                //                    }
                //                    catch (ApplicationException ex)
                //                    {
                //                        _context.GetLogger().LogError("GetLayerGrafByCoords", ex);
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        _context.GetLogger().LogError("GetLayerGrafByCoords", ex);
                //                        _context.GetLogger().LogInfo(string.Format("EX - el objeto {0}={1} del componente {2} devuelve una geometria inválida -> SDO_CS.TRANSFORM(p.{3}, 8307, 22195).Get_WKT(). WKT={4}", campoId, layerGrafRead.FeatId, layer.ComponenteId, campoGeometry, ex.Message));
                //                    }
                //                    #endregion
                //                    if (data.GetSchemaTable().Columns.Contains("GEOMETRY_ORIENT"))
                //                    {
                //                        layerGrafRead.Rotation = data.GetNullableInt32(++idx);
                //                    }
                //                    #endregion
                //                }
                //                return lstLayerGrafs.ToArray();
                //            }
                //        }
                //        catch (DataException)
                //        {
                //            if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
                //            _context.GetLogger().LogInfo(string.Format("sql query: {0}", sSql));
                //            throw;
                //        }
                //    }
                //}
                //catch (Exception e)
                //{
                //    _context.GetLogger().LogError("GetLayerGrafByCoords3", e);
                //    return null;
                //}
                //finally
                //{
                //    if (_context.Database.Connection.State == ConnectionState.Open)
                //        _context.Database.Connection.Close();
                //}
            }
            return null;
        }

        public LayerGraf[] GetLayerGrafByIds(Layer layer, Componente componente, string ids, List<string> lstCoordsGeometry)
        {
            Atributo campoFeatId = null;
            Atributo campoLabel = null;
            Atributo campoGeometry = null;
            Atributo campoId = null;

            try
            {
                campoId = componente.Atributos.GetAtributoClave();
                campoFeatId = componente.Atributos.GetAtributoFeatId();
                campoGeometry = componente.Atributos.GetAtributoGeometry();

                if (layer.EtiquetaIdAtributo != null && layer.EtiquetaIdAtributo > 0)
                {
                    try
                    {
                        campoLabel = componente.Atributos.FirstOrDefault(p => p.AtributoId == layer.EtiquetaIdAtributo);
                    }
                    catch
                    {
                        throw new ApplicationException(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el layer {1} de la plantilla {2}", layer.EtiquetaIdAtributo, layer.Nombre, layer.IdPlantilla));
                    }
                }
            }
            catch (ApplicationException appEx)
            {
                _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
                return null;
            }

            try
            {
                int MAX_CANT_ID = 900;
                int idx = 0;
                var lista = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var final = new List<LayerGraf>();
                var aliasTablaGrafica = "t1";
                var campoOrientacion = layer.PuntoAtributoOrientacion ? new Atributo() { Campo = "GEOMETRY_ORIENT", ComponenteId = componente.ComponenteId } : null;

                while (idx < lista.Count)
                {
                    var objetos = lista.GetRange(idx, Math.Min(MAX_CANT_ID, lista.Count - idx));
                    idx += MAX_CANT_ID;

                    using (var builder = _context.CreateSQLQueryBuilder())
                    {
                        try
                        {
                            builder.AddTable(componente, "t1")
                                   .AddFilter(campoId, string.Format("({0})", string.Join(",", objetos)), SQLOperators.In);

                            if (!string.IsNullOrEmpty(layer.CapaFiltro))
                            {
                                builder.AddRawFilter(layer.CapaFiltro, SQLConnectors.And);
                            }

                            if (componente.Tabla != componente.TablaGrafica)
                            {
                                aliasTablaGrafica = "graf";
                                builder.AddJoin(new Componente() { ComponenteId = -1, Esquema = componente.Esquema, Tabla = componente.TablaGrafica },
                                                aliasTablaGrafica,
                                                new Atributo() { ComponenteId = -1, Campo = campoFeatId.Campo },
                                                campoFeatId);
                            }
                            var geom = builder.CreateGeometryFieldBuilder(campoGeometry, aliasTablaGrafica)
                                              .IntersectionWith(builder.CreateGeometryFieldBuilder("", SRID.DB))
                                              .ChangeToSRID(SRID.App).ToWKT();

                            builder.AddFields(campoFeatId, campoLabel, campoOrientacion)
                                   .AddGeometryField(geom, "geom");

                            final.AddRange(builder.ExecuteQuery((IDataReader reader) =>
                                                  {
                                                      return new LayerGraf
                                                      {
                                                          FeatId = reader.GetNullableInt64(0).Value,
                                                          Nombre = campoLabel == null ? string.Empty : reader.GetStringOrEmpty(reader.GetOrdinal(campoLabel.Campo)),
                                                          Rotation = campoOrientacion == null ? null : reader.GetNullableInt32(reader.GetOrdinal(campoOrientacion.Campo)),
                                                          Geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"))
                                                      };
                                                  }));
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
                return final.ToArray();
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError($"GetLayerGrafById(layer: {layer.IdLayer}, componente: {componente.ComponenteId}, id in {ids})", ex);
                throw;
            }
            #region comento por ahora hasta borrar en un futuro
            //string campoFeatId = string.Empty;
            //string campoLabel = string.Empty;
            //string campoGeometry = string.Empty;
            //Atributo campoId = null;

            //try
            //{
            //    campoId = componente.Atributos.GetAtributoClave();
            //    campoFeatId = componente.Atributos.GetAtributoFeatId().Campo;

            //    campoGeometry = componente.Atributos.GetAtributoGeometry().Campo;

            //    if (layer.EtiquetaIdAtributo != null && layer.EtiquetaIdAtributo > 0)
            //    {
            //        try
            //        {
            //            campoLabel = componente.Atributos.FirstOrDefault(p => p.AtributoId == layer.EtiquetaIdAtributo).Campo;
            //        }
            //        catch
            //        {
            //            throw new ApplicationException(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el layer {1} de la plantilla {2}", layer.EtiquetaIdAtributo, layer.Nombre, layer.IdPlantilla));
            //        }
            //    }
            //}
            //catch (ApplicationException appEx)
            //{
            //    _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
            //    return null;
            //}
            //try
            //{
            //    string esquema = componente.Esquema;
            //    string tableName = componente.Tabla;
            //    string tablaGrafica = componente.TablaGrafica ?? componente.Tabla;

            //    var fields = new List<string>();
            //    var tables = new List<string>();
            //    var where = new List<string>();
            //    string aliasTablaGrafica = "t1";
            //    var lista = ids.Split(',').Select(id => campoId.GetFormattedValue(id)).ToList();
            //    int MAX_CANT_ID = 900;
            //    int idx = 0;
            //    var filtros = new List<string>();

            //    //Por defecto es conectado por lineas rectas, sino rectangulo
            //    string sdo_interpretation = lstCoordsGeometry.Count == 2 ? "3" : "1";

            //    #region formar clausulas sql
            //    if (tableName != tablaGrafica)
            //    {
            //        aliasTablaGrafica = "graf";
            //        tables.Add(esquema + "." + tablaGrafica + " " + aliasTablaGrafica);
            //        where.Add(string.Format("t1.{0} = {1}.{0}", campoId.Campo, aliasTablaGrafica));
            //    }

            //    while (idx < lista.Count)
            //    {
            //        var objetos = lista.GetRange(idx, Math.Min(MAX_CANT_ID, lista.Count - idx));
            //        idx += MAX_CANT_ID;
            //        filtros.Add(string.Format("t1.{0} IN ({1})", campoId.Campo, string.Join(",", objetos)));
            //    }
            //    where.Add(string.Format("({0})", string.Join(" OR ", filtros)));
            //    tables.Add(esquema + "." + tableName + " t1");

            //    fields.Add(string.Format("{0}.{1}", aliasTablaGrafica, campoFeatId));

            //    if (!string.IsNullOrEmpty(campoLabel))
            //    {
            //        fields.Add(string.Format("TO_CHAR(t1.{0})", campoLabel));
            //    }
            //    //fields.Add(string.Format("SDO_CS.TRANSFORM({0}.{1}, 2000000001, 22195).Get_WKT()", aliasTablaGrafica, campoGeometry));
            //    fields.Add(string.Format(" SDO_CS.TRANSFORM(SDO_GEOM.SDO_INTERSECTION({0}.{1}," +
            //            "SDO_CS.TRANSFORM(MDSYS.SDO_GEOMETRY (2003, 22195, NULL, MDSYS.SDO_ELEM_INFO_ARRAY (1,1003,{3}), MDSYS.SDO_ORDINATE_ARRAY ({2})),(SELECT SRID FROM ALL_SDO_GEOM_METADATA WHERE OWNER = '" + esquema + "' AND TABLE_NAME = '" + tablaGrafica + "' AND COLUMN_NAME = '" + campoGeometry + "')),0.05),8307,22195) " +
            //            ".Get_WKT()", aliasTablaGrafica, campoGeometry, string.Join(",", lstCoordsGeometry), sdo_interpretation));
            //    if (layer.PuntoAtributoOrientacion)
            //    {
            //        fields.Add("GEOMETRY_ORIENT");
            //    }
            //    if (!String.IsNullOrEmpty(layer.CapaFiltro))
            //    {
            //        where.Add(layer.CapaFiltro);
            //    }
            //    #endregion

            //    using (IDbCommand objComm = _context.Database.Connection.CreateCommand())
            //    {
            //        _context.Database.Connection.Open();
            //        objComm.CommandText = string.Format("select {0} from {1} where {2}", string.Join(",", fields), string.Join(",", tables), string.Join(" AND ", where));
            //        try
            //        {
            //            using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
            //            {
            //                List<LayerGraf> lstLayerGrafs = new List<LayerGraf>();
            //                LayerGraf layerGrafRead;
            //                while (data.Read())
            //                {
            //                    #region colectar datos
            //                    layerGrafRead = new LayerGraf();
            //                    layerGrafRead.FeatId = data.GetNullableInt64(0).Value;
            //                    idx = 1;
            //                    if (campoLabel != string.Empty)
            //                    {
            //                        layerGrafRead.Nombre = data.GetStringOrEmpty(1);
            //                        idx = 2;
            //                    }
            //                    else
            //                    {
            //                        layerGrafRead.Nombre = string.Empty;
            //                    }
            //                    layerGrafRead.Geom = data.GetGeometryFromField(idx);
            //                    lstLayerGrafs.Add(layerGrafRead);
            //                    #endregion
            //                }
            //                return lstLayerGrafs.ToArray();
            //            }
            //        }
            //        catch (DataException)
            //        {
            //            if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
            //            _context.GetLogger().LogInfo(string.Format("sql query: {0}", objComm.CommandText));
            //            throw;
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    _context.GetLogger().LogError(string.Format("GetLayerGrafById(layer: {0}, componente: {1}, id in {2})", layer.IdLayer, componente.ComponenteId, ids), e);
            //    return null;
            //} 
            #endregion
        }

        public string GetLayerGrafRegionByCoords(double x, double y)
        {
            try
            {
                using (var builder = _context.CreateSQLQueryBuilder())
                {
                    return builder.AddTable("ct_region", "t1")
                                  .AddFilter(builder.CreateGeometryFieldBuilder(new Atributo() { Campo = "geometry" }, "t1"),
                                             builder.CreateGeometryFieldBuilder($"POINT({x} {y})", SRID.App).ChangeToSRID(SRID.DB),
                                             SQLSpatialRelationships.AnyInteract)
                                  .AddFields("id_region", "nombre")
                                  .ExecuteQuery((IDataReader reader) =>
                                  {
                                      return $"{reader.GetStringOrEmpty(0)} - {reader.GetStringOrEmpty(1)}";
                                  }).First();
                }
                //string texto = string.Empty;

                //string sSql = "SELECT d.id_region || ' - ' || d.NOMBRE " +
                //              " FROM CT_REGION d " +
                //              " WHERE MDSYS.SDO_RELATE(d.GEOMETRY," +
                //                                    "SDO_CS.TRANSFORM(" +
                //                                        "MDSYS.SDO_GEOMETRY(2001, 22195, MDSYS.SDO_POINT_TYPE(" + x + "," + y + ",0), null, null) " +
                //                                    ",8307) " +
                //                                    ",'MASK=ANYINTERACT') = 'TRUE'";

                //IDbCommand objComm = _context.Database.Connection.CreateCommand();
                //_context.Database.Connection.Open();
                //objComm.CommandText = sSql;
                //using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
                //{
                //    while (data.Read())
                //    {
                //        texto = data.GetString(0);
                //    }
                //    return texto;
                //}
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError($"GetLayerGrafDistritoByCoords(x: {x}, y: {x})", ex);
                return string.Empty;
            }
        }

        public string GetLayerGrafKMMallaByCoords(int pIdMalla, Componente compCañeria, List<string> lstTipo)
        {
            throw new NotImplementedException();
        }

        public LayerGraf[] GetLayerGrafByObjetoBase(Layer layer, Componente componente, Componente componenteBase, List<string> lstCoordsGeometry, string idObjetoBase, string anio, bool esInformeAnual)
        {
            if (!string.IsNullOrEmpty(idObjetoBase))
            {
                Atributo campoFeatId = null;
                Atributo campoLabel = null;
                Atributo campoGeometry = null;
                Atributo campoId = null;
                Atributo campoGeometryBase = null;
                Atributo atributoCampoIdBase = null; // se usa para formatear el valor de acuerdo al tipo de dato en el "WHERE" de la consulta

                try
                {
                    campoId = componente.Atributos.GetAtributoClave();
                    campoFeatId = componente.Atributos.GetAtributoFeatId();
                    campoGeometry = componente.Atributos.GetAtributoGeometry();

                    if (layer.EtiquetaIdAtributo != null && layer.EtiquetaIdAtributo > 0)
                    {
                        try
                        {
                            campoLabel = componente.Atributos.FirstOrDefault(p => p.AtributoId == layer.EtiquetaIdAtributo);
                        }
                        catch
                        {
                            throw new ApplicationException(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el layer {1} de la plantilla {2}", layer.EtiquetaIdAtributo, layer.Nombre, layer.IdPlantilla));
                        }
                    }
                }
                catch (ApplicationException appEx)
                {
                    _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
                    return null;
                }
                try
                {
                    atributoCampoIdBase = componenteBase.Atributos.GetAtributoClave();
                    campoGeometryBase = componenteBase.Atributos.GetAtributoGeometry();
                }
                catch (ApplicationException appEx)
                {
                    _context.GetLogger().LogError("Componente (id: " + componenteBase.ComponenteId + ") mal configurado.", appEx);
                    return null;
                }
                try
                {
                    var final = new List<LayerGraf>();
                    string aliasTablaGrafica = "t1";
                    bool filtroEspacial = false;
                    //Por defecto es conectado por lineas rectas, sino rectangulo
                    string sdo_interpretation = lstCoordsGeometry.Count == 2 ? "3" : "1";
                    var campoOrientacion = layer.PuntoAtributoOrientacion ? new Atributo() { Campo = "GEOMETRY_ORIENT", ComponenteId = componente.ComponenteId } : null;

                    using (var builder = _context.CreateSQLQueryBuilder())
                    {
                        try
                        {
                            builder.AddTable(componente, "t1");
                            if (layer.FiltroGeografico == 1)
                            {
                                if (componente.Tabla != (componente.TablaGrafica ?? componente.Tabla))
                                {
                                    aliasTablaGrafica = "graf";
                                    builder.AddJoin(new Componente() { ComponenteId = -1, Esquema = componente.Esquema, Tabla = componente.TablaGrafica },
                                                    aliasTablaGrafica,
                                                    new Atributo() { ComponenteId = -1, Campo = campoFeatId.Campo },
                                                    campoFeatId);
                                }
                                if ((filtroEspacial = componente.ComponenteId != componenteBase.ComponenteId))
                                {
                                    builder.AddTable(componenteBase, "base")
                                           .AddFilter(atributoCampoIdBase, idObjetoBase, SQLOperators.EqualsTo)
                                           .AddFilter(builder.CreateGeometryFieldBuilder(campoGeometry, aliasTablaGrafica),
                                                      builder.CreateGeometryFieldBuilder(campoGeometryBase, "base"),
                                                      SQLSpatialRelationships.AnyInteract, SQLConnectors.And);

                                    var g1 = builder.CreateGeometryFieldBuilder(campoGeometry, aliasTablaGrafica);
                                    var g2 = builder.CreateGeometryFieldBuilder(campoGeometryBase, "base").OverlappingArea(g1);

                                    builder.AddRawFilter(string.Format("({0} / {1})", g2, g1.ToPolygon().Area()), 0.9, SQLOperators.GreaterThan, SQLConnectors.And);
                                }
                            }
                            if (!filtroEspacial)
                            {
                                builder.AddFilter(campoId, idObjetoBase, SQLOperators.EqualsTo);
                            }
                            if (!string.IsNullOrEmpty(layer.CapaFiltro))
                            {
                                builder.AddRawFilter(layer.CapaFiltro, SQLConnectors.And);
                            }
                            string wkt = $"POLYGON(({string.Join(",", lstCoordsGeometry.Select(elem => string.Join(" ", elem.Split(',').Select(c => c.Trim()))))}))";
                            var geom = builder.CreateGeometryFieldBuilder(campoGeometry, aliasTablaGrafica)
                                              .ChangeToSRID(SRID.App)
                                              .IntersectionWith(builder.CreateGeometryFieldBuilder(wkt, SRID.App))
                                              .ToWKT();

                            builder.AddFields(campoFeatId, campoLabel, campoOrientacion)
                                   .AddGeometryField(geom, "geom");

                            final.AddRange(builder.ExecuteQuery((IDataReader reader) =>
                            {
                                return new LayerGraf
                                {
                                    FeatId = reader.GetNullableInt64(0).Value,
                                    Nombre = campoLabel == null ? string.Empty : reader.GetStringOrEmpty(reader.GetOrdinal(campoLabel.Campo)),
                                    Rotation = campoOrientacion == null ? null : reader.GetNullableInt32(reader.GetOrdinal(campoOrientacion.Campo)),
                                    Geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"))
                                };
                            }));
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    return final.ToArray();
                }
                catch (Exception ex)
                {
                    _context.GetLogger().LogError($"GetLayerGrafByObjetoBase(idObjetoBase: {idObjetoBase})", ex);
                    throw;
                }
                #region comento por ahora hasta borrar en un futuro
                //string campoFeatId = string.Empty;
                //string campoLabel = string.Empty;
                //string campoGeometry = string.Empty;
                //string campoId = string.Empty;
                //string campoGeometryBase = string.Empty;
                //Atributo atributoCampoIdBase = null; // se usa para formatear el valor de acuerdo al tipo de dato en el "WHERE" de la consulta

                //try
                //{
                //    campoFeatId = componente.Atributos.GetAtributoFeatId().Campo;
                //    campoId = componente.Atributos.GetAtributoClave().Campo;

                //    campoGeometry = componente.Atributos.GetAtributoGeometry().Campo;

                //    if (layer.EtiquetaIdAtributo != null && layer.EtiquetaIdAtributo > 0)
                //    {
                //        var aAtributo = componente.Atributos.FirstOrDefault(p => p.AtributoId == layer.EtiquetaIdAtributo);
                //        if (aAtributo != null)
                //        {
                //            campoLabel = aAtributo.Campo;
                //        }
                //        else
                //        {
                //            throw new ApplicationException(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el layer {1} de la plantilla {2}", layer.EtiquetaIdAtributo, layer.Nombre, layer.IdPlantilla));
                //        }
                //    }
                //}
                //catch (ApplicationException appEx)
                //{
                //    _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
                //    return null;
                //}

                //try
                //{
                //    atributoCampoIdBase = componenteBase.Atributos.GetAtributoClave();
                //    campoGeometryBase = componenteBase.Atributos.GetAtributoGeometry().Campo;
                //}
                //catch (ApplicationException appEx)
                //{
                //    _context.GetLogger().LogError("Componente (id: " + componenteBase.ComponenteId + ") mal configurado.", appEx);
                //    return null;
                //}
                //try
                //{
                //    string esquema = componente.Esquema;
                //    string tableName = componente.Tabla;
                //    string tablaGrafica = componente.TablaGrafica ?? tableName;
                //    string esquemaBase = componenteBase.Esquema;
                //    string tableNameBase = componenteBase.TablaGrafica ?? componenteBase.Tabla;

                //    var fields = new List<string>();
                //    var tables = new List<string>();
                //    var where = new List<string>();
                //    string aliasTablaGrafica = "t1";

                //    //Por defecto es conectado por lineas rectas, sino rectangulo
                //    string sdo_interpretation = lstCoordsGeometry.Count == 2 ? "3" : "1";

                //    #region formar clausulas sql
                //    if (!string.IsNullOrEmpty(anio) && layer.FiltroIdAtributo != null)
                //    {
                //        Atributo atributoFiltro = _context.Atributo.Find(layer.FiltroIdAtributo);
                //        if (atributoFiltro != null)
                //        {
                //            where.Add(string.Format("t1.{0} = {1}", atributoFiltro.Campo, anio));
                //        }
                //    }
                //    if (layer.FiltroGeografico == 1)
                //    {
                //        if (tableName != tablaGrafica)
                //        {
                //            aliasTablaGrafica = "graf";
                //            tables.Add(esquema + "." + tablaGrafica + " " + aliasTablaGrafica);
                //            where.Add(string.Format("t1.{0} = {1}.{0}", campoId, aliasTablaGrafica));
                //        }

                //        if (componente.ComponenteId != componenteBase.ComponenteId)
                //        {
                //            /* es una negrada, pero es imposible poder contemplar todas las posibles variaciones 
                //             * a una consulta con el sdo_relate cuando una o las 2 geometrias son vistas de vistas */
                //            bool invierteGeometrias = esInformeAnual && componente.Tabla.ToUpper().StartsWith("VW_");
                //            string auxAlias1 = aliasTablaGrafica;
                //            string auxAlias2 = tableNameBase;
                //            string auxGeom1 = campoGeometry;
                //            string auxGeom2 = campoGeometryBase;
                //            if (invierteGeometrias)
                //            {
                //                auxAlias2 = aliasTablaGrafica;
                //                auxAlias1 = tableNameBase;
                //                auxGeom2 = campoGeometry;
                //                auxGeom1 = campoGeometryBase;
                //            }

                //            tables.Add(esquemaBase + "." + tableNameBase);

                //            where.Add(string.Format("MDSYS.SDO_RELATE({0}.{1}, {2}.{3},'MASK=ANYINTERACT') = 'TRUE'", auxAlias1, auxGeom1, auxAlias2, auxGeom2));

                //            where.Add(string.Format("({0}.{1}.GET_GTYPE() <> 3 OR (SDO_GEOM.SDO_AREA(SDO_GEOM.SDO_INTERSECTION({0}.{1}, {2}.{3}, 0.1),0.1) / SDO_GEOM.SDO_AREA({0}.{1},0.1) ) > 0.9) ",
                //                                    aliasTablaGrafica, campoGeometry, tableNameBase, campoGeometryBase));
                //        }
                //        else
                //        {
                //            tableNameBase = aliasTablaGrafica;
                //        }
                //    }
                //    tables.Add(esquema + "." + tableName + " t1");
                //    where.Add(string.Format("{0}.{1} = {2}", tableNameBase, atributoCampoIdBase.Campo, atributoCampoIdBase.GetFormattedValue(idObjetoBase)));

                //    fields.Add(string.Format("{0}.{1}", aliasTablaGrafica, campoFeatId));

                //    if (!string.IsNullOrEmpty(campoLabel))
                //    {
                //        fields.Add(string.Format("TO_CHAR(t1.{0})", campoLabel));
                //    }
                //    //fields.Add(string.Format("SDO_CS.TRANSFORM({0}.{1}, 2000000001, 22195).Get_WKT()", aliasTablaGrafica, campoGeometry));
                //    fields.Add(string.Format(" SDO_CS.TRANSFORM(SDO_GEOM.SDO_INTERSECTION({0}.{1}," +
                //        "SDO_CS.TRANSFORM(MDSYS.SDO_GEOMETRY (2003, 22195, NULL, MDSYS.SDO_ELEM_INFO_ARRAY (1,1003,{3}), MDSYS.SDO_ORDINATE_ARRAY ({2})),(SELECT SRID FROM ALL_SDO_GEOM_METADATA WHERE OWNER = '" + esquema + "' AND TABLE_NAME = '" + tablaGrafica + "' AND COLUMN_NAME = '" + campoGeometry + "')),0.05),8307,22195) " +
                //        ".Get_WKT()", aliasTablaGrafica, campoGeometry, string.Join(",", lstCoordsGeometry), sdo_interpretation));

                //    if (layer.PuntoAtributoOrientacion)
                //    {
                //        fields.Add("GEOMETRY_ORIENT");
                //    }
                //    if (!String.IsNullOrEmpty(layer.CapaFiltro))
                //    {
                //        where.Add(layer.CapaFiltro);
                //    }
                //    #endregion
                //    //where.Add("rownum < 10");//Para Prueba/
                //    using (IDbCommand objComm = _context.Database.Connection.CreateCommand())
                //    {
                //        _context.Database.Connection.Open();
                //        objComm.CommandText = string.Format("SELECT {0} from {1} WHERE {2}", string.Join(",", fields), string.Join(",", tables), string.Join(" AND ", where));
                //        try
                //        {
                //            using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
                //            {
                //                List<LayerGraf> lstLayerGrafs = new List<LayerGraf>();
                //                LayerGraf layerGrafRead;
                //                int i = 0;
                //                while (data.Read())
                //                {
                //                    #region colectar datos
                //                    layerGrafRead = new LayerGraf();
                //                    layerGrafRead.FeatId = data.GetNullableInt64(0).Value;
                //                    int idx = 1;
                //                    if (!string.IsNullOrEmpty(campoLabel))
                //                    {
                //                        layerGrafRead.Nombre = data.GetStringOrEmpty(1);
                //                        idx = 2;
                //                    }
                //                    else
                //                    {
                //                        layerGrafRead.Nombre = string.Empty;
                //                    }
                //                    try
                //                    {
                //                        layerGrafRead.Geom = data.GetGeometryFromField(idx);
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        _context.GetLogger().LogError("Objeto (ID: " + layerGrafRead.FeatId + ") con Componente (ID: " + componente.ComponenteId + " , NOMBRE: " + componente.Nombre + ") con geometria erronea.", ex);
                //                        continue;
                //                    }
                //                    if (data.GetSchemaTable().Columns.Contains("GEOMETRY_ORIENT"))
                //                    {
                //                        layerGrafRead.Rotation = data.GetNullableInt32(++idx);
                //                    }

                //                    if (layerGrafRead.Geom != null)
                //                    {
                //                        if (layer.ComponenteId == COMPONENTE_ID_ARCO)
                //                        {
                //                            //Para calles salteo cuatro para que no se empasten los nombres
                //                            if ((i % 4 == 0))
                //                            {
                //                                lstLayerGrafs.Add(layerGrafRead);
                //                            }
                //                        }
                //                        else
                //                        {
                //                            lstLayerGrafs.Add(layerGrafRead);
                //                        }
                //                        i++;
                //                    }
                //                    #endregion
                //                }
                //                return lstLayerGrafs.ToArray();
                //            }
                //        }
                //        catch (DataException)
                //        {
                //            if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
                //            _context.GetLogger().LogInfo(string.Format("sql query: {0}", objComm.CommandText));
                //            throw;
                //        }
                //    }
                //}
                //catch (Exception e)
                //{
                //    _context.GetLogger().LogError("GetLayerGrafByObjetoBase", e);
                //    return null;
                //}
                #endregion
            }
            return null;
        }

        public LayerGraf[] GetLayerGrafByObjetoBaseIntersect(Layer layer, Componente componente, Componente componenteBase, string idObjetoBase, string anio, bool esInformeAnual)
        {
            if (!string.IsNullOrEmpty(idObjetoBase))
            {
                Atributo campoFeatId = null;
                Atributo campoLabel = null;
                Atributo campoGeometry = null;
                Atributo campoId = null;
                Componente cmpBase = new Componente() { Esquema = componenteBase.Esquema, Tabla = componenteBase.TablaGrafica ?? componenteBase.Tabla, ComponenteId = componenteBase.ComponenteId };
                Atributo campoGeometryBase = null;
                Atributo atributoCampoIdBase = null; // se usa para formatear el valor de acuerdo al tipo de dato en el "WHERE" de la consulta

                try
                {
                    campoId = componente.Atributos.GetAtributoClave();
                    campoFeatId = componente.Atributos.GetAtributoFeatId();
                    campoGeometry = componente.Atributos.GetAtributoGeometry();

                    if (layer.EtiquetaIdAtributo != null && layer.EtiquetaIdAtributo > 0)
                    {
                        try
                        {
                            campoLabel = componente.Atributos.FirstOrDefault(p => p.AtributoId == layer.EtiquetaIdAtributo);
                        }
                        catch
                        {
                            throw new ApplicationException(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el layer {1} de la plantilla {2}", layer.EtiquetaIdAtributo, layer.Nombre, layer.IdPlantilla));
                        }
                    }
                }
                catch (ApplicationException appEx)
                {
                    _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
                    return null;
                }
                try
                {
                    atributoCampoIdBase = componenteBase.Atributos.GetAtributoClave();
                    campoGeometryBase = componenteBase.Atributos.GetAtributoGeometry();
                }
                catch (ApplicationException appEx)
                {
                    _context.GetLogger().LogError("Componente (id: " + componenteBase.ComponenteId + ") mal configurado.", appEx);
                    return null;
                }
                try
                {
                    var final = new List<LayerGraf>();
                    string aliasTablaGrafica = "t1";
                    bool filtroEspacial = false;
                    var campoOrientacion = layer.PuntoAtributoOrientacion ? new Atributo() { Campo = "GEOMETRY_ORIENT", ComponenteId = componente.ComponenteId } : null;

                    using (var builder = _context.CreateSQLQueryBuilder())
                    {
                        try
                        {
                            builder.AddTable(componente, "t1");
                            if (layer.FiltroGeografico == 1 || layer.FiltroGeografico == 3)
                            {
                                if (componente.Tabla != (componente.TablaGrafica ?? componente.Tabla))
                                {
                                    aliasTablaGrafica = "graf";
                                    builder.AddJoin(new Componente() { ComponenteId = -1, Esquema = componente.Esquema, Tabla = componente.TablaGrafica },
                                                    aliasTablaGrafica,
                                                    new Atributo() { ComponenteId = -1, Campo = campoFeatId.Campo },
                                                    campoFeatId);
                                }
                                if ((filtroEspacial = componente.ComponenteId != cmpBase.ComponenteId))
                                {
                                    builder.AddTable(cmpBase, "base")
                                           .AddFilter(atributoCampoIdBase, idObjetoBase, SQLOperators.EqualsTo)
                                           .AddFilter(builder.CreateGeometryFieldBuilder(campoGeometry, aliasTablaGrafica),
                                                      builder.CreateGeometryFieldBuilder(campoGeometryBase, "base"),
                                                      SQLSpatialRelationships.AnyInteract, SQLConnectors.And);

                                    var g1 = builder.CreateGeometryFieldBuilder(campoGeometry, aliasTablaGrafica);
                                    var g2 = builder.CreateGeometryFieldBuilder(campoGeometryBase, "base").OverlappingArea(g1);

                                    builder.AddRawFilter(string.Format("({0} / {1})", g2, g1.ToPolygon().Area()), 0.9, SQLOperators.GreaterThan, SQLConnectors.And);
                                }
                            }
                            if (!filtroEspacial)
                            {
                                builder.AddFilter(campoId, idObjetoBase, SQLOperators.EqualsTo);
                            }
                            if (!string.IsNullOrEmpty(layer.CapaFiltro))
                            {
                                builder.AddRawFilter(layer.CapaFiltro, SQLConnectors.And);
                            }
                            var geom = builder.CreateGeometryFieldBuilder(campoGeometry, aliasTablaGrafica)
                                              .IntersectionWith(builder.CreateGeometryFieldBuilder(campoGeometryBase, "base"))
                                              .ChangeToSRID(SRID.App)
                                              .ToWKT();

                            builder.AddFields(campoFeatId, campoLabel, campoOrientacion)
                                   .AddGeometryField(geom, "geom");

                            final.AddRange(builder.ExecuteQuery((IDataReader reader) =>
                            {
                                return new LayerGraf
                                {
                                    FeatId = reader.GetNullableInt64(0).Value,
                                    Nombre = campoLabel == null ? string.Empty : reader.GetStringOrEmpty(reader.GetOrdinal(campoLabel.Campo)),
                                    Rotation = campoOrientacion == null ? null : reader.GetNullableInt32(reader.GetOrdinal(campoOrientacion.Campo)),
                                    Geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"))
                                };
                            }));
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    return final.ToArray();
                }
                catch (Exception ex)
                {
                    _context.GetLogger().LogError($"GetLayerGrafByObjetoBase(idObjetoBase: {idObjetoBase})", ex);
                    throw;
                }
            }
            return null;
            #region comento por ahora hasta borrar en un futuro
            //if (!string.IsNullOrEmpty(idObjetoBase))
            //{
            //    string campoFeatId = string.Empty;
            //    string campoLabel = string.Empty;
            //    string campoGeometry = string.Empty;
            //    string campoId = string.Empty;
            //    string campoGeometryBase = string.Empty;
            //    Atributo atributoCampoIdBase = null; // se usa para formatear el valor de acuerdo al tipo de dato en el "WHERE" de la consulta

            //    try
            //    {


            //        campoFeatId = componente.Atributos.GetAtributoFeatId().Campo;
            //        campoId = componente.Atributos.GetAtributoClave().Campo;

            //        campoGeometry = componente.Atributos.GetAtributoGeometry().Campo;

            //        if (layer.EtiquetaIdAtributo != null && layer.EtiquetaIdAtributo > 0)
            //        {
            //            var aAtributo = componente.Atributos.FirstOrDefault(p => p.AtributoId == layer.EtiquetaIdAtributo);
            //            if (aAtributo != null)
            //            {
            //                campoLabel = aAtributo.Campo;
            //            }
            //            else
            //            {
            //                throw new ApplicationException(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el layer {1} de la plantilla {2}", layer.EtiquetaIdAtributo, layer.Nombre, layer.IdPlantilla));
            //            }
            //        }
            //    }
            //    catch (ApplicationException appEx)
            //    {
            //        _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
            //        return null;
            //    }

            //    try
            //    {
            //        atributoCampoIdBase = componenteBase.Atributos.GetAtributoClave();
            //        campoGeometryBase = componenteBase.Atributos.GetAtributoGeometry().Campo;
            //    }
            //    catch (ApplicationException appEx)
            //    {
            //        _context.GetLogger().LogError("Componente (id: " + componenteBase.ComponenteId + ") mal configurado.", appEx);
            //        return null;
            //    }
            //    try
            //    {
            //        string esquema = componente.Esquema;
            //        string tableName = componente.Tabla;
            //        string tablaGrafica = componente.TablaGrafica ?? tableName;
            //        string esquemaBase = componenteBase.Esquema;
            //        string tableNameBase = componenteBase.TablaGrafica ?? componenteBase.Tabla;

            //        var fields = new List<string>();
            //        var tables = new List<string>();
            //        var where = new List<string>();
            //        string aliasTablaGrafica = "t1";

            //        #region formar clausulas sql
            //        if (!string.IsNullOrEmpty(anio) && layer.FiltroIdAtributo != null)
            //        {
            //            Atributo atributoFiltro = _context.Atributo.Find(layer.FiltroIdAtributo);
            //            if (atributoFiltro != null)
            //            {
            //                where.Add(string.Format("t1.{0} = {1}", atributoFiltro.Campo, anio));
            //            }
            //        }
            //        if (layer.FiltroGeografico == 1 || layer.FiltroGeografico == 3)
            //        {
            //            if (tableName != tablaGrafica)
            //            {
            //                aliasTablaGrafica = "graf";
            //                tables.Add(esquema + "." + tablaGrafica + " " + aliasTablaGrafica);
            //                where.Add(string.Format("t1.{0} = {1}.{0}", campoId, aliasTablaGrafica));
            //            }

            //            if (componente.ComponenteId != componenteBase.ComponenteId)
            //            {
            //                /* es una negrada, pero es imposible poder contemplar todas las posibles variaciones 
            //                 * a una consulta con el sdo_relate cuando una o las 2 geometrias son vistas de vistas */
            //                bool invierteGeometrias = esInformeAnual && componente.Tabla.ToUpper().StartsWith("VW_");
            //                string auxAlias1 = aliasTablaGrafica;
            //                string auxAlias2 = tableNameBase;
            //                string auxGeom1 = campoGeometry;
            //                string auxGeom2 = campoGeometryBase;
            //                if (invierteGeometrias)
            //                {
            //                    auxAlias2 = aliasTablaGrafica;
            //                    auxAlias1 = tableNameBase;
            //                    auxGeom2 = campoGeometry;
            //                    auxGeom1 = campoGeometryBase;
            //                }

            //                tables.Add(esquemaBase + "." + tableNameBase);

            //                where.Add(string.Format("MDSYS.SDO_RELATE({0}.{1}, {2}.{3},'MASK=ANYINTERACT') = 'TRUE'", auxAlias1, auxGeom1, auxAlias2, auxGeom2));

            //                where.Add(string.Format("({0}.{1}.GET_GTYPE() <> 3 OR (SDO_GEOM.SDO_AREA(SDO_GEOM.SDO_INTERSECTION({0}.{1}, {2}.{3}, 0.1),0.1) / SDO_GEOM.SDO_AREA({0}.{1},0.1) ) > 0.9) ",
            //                                        aliasTablaGrafica, campoGeometry, tableNameBase, campoGeometryBase));
            //            }
            //            else
            //            {
            //                tableNameBase = aliasTablaGrafica;
            //            }
            //        }
            //        tables.Add(esquema + "." + tableName + " t1");
            //        where.Add(string.Format("{0}.{1} = {2}", tableNameBase, atributoCampoIdBase.Campo, atributoCampoIdBase.GetFormattedValue(idObjetoBase)));

            //        fields.Add(string.Format("{0}.{1}", aliasTablaGrafica, campoFeatId));

            //        if (!string.IsNullOrEmpty(campoLabel))
            //        {
            //            fields.Add(string.Format("TO_CHAR(t1.{0})", campoLabel));
            //        }
            //        //fields.Add(string.Format("SDO_CS.TRANSFORM({0}.{1}, 2000000001, 22195).Get_WKT()", aliasTablaGrafica, campoGeometry));
            //        fields.Add(string.Format(" SDO_CS.TRANSFORM(SDO_GEOM.SDO_INTERSECTION({0}.{1}," +
            //            "(select {5} from {2}.{3} where {4} = {6} ),0.05),8307,22195)" +
            //            ".Get_WKT()", aliasTablaGrafica, campoGeometry, componenteBase.Esquema, componenteBase.Tabla, atributoCampoIdBase.Campo, campoGeometryBase, idObjetoBase));

            //        if (layer.PuntoAtributoOrientacion)
            //        {
            //            fields.Add("GEOMETRY_ORIENT");
            //        }
            //        if (!String.IsNullOrEmpty(layer.CapaFiltro))
            //        {
            //            where.Add(layer.CapaFiltro);
            //        }
            //        #endregion
            //        //where.Add("rownum < 10");//Para Prueba/
            //        using (IDbCommand objComm = _context.Database.Connection.CreateCommand())
            //        {
            //            _context.Database.Connection.Open();
            //            objComm.CommandText = string.Format("SELECT {0} from {1} WHERE {2}", string.Join(",", fields), string.Join(",", tables), string.Join(" AND ", where));
            //            try
            //            {
            //                using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
            //                {
            //                    List<LayerGraf> lstLayerGrafs = new List<LayerGraf>();
            //                    LayerGraf layerGrafRead;
            //                    int i = 0;
            //                    while (data.Read())
            //                    {
            //                        #region colectar datos
            //                        layerGrafRead = new LayerGraf();
            //                        layerGrafRead.FeatId = data.GetNullableInt64(0).Value;
            //                        int idx = 1;
            //                        if (!string.IsNullOrEmpty(campoLabel))
            //                        {
            //                            layerGrafRead.Nombre = data.GetStringOrEmpty(1);
            //                            idx = 2;
            //                        }
            //                        else
            //                        {
            //                            layerGrafRead.Nombre = string.Empty;
            //                        }
            //                        try
            //                        {
            //                            layerGrafRead.Geom = data.GetGeometryFromField(idx);
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            _context.GetLogger().LogError("Objeto (ID: " + layerGrafRead.FeatId + ") con Componente (ID: " + componente.ComponenteId + " , NOMBRE: " + componente.Nombre + ") con geometria erronea.", ex);
            //                            continue;
            //                        }
            //                        if (data.GetSchemaTable().Columns.Contains("GEOMETRY_ORIENT"))
            //                        {
            //                            layerGrafRead.Rotation = data.GetNullableInt32(++idx);
            //                        }

            //                        if (layerGrafRead.Geom != null)
            //                        {
            //                            if (layer.ComponenteId == COMPONENTE_ID_ARCO)
            //                            {
            //                                //Para calles salteo cuatro para que no se empasten los nombres
            //                                if ((i % 4 == 0))
            //                                {
            //                                    lstLayerGrafs.Add(layerGrafRead);
            //                                }
            //                            }
            //                            else
            //                            {
            //                                lstLayerGrafs.Add(layerGrafRead);
            //                            }
            //                            i++;
            //                        }
            //                        #endregion
            //                    }
            //                    return lstLayerGrafs.ToArray();
            //                }
            //            }
            //            catch (DataException)
            //            {
            //                if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
            //                _context.GetLogger().LogInfo(string.Format("sql query: {0}", objComm.CommandText));
            //                throw;
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        _context.GetLogger().LogError("GetLayerGrafByObjetoBase", e);
            //        return null;
            //    }
            //}
            //return null; 
            #endregion
        }

        public double[] TransformCoords(double x1, double y1, double x2, double y2, SRID origen, SRID destino)
        {
            List<double> lstCoordsTransformadas = new List<double>();
            try
            {
                using (var qbuilder = this._context.CreateSQLQueryBuilder())
                {
                    return qbuilder.AddNoTable()
                            .AddGeometryField(qbuilder.CreateGeometryFieldBuilder($"LINESTRING({x1} {y1},{x2} {y2})", origen).ChangeToSRID(destino).ToWKT(), "geom")
                            .ExecuteQuery((IDataReader reader) =>
                            {
                                var geom = reader.GetGeometryFromField(0, destino);
                                return new[] { geom.StartPoint.XCoordinate.Value, geom.StartPoint.YCoordinate.Value, geom.EndPoint.XCoordinate.Value, geom.EndPoint.YCoordinate.Value };
                            }).Single();
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError($"TransformCoords({origen}=>{destino})", ex);
                return new double[0];
            }
        }

        public LayerGraf GetLayerGrafByComponentePrincipal(Layer layer, Componente componente, string guid)
        {
            _context.GetLogger().LogInfo($"GetLayerGrafByComponentePrincipal{Environment.NewLine}Este método hace referencia a CT_DISTRITO. Por ahora genero error hasta que se resuelva qué se hace.");
            throw new PlatformNotSupportedException();
            #region comento por ahora porque la consulta va a romper porque no existe la tabla "CT_DISTRITO". ver cuando algo falle por esto
            //string campoFeatId = string.Empty;
            //string campoLabel = string.Empty;
            //string campoGeometry = string.Empty;
            //string campoId = string.Empty;

            //try
            //{
            //    campoFeatId = componente.Atributos.GetAtributoFeatId().Campo;
            //    campoId = componente.Atributos.GetAtributoClave().Campo;

            //    campoGeometry = componente.Atributos.GetAtributoGeometry().Campo;

            //    if (layer.EtiquetaIdAtributo != null && layer.EtiquetaIdAtributo > 0)
            //    {
            //        try
            //        {
            //            campoLabel = componente.Atributos.Single(p => p.AtributoId == layer.EtiquetaIdAtributo).Campo;
            //        }
            //        catch
            //        {
            //            throw new ApplicationException(string.Format("No se ha encontrado el atributo {0} seleccionado como etiqueta en el layer {1} de la plantilla {2}", layer.EtiquetaIdAtributo, layer.Nombre, layer.IdPlantilla));
            //        }
            //    }
            //}
            //catch (ApplicationException appEx)
            //{
            //    _context.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
            //    return null;
            //}

            //try
            //{
            //    string esquema = componente.Esquema;
            //    string tableName = componente.Tabla;
            //    string tablaGrafica = componente.TablaGrafica ?? tableName;
            //    string aliasTablaGrafica = "t1";

            //    var fields = new List<string>();
            //    var tables = new List<string>();
            //    var where = new List<string>();

            //    #region formar clausulas sql
            //    tables.Add(string.Format("{0}.{1} t1", esquema, tableName));
            //    if (tableName != tablaGrafica)
            //    {
            //        aliasTablaGrafica = "graf";
            //        tables.Add(esquema + "." + tablaGrafica + " " + aliasTablaGrafica);
            //        where.Add(string.Format("t1.{0} = {1}.{0}", campoId, aliasTablaGrafica));
            //    }

            //    string innerQuery = " SELECT ID_DISTRITO FROM (" +
            //                            " SELECT ID_DISTRITO, (overlap_area/total_area)*100 PORC_AREA" +
            //                            " FROM (SELECT q.ID_DISTRITO" +
            //                                   " , SDO_GEOM.SDO_AREA(" +
            //                                            " SDO_GEOM.SDO_INTERSECTION(q.GEOMETRY" +
            //                                                                      " , (SELECT SDO_AGGR_CONVEXHULL(SDOAGGRTYPE(GEOMETRY, 1))" +
            //                                                                         " FROM MT_OBJETO_RESULTADO" +
            //                                                                         " WHERE GUID='" + guid + "')" +
            //                                                                      " , 1)" +
            //                                            " , 1) overlap_area" +
            //                                    " , SDO_GEOM.SDO_AREA(q.GEOMETRY, 1) total_area" +
            //                                  " FROM CT_DISTRITO q  " +
            //                                  " WHERE MDSYS.SDO_RELATE(q.GEOMETRY" +
            //                                                   " ,(SELECT SDO_AGGR_CONVEXHULL(SDOAGGRTYPE(GEOMETRY, 1))" +
            //                                                                         " FROM MT_OBJETO_RESULTADO" +
            //                                                                         " WHERE GUID='" + guid + "')" +
            //                                               ",'MASK=ANYINTERACT') = 'TRUE' " +
            //                                  " )" +
            //                            " ORDER BY PORC_AREA DESC" +
            //                            " )" +
            //                        " WHERE ROWNUM=1 ";

            //    where.Add(string.Format("{0}.{1} = ({2})", aliasTablaGrafica, campoFeatId, innerQuery));

            //    fields.Add(string.Format("{0}.{1}", aliasTablaGrafica, campoFeatId));

            //    if (!string.IsNullOrEmpty(campoLabel))
            //    {
            //        fields.Add(string.Format("TO_CHAR(t1.{0})", campoLabel));
            //    }
            //    fields.Add(string.Format("SDO_CS.TRANSFORM({0}.{1}, 8307, 22195).Get_WKT()", aliasTablaGrafica, campoGeometry));
            //    if (layer.PuntoAtributoOrientacion)
            //    {
            //        fields.Add("GEOMETRY_ORIENT");
            //    }
            //    if (!String.IsNullOrEmpty(layer.CapaFiltro))
            //    {
            //        where.Add(layer.CapaFiltro);
            //    }
            //    #endregion

            //    using (IDbCommand objComm = _context.Database.Connection.CreateCommand())
            //    {
            //        _context.Database.Connection.Open();
            //        objComm.CommandText = string.Format("SELECT {0} from {1} WHERE {2}", string.Join(",", fields), string.Join(",", tables), string.Join(" AND ", where));
            //        try
            //        {
            //            using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
            //            {
            //                LayerGraf layerGrafRead = null;
            //                while (data.Read())
            //                {
            //                    #region colectar datos
            //                    layerGrafRead = new LayerGraf();
            //                    layerGrafRead.FeatId = data.GetNullableInt64(0).Value;
            //                    int idx = 1;
            //                    if (!string.IsNullOrEmpty(campoLabel))
            //                    {
            //                        layerGrafRead.Nombre = data.GetStringOrEmpty(1);
            //                        idx = 2;
            //                    }
            //                    else
            //                    {
            //                        layerGrafRead.Nombre = string.Empty;
            //                    }

            //                    layerGrafRead.Geom = data.GetGeometryFromField(idx);
            //                    layerGrafRead.Rotation = data.GetNullableInt32(++idx);
            //                    #endregion
            //                }
            //                return layerGrafRead;
            //            }
            //        }
            //        catch (DataException)
            //        {
            //            if (_context.Database.Connection.State == ConnectionState.Open) _context.Database.Connection.Close();
            //            _context.GetLogger().LogInfo(string.Format("sql query: {0}", objComm.CommandText));
            //            throw;
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    _context.GetLogger().LogError(string.Format("GetLayerGrafByComponentePrincipal(layer: {0}, componente: {1}, guid: {2})", layer.IdLayer, componente.ComponenteId, guid), e);
            //    return null;
            //}
            //finally
            //{
            //    if (_context.Database.Connection.State == ConnectionState.Open)
            //        _context.Database.Connection.Close();
            //}
            #endregion
        }
    }
}
