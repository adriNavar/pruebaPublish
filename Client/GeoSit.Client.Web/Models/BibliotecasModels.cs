using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace GeoSit.Client.Web.Models
{

    public  class BibliotecasViewModel
    {
        public BibliotecasViewModel()
        {
            bibliotecasPrivadas = new List<BibliotecasItemModel>();
            bibliotecasPublicas = new List<BibliotecasItemModel>();
        }
        public List<BibliotecasItemModel> bibliotecasPublicas { set; get; }
        public List<BibliotecasItemModel> bibliotecasPrivadas { set; get; }
    }
    public class BibliotecasItemModel
    {
        public BibliotecasItemModel()
        {
            bibliotecas = new List<MapaTematicoConfiguracionModelo>();
        }
        public string ComponenteNombre { set; get; }
        public List<MapaTematicoConfiguracionModelo> bibliotecas { set; get; }
        
    }

}