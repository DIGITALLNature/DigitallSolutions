using System;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using Microsoft.Xrm.Sdk;

namespace dgt.solutions.Plugins.Processor
{
    internal class FinalizeProcessor : WorkbenchProcessor
    {
        public FinalizeProcessor(Executor executor) : base(executor)
        {
        }

        internal override ExecutionResult Execute(DgtWorkbench workbench)
        {
            try
            {
                var service = Executor.SecuredOrganizationService;
                using (var dataContext = Executor.DataContext(service))
                {
                    var records = (from rec in dataContext.DgtWorkbenchHistorySet
                                   where rec.DgtWorkbenchId != null
                                   where rec.DgtWorkbenchId.Id == workbench.Id
                                   where rec.Statecode.Value == DgtWorkbenchHistory.Options.Statecode.Active
                                   select rec).ToList();
                    foreach (var record in records)
                    {
                        service.Update(new DgtWorkbenchHistory(record.Id)
                        {
                            Statecode = new OptionSetValue(DgtWorkbenchHistory.Options.Statecode.Inactive),
                            Statuscode = new OptionSetValue(DgtWorkbenchHistory.Options.Statuscode.Inactive)
                        });
                    }
                }
                Executor.ElevatedOrganizationService.Delete(Solution.EntityLogicalName, Guid.Parse(workbench.DgtSolutionid));
                return ExecutionResult.Ok;
            }
            catch (Exception e)
            {
                Executor.LoggingService.Error(e.RootMessage());
                return ExecutionResult.Failure;
            }
        }
    }
}
