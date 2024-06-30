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

	}

	[ResponseProxy("dgt_prevent_flows")]
	public class DgtPreventFlowsResponse : OrganizationResponse
	{
		#region OutParameters
		public struct OutParameters
		{
			public const string ConstraintLog_PreventFlows = "Log";
		}
		#endregion

		public string ConstraintLog_PreventFlows
		{
			get
			{
				if(base.Results.Contains("Log"))
				{
					return (string)base.Results["Log"];
				}
				return default(string);
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

	}

	[ResponseProxy("dgt_run_carrier_constraints_check")]
	public class DgtRunCarrierConstraintsCheckResponse : OrganizationResponse
	{
		#region OutParameters
		public struct OutParameters
		{
			public const string CarrierConstraintsLog = "Log";
			public const string CarrierConstraintsSuccessStatus = "Success";
		}
		#endregion

		public string CarrierConstraintsLog
		{
			get
			{
				if(base.Results.Contains("Log"))
				{
					return (string)base.Results["Log"];
				}
				return default(string);
			}
		}

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
