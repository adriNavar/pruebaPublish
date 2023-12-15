using System.Collections.Generic;
using System.Data;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System;
using System.Linq;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Common.ExtensionMethods;

namespace GeoSit.Data.DAL.Repositories
{
    public class PartidoRepository : IPartidoRepository
    {
        private readonly GeoSITMContext _context;

        public PartidoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Partido> GetPartidos()
        {
            return _context.Partidos.OrderBy(l => l.Nombre).ToList();
        }

        public Partido GetPartidoById(long idPartido)
        {
            return _context.Partidos.FirstOrDefault(p => p.IdPartido == idPartido);
        }

        public string GetRegionNombre(Componente componentePartido, long idPartido)
        {
            throw new NotImplementedException();
            #region comento por ahora porque CT_REGION no existe en GEOSITM, se evalua cuando la ausencia de ésto genere error
            //string regionNombre = string.Empty;
            //try
            //{
            //    string esquemaPartido = componentePartido.Esquema;
            //    string tablaPartido = componentePartido.Tabla;
            //    string campoGeometryPartido = string.Empty;
            //    string campoFeatIdPartido = string.Empty;
            //    long idComponentePartido = componentePartido.ComponenteId;
            //    string esquemaRegion = esquemaPartido;
            //    string tablaRegion = "CT_REGION";
            //    string campoGeometryRegion = "GEOMETRY";
            //    try
            //    {
            //        campoFeatIdPartido = componentePartido.Atributos.GetAtributoClave().Campo;
            //        campoGeometryPartido = componentePartido.Atributos.GetAtributoGeometry().Campo;
            //    }
            //    catch (ApplicationException appEx)
            //    {
            //        _context.GetLogger().LogError("Componente (id: " + idComponentePartido + ") mal configurado.", appEx);
            //    }

            //    string sSql = "SELECT r.NOMBRE " +
            //            " FROM " + esquemaRegion + "." + tablaRegion + " r, " + esquemaPartido + "." + tablaPartido + " m " +
            //            " WHERE m." + campoFeatIdPartido + " = " + idPartido + " " +
            //              " AND MDSYS.SDO_RELATE(m." + campoGeometryPartido + ",r." + campoGeometryRegion + ",'MASK=ANYINTERACT') = 'TRUE' " +
            //              " AND (SDO_GEOM.SDO_AREA(SDO_GEOM.SDO_INTERSECTION(r." + campoGeometryRegion + ", m." + campoGeometryPartido + ", 0.1),0.1) / " +
            //                   " SDO_GEOM.SDO_AREA(m." + campoGeometryPartido + ",0.1) ) > 0.9 ";
            //    IDbCommand objComm = _context.Database.Connection.CreateCommand();
            //    _context.Database.Connection.Open();
            //    objComm.CommandText = sSql;
            //    IDataReader data = objComm.ExecuteReader();
            //    while (data.Read())
            //    {
            //        regionNombre = GetStringValue(data, 0);
            //    }
            //    _context.Database.Connection.Close();
            //}
            //catch (Exception e)
            //{
            //    _context.GetLogger().LogError("GetRegionNombre", e);
            //    try
            //    {
            //        _context.Database.Connection.Close();
            //    }
            //    catch { }
            //}
            //return regionNombre; 
            #endregion
        }
    }
}
