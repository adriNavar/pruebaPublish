using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Client.Web.Models.ObrasPublicas
{
    public class ObjetoInfraestructuraModel
    {
        public ObjetoInfraestructuraModel()
        {
            EditorComparador = new List<EditorComparador>();
            EditorConector = new List<EditorConector>();
            EditorSintactico = new List<EditorSintactico>();
            Clases = new List<Clases>();

            EditorComparador.Add(new EditorComparador(1, "Igual", "=="));
            EditorComparador.Add(new EditorComparador(2, "Distinto", "<>"));
            EditorComparador.Add(new EditorComparador(3, "Menor", "<"));
            EditorComparador.Add(new EditorComparador(4, "Menor o igual", "<="));
            EditorComparador.Add(new EditorComparador(5, "Mayor", ">"));
            EditorComparador.Add(new EditorComparador(6, "Mayor o igual", ">="));
            EditorComparador.Add(new EditorComparadorLike(7, "Contiene", "=="));

            EditorConector.Add(new EditorConector(1, "Y", "and"));
            EditorConector.Add(new EditorConector(2, "O", "or"));
            EditorConector.Add(new EditorConector(3, "(", "("));
            EditorConector.Add(new EditorConector(4, ")", ")"));

            Clases.Add(new Clases { ClassID = 1, Descripcion = "Punto" });
            Clases.Add(new Clases { ClassID = 2, Descripcion = "Línea" });
            Clases.Add(new Clases { ClassID = 3, Descripcion = "Polígono" });
        }

        public virtual List<EditorComparador> EditorComparador { get; set; }
        public virtual List<EditorConector> EditorConector { get; set; }
        public virtual List<EditorSintactico> EditorSintactico { get; set; }
        public virtual List<Clases> Clases { get; set; }
        public ObjetoInfraestructura ObjetoInfraestructura { get; set; }
    }
    public class EditorComparadorNull : EditorComparador
    {
        public EditorComparadorNull(int pId, string pVisual, string pInterno) : base(pId, pVisual, pInterno) { }
        protected override string GetComparacion(string atributo, string valor)
        {
            return string.Format(" _Atributos[\"{0}\"]{1}", atributo, this._interno);
        }
    }
    public class EditorComparadorLike : EditorComparador
    {
        public EditorComparadorLike(int pId, string pVisual, string pInterno) : base(pId, pVisual, pInterno) { }
        protected override string GetComparacion(string atributo, string valor)
        {
            return string.Format(" _Atributos[\"{0}\"].ToString().ToLower().Contains(\"{1}\")", atributo, valor.ToLower());
        }
        protected override string GetComparacionCampo(string atributo, string valor)
        {
            return string.Format("{0}.ToString().ToLower().Contains(\"{1}\")", atributo, valor.ToLower());
        }
    }
    public class EditorComparador
    {
        protected string _interno = string.Empty;
        public EditorComparador(int pId, string pVisual, string pInterno)
        {
            Id = pId;
            Visual = pVisual;
            _interno = pInterno;
        }

        public int Id { get; set; }
        public string Visual { get; set; }
        protected virtual string GetComparacion(string atributo, string valor)
        {
            return string.Format(" _Atributos[\"{0}\"].ToString().ToLower(){1}\"{2}\"", atributo, this._interno, valor.ToLower());
        }
        protected virtual string GetComparacionCampo(string atributo, string valor)
        {
            return string.Format("{0}.ToString().ToLower(){1}\"{2}\"", atributo, this._interno, valor.ToLower());
        }

        internal string Get(string atributo, string valor)
        {
            return string.Format(" (Atributos.Contains(\"{0}\") and {1})", atributo, this.GetComparacion(atributo, valor));
        }
        internal string GetCampo(string atributo, string valor)
        {
            return string.Format(" ({0})", GetComparacionCampo(atributo, valor));
        }
    }

    public class EditorConector
    {
        private string _value = string.Empty;
        public EditorConector(int pId, string pVisual, string pInterno)
        {
            Id = pId;
            Visual = pVisual;
            _value = pInterno;
        }

        public int Id { get; set; }
        public string Visual { get; set; }
        internal string GetConectorValue()
        {
            return this._value;
        }
    }

    public class EditorSintactico
    {
        public long Id { get; set; }
        public string Atributo { get; set; }
        public EditorComparador Comparador { get; set; }
        public string Valor { get; set; }
        public EditorConector Conector { get; set; }
        internal string GetCondicion(bool esUltima)
        {
            return string.Format("{0}{1}", this.Comparador.Get(this.Atributo, this.Valor), (esUltima ? "" : string.Format(" {0} ", this.Conector.GetConectorValue())));
        }

        internal string GetCondicionCampo(bool esUltima)
        {
            return string.Format("{0}{1}", this.Comparador.GetCampo(this.Atributo, this.Valor), (esUltima ? "" : string.Format(" {0} ", this.Conector.GetConectorValue())));
        }
    }

    public class Clases
    {
        public long ClassID { get; set; }
        public string Descripcion { get; set; }
    }
}