﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace GeoSit.Web.Api.inmEdita {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="Alta y Modificación de InmuebleBinding", Namespace="urn:inmEdita")]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(titulares_inmueble))]
    public partial class AltayModificacióndeInmueble : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback editaOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public AltayModificacióndeInmueble() {
            this.Url = global::GeoSit.Web.Api.Properties.Settings.Default.GeoSit_Web_Api_inmEdita_Alta_y_Modificación_de_Inmueble;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event editaCompletedEventHandler editaCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:inmuebles#edita", RequestNamespace="urn:inmModifica", ResponseNamespace="urn:inmModifica")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public string edita(datos_inmueble datos_inmueble, titulares_inmueble[] titulares_inmueble) {
            object[] results = this.Invoke("edita", new object[] {
                        datos_inmueble,
                        titulares_inmueble});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void editaAsync(datos_inmueble datos_inmueble, titulares_inmueble[] titulares_inmueble) {
            this.editaAsync(datos_inmueble, titulares_inmueble, null);
        }
        
        /// <remarks/>
        public void editaAsync(datos_inmueble datos_inmueble, titulares_inmueble[] titulares_inmueble, object userState) {
            if ((this.editaOperationCompleted == null)) {
                this.editaOperationCompleted = new System.Threading.SendOrPostCallback(this.OneditaOperationCompleted);
            }
            this.InvokeAsync("edita", new object[] {
                        datos_inmueble,
                        titulares_inmueble}, this.editaOperationCompleted, userState);
        }
        
        private void OneditaOperationCompleted(object arg) {
            if ((this.editaCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.editaCompleted(this, new editaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:inmEdita")]
    public partial class datos_inmueble {
        
        private string accionField;
        
        private int obj_idField;
        
        private string s1Field;
        
        private string s2Field;
        
        private string s3Field;
        
        private string manzField;
        
        private string parcField;
        
        private int ufField;
        
        private float porcufField;
        
        private int parpField;
        
        private int parporigenField;
        
        private int planoField;
        
        private float anio_mensuraField;
        
        private string urbsubField;
        
        private int regimenField;
        
        private string tipoField;
        
        private string titularidadField;
        
        private int usoField;
        
        private string tmatricField;
        
        private string matricField;
        
        private System.DateTime fchmatricField;
        
        private float suptField;
        
        private float supmField;
        
        private float avaltField;
        
        private float avalmField;
        
        private float frenteField;
        
        private float fondoField;
        
        private int es_esquinaField;
        
        private int es_calleppalField;
        
        private string zonatField;
        
        private int zonavField;
        
        private int zonaopField;
        
        private int aguaField;
        
        private int cloacaField;
        
        private int gasField;
        
        private int alumField;
        
        private int pavField;
        
        private int barr_idField;
        
        private int calle_idField;
        
        private string calle_nomField;
        
        private int puertaField;
        
        private string pisoField;
        
        private string dptoField;
        
        private string detField;
        
        private int vigenciaField;
        
        /// <remarks/>
        public string accion {
            get {
                return this.accionField;
            }
            set {
                this.accionField = value;
            }
        }
        
        /// <remarks/>
        public int obj_id {
            get {
                return this.obj_idField;
            }
            set {
                this.obj_idField = value;
            }
        }
        
        /// <remarks/>
        public string s1 {
            get {
                return this.s1Field;
            }
            set {
                this.s1Field = value;
            }
        }
        
        /// <remarks/>
        public string s2 {
            get {
                return this.s2Field;
            }
            set {
                this.s2Field = value;
            }
        }
        
        /// <remarks/>
        public string s3 {
            get {
                return this.s3Field;
            }
            set {
                this.s3Field = value;
            }
        }
        
        /// <remarks/>
        public string manz {
            get {
                return this.manzField;
            }
            set {
                this.manzField = value;
            }
        }
        
        /// <remarks/>
        public string parc {
            get {
                return this.parcField;
            }
            set {
                this.parcField = value;
            }
        }
        
        /// <remarks/>
        public int uf {
            get {
                return this.ufField;
            }
            set {
                this.ufField = value;
            }
        }
        
        /// <remarks/>
        public float porcuf {
            get {
                return this.porcufField;
            }
            set {
                this.porcufField = value;
            }
        }
        
        /// <remarks/>
        public int parp {
            get {
                return this.parpField;
            }
            set {
                this.parpField = value;
            }
        }
        
        /// <remarks/>
        public int parporigen {
            get {
                return this.parporigenField;
            }
            set {
                this.parporigenField = value;
            }
        }
        
        /// <remarks/>
        public int plano {
            get {
                return this.planoField;
            }
            set {
                this.planoField = value;
            }
        }
        
        /// <remarks/>
        public float anio_mensura {
            get {
                return this.anio_mensuraField;
            }
            set {
                this.anio_mensuraField = value;
            }
        }
        
        /// <remarks/>
        public string urbsub {
            get {
                return this.urbsubField;
            }
            set {
                this.urbsubField = value;
            }
        }
        
        /// <remarks/>
        public int regimen {
            get {
                return this.regimenField;
            }
            set {
                this.regimenField = value;
            }
        }
        
        /// <remarks/>
        public string tipo {
            get {
                return this.tipoField;
            }
            set {
                this.tipoField = value;
            }
        }
        
        /// <remarks/>
        public string titularidad {
            get {
                return this.titularidadField;
            }
            set {
                this.titularidadField = value;
            }
        }
        
        /// <remarks/>
        public int uso {
            get {
                return this.usoField;
            }
            set {
                this.usoField = value;
            }
        }
        
        /// <remarks/>
        public string tmatric {
            get {
                return this.tmatricField;
            }
            set {
                this.tmatricField = value;
            }
        }
        
        /// <remarks/>
        public string matric {
            get {
                return this.matricField;
            }
            set {
                this.matricField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(DataType="date")]
        public System.DateTime fchmatric {
            get {
                return this.fchmatricField;
            }
            set {
                this.fchmatricField = value;
            }
        }
        
        /// <remarks/>
        public float supt {
            get {
                return this.suptField;
            }
            set {
                this.suptField = value;
            }
        }
        
        /// <remarks/>
        public float supm {
            get {
                return this.supmField;
            }
            set {
                this.supmField = value;
            }
        }
        
        /// <remarks/>
        public float avalt {
            get {
                return this.avaltField;
            }
            set {
                this.avaltField = value;
            }
        }
        
        /// <remarks/>
        public float avalm {
            get {
                return this.avalmField;
            }
            set {
                this.avalmField = value;
            }
        }
        
        /// <remarks/>
        public float frente {
            get {
                return this.frenteField;
            }
            set {
                this.frenteField = value;
            }
        }
        
        /// <remarks/>
        public float fondo {
            get {
                return this.fondoField;
            }
            set {
                this.fondoField = value;
            }
        }
        
        /// <remarks/>
        public int es_esquina {
            get {
                return this.es_esquinaField;
            }
            set {
                this.es_esquinaField = value;
            }
        }
        
        /// <remarks/>
        public int es_calleppal {
            get {
                return this.es_calleppalField;
            }
            set {
                this.es_calleppalField = value;
            }
        }
        
        /// <remarks/>
        public string zonat {
            get {
                return this.zonatField;
            }
            set {
                this.zonatField = value;
            }
        }
        
        /// <remarks/>
        public int zonav {
            get {
                return this.zonavField;
            }
            set {
                this.zonavField = value;
            }
        }
        
        /// <remarks/>
        public int zonaop {
            get {
                return this.zonaopField;
            }
            set {
                this.zonaopField = value;
            }
        }
        
        /// <remarks/>
        public int agua {
            get {
                return this.aguaField;
            }
            set {
                this.aguaField = value;
            }
        }
        
        /// <remarks/>
        public int cloaca {
            get {
                return this.cloacaField;
            }
            set {
                this.cloacaField = value;
            }
        }
        
        /// <remarks/>
        public int gas {
            get {
                return this.gasField;
            }
            set {
                this.gasField = value;
            }
        }
        
        /// <remarks/>
        public int alum {
            get {
                return this.alumField;
            }
            set {
                this.alumField = value;
            }
        }
        
        /// <remarks/>
        public int pav {
            get {
                return this.pavField;
            }
            set {
                this.pavField = value;
            }
        }
        
        /// <remarks/>
        public int barr_id {
            get {
                return this.barr_idField;
            }
            set {
                this.barr_idField = value;
            }
        }
        
        /// <remarks/>
        public int calle_id {
            get {
                return this.calle_idField;
            }
            set {
                this.calle_idField = value;
            }
        }
        
        /// <remarks/>
        public string calle_nom {
            get {
                return this.calle_nomField;
            }
            set {
                this.calle_nomField = value;
            }
        }
        
        /// <remarks/>
        public int puerta {
            get {
                return this.puertaField;
            }
            set {
                this.puertaField = value;
            }
        }
        
        /// <remarks/>
        public string piso {
            get {
                return this.pisoField;
            }
            set {
                this.pisoField = value;
            }
        }
        
        /// <remarks/>
        public string dpto {
            get {
                return this.dptoField;
            }
            set {
                this.dptoField = value;
            }
        }
        
        /// <remarks/>
        public string det {
            get {
                return this.detField;
            }
            set {
                this.detField = value;
            }
        }
        
        /// <remarks/>
        public int vigencia {
            get {
                return this.vigenciaField;
            }
            set {
                this.vigenciaField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:inmEdita")]
    public partial class titulares_inmueble {
        
        private string numField;
        
        private string tipoField;
        
        private string porcField;
        
        /// <remarks/>
        public string num {
            get {
                return this.numField;
            }
            set {
                this.numField = value;
            }
        }
        
        /// <remarks/>
        public string tipo {
            get {
                return this.tipoField;
            }
            set {
                this.tipoField = value;
            }
        }
        
        /// <remarks/>
        public string porc {
            get {
                return this.porcField;
            }
            set {
                this.porcField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void editaCompletedEventHandler(object sender, editaCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class editaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal editaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591