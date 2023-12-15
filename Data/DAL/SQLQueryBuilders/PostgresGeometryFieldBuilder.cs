using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.DAL.SQLQueryBuilders
{
    internal class PostgresGeometryFieldBuilder : ISQLGeometryFieldBuilder
    {
        private string field = string.Empty;

        private string original = string.Empty;
        private SRIDParser sridParser;

        public PostgresGeometryFieldBuilder(SRIDParser parser)
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
            this.field = string.Format("st_geomfromewkt('SRID={0};{1}')", srid, wkt);
            this.original = this.field;
            return this;
        }
        public ISQLGeometryFieldBuilder FromWKT(string wkt, SRID srid)
        {
            return this.FromWKT(wkt, this.sridParser.Parse(srid));
        }
        public ISQLGeometryFieldBuilder FromParameter(DbParameter param, int srid)
        {
            this.field = string.Format("st_geomfromewkt('SRID={0};' || {1})", srid, param.ParameterName);
            this.original = this.field;
            return this;
        }
        public ISQLGeometryFieldBuilder FromParameter(DbParameter param, SRID srid)
        {
            return this.FromParameter(param, this.sridParser.Parse(srid));
        }
        public ISQLGeometryFieldBuilder ToExtendedWKT()
        {
            this.field = string.Format("st_asewkt({0})", this.field);
            return this;
        }
        public ISQLGeometryFieldBuilder ToWKT()
        {
            this.field = string.Format("st_astext({0})", this.field);
            return this;
        }
        public ISQLGeometryFieldBuilder AddBuffer(double buffer)
        {
            this.field = string.Format("st_buffer({0},{1})", this.field, buffer);
            return this;
        }
        public ISQLGeometryFieldBuilder ChangeToSRID(int srid)
        {
            this.field = string.Format("st_transform({0},{1})", this.field, srid);
            return this;
        }
        public ISQLGeometryFieldBuilder ChangeToSRID(SRID srid)
        {
            return this.ChangeToSRID(this.sridParser.Parse(srid));
        }
        public ISQLGeometryFieldBuilder Centroid()
        {
            this.field = string.Format("(case when geometrytype({0}) = 'LINESTRING' then st_lineinterpolatepoint({0},0.5) else st_pointonsurface({0}) end)", this.field);
            return this;
        }
        public ISQLGeometryFieldBuilder IntersectionWith(ISQLGeometryFieldBuilder geometry)
        {
            this.field = string.Format("st_intersection({0},{1})", this.field, geometry);
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
            return this.length();
        }
        public ISQLGeometryFieldBuilder LengthMeters()
        {
            if (!this.sridParser.IsDbLatLon())
            {
                this.ChangeToSRID(this.sridParser.Parse(SRID.LL84));
            }
            return this.length(true);
        }
        public ISQLGeometryFieldBuilder Perimeter()
        {
            return this.perimeter();
        }
        public ISQLGeometryFieldBuilder PerimeterMeters()
        {
            if (!this.sridParser.IsDbLatLon())
            {
                this.ChangeToSRID(this.sridParser.Parse(SRID.LL84));
            }
            return this.perimeter(true);
        }
        public ISQLGeometryFieldBuilder NumPoints()
        {
            this.field = string.Format("st_npoints({0})", this.field);
            return this;
        }
        public ISQLGeometryFieldBuilder OverlappingArea(ISQLGeometryFieldBuilder geometry)
        {
            return this.IntersectionWith(geometry).Area();
        }
        public ISQLGeometryFieldBuilder BoundingBox()
        {
            this.field = string.Format("st_extent({0})", this.field);
            return this;
        }
        public ISQLGeometryFieldBuilder ConvexHull()
        {
            this.field = string.Format("st_convexhull(st_collect({0}))", this.field);
            return this;
        }
        public ISQLGeometryFieldBuilder GType()
        {
            this.field = string.Format(@"case 
                                       when position('polygon' in lower(st_geometrytype({0}))) != 0 then 1 
                                       when position('line' in lower(st_geometrytype({0}))) != 0 then 2  
                                       when position('curve' in lower(st_geometrytype({0}))) != 0 then 2  
                                       when position('point' in lower(st_geometrytype({0}))) != 0 then 3
                                       else 5 end", this.original);
            return this;
        }
        public ISQLGeometryFieldBuilder Distance(ISQLGeometryFieldBuilder geometry)
        {
            this.field = $"round(cast(st_distance({this.field},{geometry}) as numeric),2)";
            return this;
        }
        public string FormatSpatialFilter(ISQLGeometryFieldBuilder geometry, SQLSpatialRelationships relations)
        {
            var lista = new List<string>();
            if ((relations & SQLSpatialRelationships.AnyInteract) != 0)
            {
                lista.Add("st_intersects");
            }
            if ((relations & SQLSpatialRelationships.Contains) != 0)
            {
                lista.Add("st_contains");
            }
            if ((relations & SQLSpatialRelationships.CoveredBy) != 0)
            {
                lista.Add("st_coveredby");
            }
            if ((relations & SQLSpatialRelationships.Covers) != 0)
            {
                lista.Add("st_covers");
            }
            if ((relations & SQLSpatialRelationships.Crosses) != 0)
            {
                lista.Add("st_crosses");
            }
            if ((relations & SQLSpatialRelationships.Disjoint) != 0)
            {
                lista.Add("st_disjoint");
            }
            if ((relations & SQLSpatialRelationships.Equal) != 0)
            {
                lista.Add("st_equals");
            }
            if ((relations & SQLSpatialRelationships.Inside) != 0)
            {
                lista.Add("st_within");
            }
            if ((relations & SQLSpatialRelationships.Overlaps) != 0)
            {
                lista.Add("st_overlaps");
            }
            if ((relations & SQLSpatialRelationships.Touch) != 0)
            {
                lista.Add("st_touches");
            }
            return $"({string.Join(" or ", lista.Select(op => $"{op}({this},{geometry})"))})";
        }
        public override string ToString()
        {
            return this.field;
        }

        private ISQLGeometryFieldBuilder area(bool sqrmeters = false)
        {
            this.field = string.Format("round(cast(st_area({0}{1}) as numeric),2)", this.field, (sqrmeters ? "::geography" : string.Empty));
            return this;
        }
        private ISQLGeometryFieldBuilder perimeter(bool meters = false)
        {
            this.field = string.Format("round(cast(st_perimeter({0}{1}) as numeric),2)", this.field, (meters ? "::geography" : string.Empty));
            return this;
        }
        private ISQLGeometryFieldBuilder length(bool meters = false)
        {
            this.field = string.Format("round(cast(st_length({0}{1}) as numeric),2)", this.field, (meters ? "::geography" : string.Empty));
            return this;
        }
    }
}
