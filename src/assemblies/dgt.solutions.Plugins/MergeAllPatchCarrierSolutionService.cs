using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Contract;
using dgt.solutions.Plugins.Helper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins
{
    [CustomApiRegistration(SdkMessageNames.DgtMergeAllPatchCarrierSolution)]
    public class MergeAllPatchCarrierSolutionService : Executor
    {
        private const string SolutionConceptPatchPattern = "SolutionConcept.PatchPattern";

        public MergeAllPatchCarrierSolutionService(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        public override string GetConfig(string key, int lcid = 1033, string defaultValue = null)
        {
            switch (key)
            {
                case SolutionConceptPatchPattern:
                    //null for default or { "PatchPattern": "[major|minor]" }
                    return string.IsNullOrWhiteSpace(SecureConfig) ? "minor" : SerializerService.JsonDeserialize<MergeAllPatchCarrierSolutionConfig>(SecureConfig).PatchPattern?.ToLowerInvariant() == "major" ? "major" : "minor";
                default:
                    return defaultValue;
            }
        }

        protected override ExecutionResult Execute()
        {
            var lookup = new SolutionLookup(this);
            //read carrier info
            var carrier = ElevatedOrganizationService.Retrieve(DgtCarrier.EntityLogicalName, Delegate.PluginExecutionContext.PrimaryEntityId, new ColumnSet(true)).ToEntity<DgtCarrier>();
            //retrieve solution behind carrier
            var solution = lookup.GetByName(carrier.DgtSolutionuniquename);
            //check solution type
            var isFullSolution = solution.ParentSolutionId == null;
            //parse current solution version 
            var version = Version.Parse(solution.Version);
            if (!isFullSolution)
            {
                //switch to root solution
                solution = ElevatedOrganizationService.Retrieve(Solution.EntityLogicalName, solution.ParentSolutionId.Id, new ColumnSet(true)).ToEntity<Solution>();
            }
            //get current carrier order
            var order = carrier.DgtTransportOrderNo.GetValueOrDefault(1);
            //get all patches
            var patches = lookup.GetPatchesByParent(solution.Id).OrderByDescending(s => Version.Parse(s.Version)).ToList();
            var carriers = new List<DgtCarrier>
            {
                carrier
            };
            if (patches.Any())
            {
                var uniqueNames = patches.Select(s => s.UniqueName).ToList();
                var query = new QueryExpression
                {
                    EntityName = DgtCarrier.EntityLogicalName,
                    ColumnSet = new ColumnSet(true),
                    Criteria = {
                        Conditions = {
                            new ConditionExpression(DgtCarrier.LogicalNames.DgtSolutionuniquename, ConditionOperator.In, uniqueNames),
                            new ConditionExpression(DgtCarrier.LogicalNames.Statecode, ConditionOperator.Equal, DgtCarrier.Options.Statecode.Active)
                        }
                    }
                };
                carriers = ElevatedOrganizationService.RetrieveMultiple(query).Entities.Select(s => s.ToEntity<DgtCarrier>()).OrderByDescending(s => Version.Parse(s.DgtSolutionversion)).ToList();
                if (carriers.Any())
                {
                    //reset carrier order
                    order = carriers.Last().DgtTransportOrderNo.GetValueOrDefault(1);
                }
            }
            //get update pattern
            var pattern = GetConfig(SolutionConceptPatchPattern);
            switch (pattern)
            {
                //increment revision
                case "major":
                    version = new Version(version.Major + 1, 0, 0, 0);
                    break;
                //increment build; set revision to 0
                default:
                    version = new Version(version.Major, version.Minor + 1, 0, 0);
                    break;
            }
            //clone as solution request
            var request = new DgtCloneASolutionRequest
            {
                Target = carrier.ToEntityReference(),
                CloneASolutionInUniqueName = solution.UniqueName,
                CloneASolutionInFriendlyName = solution.FriendlyName,
                CloneASolutionInVersion = version.ToString()
            };
            var executeMultipleRequest = new ExecuteMultipleRequest
            {
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };
            executeMultipleRequest.Requests.Add(request);
            //clone as patch execute and response
            //var response = (Ec4uCloneASolutionResponse)ElevatedOrganizationService.Execute(request);
            var executeMultipleResponse = (ExecuteMultipleResponse)ElevatedOrganizationService.Execute(executeMultipleRequest);
            if (executeMultipleResponse.IsFaulted)
            {
                throw new InvalidPluginExecutionException(executeMultipleResponse.Responses[0].Fault.TraceText);
            }
            var response = (DgtCloneASolutionResponse)executeMultipleResponse.Responses[0].Response;
            //retrieve solution
            var full = ElevatedOrganizationService.Retrieve(Solution.EntityLogicalName, response.CloneASolutionOutSolutionId, new ColumnSet(true)).ToEntity<Solution>();
            //create a new carrier
            var carrierId = SecuredOrganizationService.Create(new DgtCarrier
            {
                DgtTransportOrderNo = order,
                DgtReference = full.FriendlyName,
                DgtSolutionfriendlyname = full.FriendlyName,
                DgtSolutionuniquename = full.UniqueName,
                DgtSolutionversion = full.Version,
                DgtSolutionid = full.Id.ToString("D")
            });
            //switch all workbenches to patch
            using (var context = DataContext(ElevatedOrganizationService))
            {
                // ReSharper disable once PossibleInvalidOperationException
                var carrierIds = carriers.Select(s => s.DgtCarrierId.Value).ToList();
                var workbenches = (from rec in context.DgtWorkbenchSet
                                   where rec.DgtTargetCarrierId != null
                                   where rec.Statecode != null
                                   where rec.Statecode.Value == DgtWorkbench.Options.Statecode.Active
                                   select rec).ToList();
                foreach (var workbench in workbenches.Where(workbench => carrierIds.Contains(workbench.DgtTargetCarrierId.Id)))
                {
                    ElevatedOrganizationService.Update(new DgtWorkbench(workbench.Id)
                    {
                        DgtTargetCarrierId = new EntityReference(DgtCarrier.EntityLogicalName, carrierId)
                    });
                }
            }
            //disable carriers
            foreach (var c in carriers)
            {
                //disable carrier
                SecuredOrganizationService.Update(new DgtCarrier(c.Id)
                {
                    Statecode = new OptionSetValue(DgtCarrier.Options.Statecode.Inactive),
                    Statuscode = new OptionSetValue(DgtCarrier.Options.Statuscode.Inactive)
                });
            }
            return ExecutionResult.Ok;
        }
    }
}
