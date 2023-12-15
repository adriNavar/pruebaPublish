using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.Actions.Abstract
{
    abstract class AccionEntrada : Accion
    {
        protected int IdEntrada { get; private set; }
        protected AccionEntrada(int idEntrada, METramite tramite, GeoSITMContext contexto)
            : base(tramite, contexto)
        {
            Ambito = Ambito.Entrada;
            IdEntrada = idEntrada;
        }
        public override bool Execute()
        {
            try
            {
                foreach (var entrada in GetEntradas(IdEntrada).ToList())
                {
                    ExecuteEntrada(entrada);
                    Contexto.SaveChanges();
                }
                Resultado = ResultadoValidacion.Ok;
                return true;
            }
            catch (Exception ex)
            {
                Resultado = ResultadoValidacion.Error;
                Errores = new List<string>() { { ex.Message } };
                return false;
            }
        }
        abstract protected void ExecuteEntrada(METramiteEntrada entrada);

        protected void CopyToTemporal(string tabla, Dictionary<string, object> filtros, List<KeyValuePair<Atributo, object>> campos = null)
        {
            using (var queryEsquema = Contexto.CreateSQLQueryBuilder())
            using (var insertBuilder = Contexto.CreateSQLQueryBuilder())
            using (var querybuilder = Contexto.CreateSQLQueryBuilder())
            {
                try
                {
                    var camposEsquema = campos ?? queryEsquema.GetTableFields(ConfigurationManager.AppSettings["DATABASE"], tabla)
                                                              .Select(c => new KeyValuePair<Atributo, object>(new Atributo() { Campo = c }, c));

                    querybuilder.AddTable(ConfigurationManager.AppSettings["DATABASE"], tabla, null);

                    Common.Enums.SQLConnectors conector = Common.Enums.SQLConnectors.None;
                    foreach (var filtro in filtros)
                    {
                        querybuilder.AddFilter(filtro.Key, filtro.Value, Common.Enums.SQLOperators.EqualsTo, conector);
                        conector = Common.Enums.SQLConnectors.And;
                    }


                    var fechaBaja = camposEsquema.Select(c => c.Key).SingleOrDefault(c => c.Campo.ToLower() == "fecha_baja");

                    if (fechaBaja != null)
                    {
                        querybuilder.AddFilter(fechaBaja.Campo, null, Common.Enums.SQLOperators.IsNull, Common.Enums.SQLConnectors.And);
                    }

                    insertBuilder.AddTable("temporal", tabla, null)
                                 .AddQueryToInsert(querybuilder, camposEsquema.Concat(new[] { new KeyValuePair<Atributo, object>(new Atributo() { Campo = "id_tramite" }, Tramite.IdTramite) }).ToArray())
                                 .ExecuteInsert();

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
