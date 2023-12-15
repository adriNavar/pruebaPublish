using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Geosit.Web.Api
{
    [TestClass]
    public class ExpedienteObraControllerTest
    {
        [TestMethod]
        public void InsertExpedienteObra()
        {
            #region La prueba queda obsoleta porque cambio la arquitectura de como salvar objetos en la DB
            ////Arrange
            //var expedienteObra = new ExpedienteObra
            //{
            //    ExpedienteObraId = 0,
            //    NumeroExpediente = "1",
            //    FechaExpediente = DateTime.Now,
            //    NumeroLegajo = "1-2015",
            //    FechaLegajo = DateTime.Now,
            //    PlanId = 1,
            //    UsuarioAltaId = 1,
            //    FechaAlta = DateTime.Now,
            //    UsuarioModificacionId = 1,
            //    FechaModificacion = DateTime.Now
            //};

            //const bool enPosesion = true;
            //const string chapa = "ABC123";
            //const bool ph = true;
            //const bool permisosProvisorios = true;

            //expedienteObra.AttributosCreate(enPosesion, chapa, ph, permisosProvisorios);

            //var unitOfWork = new UnitOfWork();
            //var saveObjects = new SaveObjects();

            //var unidadTributariaExpedienteObra = new UnidadTributariaExpedienteObra
            //{
            //    ExpedienteObraId = 0,
            //    UnidadTributariaId = 580,
            //    UsuarioAltaId = 1,
            //    FechaAlta = DateTime.Now,
            //    UsuarioModificacionId = 1,
            //    FechaModificacion = DateTime.Now
            //};

            //saveObjects.Add(Operation.Add, null, unidadTributariaExpedienteObra);

            //var expedienteObraWebApi = new ExpedienteObraController(unitOfWork, saveObjects);

            ////Act
            //var response = expedienteObraWebApi.Post(expedienteObra);
            //var responseSaveAll = expedienteObraWebApi.Post();

            ////Assert
            //Assert.IsNotNull(response);
            //Assert.IsInstanceOfType(response, typeof(OkResult));

            //Assert.IsNotNull(responseSaveAll);
            //Assert.IsInstanceOfType(responseSaveAll, typeof(OkResult)); 
            #endregion
        }

        public void UpdateExpedienteObra()
        {

        }
    }
}
