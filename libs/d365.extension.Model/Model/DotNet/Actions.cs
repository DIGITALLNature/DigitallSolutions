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

}
