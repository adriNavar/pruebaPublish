using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace GeoSit.Data.DAL.SQLQueryBuilders
{
    internal class OracleGeometryFieldBuilder : ISQLGeometryFieldBuilder
    {
        private string field = string.Empty;
        private string original = string.Empty;
        private readonly SRIDParser sridParser;
        private const decimal TOLERANCE = 0.005M;

        public OracleGeometryFieldBuilder(SRIDParser parser)
        {
            this.sridParser = parser;
        }

        public ISQLGeometryFieldBuilder FromField(Atributo atributo, string tableAlias)
        {
            this.field = string.Format("{0}.{1}", tableAlias, atributo.Campo.ToLower());
            this.original = this.field;
            return this;
        }
        public ISQLGeometryFieldBuilder FromWKT(string wkt, int srid)
        {
            this.field = string.Format("SDO_GEOMETRY('{0}',{1})", wkt, srid);
            this.original = this.field;
            return this;
        }
        public ISQLGeometryFieldBuilder FromWKT(string wkt, SRID srid)
        {
            return this.FromWKT(wkt, sridParser.Parse(srid));
        }
        public ISQLGeometryFieldBuilder FromParameter(DbParameter param, int srid)
        {
            ((OracleParameter)param).OracleDbType = OracleDbType.Clob;
            this.field = string.Format("SDO_GEOMETRY({0},{1})", param.ParameterName, srid);
            this.original = this.field;
            return this;
        }
        public ISQLGeometryFieldBuilder FromParameter(DbParameter param, SRID srid)
        {
            return this.FromParameter(param, sridParser.Parse(srid));
        }
        public ISQLGeometryFieldBuilder Centroid()
        {
            this.field = string.Format("SDO_GEOM.SDO_CENTROID({0},{1})", this.field, TOLERANCE);
            return this;
        }
        public ISQLGeometryFieldBuilder ToWKT()
        {
            this.field = string.Format("SDO_UTIL.TO_WKTGEOMETRY({0})", this.field);
            return this;
        }
        public ISQLGeometryFieldBuilder AddBuffer(double buffer)
        {
            this.field = string.Format("SDO_GEOM.SDO_BUFFER({0},{1},{2},'UNIT=M')", this.field, buffer, TOLERANCE);
            return this;
        }
        public ISQLGeometryFieldBuilder ChangeToSRID(int srid)
        {
            this.field = string.Format("SDO_CS.TRANSFORM({0},{1})", this.field, srid);
            return this;
        }
        public ISQLGeometryFieldBuilder ChangeToSRID(SRID srid)
        {
            return this.ChangeToSRID(sridParser.Parse(srid));
        }
        public ISQLGeometryFieldBuilder IntersectionWith(ISQLGeometryFieldBuilder geometry)
        {
            this.field = string.Format("SDO_GEOM.SDO_INTERSECTION({0},{1},{2})", this.field, geometry, TOLERANCE);
            return this;
        }
        public ISQLGeometryFieldBuilder ToPolygon()
        {
            return this.AddBuffer(0.000001);
        }
        public ISQLGeometryFieldBuilder Area()
        {
            return this.area();
        }
        public ISQLGeometryFieldBuilder AreaSqrMeters()
        {
            if (!this.sridParser.IsDbLatLon())
            {
                this.ChangeToSRID(this.sridParser.Parse(SRID.LL84));
            }
            return this.area(true);
        }
        public ISQLGeometryFieldBuilder Length()
        {
            return this.perimeter_length();
        }
        public ISQLGeometryFieldBuilder LengthMeters()
        {
            if (!this.sridParser.IsDbLatLon())
            {
                this.ChangeToSRID(this.sridParser.Parse(SRID.LL84));
            }
            return this.perimeter_length(true);
        }
        public ISQLGeometryFieldBuilder Perimeter()
        {
            return this.perimeter_length();
        }
        public ISQLGeometryFieldBuilder PerimeterMeters()
        {
            if (!this.sridParser.IsDbLatLon())
            {
                this.ChangeToSRID(this.sridParser.Parse(SRID.LL84));
            }
            return this.perimeter_length(true);
        }
        public ISQLGeometryFieldBuilder NumPoints()
        {
            this.field = string.Format("CASE {0}.GET_GTYPE() WHEN 1 THEN 1 ELSE SDO_UTIL.GETNUMVERTICES({1}) END", this.original, this.field);
            return this;
        }
        public ISQLGeometryFieldBuilder OverlappingArea(ISQLGeometryFieldBuilder geometry)
        {
            return this.IntersectionWith(geometry).Area();
        }
        public ISQLGeometryFieldBuilder BoundingBox()
        {
            this.field = string.Format("SDO_AGGR_MBR({0})", this.field);
            return this;
        }
        public ISQLGeometryFieldBuilder ConvexHull()
        {
            this.field = string.Format("SDO_AGGR_CONVEXHULL(SDOAGGRTYPE({0}, {1}))", this.field, TOLERANCE);
            return this;
        }
        public ISQLGeometryFieldBuilder GType()
        {
            this.field = string.Format(@"CASE {0}.GET_GTYPE()
                                       WHEN 1 THEN 3 
                                       WHEN 2 THEN 2 
                                       WHEN 3 THEN 1
                                       WHEN 5 THEN 3 
                                       WHEN 6 THEN 2 
                                       WHEN 7 THEN 1
                                       ELSE 5 END", this.original);
            return this;
        }
        public ISQLGeometryFieldBuilder Distance(ISQLGeometryFieldBuilder geometry)
        {
            this.field = $"ROUND(SDO_GEOM.SDO_DISTANCE({this.field},{geometry},{TOLERANCE}),2)";
            return this;
        }
        public string FormatSpatialFilter(ISQLGeometryFieldBuilder geometry, SQLSpatialRelationships relations)
        {
            var lista = new List<string>();
            if ((relations & SQLSpatialRelationships.AnyInteract) != 0)
            {
                lista.Add("ANYINTERACT");
            }
            if ((relations & SQLSpatialRelationships.Contains) != 0)
            {
                lista.Add("CONTAINS");
            }
            if ((relations & SQLSpatialRelationships.CoveredBy) != 0)
            {
                lista.Add("COVEREDBY");
            }
            if ((relations & SQLSpatialRelationships.Covers) != 0)
            {
                lista.Add("COVERS");
            }
            if ((relations & SQLSpatialRelationships.Crosses) != 0)
            {
                lista.Add("OVERLAPBDYINTERSECT");
            }
            if((relations & SQLSpatialRelationships.Disjoint)!= 0)
            {
                lista.Add("DISJOINT");
            }
            if ((relations & SQLSpatialRelationships.Equal) != 0)
            {
                lista.Add("EQUAL");
            }
            if ((relations & SQLSpatialRelationships.Inside) != 0)
            {
                lista.Add("INSIDE");
            }
            if ((relations & SQLSpatialRelationships.Overlaps) != 0)
            {
                lista.Add("OVERLAPBDYDISJOINT");
                lista.Add("OVERLAPBDYINTERSECT");
            }
            if ((relations & SQLSpatialRelationships.Touch) != 0)
            {
                lista.Add("TOUCH");
            }
            return $"SDO_RELATE({this},{geometry}, 'MASK={string.Join("+", lista)}')='TRUE'";
        }
        public override string ToString()
        {
            return this.field;
        }

        private ISQLGeometryFieldBuilder area(bool sqrmeters = false)
        {
            this.field = string.Format("ROUND(SDO_GEOM.SDO_AREA({0},{1},'{2}'),2)", this.field, TOLERANCE, (sqrmeters ? "unit=SQ_M" : string.Empty));
            return this;
        }
        private ISQLGeometryFieldBuilder perimeter_length(bool meters = false)
        {
            this.field = string.Format("ROUND(SDO_GEOM.SDO_LENGTH({0},{1},'{2}'),2)", this.field, TOLERANCE, (meters ? "unit=M" : string.Empty));
            return this;
        }
    }
}
