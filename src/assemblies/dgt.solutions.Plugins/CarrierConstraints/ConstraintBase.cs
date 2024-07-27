using System;
using D365.Extension.Core;
using D365.Extension.Model;
using dgt.solutions.Plugins.Contract;
using Microsoft.Xrm.Sdk;

namespace dgt.solutions.Plugins.CarrierConstraints
{
    public abstract class ConstraintBase : Executor
    {
        protected sealed override ExecutionResult Execute()
        {
            GetInputParameter("Target", out EntityReference solutionReference);

            var constraintCheckLog = RunCheck(solutionReference.Id);
            var constraintCheckLogJson = new SerializerService().JsonSerialize<ConstraintCheckLogEntry>(constraintCheckLog);
            SetOutputParameter(DgtPreventFlowsResponse.OutParameters.ConstraintLog_PreventFlows, constraintCheckLogJson);

            return ExecutionResult.Ok;
        }

        protected abstract ConstraintCheckLogEntry RunCheck(Guid solutionId);
    }
}