using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.EditorGrafico.DTO;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos.DTO;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Implementaciones;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace GeoSit.Data.DAL.Repositories.EditoresGraficos
{
    public class EditorGraficoSeccionesRepository : BaseRepository<Objeto, long>
    {
        const string CLAVE_PARAMETRO_COMPONENTE = "ID_COMPONENTE_SECCION";
        const long EMPTY_ID = -1;
        readonly long TipoObjetoId;
        public EditorGraficoSeccionesRepository(GeoSITMContext ctx)
            : base(ctx)
        {
            TipoObjetoId = long.Parse(TiposObjetoAdministrativo.SECCION);
        }
        public DataTableResult<GrillaSeccion> RecuperarSecciones(DataTableParameters parametros)
        {
            var joins = new Expression<Func<Objeto, dynamic>>[] { x => x.Padre };
            var filtros = new List<Expression<Func<Objeto, bool>>>()
            {
                seccion => seccion.TipoObjetoId == TipoObjetoId,
                seccion => seccion.FechaBaja == null
            };
            var sorts = new List<SortClause<Objeto>>();

            foreach (var column in parametros.columns.Where(c => !string.IsNullOrEmpty(c.search.value)))
            {
                switch (column.name)
                {
                    case "Nombre":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(seccion => seccion.Nombre.ToLower().Contains(val));
                        }
                        break;
                    case "Codigo":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(seccion => seccion.Codigo.ToLower().Contains(val));
                        }
                        break;
                    case "Nomenclatura":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(seccion => seccion.Nomenclatura.ToLower().Contains(val));
                        }
                        break;
                    case "Departamento":
                        {
                            long val = long.Parse(column.search.value);
                            if (EMPTY_ID != val)
                            {
                                filtros.Add(seccion => seccion.ObjetoPadreId == val);
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
                    sorts.Add(new SortClause<Objeto>() { Expresion = seccion => seccion.Codigo, ASC = asc });
                    break;
                case "Nomenclatura":
                    sorts.Add(new SortClause<Objeto>() { Expresion = seccion => seccion.Nomenclatura, ASC = asc });
                    break;
                case "Departamento":
                    sorts.Add(new SortClause<Objeto>() { Expresion = seccion => seccion.Padre.Nombre, ASC = asc });
                    break;
                default:
                    OrdenDefaultASC = asc;
                    break;
            }
            return ObtenerPagina(GetBaseQuery(joins, filtros, sorts), parametros, map);
        }
        public SeccionDTO RecuperarSeccion(long featid)
        {
            using (var qbuilder = GetContexto().CreateSQLQueryBuilder())
            {
                var objeto = ObtenerDbSet().Single(o => o.FeatId == featid && o.TipoObjetoId == TipoObjetoId);

                long idComponente = long.Parse(GetContexto().ParametrosGenerales.Single(pg => pg.Clave == CLAVE_PARAMETRO_COMPONENTE).Valor);
                var cmp = GetContexto().Componente
                                       .Include(c => c.Atributos)
                                       .Single(c => c.ComponenteId == idComponente);

                var geomField = qbuilder.CreateGeometryFieldBuilder(cmp.Atributos.GetAtributoGeometry(), "t1")
                                        .ToWKT();

                string wkt = qbuilder.AddTable(cmp, "t1")
                                       .AddFilter(cmp.Atributos.GetAtributoClave(), featid, Common.Enums.SQLOperators.EqualsTo)
                                       .AddFilter(cmp.Atributos.GetAtributoGeometry(), null, Common.Enums.SQLOperators.IsNotNull, Common.Enums.SQLConnectors.And)
                                       .AddGeometryField(geomField, "wkt")
                                       .ExecuteQuery((IDataReader reader) =>
                                       {
                                           return reader.GetString(0);
                                       })
                                       .SingleOrDefault();

                return new SeccionDTO()
                {
                    FeatId = objeto.FeatId,
                    Alias = objeto.Alias,
                    Codigo = objeto.Codigo,
                    DepartamentoId = objeto.ObjetoPadreId,
                    Descripcion = objeto.Descripcion,
                    Nombre = objeto.Nombre,
                    Nomenclatura = objeto.Nomenclatura,
                    WKT = wkt,
                };
            }
        }
        public long GuardarSeccion(SeccionDTO seccion)
        {
            var valRepo = new ValidacionDBRepository(this.GetContexto());
            var obj = new ObjetoValidable()
            {
                TipoObjeto = TipoObjetoValidable.EdicionSeccion,
                IdObjeto = 0,
                WKT = seccion.WKT,
                Codigo1 = seccion.FeatId.ToString(),
                Codigo2 = seccion.DepartamentoId.ToString(),
            };
            ResultadoValidacion resultado;
            obj.Funcion = FuncionValidable.Todas;
            resultado = valRepo.Validar(obj, out List<string> erroresValidacion);

            using (var trans = GetContexto().Database.BeginTransaction())
            using (var qbuilder = GetContexto().CreateSQLQueryBuilder())
            {
                try
                {
                    Objeto existente;
                    DateTime now = DateTime.Now;
                    if (seccion.FeatId == 0)
                    {
                        existente = ObtenerDbSet()
                                        .Add(new Objeto
                                        {
                                            TipoObjetoId = TipoObjetoId,
                                            FechaAlta = now,
                                            UsuarioAlta = seccion.UsuarioId
                                        });
                    }
                    else
                    {
                        existente = ObtenerDbSet().Single(o => o.FeatId == seccion.FeatId && o.TipoObjetoId == TipoObjetoId);
                        GetContexto().Entry(existente).Property(p => p.TipoObjetoId).IsModified = false;
                        GetContexto().Entry(existente).Property(p => p.FechaAlta).IsModified = false;
                        GetContexto().Entry(existente).Property(p => p.UsuarioAlta).IsModified = false;
                    }

                    existente.Alias = seccion.Alias;
                    existente.Codigo = seccion.Codigo;
                    existente.Descripcion = seccion.Descripcion;
                    existente.Nombre = seccion.Nombre;
                    existente.Nomenclatura = seccion.Nomenclatura;
                    existente.ObjetoPadreId = seccion.DepartamentoId;

                    existente.FechaModificacion = now;
                    existente.UsuarioModificacion = seccion.UsuarioId;

                    GetContexto().SaveChanges();

                    long idComponente = long.Parse(GetContexto().ParametrosGenerales.Single(pg => pg.Clave == CLAVE_PARAMETRO_COMPONENTE).Valor);
                    var cmp = GetContexto().Componente
                                           .Include(c => c.Atributos)
                                           .Single(c => c.ComponenteId == idComponente);

                    object geometry = null;
                    if (!string.IsNullOrEmpty(seccion.WKT))
                    {
                        geometry = qbuilder.CreateGeometryFieldBuilder(seccion.WKT, Common.Enums.SRID.DB);
                    }

                    qbuilder.AddTable(new Componente() { Esquema = cmp.Esquema, Tabla = "OA_OBJETO", ComponenteId = cmp.ComponenteId }, null)
                            .AddFilter(cmp.Atributos.GetAtributoClave(), existente.FeatId, Common.Enums.SQLOperators.EqualsTo)
                            .AddFieldsToUpdate(new KeyValuePair<Atributo, object>(cmp.Atributos.GetAtributoGeometry(), geometry))
                            .ExecuteUpdate();

                    trans.Commit();
                    return existente.FeatId;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    GetContexto().GetLogger().LogError("EditorGraficoSeccionesRepository/GuardarSeccion", ex);
                    throw;
                }
            }
        }
        public bool EliminarSeccion(long featid, long usuario)
        {
            try
            {
                var seccion = ObtenerDbSet().Single(o => o.FeatId == featid && o.TipoObjetoId == TipoObjetoId);
                seccion.UsuarioBaja = seccion.UsuarioModificacion = usuario;
                seccion.FechaBaja = seccion.FechaModificacion = DateTime.Now;
                GetContexto().SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError("EditorGraficoSeccionesRepository/EliminarSeccion", ex);
                throw;
            }
        }
        protected override DbSet<Objeto> ObtenerDbSet()
        {
            return GetContexto().Objetos;
        }
        protected override Expression<Func<Objeto, object>> OrdenDefault()
        {
            return seccion => seccion.Nombre;
        }
        private GrillaSeccion map(Objeto seccion)
        {
            return new GrillaSeccion()
            {
                Codigo = seccion.Codigo,
                FeatId = seccion.FeatId,
                Nombre = seccion.Nombre,
                Nomenclatura = seccion.Nomenclatura,
                Departamento = seccion.Padre?.Nombre
            };
        }
    }
}
