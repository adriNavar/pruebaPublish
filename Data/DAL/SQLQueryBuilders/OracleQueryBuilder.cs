using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Contexts;
using System;
using System.Linq;

namespace GeoSit.Data.DAL.SQLQueryBuilders
{
    internal class OracleQueryBuilder : QueryBuilder
    {
        public OracleQueryBuilder(GeoSITMContext context)
            : base(context, new OracleGeometryFieldBuilder(context.GetSRIDParser())) { }

        protected override string getFormattedTableName(Componente componente, string alias)
        {
            string key = alias ?? string.Empty;
            if (!tableAlias.ContainsKey(key))
            {
                tableAlias.Add(key, componente.ComponenteId);
            }
            return string.Format("{0}{1} {2}", (string.IsNullOrEmpty(componente.Esquema) ? string.Empty : string.Concat(componente.Esquema, ".")), componente.Tabla.ToUpper(), alias);
        }
        protected override string getFormattedFunctionTableName(string esquema, string funcion, string alias)
        {
            string key = alias ?? string.Empty;
            if (!tableAlias.ContainsKey(key))
            {
                tableAlias.Add(alias, long.MinValue);
            }
            return string.Format("{0}{1} {2}", (string.IsNullOrEmpty(esquema) ? string.Empty : string.Concat(esquema, ".")), funcion.ToUpper(), alias);
        }
        protected override string getFormattedField(Atributo attr, string customtablealias = null)
        {
            var alias = tableAlias.LastOrDefault(a => a.Value == attr.ComponenteId).Key ?? attr.Componente.Tabla;
            if (!string.IsNullOrEmpty(attr.Funcion))
            {
                return attr.Funcion.Replace("@T@", alias).Replace("@C@", attr.Campo.ToUpper());
            }
            else
            {
                alias = customtablealias ?? alias;
                return string.Format("{0}\"{1}\"", (string.IsNullOrEmpty(alias) ? string.Empty : string.Concat(alias, ".")), attr.Campo.ToUpper());
            }
        }
        protected override string getSelectQueryFormat()
        {
            return "select {0} {1} from {2} {3} {4} {5} {6} {7} {8}";
        }
        protected override string getFormattedLimit(int max_results)
        {
            return $"ROWNUM < {max_results + 1}";
        }
        protected override string getFreeSpaceQueryFormat()
        {
            /*
             * la sentencia para liberar espacio desperdiciado es:
             *      => ALTER TABLE NOMBRE_TABLA SHRINK SPACE
             * pero tiene un problema, la tabla necesita tener habilitada 
             * la opcion ROW MOVEMENT, la cual se habilita 
             *      => ALTER TABLE NOMBRE_TABLA ENABLE ROW MOVEMENT
             * Ademas, tiene un problema, no permite purgar tablas con 
             * índices espaciales (mdsys.spatial_index), por lo que para 
             * que funcione el SHRINK SPACE, es necesario hacer drop del 
             * indice y luego volver a crearlo. por este motivo, no implemento
             * esta funcion para Oracle.
             */
            throw new NotSupportedException();
        }
        protected override void addTableFieldsMetadata(string esquema, string tabla)
        {
            this.AddTable("SYS", "ALL_TAB_COLUMNS", string.Empty)
                .AddFields("COLUMN_NAME")
                .AddFilter("OWNER", $"'{esquema}'", SQLOperators.EqualsTo)
                .AddFilter("TABLE_NAME", $"'{tabla}'", SQLOperators.EqualsTo, SQLConnectors.And);
        }
        protected override void addNoTable()
        {
            this.AddTable(null, "dual", string.Empty);
        }

        protected override string getFormattedMaterializedViewQuery(string esquema, string vista)
        {
            /*
             * por ahora no implemento, porque habría que revisar:
             *      => DBMS_MVIEW.REFRESH('VIEW_NAME', method => '?', atomic_refresh => FALSE, out_of_place => TRUE);
             */
            throw new NotImplementedException();
        }
    }
}