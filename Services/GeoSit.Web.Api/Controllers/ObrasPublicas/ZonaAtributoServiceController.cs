using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.DAL;
using System.IO;
using System.Web;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Schema;
using System.Text;
using System.Xml.Serialization;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Web.Api.Common;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers.ObrasPublicas
{
    public class ZonaAtributoServiceController : ApiController
    {
        [ResponseType(typeof(ICollection<Objeto>))]
        public IHttpActionResult GetZonas(int idTipo)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    var zonas = db.Objetos.Where(m => m.TipoObjetoId == idTipo && m.FechaBaja == null).ToList() ?? new List<Objeto>();
                    return Ok(zonas);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<PLN_Atributo>))]
        public IHttpActionResult GetAtributos()
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                var Atributos = db.PLN_Atributo.ToList() ?? new List<PLN_Atributo>();

                return Ok(Atributos);
            }
        }

        [ResponseType(typeof(PLN_Atributo))]
        public IHttpActionResult GetAtributos(long Id)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                PLN_Atributo Atributos = db.PLN_Atributo.Find(Id);

                if (Atributos == null)
                {
                    return NotFound();
                }

                return Ok(Atributos);
            }
        }

        [ResponseType(typeof(ICollection<PLN_ZonaAtributo>))]
        public IHttpActionResult GetAtributosZona()
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                var AtributosZonas = db.ZonaAtributo.Where(m => m.Fecha_Baja == null).ToList() ?? new List<PLN_ZonaAtributo>();

                foreach (var item in AtributosZonas)
                {
                    item.Atributo = db.PLN_Atributo.Find(item.Id_Atributo_Zona);
                    item.ObjetoAdministrativo = db.Objetos.Find(item.FeatId_Objeto);
                }

                return Ok(AtributosZonas);

            }
        }

        [Route("api/ZonaAtributoService/GetAtributosZona/{FeatId}")]
        [ResponseType(typeof(ICollection<PLN_ZonaAtributo>))]
        public IHttpActionResult GetAtributosZona(long FeatId)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                List<PLN_ZonaAtributo> AtributosZonas = new List<PLN_ZonaAtributo>(db.ZonaAtributo.Where(m => m.FeatId_Objeto == FeatId && m.Id_Usu_Baja == null).ToList());

                if (AtributosZonas == null)
                {
                    return NotFound();
                }

                foreach (var item in AtributosZonas)
                {
                    item.Atributo = db.PLN_Atributo.Find(item.Id_Atributo_Zona);
                    item.ObjetoAdministrativo = db.Objetos.Find(item.FeatId_Objeto);
                }

                return Ok(AtributosZonas.OrderBy(o=>o.ObjetoAdministrativo.Codigo));
                //return Ok(AtributosZonas);

            }
        }

        [ResponseType(typeof(Objeto))]
        public IHttpActionResult GetObjetoAdministrativo(long FeatId)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                Objeto mObjetoAdministrativo = db.Objetos.FirstOrDefault(m => m.FeatId == FeatId);

                return Ok(mObjetoAdministrativo);

            }
        }

        [ResponseType(typeof(PLN_ZonaAtributo))]
        public IHttpActionResult GetValAtributosZona(long FeatId, long AtributoZonaId)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                PLN_ZonaAtributo mZonaAtributo = (db.ZonaAtributo.Where(m => m.FeatId_Objeto == FeatId && m.Id_Atributo_Zona == AtributoZonaId && m.Id_Usu_Baja == null).FirstOrDefault());

                return Ok(mZonaAtributo);

            }
        }

        [HttpPost]
        public IHttpActionResult DeleteZonaAtributo(PLN_ZonaAtributo zonaAtributo)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                PLN_ZonaAtributo existente = db.ZonaAtributo.Find(zonaAtributo.Id_Zona_Atributo);
                if (existente == null)
                {
                    return BadRequest("No se encontro el registro");
                }
                existente.Id_Usu_Baja = zonaAtributo.Id_Usu_Baja;
                existente.Id_Usu_Modif = existente.Id_Usu_Baja.Value;
                existente.Fecha_Baja = DateTime.Now;
                existente.Fecha_Modif = existente.Fecha_Baja.Value;

                db.Entry(existente).State = EntityState.Modified;
                db.SaveChanges(new Auditoria(existente.Id_Usu_Baja ?? 0, Eventos.BajaAtributoZona, "Se Elimino el Atributo Zona", zonaAtributo._Machine_Name,
                   zonaAtributo._Machine_Name, Autorizado.Si, existente, null, "PLN_ZonaAtributo", 1, TiposOperacion.Baja));
                return Ok(existente);
            }
        }

        [HttpPost]
        public IHttpActionResult AgregarAtributoZona(PLN_ZonaAtributo mZonaAtributo)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                try
                {
                    mZonaAtributo.Id_Usu_Modif = mZonaAtributo.Id_Usu_Alta;
                    mZonaAtributo.Fecha_Alta = DateTime.Now;
                    mZonaAtributo.Fecha_Modif = mZonaAtributo.Fecha_Alta;
                    db.ZonaAtributo.Attach(mZonaAtributo);
                    db.Entry(mZonaAtributo).State = EntityState.Added;
                    db.SaveChanges(new Auditoria(mZonaAtributo.Id_Usu_Modif, Eventos.AltaAtributoZona, "Se Agregó el Atributo Zona", mZonaAtributo._Machine_Name,
                   mZonaAtributo._Machine_Name, Autorizado.Si, null, mZonaAtributo, "PLN_ZonaAtributo", 1, TiposOperacion.Alta));
                }


                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }


            }

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult ModificarAtributoZona(PLN_ZonaAtributo mZonaAtributo)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                PLN_ZonaAtributo mZA = db.ZonaAtributo.Find(mZonaAtributo.Id_Zona_Atributo);

                if (mZonaAtributo == null)
                {
                    return BadRequest("No se encontro el registro");
                }

                try
                {
                    
                    mZA.Valor = mZonaAtributo.Valor;
                    mZA.U_Medida = mZonaAtributo.U_Medida;
                    mZA.Id_Usu_Modif = mZonaAtributo.Id_Usu_Modif;
                    mZA.Fecha_Modif = DateTime.Now;
                    db.Entry(mZA).State = EntityState.Modified;
                    db.SaveChanges(new Auditoria(mZonaAtributo.Id_Usu_Modif, Eventos.ModificacionAtributoZona, "Se modificó el Atributo Zona", mZA._Machine_Name,
                  mZA._Machine_Name, Autorizado.Si, mZA, mZA, "PLN_ZonaAtributo", 1, TiposOperacion.Modificacion));
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }

            return Ok();
        }

        //[Route("api/ZonaAtributoService/PostAtributoZona/{data}")]
        [ResponseType(typeof(Objeto))]
        public IHttpActionResult PostAtributoZona(PostAtributoZona data)
        {
            var aux = Convert.FromBase64String(data.observaciones);
            var Observacion = Encoding.UTF8.GetString(aux);
            TipoObjeto mTipoObjetoAdministrativo;
            Objeto mObjetoAdministrativo;
            //XmlSchema mSchema;

            using (var db = GeoSITMContext.CreateContext())
            {

                try
                {
                    int idTipoObjeto = Convert.ToInt32(db.ParametrosGenerales.ToList().Where(x => x.Clave == "ZONA_PLANEAMIENTO").FirstOrDefault().Valor);
                    //Se obtiene el esquema XSD desde el campo OA_TIPO_OBJETO.ESQUEMA
                    mTipoObjetoAdministrativo = db.TiposObjeto.Find(idTipoObjeto);

                    DataSet dataSet = new DataSet("Atributos");

                    dataSet.ReadXmlSchema(new StringReader(mTipoObjetoAdministrativo.Esquema));

                    dataSet.Tables["Datos"].Rows.Add(Observacion ?? string.Empty);

                    StringWriter sw = new StringWriter();
                    dataSet.WriteXml(sw);

                    //Obtengo el item a actualizar con la observación
                    mObjetoAdministrativo = db.Objetos.Find(data.featId);
                    mObjetoAdministrativo.Atributos = sw.ToString();
                    mObjetoAdministrativo.UsuarioModificacion = data.usuario;
                    mObjetoAdministrativo.FechaModificacion = DateTime.Now;

                    db.SaveChanges(new Auditoria(mObjetoAdministrativo.UsuarioModificacion ?? 0, Eventos.ModificacionObjetoAdministrativo
                    , "Se modifico el objeto administrativo", mObjetoAdministrativo._Machine_Name,
                    mObjetoAdministrativo._Ip, Autorizado.Si, mObjetoAdministrativo, mObjetoAdministrativo, "Objeto Administrativo", 1, TiposOperacion.Modificacion));

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }


            }
            return Ok(mObjetoAdministrativo);
        }

    }
}