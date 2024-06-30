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
	[EntityLogicalNameAttribute("dgt_carrier_constraint_dgt_carrier")]
	[System.CodeDom.Compiler.GeneratedCode("dgtp", "2023")]
    [ExcludeFromCodeCoverage]
	public partial class DgtCarrierConstraintDgtCarrier : Entity, INotifyPropertyChanging, INotifyPropertyChanged
    {
	    #region ctor
		[DebuggerNonUserCode]
		public DgtCarrierConstraintDgtCarrier() : this(false)
        {
        }

        [DebuggerNonUserCode]
		public DgtCarrierConstraintDgtCarrier(bool trackChanges = false) : base(EntityLogicalName)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public DgtCarrierConstraintDgtCarrier(Guid id, bool trackChanges = false) : base(EntityLogicalName,id)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public DgtCarrierConstraintDgtCarrier(KeyAttributeCollection keyAttributes, bool trackChanges = false) : base(EntityLogicalName,keyAttributes)
        {
			_trackChanges = trackChanges;
        }

        [DebuggerNonUserCode]
		public DgtCarrierConstraintDgtCarrier(string keyName, object keyValue, bool trackChanges = false) : base(EntityLogicalName, keyName, keyValue)
        {
			_trackChanges = trackChanges;
        }
        #endregion

		#region fields
        private readonly bool _trackChanges;
        private readonly Lazy<HashSet<string>> _changedProperties = new Lazy<HashSet<string>>();
        #endregion

        #region consts
        public const string EntityLogicalName = "dgt_carrier_constraint_dgt_carrier";
        public const int EntityTypeCode = 10429;
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
		[AttributeLogicalNameAttribute("dgt_carrier_constraint_dgt_carrierid")]
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
				base.Id = value;	
			}
		}

		
		[AttributeLogicalName("dgt_carrier_constraint_dgt_carrierid")]
        public Guid? DgtCarrierConstraintDgtCarrierId
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("dgt_carrier_constraint_dgt_carrierid");
            }
        }

		
		[AttributeLogicalName("dgt_carrier_constraintid")]
        public Guid? DgtCarrierConstraintid
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("dgt_carrier_constraintid");
            }
        }

		
		[AttributeLogicalName("dgt_carrierid")]
        public Guid? DgtCarrierid
        {
            [DebuggerNonUserCode]
			get
            {
                return GetAttributeValue<Guid?>("dgt_carrierid");
            }
        }

		
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
		}
		#endregion

		#region LogicalNames
		public static class LogicalNames
		{
				public const string DgtCarrierConstraintDgtCarrierId = "dgt_carrier_constraint_dgt_carrierid";
				public const string DgtCarrierConstraintid = "dgt_carrier_constraintid";
				public const string DgtCarrierid = "dgt_carrierid";
				public const string VersionNumber = "versionnumber";
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
            }

            public static class ManyToMany
            {
				public const string DgtCarrierConstraintDgtCarrierDgtCarrier = "dgt_carrier_constraint_dgt_carrier_dgt_carrier";
            }
        }

        #endregion

		#region Methods
        public static DgtCarrierConstraintDgtCarrier Retrieve(IOrganizationService service, Guid id)
        {
            return Retrieve(service,id, new ColumnSet(true));
        }

        public static DgtCarrierConstraintDgtCarrier Retrieve(IOrganizationService service, Guid id, ColumnSet columnSet)
        {
            return service.Retrieve("dgt_carrier_constraint_dgt_carrier", id, columnSet).ToEntity<DgtCarrierConstraintDgtCarrier>();
        }

        public DgtCarrierConstraintDgtCarrier GetChangedEntity()
        {
            if (_trackChanges)
            {
                var attr = new AttributeCollection();
                foreach (var attrName in _changedProperties.Value.Select(changedProperty => ((AttributeLogicalNameAttribute) GetType().GetProperty(changedProperty).GetCustomAttribute(typeof (AttributeLogicalNameAttribute))).LogicalName).Where(attrName => Contains(attrName)))
                {
                    attr.Add(attrName,this[attrName]);
                }
                return new  DgtCarrierConstraintDgtCarrier(Id) {Attributes = attr };
            }
            return this;
        }
        #endregion
	}

	#region Context
	public partial class DataContext
	{
		public IQueryable<DgtCarrierConstraintDgtCarrier> DgtCarrierConstraintDgtCarrierSet
		{
			get
			{
				return CreateQuery<DgtCarrierConstraintDgtCarrier>();
			}
		}
	}
	#endregion
}
