using GeoSit.Data.Intercambio.DAL.Contexts;
using System.Linq;
using System.Web.Http;
using GeoSit.Data.DAL.Repositories;
using System.Web.Http.Description;
using GeoSit.Data.Intercambio.BusinessEntities;
using System.Collections.Generic;
using System;

namespace Geosit.Intercambio.Api.Controllers
{
    public class IntercambioController : ApiController
    {
        IntercambioContext db = IntercambioContext.CreateContext();

        [Route("api/intercambio/test")]
        [HttpGet]
        public IHttpActionResult test()
        {
            return Ok(db.ReporteParcelarios.Take(20).ToList());
        }

        [Route("api/intercambio/GetMunicipios")]
        [HttpGet]
        public IHttpActionResult GetMunicipios()
        {
            return Ok(db.Municipios.ToList());
        }

        [Route("api/intercambio/GetEstados/{idMunicipio}")]
        [HttpGet]
        public IHttpActionResult GetEstados(int idMunicipio)
        {
            return Ok(db.Estados.Where(x => x.IdMunicipio == idMunicipio || x.IdMunicipio == null).OrderBy(x => x.Orden).ToList());
        }

        [Route("api/intercambio/GetDiferencias/{idMunicipio}")]
        [HttpGet]
        public IHttpActionResult GetDiferencias(int idMunicipio)
        {
            return Ok(db.Diferencias.Where(x => x.IdMunicipio == idMunicipio).OrderBy(x => x.IdDiferencia).ToList());
        }

        [Route("api/intercambio/GetDiferenciasDetalle/{idDiferencia}")]
        [HttpGet]
        public IHttpActionResult GetDiferenciasDetalle(int idDiferencia)
        {
            return Ok(db.Diferencias.Where(x => x.IdDiferencia == idDiferencia).FirstOrDefault());
        }

        [Route("api/intercambio/GetDiferencia/{idDiferencia}")]
        [HttpGet]
        public IHttpActionResult GetDiferencia(int idDiferencia)
        {
            return Ok(db.Diferencias.Where(x => x.IdDiferencia == idDiferencia).FirstOrDefault());
        }

        [Route("api/intercambio/GetFiltrosParametros/{parametros}")]
        [HttpGet]
        public IHttpActionResult GetFiltrosParametros(string parametros)
        {
            string[] param = parametros.Split(';');
            int idMun = Convert.ToInt32(param[0]);
            string fDesde = param[1];

            bool todo = Convert.ToBoolean(param[2]);
            bool nuevas = Convert.ToBoolean(param[3]);
            bool descartadas = Convert.ToBoolean(param[4]);

            List<Diferencia> ret = new List<Diferencia>();

            if (!string.IsNullOrEmpty(fDesde))
            {
                DateTime fecha = Convert.ToDateTime(fDesde);

                if (!todo && !nuevas && !descartadas)
                {
                    ret = db.Diferencias.Where(x => x.IdMunicipio == idMun && x.PrvFechaValuacion >= fecha).OrderBy(x => x.IdDiferencia).ToList();
                }
                else if (!todo && !nuevas && descartadas)
                {
                    ret = db.Diferencias.Where(x => x.IdMunicipio == idMun && x.PrvFechaValuacion >= fecha && x.MunIdEstado == 2).OrderBy(x => x.IdDiferencia).ToList();
                }
                else if (!todo && nuevas && !descartadas)
                {
                    ret = db.Diferencias.Where(x => x.IdMunicipio == idMun && x.PrvFechaValuacion >= fecha && x.MunIdEstado == 1).OrderBy(x => x.IdDiferencia).ToList();
                }
                else if (!todo && nuevas && descartadas)
                {
                    ret = db.Diferencias.Where(x => x.IdMunicipio == idMun && x.PrvFechaValuacion >= fecha && (x.MunIdEstado == 1 || x.MunIdEstado == 2)).OrderBy(x => x.IdDiferencia).ToList();
                }
                else if (todo)
                {
                    ret = db.Diferencias.Where(x => x.IdMunicipio == idMun && x.PrvFechaValuacion >= fecha).OrderBy(x => x.IdDiferencia).ToList();
                }
            }
            else
            {
                if (!todo && !nuevas && !descartadas)
                {
                    ret = db.Diferencias.Where(x => x.IdMunicipio == idMun).OrderBy(x => x.IdDiferencia).ToList();
                }
                else if (!todo && !nuevas && descartadas)
                {
                    ret = db.Diferencias.Where(x => x.IdMunicipio == idMun && x.MunIdEstado == 2).OrderBy(x => x.IdDiferencia).ToList();
                }
                else if (!todo && nuevas && !descartadas)
                {
                    ret = db.Diferencias.Where(x => x.IdMunicipio == idMun && x.MunIdEstado == 1).OrderBy(x => x.IdDiferencia).ToList();
                }
                else if (!todo && nuevas && descartadas)
                {
                    ret = db.Diferencias.Where(x => x.IdMunicipio == idMun && (x.MunIdEstado == 1 || x.MunIdEstado == 2)).OrderBy(x => x.IdDiferencia).ToList();
                }
                else if (todo)
                {
                    ret = db.Diferencias.Where(x => x.IdMunicipio == idMun).OrderBy(x => x.IdDiferencia).ToList();
                }
            }

            return Ok(ret);
        }

        [ResponseType(typeof(ICollection<ReporteParcelario>))]
        [Route("api/intercambio/GetReporteParcelario/{id_municipio}")]
        public IHttpActionResult GetReporteParcelario(string id_municipio)
        {
            return Ok(db.ReporteParcelarios.Where(x => x.Municipio == id_municipio));
        }

        [Route("api/intercambio/AplicaCambioEstado/{parametros}")]
        [HttpGet]
        public IHttpActionResult AplicaCambioEstado(string parametros)
        {
            string[] param = parametros.Split(';');
            int idMuni = Convert.ToInt32(param[0]);
            int idEstado = Convert.ToInt32(param[1]);

            param = param.Where((source, index) => index > 1).ToArray();

            Diferencia dif;

            foreach (var i in param)
            {
                long d = Convert.ToInt64(i);
                dif = db.Diferencias.Where(x => x.IdDiferencia == d).FirstOrDefault();
                dif.MunIdEstado = idEstado;
                dif.PrvIdEstado = idEstado;

                db.SaveChanges();
            }

            return Ok(db.Diferencias.Where(x => x.IdMunicipio == idMuni).OrderBy(x => x.IdDiferencia).ToList());
        }

        [Route("api/intercambio/ProcesarDiferencias/{idMunicipio}")]
        [HttpGet]
        public IHttpActionResult ProcesarDiferencias(int idMunicipio)
        {
            Municipio municipio = db.Municipios.Find(idMunicipio);
            string muni = municipio.Codigo;

            var rptMunicipal = db.Database.SqlQuery<ReporteParcelarioMunicipio>($"Select * from intercambio.VW_REPORTE_PARCELARIO_{muni}").ToList();
            List<ReporteParcelario> rptProvincial = db.ReporteParcelarios.Where(x => x.Municipio == muni).ToList();

            ReporteParcelario prov = new ReporteParcelario();

            foreach (var dife in rptMunicipal)
            {
                //Arreglar diferencias - puede haber en muni y no en prov y al revés y se tiene que agregar
                prov = rptProvincial.FirstOrDefault(x => x.Partida == dife.Partida);

                if (dife.Nomenclatura != prov.Nomenclatura || dife.Tipo != prov.Tipo || dife.Ph != prov.Ph || dife.Sup_Tierra_Regis != prov.SuperficieTierraRegistrada || dife.Sup_Tierra_Relev != prov.SuperficieTierraRelevada || dife.Unidad_Medida != prov.UnidadMedida || dife.Sup_Mejora_Regis != prov.SuperficieMejoraRegistrada || 
                    dife.Sup_Mejora_Relev != prov.SuperficieMejoraRelevada || dife.Sup_Cubierta_Regis != prov.SuperficieCubiertaRegistrada || dife.Sup_Cubierta_Relev != prov.SuperficieCubiertaRelevada || dife.Sup_Semicub_Regis != prov.SuperficieSemicubiertaRegistrada || dife.Sup_Semicub_Relev != prov.SuperficieSemicubiertaRelevada ||
                    dife.Sup_En_Const_Relev != prov.SuperficieEnConstruccionRelevada || dife.Sup_Negocio_Regis != prov.SuperficieNegocioRegistrada || dife.Sup_Piscina_Regis != prov.SuperficiePiscinaRegistrada || dife.Sup_Piscina_Relev != prov.SuperficiePiscinaRelevada || dife.Sup_Pavimento_Regis != prov.SuperficiePavimentoRegistrada || 
                    dife.Sup_Galpon_Relev != prov.SuperficieGalponRelevada || dife.Sup_Deportiva_Relev != prov.SuperficieDeportivaRelevada || dife.Sup_Precaria_Relev != prov.SuperficiePrecariaRelevada)
                {
                    InsertaDiferencia(dife, prov, idMunicipio);
                }
            }

            
            municipio.UltimoProceso = DateTime.Now;

            db.SaveChanges();

            return Ok(db.Diferencias.Where(x => x.IdMunicipio == idMunicipio).OrderBy(x => x.IdDiferencia).ToList());
        }

        public void InsertaDiferencia(ReporteParcelarioMunicipio dfMuni, ReporteParcelario dfProv, int idMunicipio)
        {
            Diferencia dif = new Diferencia();

            dif.MunFechaValuacion = dfMuni.Fecha_Valuacion;
            dif.IdMunicipio = idMunicipio;
            dif.MunCoordenadas = dfMuni.Coordenadas;
            dif.MunDominio = dfMuni.Dominio;
            dif.MunIdEstado = 1;

            dif.MunIdParcela = dfMuni.Id_Parcela;
            dif.MunNomenclatura = dfMuni.Nomenclatura;
            dif.MunPartida = dfMuni.Partida;
            dif.MunPH = dfMuni.Ph;
            dif.MunSupCubiertaRegis = dfMuni.Sup_Cubierta_Regis;
            dif.MunSupCubiertaRelev = dfMuni.Sup_Cubierta_Relev;
            dif.MunSupDeportivaRelev = dfMuni.Sup_Deportiva_Relev;
            dif.MunSupEnContRelev = dfMuni.Sup_En_Const_Relev;
            dif.MunSupGalponRelev = dfMuni.Sup_Galpon_Relev;
            dif.MunSupMejoraRegis = dfMuni.Sup_Mejora_Regis;
            dif.MunSupMejoraRelev = dfMuni.Sup_Mejora_Relev;
            dif.MunSupNegocioRegis = dfMuni.Sup_Negocio_Regis;
            dif.MunSupPavimentoRegis = dfMuni.Sup_Pavimento_Regis;
            dif.MunSupPiscinaRegis = dfMuni.Sup_Piscina_Regis;
            dif.MunSupPiscinaRelev = dfMuni.Sup_Piscina_Relev;
            dif.MunSupPrecariaRelev = dfMuni.Sup_Precaria_Relev;
            dif.MunSupSemicubRegis = dfMuni.Sup_Semicub_Regis;
            dif.MunSupSemicubRelev = dfMuni.Sup_Semicub_Relev;
            dif.MunSupTierraRegis = dfMuni.Sup_Tierra_Regis;
            dif.MunSupTierraRelev = dfMuni.Sup_Tierra_Relev;
            dif.MunTipo = dfMuni.Tipo;
            dif.MunUbicacion = dfMuni.Ubicacion;
            dif.MunUltCambio = DateTime.Now;
            dif.MunUnidadMedida = dfMuni.Unidad_Medida;
            dif.MunValorMejoras = dfMuni.Valor_Mejoras;
            dif.MunValorTierra = dfMuni.Valor_Tierra;
            dif.MunValorTotal = dfMuni.Valor_Total;


            dif.PrvFechaValuacion = dfProv.FechaValuacion;
            dif.PrvCoordenadas = dfProv.Coordenadas;
            dif.PrvDominio = dfProv.Dominio;
            dif.PrvIdEstado = 1;
            dif.PrvIdParcela = dfProv.IdParcela;
            dif.PrvNomenclatura = dfProv.Nomenclatura;
            dif.PrvPartida = dfProv.Partida;
            dif.PrvPH = dfMuni.Ph;
            dif.PrvSupCubiertaRegis = dfProv.SuperficieCubiertaRegistrada;
            dif.PrvSupCubiertaRelev = dfProv.SuperficieCubiertaRelevada;
            dif.PrvSupDeportivaRelev = dfProv.SuperficieDeportivaRelevada;
            dif.PrvSupEnConstRelev = dfProv.SuperficieEnConstruccionRelevada;
            dif.PrvSupGalponRelev = dfProv.SuperficieGalponRelevada;
            dif.PrvSupMejoraRegis = dfProv.SuperficieMejoraRegistrada;
            dif.PrvSupMejoraRelev = dfProv.SuperficieMejoraRelevada;
            dif.PrvSupNegocioRegis = dfProv.SuperficieNegocioRegistrada;
            dif.PrvSupPavimentoRegis = dfProv.SuperficiePavimentoRegistrada;
            dif.PrvSupPiscinaRegis = dfProv.SuperficiePiscinaRegistrada;
            dif.PrvSupPiscinaRelev = dfProv.SuperficiePiscinaRelevada;
            dif.PrvSupPrecariaRelev = dfProv.SuperficiePrecariaRelevada;
            dif.PrvSupSemicubRegis = dfProv.SuperficieSemicubiertaRegistrada;
            dif.PrvSupSemicubRelev = dfProv.SuperficieSemicubiertaRelevada;
            dif.PrvSupTierraRegis = dfProv.SuperficieTierraRegistrada;
            dif.PrvSupTierraRelev = dfProv.SuperficieTierraRelevada;
            dif.PrvTipo = dfProv.Tipo;
            dif.PrvUbicacion = dfProv.Ubicacion;
            dif.PrvUltCambio = DateTime.Now;
            dif.PrvUnidadMedida = dfProv.UnidadMedida;
            dif.PrvValorMejoras = dfProv.ValorMejoras;
            dif.PrvValorTierra = dfProv.ValorTierra;
            dif.PrvValorTotal = dfProv.ValorTotal;

            Diferencia d1 = db.Diferencias.Where(x => x.MunPartida == dfMuni.Partida).FirstOrDefault();

            if (d1 != null)
            {
                if (HayDiferencia(dif, d1))
                {
                    db.Diferencias.Add(dif);
                }
            }
            else
            {
                db.Diferencias.Add(dif);
            }
        }

        public bool HayDiferencia(Diferencia difActual, Diferencia difAnterior)
        {
            bool hayDif = false;

            if (difActual.IdMunicipio != difAnterior.IdMunicipio || difActual.MunCoordenadas != difAnterior.MunCoordenadas || difActual.MunDominio != difAnterior.MunDominio || difActual.MunFechaValuacion != difAnterior.MunFechaValuacion || difActual.MunPH != difAnterior.MunPH || difActual.MunSupCubiertaRegis != difAnterior.MunSupCubiertaRegis || difActual.MunSupCubiertaRelev != difAnterior.MunSupCubiertaRelev ||
                    difActual.MunSupDeportivaRelev != difAnterior.MunSupDeportivaRelev || difActual.MunSupEnContRelev != difAnterior.MunSupEnContRelev || difActual.MunSupGalponRelev != difAnterior.MunSupGalponRelev || difActual.MunSupMejoraRegis != difAnterior.MunSupMejoraRegis || difActual.MunSupMejoraRelev != difAnterior.MunSupMejoraRelev || difActual.MunValorMejoras != difAnterior.MunValorMejoras || difActual.MunValorTotal != difAnterior.MunValorTotal ||
                    difActual.MunSupNegocioRegis != difAnterior.MunSupNegocioRegis || difActual.MunSupPavimentoRegis != difAnterior.MunSupPavimentoRegis || difActual.MunSupPiscinaRegis != difAnterior.MunSupPiscinaRegis || difActual.MunSupPiscinaRelev != difAnterior.MunSupPiscinaRelev || difActual.MunSupPrecariaRelev != difAnterior.MunSupPrecariaRelev || difActual.MunUnidadMedida != difAnterior.MunUnidadMedida || 
                    difActual.MunSupSemicubRegis != difAnterior.MunSupSemicubRegis || difActual.MunSupSemicubRelev != difAnterior.MunSupSemicubRelev || difActual.MunSupTierraRegis != difAnterior.MunSupTierraRegis || difActual.MunSupTierraRelev != difAnterior.MunSupTierraRelev || difActual.MunTipo != difAnterior.MunTipo || difActual.MunUbicacion != difAnterior.MunUbicacion || difActual.MunValorTierra != difAnterior.MunValorTierra ||
                    difActual.PrvCoordenadas != difAnterior.PrvCoordenadas || difActual.PrvDominio != difAnterior.PrvDominio || difActual.PrvFechaValuacion != difAnterior.PrvFechaValuacion || difActual.PrvPH != difAnterior.PrvPH || difActual.PrvSupCubiertaRegis != difAnterior.PrvSupCubiertaRegis || difActual.PrvSupCubiertaRelev != difAnterior.PrvSupCubiertaRelev ||
                    difActual.PrvSupDeportivaRelev != difAnterior.PrvSupDeportivaRelev || difActual.PrvSupGalponRelev != difAnterior.PrvSupGalponRelev || difActual.PrvSupMejoraRegis != difAnterior.PrvSupMejoraRegis || difActual.PrvSupMejoraRelev != difAnterior.PrvSupMejoraRelev || difActual.PrvValorMejoras != difAnterior.PrvValorMejoras || difActual.PrvValorTotal != difAnterior.PrvValorTotal ||
                    difActual.PrvSupNegocioRegis != difAnterior.PrvSupNegocioRegis || difActual.PrvSupPavimentoRegis != difAnterior.PrvSupPavimentoRegis || difActual.PrvSupPiscinaRegis != difAnterior.PrvSupPiscinaRegis || difActual.PrvSupPiscinaRelev != difAnterior.PrvSupPiscinaRelev || difActual.PrvSupPrecariaRelev != difAnterior.PrvSupPrecariaRelev || difActual.PrvUnidadMedida != difAnterior.PrvUnidadMedida ||
                    difActual.PrvSupSemicubRegis != difAnterior.PrvSupSemicubRegis || difActual.PrvSupSemicubRelev != difAnterior.PrvSupSemicubRelev || difActual.PrvSupTierraRegis != difAnterior.PrvSupTierraRegis || difActual.PrvSupTierraRelev != difAnterior.PrvSupTierraRelev || difActual.PrvTipo != difAnterior.PrvTipo || difActual.PrvUbicacion != difAnterior.PrvUbicacion || difActual.PrvValorTierra != difAnterior.PrvValorTierra)
            {
                hayDif = true;
            }

            return hayDif;
        }
    }
}
