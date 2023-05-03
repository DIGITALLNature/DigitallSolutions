using System;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins
{
    [CustomApiRegistration(SdkMessageNames.DgtAddComponentToCarrierSolution)]
    public class AddComponentToCarrierSolutionService : Executor
    {
        protected override ExecutionResult Execute()
        {
            var carrierMissingDependency = ElevatedOrganizationService.Retrieve(DgtCarrierMissingDependency.EntityLogicalName, Delegate.PluginExecutionContext.PrimaryEntityId, new ColumnSet(true)).ToEntity<DgtCarrierMissingDependency>();
            var carrier = ElevatedOrganizationService.Retrieve(DgtCarrier.EntityLogicalName, carrierMissingDependency.DgtCarrierId.Id, new ColumnSet(DgtCarrier.LogicalNames.DgtSolutionuniquename
                )).ToEntity<DgtCarrier>();
            using (var context = DataContext(ElevatedOrganizationService))
            {
                var guid = Guid.Parse(carrierMissingDependency.DgtRequiredComponentObjectid);
                var component = (from rec in context.SolutionComponentSet
                                 where rec.ObjectId != null
                                 where rec.ObjectId == guid
                                 select rec).FirstOrDefault();

                if (component == null) return ExecutionResult.Ok;

                Delegate.TracingService.Trace($"Add {carrierMissingDependency.DgtComponent} to {carrier.DgtSolutionuniquename}");

                var recordid = AddComponent(component, carrier.DgtSolutionuniquename);
                ElevatedOrganizationService.Update(new DgtCarrierMissingDependency(Delegate.PluginExecutionContext.PrimaryEntityId)
                {
                    DgtSolutionComponentRecordid = recordid.ToString("D")
                });
            }

            return ExecutionResult.Ok;
        }

        private Guid AddComponent(SolutionComponent component, string destinationName)
        {
            var response = (AddSolutionComponentResponse)ElevatedOrganizationService.Execute(new AddSolutionComponentRequest
            {
                AddRequiredComponents = false,
                ComponentId = component.ObjectId.GetValueOrDefault(),
                ComponentType = component.ComponentType.Value,
                SolutionUniqueName = destinationName,
                DoNotIncludeSubcomponents =
                     component.RootComponentBehavior?.Value == SolutionComponent.Options.RootComponentBehavior.DoNotIncludeSubcomponents ||
                     component.RootComponentBehavior?.Value == SolutionComponent.Options.RootComponentBehavior.IncludeAsShellOnly
            });
            Delegate.TracingService.Trace($"{response.id}");
            return response.id;
        }

        public override string GetConfig(string key, int lcid = 1033, string defaultValue = null)
        {
            return null;
        }
    }
}
