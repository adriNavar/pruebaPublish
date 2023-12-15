using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.Inmuebles;

using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
//using Oracle.ManagedDataAccess.Client;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Common.ExtensionMethods.DecimalDegreesToDMS;
using System.Xml;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common;
using System.Linq.Expressions;
using System.Configuration;
using System.Data.Entity.SqlServer;

namespace GeoSit.Data.DAL.Repositories
{
    public class PartidaSecuenciaRepository : IPartidaSecuenciaRepository
    {
        private readonly GeoSITMContext _context;

        public PartidaSecuenciaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public void UpdatePartidaSecuencia(PartidaSecuencia partida)
        {
            _context.Entry(partida).State = EntityState.Modified;
        }

        public void InsertPartidaSecuencia(PartidaSecuencia partida)
        {
            _context.PartidaSecuencias.Add(partida);
        }

    }
}
