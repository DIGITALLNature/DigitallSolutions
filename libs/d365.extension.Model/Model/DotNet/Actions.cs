using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

// ReSharper disable All
namespace D365.Extension.Model
{
	[RequestProxy("dgt_clone_a_solution")]
	public class DgtCloneASolutionRequest : OrganizationRequest
	{
		public DgtCloneASolutionRequest()
		{
			RequestName = "dgt_clone_a_solution";
		}

		#region consts
		public const string RequestLogicalName = "dgt_clone_a_solution";
		#endregion

		#region InParameters
		public struct InParameters
		{
			public const string Target = "Target";
			public const string CloneASolutionInFriendlyName = "FriendlyName";
			public const string CloneASolutionInUniqueName = "UniqueName";
			public const string CloneASolutionInVersion = "Version";
		}
		#endregion

		public EntityReference Target
		{
			get
			{
				if(base.Parameters.Contains("Target"))
				{
					return (EntityReference)base.Parameters["Target"];
				}
				return default(EntityReference);
			}
			set
			{
				base.Parameters["Target"] = value;
			}
		}

		public string CloneASolutionInFriendlyName
		{
			get
			{
				if(base.Parameters.Contains("FriendlyName"))
				{
					return (string)base.Parameters["FriendlyName"];
				}
				return default(string);
			}
			set
			{
				base.Parameters["FriendlyName"] = value;
			}
		}

		public string CloneASolutionInUniqueName
		{
			get
			{
				if(base.Parameters.Contains("UniqueName"))
				{
					return (string)base.Parameters["UniqueName"];
				}
				return default(string);
			}
			set
			{
				base.Parameters["UniqueName"] = value;
			}
		}

		public string CloneASolutionInVersion
		{
			get
			{
				if(base.Parameters.Contains("Version"))
				{
					return (string)base.Parameters["Version"];
				}
				return default(string);
			}
			set
			{
				base.Parameters["Version"] = value;
			}
		}

	}

	[ResponseProxy("dgt_clone_a_solution")]
	public class DgtCloneASolutionResponse : OrganizationResponse
	{
		#region OutParameters
		public struct OutParameters
		{
			public const string CloneASolutionOutSolutionId = "SolutionId";
		}
		#endregion

		public Guid CloneASolutionOutSolutionId
		{
			get
			{
				if(base.Results.Contains("SolutionId"))
				{
					return (Guid)base.Results["SolutionId"];
				}
				return default(Guid);
			}
		}

	}

	[RequestProxy("dgt_prevent_flows")]
	public class DgtPreventFlowsRequest : OrganizationRequest
	{
		public DgtPreventFlowsRequest()
		{
			RequestName = "dgt_prevent_flows";
		}

		#region consts
		public const string RequestLogicalName = "dgt_prevent_flows";
		#endregion

		#region InParameters
		public struct InParameters
		{
			public const string Target = "Target";
			public const string ConstraintWorkbenchHistory_PreventFlows = "WorkbenchHistory";
		}
		#endregion

		public EntityReference Target
		{
			get
			{
				if(base.Parameters.Contains("Target"))
				{
					return (EntityReference)base.Parameters["Target"];
				}
				return default(EntityReference);
			}
			set
			{
				base.Parameters["Target"] = value;
			}
		}

		public EntityReference ConstraintWorkbenchHistory_PreventFlows
		{
			get
			{
				if(base.Parameters.Contains("WorkbenchHistory"))
				{
					return (EntityReference)base.Parameters["WorkbenchHistory"];
				}
				return default(EntityReference);
			}
			set
			{
				base.Parameters["WorkbenchHistory"] = value;
			}
		}

	}

	[ResponseProxy("dgt_prevent_flows")]
	public class DgtPreventFlowsResponse : OrganizationResponse
	{
		#region OutParameters
		public struct OutParameters
		{
			public const string ConstraintSuccess_PreventFlows = "Success";
		}
		#endregion

		public bool ConstraintSuccess_PreventFlows
		{
			get
			{
				if(base.Results.Contains("Success"))
				{
					return (bool)base.Results["Success"];
				}
				return default(bool);
			}
		}

	}

	[RequestProxy("dgt_prevent_items_without_active_layer")]
	public class DgtPreventItemsWithoutActiveLayerRequest : OrganizationRequest
	{
		public DgtPreventItemsWithoutActiveLayerRequest()
		{
			RequestName = "dgt_prevent_items_without_active_layer";
		}

		#region consts
		public const string RequestLogicalName = "dgt_prevent_items_without_active_layer";
		#endregion

		#region InParameters
		public struct InParameters
		{
			public const string Target = "Target";
			public const string ConstraintWorkbenchHistory_PreventItemsWithoutActiveLayer = "WorkbenchHistory";
		}
		#endregion

		public EntityReference Target
		{
			get
			{
				if(base.Parameters.Contains("Target"))
				{
					return (EntityReference)base.Parameters["Target"];
				}
				return default(EntityReference);
			}
			set
			{
				base.Parameters["Target"] = value;
			}
		}

		public EntityReference ConstraintWorkbenchHistory_PreventItemsWithoutActiveLayer
		{
			get
			{
				if(base.Parameters.Contains("WorkbenchHistory"))
				{
					return (EntityReference)base.Parameters["WorkbenchHistory"];
				}
				return default(EntityReference);
			}
			set
			{
				base.Parameters["WorkbenchHistory"] = value;
			}
		}

	}

	[ResponseProxy("dgt_prevent_items_without_active_layer")]
	public class DgtPreventItemsWithoutActiveLayerResponse : OrganizationResponse
	{
		#region OutParameters
		public struct OutParameters
		{
			public const string ConstraintSuccess_PreventItemsWithoutActiveLayer = "Success";
		}
		#endregion

		public bool ConstraintSuccess_PreventItemsWithoutActiveLayer
		{
			get
			{
				if(base.Results.Contains("Success"))
				{
					return (bool)base.Results["Success"];
				}
				return default(bool);
			}
		}

	}

	[RequestProxy("dgt_prevent_managed_tables_with_all_assets")]
	public class DgtPreventManagedTablesWithAllAssetsRequest : OrganizationRequest
	{
		public DgtPreventManagedTablesWithAllAssetsRequest()
		{
			RequestName = "dgt_prevent_managed_tables_with_all_assets";
		}

		#region consts
		public const string RequestLogicalName = "dgt_prevent_managed_tables_with_all_assets";
		#endregion

		#region InParameters
		public struct InParameters
		{
			public const string Target = "Target";
			public const string ConstraintWorkbenchHistory_PreventManagedTablesWithAllAssets = "WorkbenchHistory";
		}
		#endregion

		public EntityReference Target
		{
			get
			{
				if(base.Parameters.Contains("Target"))
				{
					return (EntityReference)base.Parameters["Target"];
				}
				return default(EntityReference);
			}
			set
			{
				base.Parameters["Target"] = value;
			}
		}

		public EntityReference ConstraintWorkbenchHistory_PreventManagedTablesWithAllAssets
		{
			get
			{
				if(base.Parameters.Contains("WorkbenchHistory"))
				{
					return (EntityReference)base.Parameters["WorkbenchHistory"];
				}
				return default(EntityReference);
			}
			set
			{
				base.Parameters["WorkbenchHistory"] = value;
			}
		}

	}

	[ResponseProxy("dgt_prevent_managed_tables_with_all_assets")]
	public class DgtPreventManagedTablesWithAllAssetsResponse : OrganizationResponse
	{
		#region OutParameters
		public struct OutParameters
		{
			public const string ConstraintSuccess_PreventManagedTablesWithAllAssets = "Success";
		}
		#endregion

		public bool ConstraintSuccess_PreventManagedTablesWithAllAssets
		{
			get
			{
				if(base.Results.Contains("Success"))
				{
					return (bool)base.Results["Success"];
				}
				return default(bool);
			}
		}

	}

	[RequestProxy("dgt_prevent_plugin_assemblies")]
	public class DgtPreventPluginAssembliesRequest : OrganizationRequest
	{
		public DgtPreventPluginAssembliesRequest()
		{
			RequestName = "dgt_prevent_plugin_assemblies";
		}

		#region consts
		public const string RequestLogicalName = "dgt_prevent_plugin_assemblies";
		#endregion

		#region InParameters
		public struct InParameters
		{
			public const string Target = "Target";
			public const string ConstraintWorkbenchHistory_PreventPluginAssemblies = "WorkbenchHistory";
		}
		#endregion

		public EntityReference Target
		{
			get
			{
				if(base.Parameters.Contains("Target"))
				{
					return (EntityReference)base.Parameters["Target"];
				}
				return default(EntityReference);
			}
			set
			{
				base.Parameters["Target"] = value;
			}
		}

		public EntityReference ConstraintWorkbenchHistory_PreventPluginAssemblies
		{
			get
			{
				if(base.Parameters.Contains("WorkbenchHistory"))
				{
					return (EntityReference)base.Parameters["WorkbenchHistory"];
				}
				return default(EntityReference);
			}
			set
			{
				base.Parameters["WorkbenchHistory"] = value;
			}
		}

	}

	[ResponseProxy("dgt_prevent_plugin_assemblies")]
	public class DgtPreventPluginAssembliesResponse : OrganizationResponse
	{
		#region OutParameters
		public struct OutParameters
		{
			public const string ConstraintSuccess_PreventPluginAssemblies = "Success";
		}
		#endregion

		public bool ConstraintSuccess_PreventPluginAssemblies
		{
			get
			{
				if(base.Results.Contains("Success"))
				{
					return (bool)base.Results["Success"];
				}
				return default(bool);
			}
		}

	}

	[RequestProxy("dgt_run_carrier_constraints_check")]
	public class DgtRunCarrierConstraintsCheckRequest : OrganizationRequest
	{
		public DgtRunCarrierConstraintsCheckRequest()
		{
			RequestName = "dgt_run_carrier_constraints_check";
		}

		#region consts
		public const string RequestLogicalName = "dgt_run_carrier_constraints_check";
		#endregion

		#region InParameters
		public struct InParameters
		{
			public const string Target = "Target";
			public const string Workbench = "Workbench";
			public const string ConstraintWorkbenchHistory_RunCarrierConstraintsCheck = "WorkbenchHistory";
		}
		#endregion

		public EntityReference Target
		{
			get
			{
				if(base.Parameters.Contains("Target"))
				{
					return (EntityReference)base.Parameters["Target"];
				}
				return default(EntityReference);
			}
			set
			{
				base.Parameters["Target"] = value;
			}
		}

		public EntityReference Workbench
		{
			get
			{
				if(base.Parameters.Contains("Workbench"))
				{
					return (EntityReference)base.Parameters["Workbench"];
				}
				return default(EntityReference);
			}
			set
			{
				base.Parameters["Workbench"] = value;
			}
		}

		public EntityReference ConstraintWorkbenchHistory_RunCarrierConstraintsCheck
		{
			get
			{
				if(base.Parameters.Contains("WorkbenchHistory"))
				{
					return (EntityReference)base.Parameters["WorkbenchHistory"];
				}
				return default(EntityReference);
			}
			set
			{
				base.Parameters["WorkbenchHistory"] = value;
			}
		}

	}

	[ResponseProxy("dgt_run_carrier_constraints_check")]
	public class DgtRunCarrierConstraintsCheckResponse : OrganizationResponse
	{
		#region OutParameters
		public struct OutParameters
		{
			public const string CarrierConstraintsSuccessStatus = "Success";
		}
		#endregion

		public bool CarrierConstraintsSuccessStatus
		{
			get
			{
				if(base.Results.Contains("Success"))
				{
					return (bool)base.Results["Success"];
				}
				return default(bool);
			}
		}

	}

}
