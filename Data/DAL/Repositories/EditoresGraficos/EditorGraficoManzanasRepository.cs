using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.EditorGrafico.DTO;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Implementaciones;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace GeoSit.Data.DAL.Repositories.EditoresGraficos
{
    public class EditorGraficoManzanasRepository : BaseRepository<Division, long>
    {
        const string CLAVE_PARAMETRO_COMPONENTE = "ID_COMPONENTE_MANZANA";
        const string TABLA = "OA_DIVISION";
        const long EMPTY_ID = -1;
        public EditorGraficoManzanasRepository(GeoSITMContext ctx)
            : base(ctx) { }
        public DataTableResult<GrillaManzana> RecuperarManzanas(DataTableParameters parametros)
        {
            var joins = new Expression<Func<Division, dynamic>>[] { x => x.Localidad, x => x.TipoDivision };
            var filtros = new List<Expression<Func<Division, bool>>>()
            {
                division => division.FechaBaja == null
            };
            var sorts = new List<SortClause<Division>>();

            foreach (var column in parametros.columns.Where(c => !string.IsNullOrEmpty(c.search.value)))
            {
                switch (column.name)
                {
                    case "Nombre":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(division => division.Nombre.ToLower().Contains(val));
                        }
                        break;
                    case "Codigo":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(division => division.Codigo.ToLower().Contains(val));
                        }
                        break;
                    case "Nomenclatura":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(division => division.Nomenclatura.ToLower().Contains(val));
                        }
                        break;
                    case "TipoDivision":
                        {
                            long val = long.Parse(column.search.value);
                            if (EMPTY_ID != val)
                            {
                                filtros.Add(division => division.TipoDivisionId == val);
                            }
                        }
                        break;
                    case "Localidad":
                        {
                            long val = long.Parse(column.search.value);
                            if (EMPTY_ID != val)
                            {
                                filtros.Add(division => division.ObjetoPadreId == val);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            bool asc = parametros.order.FirstOrDefault().dir == "asc";
            switch (parametros.columns[parametros.order.FirstOrDefault().column].name)
            {
                case "Codigo":
                    sorts.Add(new SortClause<Division>() { Expresion = division => division.Codigo, ASC = asc });
                    break;
                case "Nomenclatura":
                    sorts.Add(new SortClause<Division>() { Expresion = division => division.Nomenclatura, ASC = asc });
                    break;
                case "TipoDivision":
                    sorts.Add(new SortClause<Division>() { Expresion = division => division.TipoDivision.Nombre, ASC = asc });
                    break;
                case "Localidad":
                    sorts.Add(new SortClause<Division>() { Expresion = division => division.Localidad.Nombre, ASC = asc });
                    break;
                default:
                    OrdenDefaultASC = asc;
                    break;
            }
            return ObtenerPagina(GetBaseQuery(joins, filtros, sorts), parametros, map);
        }
        public Division RecuperarManzana(long featid)
        {
            using (var qbuilder = GetContexto().CreateSQLQueryBuilder())
            {
                var division = ObtenerDbSet().Find(featid);
                long idComponente = long.Parse(GetContexto().ParametrosGenerales.Single(pg => pg.Clave == CLAVE_PARAMETRO_COMPONENTE).Valor);
                var cmp = GetContexto().Componente
                                       .Include(c => c.Atributos)
                                       .Single(c => c.ComponenteId == idComponente);

                var geomField = qbuilder.CreateGeometryFieldBuilder(cmp.Atributos.GetAtributoGeometry(), "t1")
                                        .ToWKT();

                division.WKT = qbuilder.AddTable(new Componente() { Esquema = cmp.Esquema, Tabla = TABLA, ComponenteId = cmp.ComponenteId }, "t1")
                                       .AddFilter(cmp.Atributos.GetAtributoClave(), featid, Common.Enums.SQLOperators.EqualsTo)
                                       .AddFilter(cmp.Atributos.GetAtributoGeometry(), null, Common.Enums.SQLOperators.IsNotNull, Common.Enums.SQLConnectors.And)
                                       .AddGeometryField(geomField, "wkt")
                                       .ExecuteQuery<string>((IDataReader reader) =>
                                       {
                                           return reader.GetString(0);
                                       })
                                       .SingleOrDefault();

                return division;
            }
        }
        public long GuardarManzana(Division division)
        {
            var valRepo = new ValidacionDBRepository(this.GetContexto());
            var obj = new ObjetoValidable()
            {
                TipoObjeto = TipoObjetoValidable.EdicionDivision,
                IdObjeto = 0,
                WKT = division.WKT,
                Codigo1 = division.FeatId.ToString(),
                Codigo2 = division.ObjetoPadreId.ToString(),
            };
            ResultadoValidacion resultado;
            obj.Funcion = FuncionValidable.Todas;
            resultado = valRepo.Validar(obj, out List<string> erroresValidacion);

            using (var trans = GetContexto().Database.BeginTransaction())
            using (var qbuilder = GetContexto().CreateSQLQueryBuilder())
            {
                try
                {
                    Division existente;
                    DateTime now = DateTime.Now;
                    if (division.FeatId == 0)
                    {
                        division.FechaAlta = now;
                        division.UsuarioAltaId = division._Id_Usuario;
                        existente = GetContexto().Divisiones.Add(division);
                    }
                    else
                    {
                        existente = ObtenerDbSet().Find(division.FeatId);
                        GetContexto().Entry(existente).Property(p => p.FechaAlta).IsModified = false;
                        GetContexto().Entry(existente).Property(p => p.UsuarioAltaId).IsModified = false;

                        existente.Alias = division.Alias;
                        existente.Codigo = division.Codigo;
                        existente.Descripcion = division.Descripcion;
                        existente.Nombre = division.Nombre;
                        existente.Nomenclatura = division.Nomenclatura;
                        existente.ObjetoPadreId = division.ObjetoPadreId;
                        existente.TipoDivisionId = division.TipoDivisionId;
                    }
                    existente.FechaModificacion = now;
                    existente.UsuarioModificacionId = division._Id_Usuario;

                    GetContexto().SaveChanges();

                    long idComponente = long.Parse(GetContexto().ParametrosGenerales.Single(pg => pg.Clave == CLAVE_PARAMETRO_COMPONENTE).Valor);
                    var cmp = GetContexto().Componente
                                           .Include(c => c.Atributos)
                                           .Single(c => c.ComponenteId == idComponente);

                    object geometry = null;
                    if (!string.IsNullOrEmpty(division.WKT))
                    {
                        geometry = qbuilder.CreateGeometryFieldBuilder(division.WKT, Common.Enums.SRID.DB);
                    }

                    qbuilder.AddTable(new Componente() { Esquema = cmp.Esquema, Tabla = TABLA, ComponenteId = cmp.ComponenteId }, null)
                            .AddFilter(cmp.Atributos.GetAtributoClave(), existente.FeatId, Common.Enums.SQLOperators.EqualsTo)
                            .AddFieldsToUpdate(new KeyValuePair<Atributo, object>(cmp.Atributos.GetAtributoGeometry(), geometry))
                            .ExecuteUpdate();

                    trans.Commit();
                    return existente.FeatId;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    GetContexto().GetLogger().LogError("EditorGraficoManzanasRepository/GuardarManzana", ex);
                    throw;
                }
            }
        }
        public bool EliminarManzana(long featid, long usuario)
        {
            try
            {
                var manzana = ObtenerDbSet().Find(featid);
                manzana.UsuarioBajaId = manzana.UsuarioModificacionId = usuario;
                manzana.FechaBaja = manzana.FechaModificacion = DateTime.Now;
                GetContexto().SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError("EditorGraficoManzanasRepository/EliminarManzana", ex);
                throw;
            }
        }
        protected override DbSet<Division> ObtenerDbSet()
        {
            return GetContexto().Divisiones;
        }
        protected override Expression<Func<Division, object>> OrdenDefault()
        {
            return division => division.Nombre;
        }
        private GrillaManzana map(Division division)
        {
            return new GrillaManzana()
            {
                Codigo = division.Codigo,
                FeatId = division.FeatId,
                Localidad = division.Localidad?.Nombre,
                Nombre = division.Nombre,
                Nomenclatura = division.Nomenclatura,
                TipoDivision = division.TipoDivision?.Nombre
            };
        }
    }
}
