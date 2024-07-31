using System;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Helper;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins
{
    [CustomApiRegistration(SdkMessageNames.DgtCreateNewPatchCarrierSolution)]
    public class CreateNewPatchCarrierSolutionService : Executor
    {
        private const string SolutionConceptPatchPattern = "dgt_solution_concept-patch_pattern";
        
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
            //get current carrier order
            var order = carrier.DgtTransportOrderNo.GetValueOrDefault(1);
            if (!isFullSolution)
            {
                //switch to root solution
                solution = ElevatedOrganizationService.Retrieve(Solution.EntityLogicalName, solution.ParentSolutionId.Id, new ColumnSet(true)).ToEntity<Solution>();
                //get all patches
                var patches = lookup.GetPatchesByParent(solution.Id).OrderByDescending(s => Version.Parse(s.Version)).ToList();
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
                    var carriers = ElevatedOrganizationService.RetrieveMultiple(query).Entities.Select(s => s.ToEntity<DgtCarrier>()).OrderByDescending(s => Version.Parse(s.DgtSolutionversion)).ToList();
                    if (carriers.Any())
                    {
                        //increment new carrier order
                        order = carriers.First().DgtTransportOrderNo.GetValueOrDefault(1) + 1;
                        //parse newest patch version 
                        version = Version.Parse(patches.First().Version);
                    }
                }
            }
            //get update pattern
            var pattern = EnvironmentVariablesService.GetConfig(SolutionConceptPatchPattern);
            switch (pattern)
            {
                //increment revision
                case "revision":
                    version = new Version(version.Major, version.Minor, version.Build, version.Revision + 1);
                    break;
                //increment build; set revision to 0
                default:
                    version = new Version(version.Major, version.Minor, version.Build + 1, 0);
                    break;
            }
            //clone as patch request
            var request = new CloneAsPatchRequest
            {
                ParentSolutionUniqueName = solution.UniqueName,
                DisplayName = $"{solution.FriendlyName} (Patch: {DateTime.UtcNow.Date:yyyy-MM-dd} - {order})",
                VersionNumber = version.ToString()
            };
            //clone as patch execute and response
            var response = (CloneAsPatchResponse)ElevatedOrganizationService.Execute(request);
            //retrieve patch solution
            var patch = ElevatedOrganizationService.Retrieve(Solution.EntityLogicalName, response.SolutionId, new ColumnSet(true)).ToEntity<Solution>();
            //create a new carrier
            var carrierId = SecuredOrganizationService.Create(new DgtCarrier
            {
                DgtTransportOrderNo = order,
                DgtReference = patch.FriendlyName,
                DgtSolutionfriendlyname = patch.FriendlyName,
                DgtSolutionuniquename = patch.UniqueName,
                DgtSolutionversion = patch.Version,
                DgtSolutionid = patch.Id.ToString("D")
            });
            if (isFullSolution)
            {
                //switch all workbenches to patch
                using (var context = DataContext(ElevatedOrganizationService))
                {
                    var workbenches = (from rec in context.DgtWorkbenchSet
                                       where rec.DgtTargetCarrierId != null
                                       where rec.DgtTargetCarrierId.Id == carrier.Id
                                       where rec.Statecode != null
                                       where rec.Statecode.Value == DgtWorkbench.Options.Statecode.Active
                                       select rec).ToList();
                    foreach (var workbench in workbenches)
                    {
                        ElevatedOrganizationService.Update(new DgtWorkbench(workbench.Id)
                        {
                            DgtTargetCarrierId = new EntityReference(DgtCarrier.EntityLogicalName, carrierId)
                        });
                    }
                }
                //disable carrier
                SecuredOrganizationService.Update(new DgtCarrier(Delegate.PluginExecutionContext.PrimaryEntityId)
                {
                    Statecode = new OptionSetValue(DgtCarrier.Options.Statecode.Inactive),
                    Statuscode = new OptionSetValue(DgtCarrier.Options.Statuscode.Inactive)
                });
            }
            return ExecutionResult.Ok;
        }
    }
}
