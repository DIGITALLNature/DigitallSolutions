using System;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using Microsoft.Xrm.Sdk;

namespace dgt.solutions.Plugins.Processor
{
    internal class FinalizeProcessor : IWorkbenchProcessor
    {
        private readonly Executor _executor;

        public FinalizeProcessor(Executor executor)
        {
            _executor = executor;
        }

        public ExecutionResult Execute(DgtWorkbench workbench)
        {
            try
            {
                var service = _executor.SecuredOrganizationService;
                using (var dataContext = _executor.DataContext(service))
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
                _executor.ElevatedOrganizationService.Delete(Solution.EntityLogicalName, Guid.Parse(workbench.DgtSolutionid));
                return ExecutionResult.Ok;
            }
            catch (Exception e)
            {
                _executor.LoggingService.Error(e.RootMessage());
                return ExecutionResult.Failure;
            }
        }
    }
}
