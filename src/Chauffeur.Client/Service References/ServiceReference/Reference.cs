﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chauffeur.Client.ServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Build", Namespace="http://schemas.datacontract.org/2004/07/Chauffeur.Jenkins.Model")]
    [System.SerializableAttribute()]
    public partial class Build : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Chauffeur.Client.ServiceReference.Artifact[] artifactsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string buildOnField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool buildingField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Chauffeur.Client.ServiceReference.ChangeSet changeSetField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Chauffeur.Client.ServiceReference.User[] culpritsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int durationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string fullDisplayNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string idField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool keepLogField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int numberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string resultField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Uri urlField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Chauffeur.Client.ServiceReference.Artifact[] artifacts {
            get {
                return this.artifactsField;
            }
            set {
                if ((object.ReferenceEquals(this.artifactsField, value) != true)) {
                    this.artifactsField = value;
                    this.RaisePropertyChanged("artifacts");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string buildOn {
            get {
                return this.buildOnField;
            }
            set {
                if ((object.ReferenceEquals(this.buildOnField, value) != true)) {
                    this.buildOnField = value;
                    this.RaisePropertyChanged("buildOn");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool building {
            get {
                return this.buildingField;
            }
            set {
                if ((this.buildingField.Equals(value) != true)) {
                    this.buildingField = value;
                    this.RaisePropertyChanged("building");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Chauffeur.Client.ServiceReference.ChangeSet changeSet {
            get {
                return this.changeSetField;
            }
            set {
                if ((object.ReferenceEquals(this.changeSetField, value) != true)) {
                    this.changeSetField = value;
                    this.RaisePropertyChanged("changeSet");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Chauffeur.Client.ServiceReference.User[] culprits {
            get {
                return this.culpritsField;
            }
            set {
                if ((object.ReferenceEquals(this.culpritsField, value) != true)) {
                    this.culpritsField = value;
                    this.RaisePropertyChanged("culprits");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int duration {
            get {
                return this.durationField;
            }
            set {
                if ((this.durationField.Equals(value) != true)) {
                    this.durationField = value;
                    this.RaisePropertyChanged("duration");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string fullDisplayName {
            get {
                return this.fullDisplayNameField;
            }
            set {
                if ((object.ReferenceEquals(this.fullDisplayNameField, value) != true)) {
                    this.fullDisplayNameField = value;
                    this.RaisePropertyChanged("fullDisplayName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string id {
            get {
                return this.idField;
            }
            set {
                if ((object.ReferenceEquals(this.idField, value) != true)) {
                    this.idField = value;
                    this.RaisePropertyChanged("id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool keepLog {
            get {
                return this.keepLogField;
            }
            set {
                if ((this.keepLogField.Equals(value) != true)) {
                    this.keepLogField = value;
                    this.RaisePropertyChanged("keepLog");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int number {
            get {
                return this.numberField;
            }
            set {
                if ((this.numberField.Equals(value) != true)) {
                    this.numberField = value;
                    this.RaisePropertyChanged("number");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string result {
            get {
                return this.resultField;
            }
            set {
                if ((object.ReferenceEquals(this.resultField, value) != true)) {
                    this.resultField = value;
                    this.RaisePropertyChanged("result");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Uri url {
            get {
                return this.urlField;
            }
            set {
                if ((object.ReferenceEquals(this.urlField, value) != true)) {
                    this.urlField = value;
                    this.RaisePropertyChanged("url");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ChangeSet", Namespace="http://schemas.datacontract.org/2004/07/Chauffeur.Jenkins.Model")]
    [System.SerializableAttribute()]
    public partial class ChangeSet : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Chauffeur.Client.ServiceReference.ChangeSetItem[] itemsField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Chauffeur.Client.ServiceReference.ChangeSetItem[] items {
            get {
                return this.itemsField;
            }
            set {
                if ((object.ReferenceEquals(this.itemsField, value) != true)) {
                    this.itemsField = value;
                    this.RaisePropertyChanged("items");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Artifact", Namespace="http://schemas.datacontract.org/2004/07/Chauffeur.Jenkins.Model")]
    [System.SerializableAttribute()]
    public partial class Artifact : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string displayPathField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string fileNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string relativePathField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string displayPath {
            get {
                return this.displayPathField;
            }
            set {
                if ((object.ReferenceEquals(this.displayPathField, value) != true)) {
                    this.displayPathField = value;
                    this.RaisePropertyChanged("displayPath");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string fileName {
            get {
                return this.fileNameField;
            }
            set {
                if ((object.ReferenceEquals(this.fileNameField, value) != true)) {
                    this.fileNameField = value;
                    this.RaisePropertyChanged("fileName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string relativePath {
            get {
                return this.relativePathField;
            }
            set {
                if ((object.ReferenceEquals(this.relativePathField, value) != true)) {
                    this.relativePathField = value;
                    this.RaisePropertyChanged("relativePath");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="User", Namespace="http://schemas.datacontract.org/2004/07/Chauffeur.Jenkins.Model")]
    [System.SerializableAttribute()]
    public partial class User : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string descriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string fullNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string idField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Uri urlField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string description {
            get {
                return this.descriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.descriptionField, value) != true)) {
                    this.descriptionField = value;
                    this.RaisePropertyChanged("description");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string fullName {
            get {
                return this.fullNameField;
            }
            set {
                if ((object.ReferenceEquals(this.fullNameField, value) != true)) {
                    this.fullNameField = value;
                    this.RaisePropertyChanged("fullName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string id {
            get {
                return this.idField;
            }
            set {
                if ((object.ReferenceEquals(this.idField, value) != true)) {
                    this.idField = value;
                    this.RaisePropertyChanged("id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Uri url {
            get {
                return this.urlField;
            }
            set {
                if ((object.ReferenceEquals(this.urlField, value) != true)) {
                    this.urlField = value;
                    this.RaisePropertyChanged("url");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ChangeSetItem", Namespace="http://schemas.datacontract.org/2004/07/Chauffeur.Jenkins.Model")]
    [System.SerializableAttribute()]
    public partial class ChangeSetItem : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Chauffeur.Client.ServiceReference.User authorField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string commentField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime dateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string idField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string msgField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Chauffeur.Client.ServiceReference.ChangeSetPath[] pathsField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Chauffeur.Client.ServiceReference.User author {
            get {
                return this.authorField;
            }
            set {
                if ((object.ReferenceEquals(this.authorField, value) != true)) {
                    this.authorField = value;
                    this.RaisePropertyChanged("author");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string comment {
            get {
                return this.commentField;
            }
            set {
                if ((object.ReferenceEquals(this.commentField, value) != true)) {
                    this.commentField = value;
                    this.RaisePropertyChanged("comment");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime date {
            get {
                return this.dateField;
            }
            set {
                if ((this.dateField.Equals(value) != true)) {
                    this.dateField = value;
                    this.RaisePropertyChanged("date");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string id {
            get {
                return this.idField;
            }
            set {
                if ((object.ReferenceEquals(this.idField, value) != true)) {
                    this.idField = value;
                    this.RaisePropertyChanged("id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string msg {
            get {
                return this.msgField;
            }
            set {
                if ((object.ReferenceEquals(this.msgField, value) != true)) {
                    this.msgField = value;
                    this.RaisePropertyChanged("msg");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Chauffeur.Client.ServiceReference.ChangeSetPath[] paths {
            get {
                return this.pathsField;
            }
            set {
                if ((object.ReferenceEquals(this.pathsField, value) != true)) {
                    this.pathsField = value;
                    this.RaisePropertyChanged("paths");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ChangeSetPath", Namespace="http://schemas.datacontract.org/2004/07/Chauffeur.Jenkins.Model")]
    [System.SerializableAttribute()]
    public partial class ChangeSetPath : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string editTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string fileField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string editType {
            get {
                return this.editTypeField;
            }
            set {
                if ((object.ReferenceEquals(this.editTypeField, value) != true)) {
                    this.editTypeField = value;
                    this.RaisePropertyChanged("editType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string file {
            get {
                return this.fileField;
            }
            set {
                if ((object.ReferenceEquals(this.fileField, value) != true)) {
                    this.fileField = value;
                    this.RaisePropertyChanged("file");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference.IChauffeurService")]
    public interface IChauffeurService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChauffeurService/InstallLastSuccessfulBuild", ReplyAction="http://tempuri.org/IChauffeurService/InstallLastSuccessfulBuildResponse")]
        Chauffeur.Client.ServiceReference.Build InstallLastSuccessfulBuild(string jobName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChauffeurService/InstallLastSuccessfulBuild", ReplyAction="http://tempuri.org/IChauffeurService/InstallLastSuccessfulBuildResponse")]
        System.Threading.Tasks.Task<Chauffeur.Client.ServiceReference.Build> InstallLastSuccessfulBuildAsync(string jobName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IChauffeurServiceChannel : Chauffeur.Client.ServiceReference.IChauffeurService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ChauffeurServiceClient : System.ServiceModel.ClientBase<Chauffeur.Client.ServiceReference.IChauffeurService>, Chauffeur.Client.ServiceReference.IChauffeurService {
        
        public ChauffeurServiceClient() {
        }
        
        public ChauffeurServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ChauffeurServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ChauffeurServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ChauffeurServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Chauffeur.Client.ServiceReference.Build InstallLastSuccessfulBuild(string jobName) {
            return base.Channel.InstallLastSuccessfulBuild(jobName);
        }
        
        public System.Threading.Tasks.Task<Chauffeur.Client.ServiceReference.Build> InstallLastSuccessfulBuildAsync(string jobName) {
            return base.Channel.InstallLastSuccessfulBuildAsync(jobName);
        }
    }
}
