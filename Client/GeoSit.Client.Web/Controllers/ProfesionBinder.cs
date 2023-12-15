using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GeoSit.Client.Web.Models;
using System.Web.Mvc;

namespace GeoSit.Client.Web.Controllers
{
    public class ProfesionBinder:IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, 
	        ModelBindingContext bindingContext)
        {
	        HttpRequestBase request = controllerContext.HttpContext.Request;
            string strMatricula = request.Form["Matricula"];
            string strIdTipoProfesional = request.Form["TipoProfesionId"];
            string strIdPersona = request.Form["PersonaId"];  
	        return new ProfesionModel
	        {
                PersonaId = Convert.ToInt32(strIdPersona),
                TipoProfesionId = Convert.ToInt32(strIdTipoProfesional),
                Matricula = strMatricula
	        };
        }
    }
}