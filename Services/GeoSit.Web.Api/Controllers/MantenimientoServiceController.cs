using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.Mantenimiento;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class MantenimientoServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        [HttpGet]
        [ResponseType(typeof(List<ComponenteTA>))]
        public IHttpActionResult GetListaComponenteTA()
        {
            List<ComponenteTA> lista = db.ComponenteTA.ToList();
            return Ok(lista);
        }

        [HttpGet]
        [ResponseType(typeof(List<AtributoTA>))]
        public IHttpActionResult GetListaAtributoTA()
        {
            List<AtributoTA> lista = db.AtributoTA.OrderBy(x => x.Orden).ToList();
            return Ok(lista);
        }

        [HttpGet]
        [ResponseType(typeof(List<AtributoTA>))]
        public IHttpActionResult GetListaAtributoTAById(long Id)
        {
            List<AtributoTA> lista = db.AtributoTA.Where(x => x.Id_Componente == Id && (x.Es_Visible || x.Es_Clave == 1)).OrderBy(x => x.Orden).ToList();
            foreach (var elemento in lista)
            {
                if (!String.IsNullOrEmpty(elemento.Tabla))
                {
                    elemento.Opciones = GetListaAtributoRelacionado(elemento.Tabla, elemento.Esquema, elemento.Campo_Relac, elemento.Descriptor);
                }
            }
            return Ok(lista);
        }

        [HttpGet]
        [ResponseType(typeof(DataTable))]
        public IHttpActionResult GetContenidoTabla(long Id)
        {
            try
            {
                var agrupado = (from componente in db.ComponenteTA
                                join attr in db.AtributoTA on componente.Id_Compoente equals attr.Id_Componente
                                where componente.Id_Compoente == Id && (attr.Es_Visible || attr.Es_Clave == 1)
                                group attr by componente into grupo
                                select new { componente = grupo.Key, atributos = grupo }).Single();

                if (agrupado.atributos.Any(attr => attr.Es_Clave == 1))
                {
                    return Ok(db.CreateSQLQueryBuilder()
                                    .AddTable(agrupado.componente.Esquema, agrupado.componente.Tabla, "t1")
                                    .AddFields(agrupado.atributos
                                                       .Where(atr => atr.Es_Clave == 1).Concat(agrupado.atributos.Where(atr => atr.Es_Clave != 1).OrderBy(atr => atr.Orden))
                                                       .Select(atr => atr.Campo).ToArray())
                                    .AddFilter("fecha_baja", null, SQLOperators.IsNull)
                                    .ExecuteDataTable());
                }
                return Ok(new DataTable());
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError(string.Format("GetContenidoTabla({0})", Id), ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ResponseType(typeof(Dictionary<string, string>))]
        public Dictionary<string, string> GetContenidoTablaAsignacion(long Id, long IdTabla)
        {
            try
            {
                var agrupado = (from comp in db.ComponenteTA
                                where comp.Id_Compoente == Id
                                join attr in db.AtributoTA on comp.Id_Compoente equals attr.Id_Componente
                                group attr by comp into grupo
                                select new { componente = grupo.Key, atributos = grupo }).Single();

                var valores = new Dictionary<string, string>();
                db.CreateSQLQueryBuilder()
                         .AddTable(agrupado.componente.Esquema, agrupado.componente.Tabla, "t1")
                         .AddFilter(agrupado.atributos.First(f => f.Es_Clave == 1).Campo, IdTabla, SQLOperators.EqualsTo)
                         .AddFields(agrupado.atributos.Select(f => f.Campo).ToArray())
                         .ExecuteQuery((IDataReader reader) =>
                         {
                             for (int i = 0; i < reader.FieldCount; i++)
                             {
                                 valores.Add(reader.GetName(i), reader.GetTypeFormattedStringValue(i));
                             }
                         });
                return valores;
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError(string.Format("GetContenidoTablaAsignacion({0},{1})", Id, IdTabla), ex);
                return new Dictionary<string, string>();
            }
        }

        //DELETE LOGICO REGISTRO
        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult GetEliminaRegistroAsignacion(TablasAuxiliares Valores)
        {
            try
            {
                long idComponente = long.Parse(Valores.ComponentesId);
                var metadata = (from comp in db.ComponenteTA
                                where comp.Id_Compoente == idComponente
                                join attr in db.AtributoTA on comp.Id_Compoente equals attr.Id_Componente
                                where attr.Es_Clave == 1
                                select new { componente = comp, clave = attr }).Single();

                if (metadata == null)
                {
                    return Ok(false);
                }

                var actualizar = new Dictionary<Atributo, object>();
                using (var metadataBuilder = db.CreateSQLQueryBuilder())
                {
                    foreach (string campo in metadataBuilder.GetTableFields(metadata.componente.Esquema, metadata.componente.Tabla))
                    {
                        if (string.Compare(campo, "fecha_baja", true) == 0 || string.Compare(campo, "fecha_modif", true) == 0)
                        {
                            actualizar.Add(new Atributo { Campo = campo, TipoDatoId = 666 }, null);
                        }
                        else if (string.Compare(campo, "id_usu_baja", true) == 0 || string.Compare(campo, "id_usu_modif", true) == 0)
                        {
                            actualizar.Add(new Atributo { Campo = campo }, Valores.Id_Usuario);
                        }
                    }
                }
                using (var updateBuilder = db.CreateSQLQueryBuilder())
                {
                    updateBuilder.AddTable(metadata.componente.Esquema, metadata.componente.Tabla, null)
                                 .AddFieldsToUpdate(actualizar.ToArray())
                                 .AddFilter(metadata.clave.Campo, Valores.TablaID, SQLOperators.EqualsTo);

                    return Ok(updateBuilder.ExecuteUpdate() != 0); //true si actualiza, false si no
                }
                #region comento hasta probarlo y luego borrar
                //ComponenteTA componente = db.ComponenteTA.FirstOrDefault(a => a.Id_Compoente == IdComponente);
                //string tabla = componente.Tabla;
                //List<AtributoTA> atributos = db.AtributoTA.Where(a => a.Id_Componente == IdComponente && (a.Es_Visible == 1 || a.Es_Clave == 1)).ToList();

                //string queryConsultaCamposAudit = "Select  COLUMN_NAME, TABLE_NAME from user_tab_columns where table_name='" + tabla + "'";
                //IDbCommand objCommConsultaC = db.Database.Connection.CreateCommand();
                //db.Database.Connection.Open();
                //objCommConsultaC.CommandText = queryConsultaCamposAudit;
                //IDataReader dataConsultaAudit = objCommConsultaC.ExecuteReader();

                //var ValoresAuditoriaModificar = "";
                //while (dataConsultaAudit.Read())
                //{
                //    switch (dataConsultaAudit.GetString(0))
                //    {
                //        case "FECHA_BAJA":
                //            ValoresAuditoriaModificar += ",FECHA_BAJA = TO_DATE('" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "', 'DD/MM/YYYY HH24:MI:SS') ";
                //            break;
                //        case "ID_USU_BAJA":
                //            ValoresAuditoriaModificar += ",ID_USU_BAJA = " + Valores.Id_Usuario + " ";
                //            break;
                //        default:
                //            break;
                //    }



                //}
                //if (ValoresAuditoriaModificar.Length > 0)
                //{
                //    ValoresAuditoriaModificar = ValoresAuditoriaModificar.Substring(1);
                //}

                //if (Valores.TablaID != null)
                //{


                //    string queryConsulta = "SELECT COUNT(*) ";
                //    var IdTabla = long.Parse(Valores.TablaID);
                //    queryConsulta = queryConsulta + " FROM " + componente.Tabla;
                //    queryConsulta += " WHERE " + atributos.Where(x => x.Es_Clave == 1).FirstOrDefault().Campo + " = " + IdTabla;
                //    //recupero el resultado de la query
                //    IDbCommand objCommConsulta = db.Database.Connection.CreateCommand();
                //    //          db.Database.Connection.Open();
                //    objCommConsulta.CommandText = queryConsulta;
                //    IDataReader dataConsulta = objCommConsulta.ExecuteReader();

                //    var existe = 0;
                //    while (dataConsulta.Read())
                //    {
                //        existe = dataConsulta.GetInt32(0);
                //    }

                //    if (existe != 0)
                //    {
                //        //MODIFICA UN REGISTRO
                //        //TipoABM = "M";
                //        string queryUpdate = "UPDATE ";
                //        queryUpdate = queryUpdate + componente.Tabla + " SET ";
                //        queryUpdate += ValoresAuditoriaModificar;
                //        queryUpdate += " WHERE " + atributos.Where(x => x.Es_Clave == 1).FirstOrDefault().Campo + " = " + IdTabla;

                //        //recupero el resultado de la query

                //        objCommConsulta.CommandText = queryUpdate;
                //        IDataReader dataUpdate = objCommConsulta.ExecuteReader();
                //        //List<object[]> lista = new List<object[]>();
                //        success = true;


                //    }
                //    db.Database.Connection.Close();
                //} 
                #endregion
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError(string.Format("GetEliminaRegistroAsignacion(comp:{0},id:{1})", Valores.ComponentesId, Valores.TablaID), ex);
                return Ok(false);
            }
        }

        //INSERT REGISTRO
        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SetAgregarRegistro(TablasAuxiliares Valores)
        {
            try
            {
                long idComponente = long.Parse(Valores.ComponentesId);
                var metadata = (from comp in db.ComponenteTA
                                where comp.Id_Compoente == idComponente
                                join attr in db.AtributoTA on comp.Id_Compoente equals attr.Id_Componente
                                where attr.Es_Clave == 1 || attr.Es_Visible
                                group attr by comp into grp
                                select new { componente = grp.Key, campos = grp }).FirstOrDefault();

                var attrClave = metadata.campos.FirstOrDefault(c => c.Es_Clave == 1);
                bool esNuevo = string.IsNullOrEmpty(Valores.TablaID);
                if (!esNuevo)
                {
                    using (var checkExistenciaBuilder = db.CreateSQLQueryBuilder())
                    {
                        checkExistenciaBuilder.AddTable(metadata.componente.Esquema, metadata.componente.Tabla, "t1")
                                              .AddFormattedField("count(1)")
                                              .AddFilter(attrClave.Campo, Valores.TablaID, SQLOperators.EqualsTo)
                                              .ExecuteQuery((IDataReader reader) =>
                                              {
                                                  esNuevo = reader.GetInt32(0) == 0;
                                              });
                    }
                }
                var actualizar = new Dictionary<Atributo, object>();
                using (var metadataBuilder = db.CreateSQLQueryBuilder())
                {
                    foreach (string campo in metadataBuilder.GetTableFields(metadata.componente.Esquema, metadata.componente.Tabla))
                    {
                        int idx = 0;
                        if (esNuevo)
                        {
                            if (string.Compare(campo, "fecha_alta", true) == 0)
                            {
                                actualizar.Add(new Atributo { Campo = campo, TipoDatoId = 666, ComponenteId = metadata.componente.Id_Compoente }, null);
                                continue;
                            }
                            else if (string.Compare(campo, "id_usu_alta", true) == 0)
                            {
                                actualizar.Add(new Atributo { Campo = campo, ComponenteId = metadata.componente.Id_Compoente }, Valores.Id_Usuario);
                                continue;
                            }
                        }
                        if (string.Compare(campo, "fecha_modif", true) == 0)
                        {
                            actualizar.Add(new Atributo { Campo = campo, TipoDatoId = 666, ComponenteId = metadata.componente.Id_Compoente }, null);
                        }
                        else if (string.Compare(campo, "id_usu_modif", true) == 0)
                        {
                            actualizar.Add(new Atributo { Campo = campo, ComponenteId = metadata.componente.Id_Compoente }, Valores.Id_Usuario);
                        }
                        else if ((idx = Valores.CamposTablas.FindIndex(ct => string.Compare(campo, ct, true) == 0)) != -1)
                        {
                            actualizar.Add(new Atributo
                            {
                                Campo = campo,
                                ComponenteId = metadata.componente.Id_Compoente,
                                TipoDatoId = metadata.campos.Single(a => string.Compare(campo, a.Campo) == 0).Id_Tipo_Dato
                            }, Valores.ValoresTablas[idx]);
                        }
                    }
                }
                using (var upsertBuilder = db.CreateSQLQueryBuilder())
                {
                    upsertBuilder.AddTable(new Componente { Esquema = metadata.componente.Esquema, Tabla = metadata.componente.Tabla, ComponenteId = metadata.componente.Id_Compoente }, null);

                    Func<int> execute = upsertBuilder.ExecuteInsert;
                    if (esNuevo)
                    {
                        upsertBuilder.AddFieldsToInsert(actualizar.ToArray());
                    }
                    else
                    {
                        execute = upsertBuilder.ExecuteUpdate;
                        upsertBuilder.AddFieldsToUpdate(actualizar.ToArray())
                                     .AddFilter(new Atributo { Campo = attrClave.Campo, TipoDatoId = attrClave.Id_Tipo_Dato, ComponenteId = attrClave.Id_Componente }, Valores.TablaID, SQLOperators.EqualsTo);
                    }
                    return Ok(execute() != 0); //true si actualiza o inserta, false si no
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError(string.Format("SetAgregarRegistro(comp:{0},id:{1})", Valores.ComponentesId, Valores.TablaID), ex);
                return Ok(false);
            }
            #region comento hasta probarlo y luego borrar
            //bool success = false;
            //try
            //{
            //    var IdComponente = long.Parse(Valores.ComponentesId);
            //    ComponenteTA componente = db.ComponenteTA.FirstOrDefault(a => a.Id_Compoente == IdComponente);
            //    string tabla = componente.Tabla;
            //    List<AtributoTA> atributos = db.AtributoTA.Where(a => a.Id_Componente == IdComponente && (a.Es_Visible == 1 || a.Es_Clave == 1)).ToList();

            //    string queryConsultaCamposAudit = "Select  COLUMN_NAME, TABLE_NAME from user_tab_columns where table_name='" + tabla + "'";
            //    IDbCommand objCommConsultaC = db.Database.Connection.CreateCommand();
            //    db.Database.Connection.Open();
            //    objCommConsultaC.CommandText = queryConsultaCamposAudit;
            //    IDataReader dataConsultaAudit = objCommConsultaC.ExecuteReader();

            //    var camposAuditoriaAgregar = "";
            //    var ValoresAuditoriaAgregar = "";
            //    var ValoresAuditoriaModificar = "";
            //    while (dataConsultaAudit.Read())
            //    {
            //        switch (dataConsultaAudit.GetString(0))
            //        {
            //            case "ID_USU_ALTA":
            //                camposAuditoriaAgregar += ",ID_USU_ALTA ";
            //                ValoresAuditoriaAgregar += "," + Valores.Id_Usuario + " ";
            //                break;
            //            case "FECHA_ALTA":
            //                camposAuditoriaAgregar += ",FECHA_ALTA ";
            //                ValoresAuditoriaAgregar += ",TO_DATE('" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "', 'DD/MM/YYYY HH24:MI:SS') ";
            //                break;
            //            case "FECHA_MODIF":
            //                camposAuditoriaAgregar += ",FECHA_MODIF ";
            //                ValoresAuditoriaAgregar += ",TO_DATE('" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "', 'DD/MM/YYYY HH24:MI:SS') ";
            //                ValoresAuditoriaModificar += ",FECHA_MODIF = TO_DATE('" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "', 'DD/MM/YYYY HH24:MI:SS') ";
            //                break;
            //            case "ID_USU_MODIF":
            //                camposAuditoriaAgregar += ",ID_USU_MODIF ";
            //                ValoresAuditoriaAgregar += "," + Valores.Id_Usuario + " ";
            //                ValoresAuditoriaModificar += ",ID_USU_MODIF = " + Valores.Id_Usuario + " ";
            //                break;
            //            //case "ID_USU_BAJA":
            //            //    camposAuditoriaAgregar = ",FECHA_ALTA ";
            //            //    ValoresAuditoriaAgregar = Valores.Id_Usuario.ToString();
            //            //    break;
            //            //case "FECHA_BAJA":
            //            //    camposAuditoriaAgregar = ",FECHA_ALTA ";
            //            //    ValoresAuditoriaAgregar = Valores.Id_Usuario.ToString();
            //            //    break;
            //            default:
            //                break;
            //        }



            //    }

            //    if (Valores.TablaID == null)
            //    {

            //        //AGREGA UN REGISTRO
            //        string queryInsert = "INSERT INTO  ";
            //        queryInsert = queryInsert + componente.Tabla + " ( ";
            //        for (int c = 0; c < Valores.CamposTablas.Count; c++)
            //        {
            //            queryInsert = queryInsert + Valores.CamposTablas[c];
            //            if (c < Valores.CamposTablas.Count() - 1)
            //            {
            //                queryInsert = queryInsert + ", ";
            //            }

            //        }
            //        queryInsert = queryInsert + camposAuditoriaAgregar + " )VALUES(";

            //        for (int i = 0; i < Valores.ValoresTablas.Count; i++)
            //        {

            //            //queryInsert = queryInsert + " '" + Valores.ValoresTablas[i] + "' ";
            //            if (Valores.ValoresTablas[i] == string.Empty || Valores.ValoresTablas[i] == "null")
            //            {
            //                queryInsert = queryInsert + " null ";
            //            }
            //            else
            //            {
            //                if (atributos.FirstOrDefault(x => x.Campo == Valores.CamposTablas[i]).Id_Tipo_Dato == 5)
            //                {

            //                    queryInsert = queryInsert + " TO_DATE('" + Valores.ValoresTablas[i] + "', 'DD/MM/YYYY HH24:MI:SS') ";
            //                }
            //                else
            //                {

            //                    queryInsert = queryInsert + " '" + Valores.ValoresTablas[i] + "' ";
            //                }
            //            }
            //            if (i < Valores.ValoresTablas.Count() - 1)
            //            {
            //                queryInsert = queryInsert + ",";
            //            }

            //        }
            //        queryInsert = queryInsert + ValoresAuditoriaAgregar + " )";

            //        //recupero el resultado de la query
            //        IDbCommand objCommConsulta = db.Database.Connection.CreateCommand();
            //        //                    db.Database.Connection.Open();
            //        objCommConsulta.CommandText = queryInsert;
            //        IDataReader dataInsert = objCommConsulta.ExecuteReader();
            //        //List<object[]> lista = new List<object[]>();
            //        success = true;
            //    }
            //    else
            //    {


            //        string queryConsulta = "SELECT COUNT(*) ";
            //        var IdTabla = long.Parse(Valores.TablaID);
            //        queryConsulta = queryConsulta + " FROM " + componente.Tabla;
            //        queryConsulta += " WHERE " + atributos.Where(x => x.Es_Clave == 1).FirstOrDefault().Campo + " = " + IdTabla;
            //        //recupero el resultado de la query
            //        IDbCommand objCommConsulta = db.Database.Connection.CreateCommand();
            //        //          db.Database.Connection.Open();
            //        objCommConsulta.CommandText = queryConsulta;
            //        IDataReader dataConsulta = objCommConsulta.ExecuteReader();

            //        var existe = 0;
            //        while (dataConsulta.Read())
            //        {
            //            existe = dataConsulta.GetInt32(0);
            //        }

            //        if (existe == 0)
            //        {
            //            //AGREGA UN REGISTRO
            //            //TipoABM = "A";
            //            string queryInsert = "INSERT INTO  ";
            //            queryInsert = queryInsert + componente.Tabla + " ( ";
            //            for (int c = 0; c < Valores.CamposTablas.Count; c++)
            //            {
            //                queryInsert = queryInsert + Valores.CamposTablas[c];
            //                if (c < Valores.CamposTablas.Count() - 1)
            //                {
            //                    queryInsert = queryInsert + ", ";
            //                }

            //            }
            //            queryInsert = queryInsert + camposAuditoriaAgregar + " )VALUES(";

            //            for (int i = 0; i < Valores.ValoresTablas.Count; i++)
            //            {

            //                //queryInsert = queryInsert + " '" + Valores.ValoresTablas[i] + "' ";
            //                if (Valores.ValoresTablas[i] == string.Empty || Valores.ValoresTablas[i] == "null")
            //                {
            //                    queryInsert = queryInsert + " null ";
            //                }
            //                else
            //                {
            //                    if (atributos.FirstOrDefault(x => x.Campo == Valores.CamposTablas[i]).Id_Tipo_Dato == 5)
            //                    {

            //                        queryInsert = queryInsert + " TO_DATE('" + Valores.ValoresTablas[i] + "', 'DD/MM/YYYY HH24:MI:SS') ";
            //                    }
            //                    else
            //                    {
            //                        queryInsert = queryInsert + " '" + Valores.ValoresTablas[i] + "' ";
            //                    }
            //                }
            //                if (i < Valores.ValoresTablas.Count() - 1)
            //                {
            //                    queryInsert = queryInsert + ",";
            //                }

            //            }
            //            queryInsert = queryInsert + ValoresAuditoriaAgregar + " )";

            //            //recupero el resultado de la query

            //            objCommConsulta.CommandText = queryInsert;
            //            IDataReader dataInsert = objCommConsulta.ExecuteReader();
            //            //List<object[]> lista = new List<object[]>();
            //            success = true;

            //        }
            //        else
            //        {
            //            //MODIFICA UN REGISTRO
            //            //TipoABM = "M";
            //            string queryUpdate = "UPDATE ";
            //            queryUpdate = queryUpdate + componente.Tabla + " SET ";

            //            for (int c = 0; c < Valores.CamposTablas.Count; c++)
            //            {
            //                if (Valores.ValoresTablas[c] == string.Empty || Valores.ValoresTablas[c] == null)
            //                {
            //                    queryUpdate = queryUpdate + Valores.CamposTablas[c] + " = null ";
            //                }
            //                else
            //                {
            //                    if (atributos.FirstOrDefault(x => x.Campo == Valores.CamposTablas[c]).Id_Tipo_Dato == 5)
            //                    {

            //                        queryUpdate = queryUpdate + Valores.CamposTablas[c] + " =  TO_DATE('" + Valores.ValoresTablas[c] + "', 'DD/MM/YYYY HH24:MI:SS') ";
            //                    }
            //                    else
            //                    {
            //                        queryUpdate = queryUpdate + Valores.CamposTablas[c] + " = '" + Valores.ValoresTablas[c] + "' ";
            //                    }


            //                }
            //                if (c < Valores.CamposTablas.Count() - 1)
            //                {
            //                    queryUpdate = queryUpdate + ", ";
            //                }

            //            }
            //            queryUpdate += ValoresAuditoriaModificar;
            //            queryUpdate += " WHERE " + atributos.Where(x => x.Es_Clave == 1).FirstOrDefault().Campo + " = " + IdTabla;

            //            //recupero el resultado de la query

            //            objCommConsulta.CommandText = queryUpdate;
            //            IDataReader dataUpdate = objCommConsulta.ExecuteReader();
            //            //List<object[]> lista = new List<object[]>();
            //            success = true;


            //        }
            //        db.Database.Connection.Close();
            //    }
            //}
            //catch (Exception)
            //{
            //    //string err = ex.Message;
            //    //return Ok(err);
            //    success = false;
            //}

            //return Ok(success); 
            #endregion
        }

        [HttpGet]
        [ResponseType(typeof(AtributoRelacionado))]
        public List<AtributoRelacionado> GetListaAtributoRelacionado(string tabla_relacion, string esquema_relacion, string campo_relacion, string descripcion_relacion)
        {
            return db.CreateSQLQueryBuilder()
                      .AddTable(esquema_relacion, tabla_relacion, "t1")
                      .AddFields(campo_relacion, descripcion_relacion)
                      .ExecuteQuery<AtributoRelacionado>((IDataReader reader) =>
                      {
                          return new AtributoRelacionado()
                          {
                              Id_Atributo = reader.GetNullableInt64(0).GetValueOrDefault(),
                              Descripcion = reader.GetStringOrEmpty(1)
                          };
                      });
        }

    }

}