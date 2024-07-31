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
	
	[DataContractAttribute()]
	[EntityLogicalNameAttribute("dgt_workbench_history_log")]
	[System.CodeDom.Compiler.GeneratedCode("dgtp", "2023")]
    [ExcludeFromCodeCoverage]
	public partial class DgtWorkbenchHistoryLog : Entity, INotifyPropertyChanging, INotifyPropertyChanged
    {
	    #region ctor
		[DebuggerNonUserCode]
		public DgtWorkbenchHistoryLog() : this(false)
        {
        }

        [DebuggerNonUserCode]
		public DgtWorkbenchHistoryLog(bool trackChanges = false) : base(EntityLogicalName)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public DgtWorkbenchHistoryLog(Guid id, bool trackChanges = false) : base(EntityLogicalName,id)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public DgtWorkbenchHistoryLog(KeyAttributeCollection keyAttributes, bool trackChanges = false) : base(EntityLogicalName,keyAttributes)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public DgtWorkbenchHistoryLog(string keyName, object keyValue, bool trackChanges = false) : base(EntityLogicalName, keyName, keyValue)
        {
			_trackChanges = trackChanges;
        }
        #endregion

		#region fields
        private readonly bool _trackChanges;
        private readonly Lazy<HashSet<string>> _changedProperties = new Lazy<HashSet<string>>();
        #endregion

        #region consts
        public const string EntityLogicalName = "dgt_workbench_history_log";
        public const string PrimaryNameAttribute = "dgt_message";
        public const int EntityTypeCode = 10524;
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
		[AttributeLogicalNameAttribute("dgt_workbench_history_logid")]
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
				DgtWorkbenchHistoryLogId = value;
			}
		}

		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[AttributeLogicalName("dgt_workbench_history_logid")]
        public Guid? DgtWorkbenchHistoryLogId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("dgt_workbench_history_logid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DgtWorkbenchHistoryLogId));
                SetAttributeValue("dgt_workbench_history_logid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
                OnPropertyChanged(nameof(DgtWorkbenchHistoryLogId));
            }
        }

		/// <summary>
		/// Unique identifier of the user who created the record.
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
		/// Date and time when the record was created.
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
		/// Unique identifier of the delegate user who created the record.
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

		
		[AttributeLogicalName("dgt_component_type")]
        public string DgtComponentType
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("dgt_component_type");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DgtComponentType));
                SetAttributeValue("dgt_component_type", value);
                OnPropertyChanged(nameof(DgtComponentType));
            }
        }

		
		[AttributeLogicalName("dgt_log_level_set")]
        public OptionSetValue DgtLogLevelSet
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("dgt_log_level_set");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DgtLogLevelSet));
                SetAttributeValue("dgt_log_level_set", value);
                OnPropertyChanged(nameof(DgtLogLevelSet));
            }
        }

		
		[AttributeLogicalName("dgt_message")]
        public string DgtMessage
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("dgt_message");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DgtMessage));
                SetAttributeValue("dgt_message", value);
                OnPropertyChanged(nameof(DgtMessage));
            }
        }

		
		[AttributeLogicalName("dgt_objectid_txt")]
        public string DgtObjectidTxt
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("dgt_objectid_txt");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DgtObjectidTxt));
                SetAttributeValue("dgt_objectid_txt", value);
                OnPropertyChanged(nameof(DgtObjectidTxt));
            }
        }

		
		[AttributeLogicalName("dgt_subtype_txt")]
        public string DgtSubtypeTxt
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("dgt_subtype_txt");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DgtSubtypeTxt));
                SetAttributeValue("dgt_subtype_txt", value);
                OnPropertyChanged(nameof(DgtSubtypeTxt));
            }
        }

		
		[AttributeLogicalName("dgt_type_set")]
        public OptionSetValue DgtTypeSet
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<OptionSetValue>("dgt_type_set");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DgtTypeSet));
                SetAttributeValue("dgt_type_set", value);
                OnPropertyChanged(nameof(DgtTypeSet));
            }
        }

		
		[AttributeLogicalName("dgt_workbench_history_id")]
        public EntityReference DgtWorkbenchHistoryId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("dgt_workbench_history_id");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(DgtWorkbenchHistoryId));
                SetAttributeValue("dgt_workbench_history_id", value);
                OnPropertyChanged(nameof(DgtWorkbenchHistoryId));
            }
        }

		/// <summary>
		/// Sequence number of the import that created this record.
		/// </summary>
		[AttributeLogicalName("importsequencenumber")]
        public int? ImportSequenceNumber
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("importsequencenumber");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(ImportSequenceNumber));
                SetAttributeValue("importsequencenumber", value);
                OnPropertyChanged(nameof(ImportSequenceNumber));
            }
        }

		/// <summary>
		/// Unique identifier of the user who modified the record.
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
		/// Date and time when the record was modified.
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
		/// Unique identifier of the delegate user who modified the record.
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
		/// Date and time that the record was migrated.
		/// </summary>
		[AttributeLogicalName("overriddencreatedon")]
        public DateTime? OverriddenCreatedOn
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<DateTime?>("overriddencreatedon");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(OverriddenCreatedOn));
                SetAttributeValue("overriddencreatedon", value);
                OnPropertyChanged(nameof(OverriddenCreatedOn));
            }
        }

		/// <summary>
		/// Owner Id
		/// </summary>
		[AttributeLogicalName("ownerid")]
        public EntityReference OwnerId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("ownerid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(OwnerId));
                SetAttributeValue("ownerid", value);
                OnPropertyChanged(nameof(OwnerId));
            }
        }

		/// <summary>
		/// Unique identifier for the business unit that owns the record
		/// </summary>
		[AttributeLogicalName("owningbusinessunit")]
        public EntityReference OwningBusinessUnit
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("owningbusinessunit");
            }
        }

		/// <summary>
		/// Unique identifier for the team that owns the record.
		/// </summary>
		[AttributeLogicalName("owningteam")]
        public EntityReference OwningTeam
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("owningteam");
            }
        }

		/// <summary>
		/// Unique identifier for the user that owns the record.
		/// </summary>
		[AttributeLogicalName("owninguser")]
        public EntityReference OwningUser
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<EntityReference>("owninguser");
            }
        }

		/// <summary>
		/// Logical partition id. A logical partition consists of a set of records with same partition id.
		/// </summary>
		[AttributeLogicalName("partitionid")]
        public string PartitionId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<string>("partitionid");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(PartitionId));
                SetAttributeValue("partitionid", value);
                OnPropertyChanged(nameof(PartitionId));
            }
        }

		/// <summary>
		/// Time to live in seconds.
		/// </summary>
		[AttributeLogicalName("ttlinseconds")]
        public int? TTLInSeconds
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<int?>("ttlinseconds");
            }
            [DebuggerNonUserCode]
			set
            {
                OnPropertyChanging(nameof(TTLInSeconds));
                SetAttributeValue("ttlinseconds", value);
                OnPropertyChanged(nameof(TTLInSeconds));
            }
        }

		/// <summary>
		/// Version Number
		/// </summary>
		[AttributeLogicalName("versionnumber")]
        public long? VersionNumber
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<long?>("versionnumber");
            }
        }


		#endregion

		#region NavigationProperties
		#endregion

		#region Options
		public static class Options
		{
			    public struct DgtLogLevelSet
                {
					public const int Error = 283510000;
					public const int Warning = 283510001;
					public const int Information = 283510002;
					public const int Debug = 283510003;
                }
			    public struct DgtTypeSet
                {
					public const int ComponentMove = 283510000;
					public const int Constraint = 283510001;
					public const int Log = 283510002;
                }
		}
		#endregion

		#region LogicalNames
		public static class LogicalNames
		{
				public const string DgtWorkbenchHistoryLogId = "dgt_workbench_history_logid";
				public const string CreatedBy = "createdby";
				public const string CreatedOn = "createdon";
				public const string CreatedOnBehalfBy = "createdonbehalfby";
				public const string DgtComponentType = "dgt_component_type";
				public const string DgtLogLevelSet = "dgt_log_level_set";
				public const string DgtMessage = "dgt_message";
				public const string DgtObjectidTxt = "dgt_objectid_txt";
				public const string DgtSubtypeTxt = "dgt_subtype_txt";
				public const string DgtTypeSet = "dgt_type_set";
				public const string DgtWorkbenchHistoryId = "dgt_workbench_history_id";
				public const string ImportSequenceNumber = "importsequencenumber";
				public const string ModifiedBy = "modifiedby";
				public const string ModifiedOn = "modifiedon";
				public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
				public const string OverriddenCreatedOn = "overriddencreatedon";
				public const string OwnerId = "ownerid";
				public const string OwningBusinessUnit = "owningbusinessunit";
				public const string OwningTeam = "owningteam";
				public const string OwningUser = "owninguser";
				public const string PartitionId = "partitionid";
				public const string TTLInSeconds = "ttlinseconds";
				public const string VersionNumber = "versionnumber";
		}
		#endregion

		#region AlternateKeys
		public static class AlternateKeys
		{
				public const string EntityKeyForNoSqlEntityThatContainsPrimaryKeyAndPartitionIdAttributes = "keyfornosqlentitywithpkpartitionid";
		}
		#endregion

		#region Relations
        public static class Relations
        {
            public static class OneToMany
            {
            }

            public static class ManyToOne
            {
				public const string BusinessUnitDgtWorkbenchHistoryLog = "business_unit_dgt_workbench_history_log";
				public const string DgtWorkbenchHistoryLogDgtWorkbenchHistoryId = "dgt_workbench_history_log_dgt_workbench_history_id";
				public const string LkDgtWorkbenchHistoryLogCreatedby = "lk_dgt_workbench_history_log_createdby";
				public const string LkDgtWorkbenchHistoryLogCreatedonbehalfby = "lk_dgt_workbench_history_log_createdonbehalfby";
				public const string LkDgtWorkbenchHistoryLogModifiedby = "lk_dgt_workbench_history_log_modifiedby";
				public const string LkDgtWorkbenchHistoryLogModifiedonbehalfby = "lk_dgt_workbench_history_log_modifiedonbehalfby";
				public const string OwnerDgtWorkbenchHistoryLog = "owner_dgt_workbench_history_log";
				public const string TeamDgtWorkbenchHistoryLog = "team_dgt_workbench_history_log";
				public const string UserDgtWorkbenchHistoryLog = "user_dgt_workbench_history_log";
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
        public static DgtWorkbenchHistoryLog Retrieve(IOrganizationService service, Guid id)
        {
            return Retrieve(service,id, new ColumnSet(true));
        }

        public static DgtWorkbenchHistoryLog Retrieve(IOrganizationService service, Guid id, ColumnSet columnSet)
        {
            return service.Retrieve("dgt_workbench_history_log", id, columnSet).ToEntity<DgtWorkbenchHistoryLog>();
        }

        public DgtWorkbenchHistoryLog GetChangedEntity()
        {
            if (_trackChanges)
            {
                var attr = new AttributeCollection();
                foreach (var attrName in _changedProperties.Value.Select(changedProperty => ((AttributeLogicalNameAttribute) GetType().GetProperty(changedProperty).GetCustomAttribute(typeof (AttributeLogicalNameAttribute))).LogicalName).Where(attrName => Contains(attrName)))
                {
                    attr.Add(attrName,this[attrName]);
                }
                return new  DgtWorkbenchHistoryLog(Id) {Attributes = attr };
            }
            return this;
        }
        #endregion
	}

	#region Context
	public partial class DataContext
	{
		public IQueryable<DgtWorkbenchHistoryLog> DgtWorkbenchHistoryLogSet
		{
			get
			{
				return CreateQuery<DgtWorkbenchHistoryLog>();
			}
		}
	}
	#endregion
}
