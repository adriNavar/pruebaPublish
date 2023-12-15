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
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;

namespace GeoSit.Data.DAL.Repositories.EditoresGraficos
{
    public class EditorGraficoMunicipiosRepository : BaseRepository<Objeto, long>
    {
        const string CLAVE_PARAMETRO_COMPONENTE = "ID_COMPONENTE_MUNICIPIO";
        const long EMPTY_ID = -1;
        readonly long TipoObjetoId;
        public EditorGraficoMunicipiosRepository(GeoSITMContext ctx)
            : base(ctx)
        {
            TipoObjetoId = long.Parse(TiposObjetoAdministrativo.MUNICIPIO);
        }

        public DataTableResult<GrillaMunicipio> RecuperarMunicipios(DataTableParameters parametros)
        {
            var joins = new Expression<Func<Objeto, dynamic>>[] { x => x.Padre };
            var filtros = new List<Expression<Func<Objeto, bool>>>()
            {
                municipio => municipio.TipoObjetoId == TipoObjetoId,
                municipio => municipio.FechaBaja == null
            };
            var sorts = new List<SortClause<Objeto>>();

            foreach (var column in parametros.columns.Where(c => !string.IsNullOrEmpty(c.search.value)))
            {
                switch (column.name)
                {
                    case "Nombre":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(municipio => municipio.Nombre.ToLower().Contains(val));
                        }
                        break;
                    case "Codigo":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(municipio => municipio.Codigo.ToLower().Contains(val));
                        }
                        break;
                    case "Nomenclatura":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(municipio => municipio.Nomenclatura.ToLower().Contains(val));
                        }
                        break;
                    case "Departamento":
                        {
                            long val = long.Parse(column.search.value);
                            if (EMPTY_ID != val)
                            {
                                filtros.Add(municipio => municipio.ObjetoPadreId == val);
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
                    sorts.Add(new SortClause<Objeto>() { Expresion = municipio => municipio.Codigo, ASC = asc });
                    break;
                case "Nomenclatura":
                    sorts.Add(new SortClause<Objeto>() { Expresion = municipio => municipio.Nomenclatura, ASC = asc });
                    break;
                case "Departamento":
                    sorts.Add(new SortClause<Objeto>() { Expresion = municipio => municipio.Padre.Nombre, ASC = asc });
                    break;
                default:
                    OrdenDefaultASC = asc;
                    break;
            }
            return ObtenerPagina(GetBaseQuery(joins, filtros, sorts), parametros, map);
        }
        public MunicipioDTO RecuperarMunicipio(long featid)
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

                string categoria = null;
                int? poblacion = null;
                decimal? superficie = null;
                if (!string.IsNullOrEmpty(objeto.Atributos))
                {
                    var xml = new XmlDocument();
                    xml.LoadXml(objeto.Atributos);
                    categoria = xml.SelectSingleNode("//datos/categoria/text()")?.Value;
                    poblacion = !int.TryParse(xml.SelectSingleNode("//datos/poblacion/text()")?.Value, out int iaux) ? (int?)null : iaux;
                    superficie = !decimal.TryParse(xml.SelectSingleNode("//datos/superficie/text()")?.Value, out decimal daux) ? (decimal?)null : daux;
                }

                return new MunicipioDTO()
                {
                    FeatId = objeto.FeatId,
                    Alias = objeto.Alias,
                    Codigo = objeto.Codigo,
                    DepartamentoId = objeto.ObjetoPadreId,
                    Descripcion = objeto.Descripcion,
                    Nombre = objeto.Nombre,
                    Nomenclatura = objeto.Nomenclatura,
                    Categoria = categoria,
                    Poblacion = poblacion,
                    Superficie = superficie,
                    WKT = wkt,
                };
            }
        }
        public long GuardarMunicipio(MunicipioDTO municipio)
        {
            var valRepo = new ValidacionDBRepository(this.GetContexto());
            var obj = new ObjetoValidable()
            {
                TipoObjeto = TipoObjetoValidable.EdicionMunicipio,
                IdObjeto = 0,
                WKT = municipio.WKT,
                Codigo1 = municipio.FeatId.ToString(),
                Codigo2 = municipio.DepartamentoId.ToString(),
            };
            obj.Funcion = FuncionValidable.Todas;
            ResultadoValidacion resultado = valRepo.Validar(obj, out List<string> erroresValidacion);

            using (var trans = GetContexto().Database.BeginTransaction())
            using (var qbuilder = GetContexto().CreateSQLQueryBuilder())
            {
                try
                {
                    Objeto existente;
                    DateTime now = DateTime.Now;
                    if (municipio.FeatId == 0)
                    {
                        existente = ObtenerDbSet()
                                        .Add(new Objeto
                                        {
                                            TipoObjetoId = TipoObjetoId,
                                            FechaAlta = now,
                                            UsuarioAlta = municipio.UsuarioId
                                        });
                    }
                    else
                    {
                        existente = ObtenerDbSet().Single(o => o.FeatId == municipio.FeatId && o.TipoObjetoId == TipoObjetoId);
                        GetContexto().Entry(existente).Property(p => p.TipoObjetoId).IsModified = false;
                        GetContexto().Entry(existente).Property(p => p.FechaAlta).IsModified = false;
                        GetContexto().Entry(existente).Property(p => p.UsuarioAlta).IsModified = false;
                    }
                    
                    existente.Alias = municipio.Alias;
                    existente.Codigo = municipio.Codigo;
                    existente.Descripcion = municipio.Descripcion;
                    existente.Nombre = municipio.Nombre;
                    existente.Nomenclatura = municipio.Nomenclatura;
                    existente.ObjetoPadreId = municipio.DepartamentoId;

                    existente.FechaModificacion = now;
                    existente.UsuarioModificacion = municipio.UsuarioId;

                    existente.Atributos = municipio.WriteAtributos(existente.Atributos);

                    GetContexto().SaveChanges();

                    long idComponente = long.Parse(GetContexto().ParametrosGenerales.Single(pg => pg.Clave == CLAVE_PARAMETRO_COMPONENTE).Valor);
                    var cmp = GetContexto().Componente
                                           .Include(c => c.Atributos)
                                           .Single(c => c.ComponenteId == idComponente);

                    ISQLGeometryFieldBuilder geometry = null;
                    if (!string.IsNullOrEmpty(municipio.WKT))
                    {
                        geometry = qbuilder.CreateGeometryFieldBuilder(municipio.WKT, Common.Enums.SRID.DB);
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
                    GetContexto().GetLogger().LogError("EditorGraficoMunicipiosRepository/GuardarMunicipio", ex);
                    throw;
                }
            }
        }
        public bool EliminarMunicipio(long featid, long usuario)
        {
            try
            {
                var municipio = ObtenerDbSet().Single(o => o.FeatId == featid && o.TipoObjetoId == TipoObjetoId);
                municipio.UsuarioBaja = municipio.UsuarioModificacion = usuario;
                municipio.FechaBaja = municipio.FechaModificacion = DateTime.Now;
                GetContexto().SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError("EditorGraficoMunicipiosRepository/EliminarMunicipio", ex);
                throw;
            }
        }
        protected override DbSet<Objeto> ObtenerDbSet()
        {
            return GetContexto().Objetos;
        }
        protected override Expression<Func<Objeto, object>> OrdenDefault()
        {
            return municipio => municipio.Nombre;
        }
        private GrillaMunicipio map(Objeto municipio)
        {
            return new GrillaMunicipio()
            {
                Codigo = municipio.Codigo,
                FeatId = municipio.FeatId,
                Nombre = municipio.Nombre,
                Nomenclatura = municipio.Nomenclatura,
                Departamento = municipio.Padre?.Nombre
            };
        }
    }
}
