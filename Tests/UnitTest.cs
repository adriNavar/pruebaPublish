using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            using (HttpClient cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri("http://localhost:42671/");
                //Call HttpClient.GetAsync to send a GET request to the appropriate URI   
                HttpResponseMessage resp = cliente.GetAsync("api/ControlTecnico").Result;
                //This method throws an exception if the HTTP response status is an error code.  
                resp.EnsureSuccessStatusCode();
                var controles = resp.Content.ReadAsAsync<IEnumerable<ControlTecnico>>().Result;
              }
        }
    }
}
