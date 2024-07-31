using System;
using System.Diagnostics;
using D365.Extension.Core;
using D365.Extension.Model;
using Microsoft.Xrm.Sdk;

namespace dgt.solutions.Plugins.CarrierConstraints
{
    public abstract class ConstraintBase : Executor
    {
        protected abstract string ConstraintType { get; }
        protected WorkbenchHistoryLogger WorkbenchHistoryLogger { get; private set; }

        protected sealed override ExecutionResult Execute()
        {
            Delegate.TracingService.Trace("starting constraint {0}", ConstraintType);
            GetInputParameter("Target", out EntityReference solutionReference);

            if (GetInputParameter("WorkbenchHistory", out EntityReference workbenchHistoryReference))
            {
                Delegate.TracingService.Trace("init workbench history logger for workbench history {0}", workbenchHistoryReference?.Id);
                WorkbenchHistoryLogger = new WorkbenchHistoryLogger(OrganizationService(), new DgtWorkbenchHistory { Id = workbenchHistoryReference.Id });
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            WorkbenchHistoryLogger?.LogDebug($"checking constraint '{ConstraintType}'");

            Delegate.TracingService.Trace("run check for solution {0}", solutionReference?.Id);
            var passed = RunCheck(solutionReference.Id);

            stopwatch.Stop();
            WorkbenchHistoryLogger?.LogDebug($"finished constraint '{ConstraintType}' in {stopwatch.ElapsedMilliseconds} ms");

            Delegate.TracingService.Trace("setting output parameters");
            SetOutputParameter(DgtPreventFlowsResponse.OutParameters.ConstraintSuccess_PreventFlows, passed);

            return ExecutionResult.Ok;
        }

        protected abstract bool RunCheck(Guid solutionId);
    }
}
