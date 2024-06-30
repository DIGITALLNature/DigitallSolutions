using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query; 
using AttributeCollection = Microsoft.Xrm.Sdk.AttributeCollection;

// ReSharper disable All
namespace D365.Extension.Model
{
	/// <inheritdoc />
	/// <summary>
	/// Service endpoint that can be contacted.
	/// </summary>
	[DataContractAttribute()]
	[EntityLogicalNameAttribute("serviceendpoint")]
	[System.CodeDom.Compiler.GeneratedCode("dgtp", "2023")]
    [ExcludeFromCodeCoverage]
	public partial class ServiceEndpoint : Entity, INotifyPropertyChanging, INotifyPropertyChanged
    {
	    #region ctor
		[DebuggerNonUserCode]
		public ServiceEndpoint() : this(false)
        {
        }

        [DebuggerNonUserCode]
		public ServiceEndpoint(bool trackChanges = false) : base(EntityLogicalName)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public ServiceEndpoint(Guid id, bool trackChanges = false) : base(EntityLogicalName,id)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public ServiceEndpoint(KeyAttributeCollection keyAttributes, bool trackChanges = false) : base(EntityLogicalName,keyAttributes)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public ServiceEndpoint(string keyName, object keyValue, bool trackChanges = false) : base(EntityLogicalName, keyName, keyValue)
        {
			_trackChanges = trackChanges;
        }
        #endregion

		#region fields
        private readonly bool _trackChanges;
        private readonly Lazy<HashSet<string>> _changedProperties = new Lazy<HashSet<string>>();
        #endregion

        #region consts
        public const string EntityLogicalName = "serviceendpoint";
        public const string PrimaryNameAttribute = "name";
        public const int EntityTypeCode = 4618;
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        [DebuggerNonUserCode]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (_trackChanges)
            {
                _changedProperties.Value.Add(propertyName);
            }
        }

        [DebuggerNonUserCode]
		private void OnPropertyChanging([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanging != null) PropertyChanging.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        #endregion

		#region Attributes
		[AttributeLogicalNameAttribute("serviceendpointid")]
		public new System.Guid Id
		{
		    [DebuggerNonUserCode]
			get
			{
				return base.Id;
			}
            [DebuggerNonUserCode]
			set
			{
				ServiceEndpointId = value;
			}
		}

		/// <summary>
		/// Unique identifier of the service endpoint.
		/// </summary>
		[AttributeLogicalName("serviceendpointid")]
        public Guid? ServiceEndpointId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("serviceendpointid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ServiceEndpointId));
                SetAttributeValue("serviceendpointid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
                OnPropertyChanged(nameof(ServiceEndpointId));
            }
        }

		/// <summary>
		/// Specifies mode of authentication with SB
		/// </summary>
		[AttributeLogicalName("authtype")]
        public OptionSetValue AuthType
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("authtype");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(AuthType));
                SetAttributeValue("authtype", value);
                OnPropertyChanged(nameof(AuthType));
            }
        }

		/// <summary>
		/// Authentication Value
		/// </summary>
		[AttributeLogicalName("authvalue")]
        public string AuthValue
        {
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(AuthValue));
                SetAttributeValue("authvalue", value);
                OnPropertyChanged(nameof(AuthValue));
            }
        }

		/// <summary>
		/// For internal use only.
		/// </summary>
		[AttributeLogicalName("componentstate")]
        public OptionSetValue ComponentState
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("componentstate");
            }
        }

		/// <summary>
		/// Connection mode to contact the service endpoint.
		/// </summary>
		[AttributeLogicalName("connectionmode")]
        public OptionSetValue ConnectionMode
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("connectionmode");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ConnectionMode));
                SetAttributeValue("connectionmode", value);
                OnPropertyChanged(nameof(ConnectionMode));
            }
        }

		/// <summary>
		/// Type of the endpoint contract.
		/// </summary>
		[AttributeLogicalName("contract")]
        public OptionSetValue Contract
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("contract");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(Contract));
                SetAttributeValue("contract", value);
                OnPropertyChanged(nameof(Contract));
            }
        }

		/// <summary>
		/// Unique identifier of the user who created the service endpoint.
		/// </summary>
		[AttributeLogicalName("createdby")]
        public EntityReference CreatedBy
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("createdby");
            }
        }

		/// <summary>
		/// Date and time when the service endpoint was created.
		/// </summary>
		[AttributeLogicalName("createdon")]
        public DateTime? CreatedOn
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<DateTime?>("createdon");
            }
        }

		/// <summary>
		/// Unique identifier of the delegate user who created the service endpoint.
		/// </summary>
		[AttributeLogicalName("createdonbehalfby")]
        public EntityReference CreatedOnBehalfBy
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("createdonbehalfby");
            }
        }

		/// <summary>
		/// Description of the service endpoint.
		/// </summary>
		[AttributeLogicalName("description")]
        public string Description
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("description");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(Description));
                SetAttributeValue("description", value);
                OnPropertyChanged(nameof(Description));
            }
        }

		/// <summary>
		/// Version in which the form is introduced.
		/// </summary>
		[AttributeLogicalName("introducedversion")]
        public string IntroducedVersion
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("introducedversion");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IntroducedVersion));
                SetAttributeValue("introducedversion", value);
                OnPropertyChanged(nameof(IntroducedVersion));
            }
        }

		
		[AttributeLogicalName("isauthvalueset")]
        public bool? IsAuthValueSet
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("isauthvalueset");
            }
        }

		/// <summary>
		/// Information that specifies whether this component can be customized.
		/// </summary>
		[AttributeLogicalName("iscustomizable")]
        public BooleanManagedProperty IsCustomizable
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<BooleanManagedProperty>("iscustomizable");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(IsCustomizable));
                SetAttributeValue("iscustomizable", value);
                OnPropertyChanged(nameof(IsCustomizable));
            }
        }

		/// <summary>
		/// Information that specifies whether this component is managed.
		/// </summary>
		[AttributeLogicalName("ismanaged")]
        public bool? IsManaged
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("ismanaged");
            }
        }

		
		[AttributeLogicalName("issaskeyset")]
        public bool? IsSASKeySet
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("issaskeyset");
            }
        }

		
		[AttributeLogicalName("issastokenset")]
        public bool? IsSASTokenSet
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("issastokenset");
            }
        }

		/// <summary>
		/// Unique identifier for keyvaultreference associated with serviceendpoint.
		/// </summary>
		[AttributeLogicalName("keyvaultreferenceid")]
        public EntityReference KeyVaultReferenceId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("keyvaultreferenceid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(KeyVaultReferenceId));
                SetAttributeValue("keyvaultreferenceid", value);
                OnPropertyChanged(nameof(KeyVaultReferenceId));
            }
        }

		/// <summary>
		/// Specifies the character encoding for message content
		/// </summary>
		[AttributeLogicalName("messagecharset")]
        public OptionSetValue MessageCharset
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("messagecharset");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(MessageCharset));
                SetAttributeValue("messagecharset", value);
                OnPropertyChanged(nameof(MessageCharset));
            }
        }

		/// <summary>
		/// Content type of the message
		/// </summary>
		[AttributeLogicalName("messageformat")]
        public OptionSetValue MessageFormat
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("messageformat");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(MessageFormat));
                SetAttributeValue("messageformat", value);
                OnPropertyChanged(nameof(MessageFormat));
            }
        }

		/// <summary>
		/// Unique identifier of the user who last modified the service endpoint.
		/// </summary>
		[AttributeLogicalName("modifiedby")]
        public EntityReference ModifiedBy
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("modifiedby");
            }
        }

		/// <summary>
		/// Date and time when the service endpoint was last modified.
		/// </summary>
		[AttributeLogicalName("modifiedon")]
        public DateTime? ModifiedOn
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<DateTime?>("modifiedon");
            }
        }

		/// <summary>
		/// Unique identifier of the delegate user who modified the service endpoint.
		/// </summary>
		[AttributeLogicalName("modifiedonbehalfby")]
        public EntityReference ModifiedOnBehalfBy
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("modifiedonbehalfby");
            }
        }

		/// <summary>
		/// Name of Service end point.
		/// </summary>
		[AttributeLogicalName("name")]
        public string Name
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("name");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(Name));
                SetAttributeValue("name", value);
                OnPropertyChanged(nameof(Name));
            }
        }

		/// <summary>
		/// Full service endpoint address.
		/// </summary>
		[AttributeLogicalName("namespaceaddress")]
        public string NamespaceAddress
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("namespaceaddress");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(NamespaceAddress));
                SetAttributeValue("namespaceaddress", value);
                OnPropertyChanged(nameof(NamespaceAddress));
            }
        }

		/// <summary>
		/// Format of Service Bus Namespace
		/// </summary>
		[AttributeLogicalName("namespaceformat")]
        public OptionSetValue NamespaceFormat
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("namespaceformat");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(NamespaceFormat));
                SetAttributeValue("namespaceformat", value);
                OnPropertyChanged(nameof(NamespaceFormat));
            }
        }

		/// <summary>
		/// Unique identifier of the organization with which the service endpoint is associated.
		/// </summary>
		[AttributeLogicalName("organizationid")]
        public EntityReference OrganizationId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("organizationid");
            }
        }

		/// <summary>
		/// For internal use only.
		/// </summary>
		[AttributeLogicalName("overwritetime")]
        public DateTime? OverwriteTime
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<DateTime?>("overwritetime");
            }
        }

		/// <summary>
		/// Path to the service endpoint.
		/// </summary>
		[AttributeLogicalName("path")]
        public string Path
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("path");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(Path));
                SetAttributeValue("path", value);
                OnPropertyChanged(nameof(Path));
            }
        }

		/// <summary>
		/// For internal use only. Holds miscellaneous properties related to runtime integration.
		/// </summary>
		[AttributeLogicalName("runtimeintegrationproperties")]
        public string RuntimeIntegrationProperties
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("runtimeintegrationproperties");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(RuntimeIntegrationProperties));
                SetAttributeValue("runtimeintegrationproperties", value);
                OnPropertyChanged(nameof(RuntimeIntegrationProperties));
            }
        }

		/// <summary>
		/// Shared Access Key
		/// </summary>
		[AttributeLogicalName("saskey")]
        public string SASKey
        {
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(SASKey));
                SetAttributeValue("saskey", value);
                OnPropertyChanged(nameof(SASKey));
            }
        }

		/// <summary>
		/// Shared Access Key Name
		/// </summary>
		[AttributeLogicalName("saskeyname")]
        public string SASKeyName
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("saskeyname");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(SASKeyName));
                SetAttributeValue("saskeyname", value);
                OnPropertyChanged(nameof(SASKeyName));
            }
        }

		/// <summary>
		/// Shared Access Token
		/// </summary>
		[AttributeLogicalName("sastoken")]
        public string SASToken
        {
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(SASToken));
                SetAttributeValue("sastoken", value);
                OnPropertyChanged(nameof(SASToken));
            }
        }

		/// <summary>
		/// Specifies schema type for event grid events
		/// </summary>
		[AttributeLogicalName("schematype")]
        public OptionSetValue SchemaType
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("schematype");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(SchemaType));
                SetAttributeValue("schematype", value);
                OnPropertyChanged(nameof(SchemaType));
            }
        }

		/// <summary>
		/// Unique identifier of the service endpoint.
		/// </summary>
		[AttributeLogicalName("serviceendpointidunique")]
        public Guid? ServiceEndpointIdUnique
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("serviceendpointidunique");
            }
        }

		/// <summary>
		/// Unique identifier of the associated solution.
		/// </summary>
		[AttributeLogicalName("solutionid")]
        public Guid? SolutionId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("solutionid");
            }
        }

		/// <summary>
		/// Namespace of the App Fabric solution.
		/// </summary>
		[AttributeLogicalName("solutionnamespace")]
        public string SolutionNamespace
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("solutionnamespace");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(SolutionNamespace));
                SetAttributeValue("solutionnamespace", value);
                OnPropertyChanged(nameof(SolutionNamespace));
            }
        }

		/// <summary>
		/// Full service endpoint Url.
		/// </summary>
		[AttributeLogicalName("url")]
        public string Url
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("url");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(Url));
                SetAttributeValue("url", value);
                OnPropertyChanged(nameof(Url));
            }
        }

		/// <summary>
		/// Use Auth Information in KeyVault
		/// </summary>
		[AttributeLogicalName("usekeyvaultconfiguration")]
        public bool? UseKeyVaultConfiguration
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<bool?>("usekeyvaultconfiguration");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(UseKeyVaultConfiguration));
                SetAttributeValue("usekeyvaultconfiguration", value);
                OnPropertyChanged(nameof(UseKeyVaultConfiguration));
            }
        }

		/// <summary>
		/// Additional user claim value type.
		/// </summary>
		[AttributeLogicalName("userclaim")]
        public OptionSetValue UserClaim
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("userclaim");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(UserClaim));
                SetAttributeValue("userclaim", value);
                OnPropertyChanged(nameof(UserClaim));
            }
        }


		#endregion

		#region NavigationProperties
		/// <summary>
		/// 1:N serviceendpoint_sdkmessageprocessingstep
		/// </summary>	
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("serviceendpoint_sdkmessageprocessingstep")]
		public System.Collections.Generic.IEnumerable<SdkMessageProcessingStep> ServiceendpointSdkmessageprocessingstep
		{
			[DebuggerNonUserCode]
			get
			{
				return this.GetRelatedEntities<SdkMessageProcessingStep>("serviceendpoint_sdkmessageprocessingstep", null);
			}
			[DebuggerNonUserCode]
			set
			{
				this.OnPropertyChanging("ServiceendpointSdkmessageprocessingstep");
				this.SetRelatedEntities<SdkMessageProcessingStep>("serviceendpoint_sdkmessageprocessingstep", null, value);
				this.OnPropertyChanged("ServiceendpointSdkmessageprocessingstep");
			}
		}

		#endregion

		#region Options
		public static class Options
		{
			    public struct AuthType
                {
					public const int ACS = 1;
					public const int SASKey = 2;
					public const int SASToken = 3;
					public const int WebhookKey = 4;
					public const int HttpHeader = 5;
					public const int HttpQueryString = 6;
					public const int ConnectionString = 7;
					public const int AccessKey = 8;
                }
			    public struct ComponentState
                {
					public const int Published = 0;
					public const int Unpublished = 1;
					public const int Deleted = 2;
					public const int DeletedUnpublished = 3;
                }
			    public struct ConnectionMode
                {
					public const int Normal = 1;
					public const int Federated = 2;
                }
			    public struct Contract
                {
					public const int OneWay = 1;
					public const int Queue = 2;
					public const int Rest = 3;
					public const int TwoWay = 4;
					public const int Topic = 5;
					public const int Queue_Persistent_ = 6;
					public const int EventHub = 7;
					public const int Webhook = 8;
					public const int EventGrid = 9;
					public const int ManagedDataLake = 10;
                }
                public struct IsAuthValueSet
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct IsManaged
                {
                    public const bool Unmanaged = false;
                    public const bool Managed = true;
                }
                public struct IsSASKeySet
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
                public struct IsSASTokenSet
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
			    public struct MessageCharset
                {
					public const int Default = 0;
					public const int UTF8 = 1;
                }
			    public struct MessageFormat
                {
					public const int BinaryXML = 1;
					public const int Json = 2;
					public const int TextXML = 3;
                }
			    public struct NamespaceFormat
                {
					public const int NamespaceName = 1;
					public const int NamespaceAddress = 2;
                }
			    public struct SchemaType
                {
					public const int EventGrid = 1;
					public const int CloudEvents = 2;
                }
                public struct UseKeyVaultConfiguration
                {
                    public const bool No = false;
                    public const bool Yes = true;
                }
			    public struct UserClaim
                {
					public const int None = 1;
					public const int UserId = 2;
					public const int UserInfo = 3;
                }
		}
		#endregion

		#region LogicalNames
		public static class LogicalNames
		{
				public const string ServiceEndpointId = "serviceendpointid";
				public const string AuthType = "authtype";
				public const string AuthValue = "authvalue";
				public const string ComponentState = "componentstate";
				public const string ConnectionMode = "connectionmode";
				public const string Contract = "contract";
				public const string CreatedBy = "createdby";
				public const string CreatedOn = "createdon";
				public const string CreatedOnBehalfBy = "createdonbehalfby";
				public const string Description = "description";
				public const string IntroducedVersion = "introducedversion";
				public const string IsAuthValueSet = "isauthvalueset";
				public const string IsCustomizable = "iscustomizable";
				public const string IsManaged = "ismanaged";
				public const string IsSASKeySet = "issaskeyset";
				public const string IsSASTokenSet = "issastokenset";
				public const string KeyVaultReferenceId = "keyvaultreferenceid";
				public const string MessageCharset = "messagecharset";
				public const string MessageFormat = "messageformat";
				public const string ModifiedBy = "modifiedby";
				public const string ModifiedOn = "modifiedon";
				public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
				public const string Name = "name";
				public const string NamespaceAddress = "namespaceaddress";
				public const string NamespaceFormat = "namespaceformat";
				public const string OrganizationId = "organizationid";
				public const string OverwriteTime = "overwritetime";
				public const string Path = "path";
				public const string RuntimeIntegrationProperties = "runtimeintegrationproperties";
				public const string SASKey = "saskey";
				public const string SASKeyName = "saskeyname";
				public const string SASToken = "sastoken";
				public const string SchemaType = "schematype";
				public const string ServiceEndpointIdUnique = "serviceendpointidunique";
				public const string SolutionId = "solutionid";
				public const string SolutionNamespace = "solutionnamespace";
				public const string Url = "url";
				public const string UseKeyVaultConfiguration = "usekeyvaultconfiguration";
				public const string UserClaim = "userclaim";
		}
		#endregion

		#region Relations
        public static class Relations
        {
            public static class OneToMany
            {
				public const string ServiceendpointSdkmessageprocessingstep = "serviceendpoint_sdkmessageprocessingstep";
				public const string UserentityinstancedataServiceendpoint = "userentityinstancedata_serviceendpoint";
            }

            public static class ManyToOne
            {
				public const string CreatedbyServiceendpoint = "createdby_serviceendpoint";
				public const string KeyvaultreferenceServiceEndpoint = "keyvaultreference_ServiceEndpoint";
				public const string LkServiceendpointbaseCreatedonbehalfby = "lk_serviceendpointbase_createdonbehalfby";
				public const string LkServiceendpointbaseModifiedonbehalfby = "lk_serviceendpointbase_modifiedonbehalfby";
				public const string ModifiedbyServiceendpoint = "modifiedby_serviceendpoint";
				public const string OrganizationServiceendpoint = "organization_serviceendpoint";
            }

            public static class ManyToMany
            {
            }
        }

        #endregion

		#region Methods
        public EntityReference ToNamedEntityReference()
        {
            var reference = ToEntityReference();
            reference.Name = GetAttributeValue<string>(PrimaryNameAttribute);

            return reference;
        }
        public static ServiceEndpoint Retrieve(IOrganizationService service, Guid id)
        {
            return Retrieve(service,id, new ColumnSet(true));
        }

        public static ServiceEndpoint Retrieve(IOrganizationService service, Guid id, ColumnSet columnSet)
        {
            return service.Retrieve("serviceendpoint", id, columnSet).ToEntity<ServiceEndpoint>();
        }

        public ServiceEndpoint GetChangedEntity()
        {
            if (_trackChanges)
            {
                var attr = new AttributeCollection();
                foreach (var attrName in _changedProperties.Value.Select(changedProperty => ((AttributeLogicalNameAttribute) GetType().GetProperty(changedProperty).GetCustomAttribute(typeof (AttributeLogicalNameAttribute))).LogicalName).Where(attrName => Contains(attrName)))
                {
                    attr.Add(attrName,this[attrName]);
                }
                return new  ServiceEndpoint(Id) {Attributes = attr };
            }
            return this;
        }
        #endregion
	}

	#region Context
	public partial class DataContext
	{
		public IQueryable<ServiceEndpoint> ServiceEndpointSet
		{
			get
			{
				return CreateQuery<ServiceEndpoint>();
			}
		}
	}
	#endregion
}
