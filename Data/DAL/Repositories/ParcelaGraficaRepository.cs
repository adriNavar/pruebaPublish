using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    internal class ParcelaGraficaRepository : IParcelaGraficaRepository
    {
        const string GEOMETRY_TEMPLATE = "MDSYS.SDO_GEOMETRY\n(\n\t{0},\n\t{1},\n\t{2},\n\t{3},\n\t{4}\n)";
        const string POINT_TEMPLATE = "MDSYS.SDO_POINT_TYPE({0},{1},{2})";
        const string ELEM_INFO_TEMPLATE = "MDSYS.SDO_ELEM_INFO_ARRAY({0})";
        const string ORDINATES_TEMPLATE = "MDSYS.SDO_ORDINATE_ARRAY({0})";

        private readonly GeoSITMContext _context;
        public ParcelaGraficaRepository(GeoSITMContext context)
        {
            this._context = context;
        }

        public ParcelaGrafica GetParcelaGraficaByIdParcela(long idParcela)
        {
            return this._context.ParcelaGrafica.SingleOrDefault(p => p.ParcelaID == idParcela && !p.FechaBaja.HasValue);
        }

        public void InsertParcelaGrafica(ParcelaGrafica parcelaGrafica)
        {
            if (parcelaGrafica.FeatID == 0)
            {
                parcelaGrafica.FeatID = _context.Database.SqlQuery<long>("SELECT F_FEATURESEQ.nextval FROM DUAL").First();
            }
            _context.ParcelaGrafica.Add(parcelaGrafica);
        }

        public void UpdateParcelaGrafica(ParcelaGrafica parcelaGrafica)
        {
            _context.Entry(parcelaGrafica).State = EntityState.Modified;
        }

        public void UpdateGeometry(long featId, int gtype, int srid, double[] point, int[] elemInfo, double[] ordinates)
        {
            string pointStr = "NULL", elemInfoStr = "NULL", ordinatesStr = "NULL";

            if (point != null && point.Length == 3)
            {
                pointStr = string.Format(POINT_TEMPLATE, point[0], point[1], point[2]);
            }
            if (elemInfo != null)
            {
                elemInfoStr = string.Format(ELEM_INFO_TEMPLATE, string.Join(",", elemInfo.Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)).ToArray()));
            }
            if (ordinates != null)
            {
                ordinatesStr = string.Format(ORDINATES_TEMPLATE, string.Join(",", ordinates.Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)).ToArray()));
            }
            string geometryStr = string.Format(GEOMETRY_TEMPLATE, gtype, srid, pointStr, elemInfoStr, ordinatesStr);
                        
            try
            {
                string sql = string.Format("UPDATE INM_PARCELA_GRAFICA SET GEOMETRY = {0} WHERE FEATID = :pFeatId", geometryStr);

                using (var db = GeoSITMContext.CreateContext())
                {
                    db.Database.ExecuteSqlCommand(sql, featId);                    
                }
            }
            catch (Exception ex)
            {                
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error);
            }
        }

        public void DeleteParcelaGrafica(ParcelaGrafica parcelaGrafica)
        {
            if (parcelaGrafica == null) return;
            this._context.Entry(parcelaGrafica).State = EntityState.Modified;
        }
    }
}
