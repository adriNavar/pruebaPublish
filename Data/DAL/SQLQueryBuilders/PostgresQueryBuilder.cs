using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Contexts;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace GeoSit.Data.DAL.SQLQueryBuilders
{
    internal class PostgresQueryBuilder : QueryBuilder
    {
        private bool hasNoTable = false;
        internal PostgresQueryBuilder(GeoSITMContext context)
            : base(context, new PostgresGeometryFieldBuilder(context.GetSRIDParser())) { }
        protected override string getFormattedTableName(Componente componente, string alias)
        {
            alias = (alias ?? string.Empty).ToLower();
            if (!tableAlias.ContainsKey(alias))
            {
                tableAlias.Add(alias, componente.ComponenteId);
            }
            return string.Format("\"{0}\".\"{1}\" {2}", (componente.Esquema ?? System.Configuration.ConfigurationManager.AppSettings["DATABASE"]).ToLower(), componente.Tabla.ToLower(), alias);
        }
        protected override string getFormattedFunctionTableName(string esquema, string funcion, string alias)
        {
            alias = (alias ?? string.Empty).ToLower();
            if (!tableAlias.ContainsKey(alias))
            {
                tableAlias.Add(alias, long.MinValue);
            }
            return string.Format("\"{0}\".{1} {2}", (esquema ?? System.Configuration.ConfigurationManager.AppSettings["DATABASE"]).ToLower(), funcion.ToLower(), alias);
        }
        protected override string getFormattedField(Atributo attr, string customtablealias = null)
        {
            var alias = tableAlias.LastOrDefault(a => a.Value == attr.ComponenteId).Key ?? attr.Componente?.Tabla;
            if (!string.IsNullOrEmpty(attr.Funcion))
            {
                //return attr.Funcion.Replace("@T@", alias).Replace("@C@", attr.Campo.ToLower());
                return attr.Funcion.Replace("@T@", alias).Replace("@C@", $"\"{attr.Campo.ToLower()}\"");
            }
            else
            {
                alias = customtablealias ?? alias;
                return string.Format("{0}\"{1}\"", (string.IsNullOrEmpty(alias) ? string.Empty : string.Concat(alias, ".")), attr.Campo.ToLower());
            }
        }
        protected override object getFormattedValue(Atributo attr, object value)
        {
            switch (attr.TipoDatoId)
            {
                case 666:
                    value = " current_timestamp ";
                    break;
                default:
                    value = base.getFormattedValue(attr, value);
                    break;
            }
            return value;
        }
        protected override string getSelectQueryFormat()
        {
            return hasNoTable ? "select {0} {1}" : "select {0} {1} from {2} {3} {4} {5} {6} {7} {8}";
        }
        protected override string getFormattedLimit(int max_results)
        {
            return string.Format("limit {0}", max_results);
        }
        protected override string getFreeSpaceQueryFormat()
        {
            return "vacuum {0}";
        }
        protected override void addTableFieldsMetadata(string esquema, string tabla)
        {
            this.AddTable("information_schema", "columns", null)
                .AddFields("column_name")
                .AddFilter("table_schema", $"'{esquema}'", SQLOperators.EqualsTo)
                .AddFilter("table_name", $"'{tabla.ToLower()}'", SQLOperators.EqualsTo, SQLConnectors.And);
        }
        protected override void addNoTable()
        {
            hasNoTable = true;
        }
        protected override string getFormattedMaterializedViewQuery(string esquema, string vista)
        {
            return $"refresh materialized view concurrently {esquema.ToLower()}.\"{vista.ToLower()}\"";
        }
    }
}
