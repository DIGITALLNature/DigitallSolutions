using System;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using Microsoft.Xrm.Sdk;

namespace dgt.solutions.Plugins
{
    [PluginRegistration(
        PluginExecutionMode.Asynchronous,
        SdkMessageNames.Update,
        PluginExecutionStage.PostOperation,
        PrimaryEntityName = DgtCarrier.EntityLogicalName,
        FilterAttributes = new[]
        {
            DgtCarrier.LogicalNames.Statecode
        }
    )]
    public class CarrierStateListener : Executor
    {
        protected override ExecutionResult Execute()
        {
            var carrier = Entity.ToEntity<DgtCarrier>();
            switch (carrier.Statecode.Value)
            {
                case DgtCarrier.Options.Statecode.Active:
                    return UpdateChildren(carrier, true);
                case DgtCarrier.Options.Statecode.Inactive:
                    return UpdateChildren(carrier, false);
                default:
                    return ExecutionResult.Skipped;
            }
        }

        private ExecutionResult UpdateChildren(DgtCarrier carrier, bool activate)
        {
            try
            {
                using (var context = DataContext(ElevatedOrganizationService))
                {
                    var dependencyCheckStateCurrent = activate ? DgtCarrierDependencyCheck.Options.Statecode.Inactive : DgtCarrierDependencyCheck.Options.Statecode.Active;
                    var dependencyCheckStateNew = activate ? DgtCarrierDependencyCheck.Options.Statecode.Active : DgtCarrierDependencyCheck.Options.Statecode.Inactive;
                    var dependencyCheckStatusNew = activate ? DgtCarrierDependencyCheck.Options.Statuscode.Active : DgtCarrierDependencyCheck.Options.Statuscode.Inactive;

                    var dependencyChecks = (from rec in context.DgtCarrierDependencyCheckSet
                                            where rec.DgtCarrierId != null
                                            where rec.DgtCarrierId.Id == carrier.Id
                                            where rec.Statecode != null
                                            where rec.Statecode.Value == dependencyCheckStateCurrent
                                            select rec).ToList();

                    foreach (var dependencyCheck in dependencyChecks)
                    {
                        ElevatedOrganizationService.Update(new DgtCarrierDependencyCheck(dependencyCheck.Id)
                        {
                            Statecode = new OptionSetValue(dependencyCheckStateNew),
                            Statuscode = new OptionSetValue(dependencyCheckStatusNew)
                        });
                    }

                    var missingDependencyStateCurrent = activate ? DgtCarrierMissingDependency.Options.Statecode.Inactive : DgtCarrierMissingDependency.Options.Statecode.Active;
                    var missingDependencyStateNew = activate ? DgtCarrierMissingDependency.Options.Statecode.Active : DgtCarrierMissingDependency.Options.Statecode.Inactive;
                    var missingDependencyNew = activate ? DgtCarrierMissingDependency.Options.Statuscode.Active : DgtCarrierMissingDependency.Options.Statuscode.Inactive;

                    var missingDependencies = (from rec in context.DgtCarrierMissingDependencySet
                                               where rec.DgtCarrierId != null
                                               where rec.DgtCarrierId.Id == carrier.Id
                                               where rec.Statecode != null
                                               where rec.Statecode.Value == missingDependencyStateCurrent
                                               select rec).ToList();

                    foreach (var missingDependency in missingDependencies)
                    {
                        ElevatedOrganizationService.Update(new DgtCarrierMissingDependency(missingDependency.Id)
                        {
                            Statecode = new OptionSetValue(missingDependencyStateNew),
                            Statuscode = new OptionSetValue(missingDependencyNew)
                        });
                    }

                    //remove the link!
                    if (!activate)
                    {
                        var workbenches = (from rec in context.DgtWorkbenchSet
                                           where rec.DgtTargetCarrierId != null
                                           where rec.DgtTargetCarrierId.Id == carrier.Id
                                           select rec).ToList();
                        foreach (var workbench in workbenches)
                        {
                            ElevatedOrganizationService.Update(new DgtWorkbench(workbench.Id)
                            {
                                DgtTargetCarrierId = null
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggingService.Error(e.RootMessage());
                return ExecutionResult.Failure;
            }
            return ExecutionResult.Ok;
        }

        public override string GetConfig(string key, int lcid = 1033, string defaultValue = null)
        {
            return null;
        }
    }
}
