﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 4.0.30319.42000 版自动生成。
// 
#pragma warning disable 1591

namespace UCHome.PayService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1038.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="EcloudSeviceSoap11Binding", Namespace="http://webservice.sdust.com")]
    public partial class EcloudSevice : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback testOperationCompleted;
        
        private System.Threading.SendOrPostCallback getOrderOperationCompleted;
        
        private System.Threading.SendOrPostCallback getDealByCountOperationCompleted;
        
        private System.Threading.SendOrPostCallback getECloudURLOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public EcloudSevice() {
            this.Url = global::UCHome.Properties.Settings.Default.UCHome_PayService_EcloudSevice;
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
        public event testCompletedEventHandler testCompleted;
        
        /// <remarks/>
        public event getOrderCompletedEventHandler getOrderCompleted;
        
        /// <remarks/>
        public event getDealByCountCompletedEventHandler getDealByCountCompleted;
        
        /// <remarks/>
        public event getECloudURLCompletedEventHandler getECloudURLCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:test", RequestNamespace="http://webservice.sdust.com", ResponseNamespace="http://webservice.sdust.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", IsNullable=true)]
        public string test() {
            object[] results = this.Invoke("test", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void testAsync() {
            this.testAsync(null);
        }
        
        /// <remarks/>
        public void testAsync(object userState) {
            if ((this.testOperationCompleted == null)) {
                this.testOperationCompleted = new System.Threading.SendOrPostCallback(this.OntestOperationCompleted);
            }
            this.InvokeAsync("test", new object[0], this.testOperationCompleted, userState);
        }
        
        private void OntestOperationCompleted(object arg) {
            if ((this.testCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.testCompleted(this, new testCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:getOrder", RequestNamespace="http://webservice.sdust.com", ResponseNamespace="http://webservice.sdust.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", IsNullable=true)]
        public string getOrder([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string userID) {
            object[] results = this.Invoke("getOrder", new object[] {
                        userID});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getOrderAsync(string userID) {
            this.getOrderAsync(userID, null);
        }
        
        /// <remarks/>
        public void getOrderAsync(string userID, object userState) {
            if ((this.getOrderOperationCompleted == null)) {
                this.getOrderOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetOrderOperationCompleted);
            }
            this.InvokeAsync("getOrder", new object[] {
                        userID}, this.getOrderOperationCompleted, userState);
        }
        
        private void OngetOrderOperationCompleted(object arg) {
            if ((this.getOrderCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getOrderCompleted(this, new getOrderCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:getDealByCount", RequestNamespace="http://webservice.sdust.com", ResponseNamespace="http://webservice.sdust.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", IsNullable=true)]
        public string getDealByCount([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string userID, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string count) {
            object[] results = this.Invoke("getDealByCount", new object[] {
                        userID,
                        count});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getDealByCountAsync(string userID, string count) {
            this.getDealByCountAsync(userID, count, null);
        }
        
        /// <remarks/>
        public void getDealByCountAsync(string userID, string count, object userState) {
            if ((this.getDealByCountOperationCompleted == null)) {
                this.getDealByCountOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetDealByCountOperationCompleted);
            }
            this.InvokeAsync("getDealByCount", new object[] {
                        userID,
                        count}, this.getDealByCountOperationCompleted, userState);
        }
        
        private void OngetDealByCountOperationCompleted(object arg) {
            if ((this.getDealByCountCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getDealByCountCompleted(this, new getDealByCountCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:getECloudURL", RequestNamespace="http://webservice.sdust.com", ResponseNamespace="http://webservice.sdust.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", IsNullable=true)]
        public string getECloudURL() {
            object[] results = this.Invoke("getECloudURL", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getECloudURLAsync() {
            this.getECloudURLAsync(null);
        }
        
        /// <remarks/>
        public void getECloudURLAsync(object userState) {
            if ((this.getECloudURLOperationCompleted == null)) {
                this.getECloudURLOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetECloudURLOperationCompleted);
            }
            this.InvokeAsync("getECloudURL", new object[0], this.getECloudURLOperationCompleted, userState);
        }
        
        private void OngetECloudURLOperationCompleted(object arg) {
            if ((this.getECloudURLCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getECloudURLCompleted(this, new getECloudURLCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1038.0")]
    public delegate void testCompletedEventHandler(object sender, testCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1038.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class testCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal testCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1038.0")]
    public delegate void getOrderCompletedEventHandler(object sender, getOrderCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1038.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getOrderCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getOrderCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1038.0")]
    public delegate void getDealByCountCompletedEventHandler(object sender, getDealByCountCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1038.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getDealByCountCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getDealByCountCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1038.0")]
    public delegate void getECloudURLCompletedEventHandler(object sender, getECloudURLCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1038.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getECloudURLCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getECloudURLCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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