using System;
using System.Diagnostics;
using D365.Extension.Core;
using D365.Extension.Model;
using dgt.solutions.Plugins.Contract;
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
            WorkbenchHistoryLogger?.LogDebug(new OptionSetValue(DgtWorkbenchHistoryLog.Options.DgtTypeSet.Constraint),
                $"checking constraint '{ConstraintType}'");

            Delegate.TracingService.Trace("run check for solution {0}", solutionReference?.Id);
            var constraintCheckLog = RunCheck(solutionReference.Id);

            Delegate.TracingService.Trace("parsing output");
            var constraintCheckLogJson = new SerializerService().JsonSerialize<ConstraintCheckLogEntry>(constraintCheckLog);
            SetOutputParameter(DgtPreventFlowsResponse.OutParameters.ConstraintLog_PreventFlows, constraintCheckLogJson);

            stopwatch.Stop();
            WorkbenchHistoryLogger?.LogDebug(new OptionSetValue(DgtWorkbenchHistoryLog.Options.DgtTypeSet.Constraint),
                $"finished constraint '{ConstraintType}' in {stopwatch.ElapsedMilliseconds} ms");

            return ExecutionResult.Ok;
        }

        protected abstract ConstraintCheckLogEntry RunCheck(Guid solutionId);
    }
}