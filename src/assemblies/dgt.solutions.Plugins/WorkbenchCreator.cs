using System;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins
{
    [PluginRegistration(
        PluginExecutionMode.Synchronous,
        SdkMessageNames.Create,
        PluginExecutionStage.PreOperation,
        PrimaryEntityName = DgtWorkbench.EntityLogicalName
    )]
    public class WorkbenchCreator : Executor
    {
        private const string SolutionConceptPublisher = "dgt_solution_concept-publisher";

        protected override ExecutionResult Execute()
        {
            var workbench = Entity.ToEntity<DgtWorkbench>();

            using (var dataContext = DataContext(ElevatedOrganizationService))
            {
                var carrierId = workbench.DgtCarrierId;
                Publisher publisher;
                if (carrierId != null && string.IsNullOrWhiteSpace(SecureConfig))
                {
                    var carrier = (from rec in dataContext.DgtCarrierSet
                                   where rec.Id == carrierId.Id
                                   select rec).Single();
                    var solutionId = Guid.Parse(carrier.DgtSolutionid);
                    publisher = (from p in dataContext.PublisherSet
                                 join s in dataContext.SolutionSet on p.Id equals s.PublisherId.Id
                                 where s.PublisherId != null
                                 where s.Id == solutionId
                                 select p).Single();
                }
                else
                {
                    var uniqueName = EnvironmentVariablesService.GetConfig(SolutionConceptPublisher);
                    publisher = (from rec in dataContext.PublisherSet
                                 where rec.UniqueName == uniqueName
                                 select rec).Single();
                }
                Delegate.TracingService.Trace(publisher.UniqueName);
                if (publisher.IsReadonly ?? false)
                {
                    throw new InvalidPluginExecutionException($"The publisher {publisher.UniqueName} is read-only! Please check your setup or specify a new or different custom publisher via plugin secure config (see documentation)!");
                }
                var guid = ElevatedOrganizationService.Create(new Solution
                {
                    UniqueName = (workbench.DgtNatureSet?.Value ?? DgtWorkbench.Options.DgtNatureSet.Workbench) != DgtWorkbench.Options.DgtNatureSet.Workbench ? $"{publisher.CustomizationPrefix}_{workbench.DgtName.ToLowerInvariant().Replace(' ', '_')}" : $"{publisher.CustomizationPrefix}_{Guid.NewGuid():N}",
                    FriendlyName = $"{workbench.DgtName} - Workbench ({DateTime.UtcNow.Date:yyyy-MM-dd})",
                    PublisherId = publisher.ToEntityReference(),
                    Version = "0.0.0.1",
                    Description = "created via Digitall WorkbenchCreator"
                });
                var solution = SecuredOrganizationService.Retrieve(
                      Solution.EntityLogicalName,
                      guid, new ColumnSet(
                          Solution.LogicalNames.UniqueName,
                          Solution.LogicalNames.FriendlyName,
                          Solution.LogicalNames.Version
                      )).ToEntity<Solution>();
                workbench.DgtSolutionfriendlyname = solution.FriendlyName;
                workbench.DgtSolutionuniquename = solution.UniqueName;
                workbench.DgtSolutionversion = solution.Version;
                workbench.DgtSolutionid = guid.ToString("D");
            }
            return ExecutionResult.Ok;
        }
    }
}
