using MT = GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL;
using GeoSit.Data.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GeoSit.Web.Api.Models;
using System.Data;
using System.Reflection;
using System.Drawing;
using System.Data.Entity.Spatial;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Common.Enums;

namespace GeoSit.Web.Api.Controllers
{
    public class DetalleObjetoController : ApiController
    {
        private const int TIPO_POLIGONO = 1;
        private const int TIPO_LINEA = 2;
        private const int TIPO_PUNTO = 3;

        private readonly UnitOfWork _unitOfWork;
        private readonly long[] functionsNotAllowedByCompId = new long[] { 135, 136 };

        public DetalleObjetoController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult Get(long objetoId, long componenteId)
        {
            try
            {
                if (objetoId <= 0 || componenteId <= 0)
                    throw new Exception("Parametros invalidos.");

                var componente = _unitOfWork.ComponenteRepository.GetComponenteById(componenteId);
                return Ok(getObjeto(objetoId, componente));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        public IHttpActionResult GetByDocType(long objetoId, string docType)
        {
            try
            {
                if (objetoId <= 0 || string.IsNullOrEmpty(docType))
                    throw new Exception("Parametros invalidos.");

                var componente = _unitOfWork.ComponenteRepository.GetComponenteByDocType(docType);
                return Ok(getObjeto(objetoId, componente));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        /*[HttpGet]
        public byte[] GetImagen(long objetoId, long componenteId)
        {
            var componente = _unitOfWork.ComponenteRepository.GetComponenteById(componenteId);
            var atributoClave = _unitOfWork.AtributoRepository.GetAtributosByIdComponente(componente.ComponenteId).FirstOrDefault(a => a.EsClave == 1);

            if (componente == null || componente.Graficos == 5) return new byte[0];

            try
            {
                #region Obtengo coordenadas
                string wkt = this.getWKT(objetoId, componente, atributoClave);
                var lista = new List<double>();
                if (!string.IsNullOrEmpty(wkt))
                {
                    DbGeometry geom = DbGeometry.FromText(wkt);

                    if (geom.PointCount.HasValue)
                    {
                        for (int i = 1; i <= geom.PointCount; i++)
                        {
                            DbGeometry punto = geom.PointAt(i);
                            lista.AddRange(new double[] { punto.XCoordinate.GetValueOrDefault(), punto.YCoordinate.GetValueOrDefault() });
                        }
                    }
                    else //si es punto
                    {
                        lista.AddRange(new double[] { geom.XCoordinate.GetValueOrDefault(), geom.YCoordinate.GetValueOrDefault() });
                    }
                }
                #endregion

                #region Calculo coordenadas maximas y minimas
                double xMax = 0;
                double xMenor = lista[0];
                double yMax = 0;
                double yMenor = lista[1];

                double lado;
                for (var i = 0; i < lista.Count(); i++)
                {
                    if (xMenor >= lista[i])
                    {
                        xMenor = lista[i];
                    }
                    if (xMax <= lista[i])
                    {
                        xMax = lista[i];
                    }

                    i++;
                    if (yMenor >= lista[i])
                    {
                        yMenor = lista[i];
                    }
                    if (yMax <= lista[i])
                    {
                        yMax = lista[i];
                    }
                }
                #endregion

                #region Dibujo imagen
                float ModuloY = Convert.ToSingle(yMax - yMenor);
                float ModuloX = Convert.ToSingle(xMax - xMenor);

                if (ModuloX >= ModuloY)
                {
                    lado = ModuloX;
                }
                else
                {
                    lado = ModuloY;
                }
                float propX = 220f / ModuloX;
                float propY = 220f / ModuloY;

                PointF[] curvePoints = new PointF[lista.Count() / 2];
                int y = 0;
                int j = 0;
                while (y < lista.Count())
                {
                    curvePoints[j++] = new PointF(Convert.ToSingle(lista[y++] - xMenor) * propX, Convert.ToSingle(lista[y++] - yMenor) * propY);
                }

                if (componente.Graficos == 4)
                { // recupero el tipo de geometría para este objeto en particular ya que el componente puede ser de varios tipos 
                    using (var db = GeoSITMContext.CreateContext())
                    {
                        componente.Graficos = this.getTipoGrafico(db, componente, atributoClave.Campo, objetoId);
                    }
                }

                using (Bitmap customImage = new Bitmap(220, 220))
                using (Graphics g = Graphics.FromImage(customImage))
                {
                    g.Clear(Color.Transparent);
                    Pen blackPen = new Pen(Color.Black, 2f);

                    Action<Pen, PointF[]> draw = g.DrawPolygon;

                    if (componente.Graficos == 2)
                        draw = g.DrawLines;

                    draw(blackPen, curvePoints);
                    g.RotateTransform(90);
                    customImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    ImageConverter converter = new ImageConverter();
                    return (byte[])converter.ConvertTo(customImage, typeof(byte[]));
                }
                #endregion
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("DetalleObjeto - GetImagen", ex);
                return new byte[0];
            }
        }*/
        [HttpGet]
        public IHttpActionResult GetImagen(string objetoId, long componenteId)
        {
            var componente = _unitOfWork.ComponenteRepository.GetComponenteById(componenteId);
            if (componente == null || componente.Graficos == 5) return Ok();

            Atributo atributoClave = null, atributoGeometry = null;
            try
            {
                atributoClave = _unitOfWork.AtributoRepository.GetAtributosByIdComponente(componente.ComponenteId).GetAtributoClave();
                atributoGeometry = _unitOfWork.AtributoRepository.GetAtributosByIdComponente(componente.ComponenteId).GetAtributoGeometry();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Componente (id: " + componenteId + ") mal configurado.", ex);
                return InternalServerError();
            }

            try
            {
                #region Obtengo geometrias
                var geometries = this.getGeometries(objetoId, componente, atributoClave, atributoGeometry);
                #endregion

                #region genero imagen para cada tipo de objeto
                var imagenes = new List<ImagenGrafico>();
                foreach (var grupo in geometries)
                {
                    #region colecto coordenadas
                    Action<DbGeometry, List<PointF>> getCoords = (DbGeometry geom, List<PointF> lista) =>
                    {
                        for (int i = 1; i <= geom.PointCount; i++)
                        {
                            lista.Add(new PointF(Convert.ToSingle(geom.PointAt(i).XCoordinate.Value), Convert.ToSingle(geom.PointAt(i).YCoordinate.GetValueOrDefault())));
                        }
                    };
                    if (grupo.Key == TIPO_PUNTO)
                    {
                        if (grupo.Value.Count == 1) continue; //si solo hay un objeto de tipo punto, no genero imagen

                        getCoords = (DbGeometry geom, List<PointF> lista) =>
                        {
                            lista.Add(new PointF(Convert.ToSingle(geom.XCoordinate.Value), Convert.ToSingle(geom.YCoordinate.GetValueOrDefault())));
                        };
                    }

                    var coordsByGeom = new List<List<PointF>>();
                    grupo.Value.ForEach(geom =>
                    {
                        coordsByGeom.Add(new List<PointF>());
                        getCoords(geom, coordsByGeom.Last());
                    });
                    #endregion

                    #region obtengo coordenadas maximas y minimas
                    var coords = coordsByGeom.SelectMany(geom => geom).ToArray();
                    float xMax = 0f;
                    float xMenor = coords[0].X;
                    float yMax = 0f;
                    float yMenor = coords[0].Y;
                    for (int i = 0; i < coords.Length; i++)
                    {
                        #region xMax y xMin
                        if (xMenor >= coords[i].X)
                        {
                            xMenor = coords[i].X;
                        }
                        if (xMax <= coords[i].X)
                        {
                            xMax = coords[i].X;
                        }
                        #endregion

                        #region yMax e yMin
                        if (yMenor >= coords[i].Y)
                        {
                            yMenor = coords[i].Y;
                        }
                        if (yMax <= coords[i].Y)
                        {
                            yMax = coords[i].Y;
                        }
                        #endregion
                    }
                    #endregion

                    #region calculo tamaño de canvas de acuerdo a la relacion de aspecto entre ancho y alto
                    float ModuloY = Math.Max(yMax - yMenor, 1f);
                    float ModuloX = Math.Max(xMax - xMenor, 1f);
                    float width = 220f;
                    float height = 220f;
                    float lado = Math.Min(ModuloX, ModuloY);
                    if (lado == ModuloX)
                    {//es mas alta que ancha ======> mantengo ancho, ajusto el alto
                        height /= (ModuloX / ModuloY);
                    }
                    else
                    {//similar pero lo contrario (comentario inutil xD)
                        width /= (ModuloY / ModuloX);
                    }

                    //ajusto para que use menos del canvas comun
                    float propX = (width - 5) / ModuloX;
                    float propY = (height - 5) / ModuloY;
                    #endregion

                    using (Bitmap customImage = new Bitmap(Convert.ToInt32(Math.Ceiling(width)), Convert.ToInt32(Math.Ceiling(height))))
                    using (Graphics g = Graphics.FromImage(customImage))
                    {
                        g.Clear(Color.White);

                        if (grupo.Key == TIPO_PUNTO)
                        {
                            float size = 5f;
                            coordsByGeom.ForEach(geom => g.FillEllipse(Brushes.Black, (geom[0].X - xMenor) * propX, (geom[0].Y - yMenor) * propY, size, size));
                        }
                        else
                        {
                            Action<Pen, PointF[]> draw = g.DrawPolygon;

                            if (grupo.Key == TIPO_LINEA)
                                draw = g.DrawLines;

                            Pen blackPen = new Pen(Color.Black, 2f);
                            coordsByGeom.ForEach(geom => draw(blackPen, geom.Select(coord => new PointF((coord.X - xMenor) * propX, (coord.Y - yMenor) * propY)).ToArray()));
                        }
                        g.RotateTransform(90);
                        customImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        imagenes.Add(new ImagenGrafico { TipoGrafico = grupo.Key, Imagen = Convert.ToBase64String((byte[])new ImageConverter().ConvertTo(customImage, typeof(byte[]))) });
                    }
                }
                return Ok(imagenes.ToArray());
                #endregion
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("DetalleObjeto - GetImagen", ex);
                return InternalServerError();
            }
        }

        private Objeto getObjeto(long objetoId, MT.Componente componente)
        {
            try
            {
                componente.Atributos = _unitOfWork.AtributoRepository.GetAtributosByIdComponente(componente.ComponenteId).ToList();

                var model = new Objeto();
                //atributos
                getAtributos(model, componente, objetoId);
                //componentes
                getRelaciones(model, componente, objetoId);
                //graficos
                getGraficos(model, componente, objetoId);
                return model;
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("getObjeto", ex);
                throw;
            }
        }
        private void getAtributos(Objeto model, MT.Componente componente, long objetoId)
        {
            using (var db = GeoSITMContext.CreateContext())
            using (var builder = db.CreateSQLQueryBuilder())
            {
                try
                {
                    var atributosRelaciones = componente.Atributos.Where(a => a.AtributoParentId.HasValue);
                    var ids = atributosRelaciones.Select(a => a.AtributoParentId).ToArray();

                    var registros = from attrRel in db.Atributo
                                    join cmpRel in db.Componente on attrRel.ComponenteId equals cmpRel.ComponenteId
                                    join attrClaveCmpRel in db.Atributo on cmpRel.ComponenteId equals attrClaveCmpRel.ComponenteId
                                    join attrId in ids on attrRel.AtributoId equals attrId
                                    where attrClaveCmpRel.EsClave == 1
                                    select new { atributoRelacion = attrRel, componenteRelacion = cmpRel, atributoClave = attrClaveCmpRel, atributoId = attrId };

                    var atributosMostrables = componente
                                                .Atributos
                                                .Where(a => (a.Campo != null || (a.Funcion != null && !functionsNotAllowedByCompId.Contains(a.ComponenteId))) && a.EsVisible & !a.EsGeometry)
                                                .OrderBy(a => a.Orden)
                                                .ToList();

                    builder.AddTable(componente, componente.Tabla)
                           .AddFilter(componente.Atributos.GetAtributoClave(), objetoId, SQLOperators.EqualsTo);

                    foreach (var registro in registros)
                    {
                        var attr = atributosRelaciones.Single(a => a.AtributoParentId == registro.atributoId);
                        builder.AddJoin(registro.componenteRelacion, registro.componenteRelacion.Tabla, registro.atributoClave, attr, SQLJoin.Left);
                        registro.atributoRelacion.Nombre = attr.Nombre;
                        registro.atributoRelacion.Orden = attr.Orden;
                        atributosMostrables[atributosMostrables.IndexOf(attr)] = registro.atributoRelacion;
                    }

                    var atributoLabel = componente.Atributos.GetAtributoLabel();
                    builder.AddFields(atributosMostrables.ToArray())
                           .Distinct()
                           .ExecuteQuery((IDataReader reader) =>
                           {
                               for (int i = 0; i < reader.FieldCount; i++)
                               {
                                   model.Atributos.Add(new AtributoObjeto
                                   {
                                       AtributoId = objetoId,
                                       Nombre = atributosMostrables.ElementAt(i).Nombre,
                                       Valor = reader.GetTypeFormattedStringValue(i),
                                       EsLabel = reader.GetName(i).ToUpper() == atributoLabel.Campo.ToUpper()
                                   });
                               }
                           });
                }
                catch (Exception ex)
                {
                    Global.GetLogger().LogError("DetalleObjetoController - getAtributos", ex);
                }
            }
        }
        private void getRelaciones(Objeto model, MT.Componente componente, long objetoId)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                try
                {
                    var componentesPadres = from jerarquia in db.Jerarquia
                                            join comp in db.Componente on jerarquia.ComponenteSuperiorId equals comp.ComponenteId
                                            join atributo in db.Atributo on comp.ComponenteId equals atributo.ComponenteId
                                            where jerarquia.ComponenteInferiorId == componente.ComponenteId
                                            group atributo by new { componente = comp, jerarquia } into grupo
                                            select new { grupo.Key.componente, grupo.Key.jerarquia, atributos = grupo };

                    Atributo atributoClaveComponente = null;
                    try
                    {
                        atributoClaveComponente = componente.Atributos.GetAtributoClave();
                    }
                    catch (Exception ex)
                    {
                        Global.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", ex);
                    }

                    foreach (var item in componentesPadres.ToList())
                    {
                        Atributo atributoClave = null;
                        Atributo atributoLabel = null;
                        Atributo atributoFeatId = null;
                        try
                        {
                            atributoClave = item.atributos.GetAtributoClave();
                            atributoLabel = item.atributos.GetAtributoLabel();
                            atributoFeatId = (item.componente.Graficos ?? 5) == 5 ? null : item.atributos.GetAtributoFeatId();
                        }
                        catch (ApplicationException appEx)
                        {
                            Global.GetLogger().LogError("Componente (id: " + item.componente.ComponenteId + ") mal configurado.", appEx);
                            continue;
                        }
                        using (var builder = db.CreateSQLQueryBuilder())
                        {
                            var campoRelacionHijo = _unitOfWork.ColeccionRepository.GetAtributosById(item.jerarquia.AtributoSuperiorId);
                            var campoRelacionPadre = _unitOfWork.ColeccionRepository.GetAtributosById(item.jerarquia.AtributoInferiorId);
                            builder.AddTable(item.componente, "t1")
                                   .AddFields(atributoClave, atributoLabel, atributoFeatId)
                                   .AddJoin(componente, "t2", campoRelacionHijo, campoRelacionPadre)
                                   .AddFilter(atributoClaveComponente, objetoId, SQLOperators.EqualsTo)
                                   .Distinct()
                                   .ExecuteQuery((IDataReader reader) =>
                                   {
                                       model.Relaciones.Add(new Relacion
                                       {
                                           ObjetoId = reader.GetStringOrEmpty(0),
                                           Valor = reader.GetStringOrEmpty(1),
                                           FeatId = atributoFeatId == null ? null : reader.GetStringOrEmpty(reader.GetOrdinal(atributoFeatId.Campo)),
                                           ComponenteId = item.componente.ComponenteId.ToString(),
                                           Nombre = item.componente.Nombre,
                                           Descripcion = item.componente.Descripcion,
                                           Capa = item.componente.Capa,
                                           Grafico = item.componente.Graficos
                                       });
                                   });
                        }
                    }
                    var componentesHijos = from jerarquia in db.Jerarquia
                                           join comp in db.Componente on jerarquia.ComponenteInferiorId equals comp.ComponenteId
                                           join atributo in db.Atributo on comp.ComponenteId equals atributo.ComponenteId
                                           where jerarquia.ComponenteSuperiorId == componente.ComponenteId
                                           group atributo by new { componente = comp, jerarquia = jerarquia } into grupo
                                           select new { componente = grupo.Key.componente, jerarquia = grupo.Key.jerarquia, atributos = grupo };

                    foreach (var item in componentesHijos.ToList())
                    {
                        Atributo atributoClave = null;
                        Atributo atributoLabel = null;
                        Atributo atributoFeatId = null;
                        try
                        {
                            atributoClave = item.atributos.GetAtributoClave();
                            atributoLabel = item.atributos.GetAtributoLabel();
                            atributoFeatId = (item.componente.Graficos ?? 5) == 5 ? null : item.atributos.GetAtributoFeatId();
                        }
                        catch (ApplicationException appEx)
                        {
                            Global.GetLogger().LogError("Componente (id: " + item.componente.ComponenteId + ") mal configurado.", appEx);
                            continue;
                        }
                        var campoRelacionHijo = _unitOfWork.ColeccionRepository.GetAtributosById(item.jerarquia.AtributoInferiorId);
                        var campoRelacionPadre = _unitOfWork.ColeccionRepository.GetAtributosById(item.jerarquia.AtributoSuperiorId);

                        using (var builder = db.CreateSQLQueryBuilder())
                        {
                            builder.AddTable(item.componente, "t1")
                                   .AddFields(atributoClave, atributoLabel, atributoFeatId)
                                   .AddFilter(campoRelacionHijo, GetValue(db, componente, objetoId, campoRelacionPadre), SQLOperators.EqualsTo)
                                   .Distinct()
                                   .ExecuteQuery((IDataReader reader) =>
                                   {
                                       model.Relaciones.Add(new Relacion
                                       {
                                           ObjetoId = reader.GetStringOrEmpty(0),
                                           Valor = reader.GetStringOrEmpty(1),
                                           FeatId = atributoFeatId == null ? null : reader.GetStringOrEmpty(reader.GetOrdinal(atributoFeatId.Campo)),
                                           ComponenteId = item.componente.ComponenteId.ToString(),
                                           Nombre = item.componente.Nombre,
                                           Descripcion = item.componente.Descripcion,
                                           Capa = item.componente.Capa,
                                           Grafico = item.componente.Graficos
                                       });
                                   });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Global.GetLogger().LogError("DetalleObjetoController - getRelaciones", ex);
                }
            }
        }
        private void getGraficos(Objeto model, MT.Componente componente, long objetoId)
        {
            if (componente.Graficos == 5)
            {
                model.Graficos.Add(new Grafico() { Nombre = "Sin representacion", Valor = null });
            }
            else
            {
                try
                {
                    using (var db = GeoSITMContext.CreateContext())
                    using (var builder = db.CreateSQLQueryBuilder())
                    {
                        var atributoClave = componente.Atributos.GetAtributoClave();
                        var atributoGeom = componente.Atributos.GetAtributoGeometry();

                        builder.AddTable(new MT.Componente { Esquema = componente.Esquema, Tabla = componente.TablaGrafica ?? componente.Tabla, ComponenteId = componente.ComponenteId }, "tabla")
                               .AddGeometryField(builder.CreateGeometryFieldBuilder(atributoGeom, "tabla").GType(), "gtype")
                               .AddGeometryField(builder.CreateGeometryFieldBuilder(atributoGeom, "tabla").AreaSqrMeters(), "area")
                               .AddGeometryField(builder.CreateGeometryFieldBuilder(atributoGeom, "tabla").PerimeterMeters(), "perimetro")
                               .AddGeometryField(builder.CreateGeometryFieldBuilder(atributoGeom, "tabla").LengthMeters(), "longitud")
                               .AddGeometryField(builder.CreateGeometryFieldBuilder(atributoGeom, "tabla").NumPoints(), "vertices")
                               .AddGeometryField(builder.CreateGeometryFieldBuilder(atributoGeom, "tabla").ChangeToSRID(SRID.DB).ToWKT(), "wkt")
                               .AddFilter(atributoClave, objetoId, SQLOperators.EqualsTo)
                               .ExecuteQuery((IDataReader reader) =>
                               {
                                   switch (reader.GetInt32(reader.GetOrdinal("gtype")))
                                   {
                                       case 1: //poligono
                                           model.Graficos.Add(new Grafico() { TipoGrafico = TIPO_POLIGONO, Nombre = "Área (m2)", Valor = reader.GetNullableDecimal(reader.GetOrdinal("area")) });
                                           model.Graficos.Add(new Grafico() { TipoGrafico = TIPO_POLIGONO, Nombre = "Perímetro (m)", Valor = reader.GetNullableDecimal(reader.GetOrdinal("perimetro")) });
                                           model.Graficos.Add(new Grafico() { TipoGrafico = TIPO_POLIGONO, Nombre = "Cant. Nodos", Valor = reader.GetNullableDecimal(reader.GetOrdinal("vertices")) });
                                           break;
                                       case 2: //linea
                                           model.Graficos.Add(new Grafico() { TipoGrafico = TIPO_LINEA, Nombre = "Longitud (m)", Valor = reader.GetNullableDecimal(reader.GetOrdinal("longitud")) });
                                           model.Graficos.Add(new Grafico() { TipoGrafico = TIPO_LINEA, Nombre = "Cant. Nodos", Valor = reader.GetNullableDecimal(reader.GetOrdinal("vertices")) });
                                           break;
                                       case 3: //punto
                                           var geom = reader.GetGeometryFromField(reader.GetOrdinal("wkt"), SRID.DB);
                                           model.Graficos.Add(new Grafico() { TipoGrafico = TIPO_PUNTO, Nombre = "X", Valor = (decimal?)geom.XCoordinate });
                                           model.Graficos.Add(new Grafico() { TipoGrafico = TIPO_PUNTO, Nombre = "Y", Valor = (decimal?)geom.YCoordinate });
                                           break;
                                       default:
                                           break;
                                   }
                               });
                        model.Graficos = model.Graficos.OrderBy(x => x.TipoGrafico).ThenBy(x => x.Nombre).ToList();
                    }
                }
                catch (Exception ex)
                {
                    Global.GetLogger().LogError("DetalleObjeto - getGraficos", ex);
                }
            }
        }

        private object GetValue(GeoSITMContext context, MT.Componente componente, long objetoId, Atributo campoRelacionPadre)
        {
            using (var builder = context.CreateSQLQueryBuilder())
            {
                return builder.AddTable(componente, "t1")
                       .AddFilter(componente.Atributos.GetAtributoClave(), objetoId, SQLOperators.EqualsTo)
                       .AddFields(campoRelacionPadre)
                       .ExecuteQuery((IDataReader reader, ReaderStatus status) =>
                       {
                           status.Break();
                           return reader.GetValue(0);
                       }).Single();
            }
        }

        private Dictionary<int, List<DbGeometry>> getGeometries(object objetoId, MT.Componente componente, Atributo atributoClave, Atributo atributoGeom)
        {
            using (var builder = GeoSITMContext.CreateContext().CreateSQLQueryBuilder())
            {
                var dictionary = new Dictionary<int, List<DbGeometry>>();
                var compGrafico = new MT.Componente { ComponenteId = componente.ComponenteId, Esquema = componente.Esquema, Tabla = componente.TablaGrafica ?? componente.Tabla };

                builder.AddTable(compGrafico, "t1")
                              .AddFilter(atributoClave, objetoId, SQLOperators.EqualsTo)
                              .AddGeometryField(builder.CreateGeometryFieldBuilder(atributoGeom, "t1").GType(), "gtype")
                              .AddGeometryField(builder.CreateGeometryFieldBuilder(atributoGeom, "t1").ChangeToSRID(SRID.App).ToWKT(), "geom")
                              .ExecuteQuery((IDataReader reader, ReaderStatus status) =>
                              {
                                  var geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"));
                                  int grafico = reader.GetInt32(reader.GetOrdinal("gtype"));
                                  if (grafico != TIPO_PUNTO && grafico != TIPO_LINEA && grafico != TIPO_POLIGONO)
                                  {
                                      return;
                                  }
                                  if (!dictionary.ContainsKey(grafico))
                                  {
                                      dictionary.Add(grafico, new List<DbGeometry>());
                                  }
                                  dictionary[grafico].Add(geom);
                              });
                return dictionary;
            }
        }
    }
}
