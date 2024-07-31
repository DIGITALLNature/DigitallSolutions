using System;
using System.Collections.Generic;
using D365.Extension.Core;
using D365.Extension.Model;
using dgt.solutions.Plugins.Helper;

namespace dgt.solutions.Plugins.Processor
{
    internal class ComponentMoveProcessor : IWorkbenchProcessor
    {
        private string _message;
        private int _state;
        private int _status;
        private readonly Executor _executor;

        public ComponentMoveProcessor(Executor executor)
        {
            _executor = executor;
        }

        internal IWorkbenchProcessor Init(string message, int state, int status)
        {
            _message = message;
            _state = state;
            _status = status;
            return this;
        }

        public ExecutionResult Execute(DgtWorkbench workbench)
        {
            var workbenchHistoryLogger = WorkbenchHistoryLogger.Create(_executor.OrganizationService(), workbench, _message);
            var workbenchProcessor = new WorkbenchProcessor(_executor, workbenchHistoryLogger);

            //Handshake
            if (!workbenchProcessor.Handshake(workbench, out var carrier))
            {
                return ExecutionResult.Failure;
            }

            try
            {
                //Constraints
                var constraintsCheckRequest = new DgtRunCarrierConstraintsCheckRequest
                {
                    Target = workbench.DgtTargetCarrierId,
                    Workbench = workbench.ToEntityReference(),
                    ConstraintWorkbenchHistory_RunCarrierConstraintsCheck = workbenchHistoryLogger.WorkbenchHistory.ToEntityReference(),
                };
                var constraintsCheckResponse = (DgtRunCarrierConstraintsCheckResponse)_executor.ElevatedOrganizationService.Execute(constraintsCheckRequest);

                if (!constraintsCheckResponse.CarrierConstraintsSuccessStatus)
                {
                    throw new ConstraintViolationException("Constraint violations occured, please check logs for details.");
                }

                //Move
                new ComponentMover(_executor, workbenchHistoryLogger).MoveComponents(Guid.Parse(workbench.DgtSolutionid), carrier.DgtSolutionuniquename);
                //Reset (Handshake)
                carrier.DgtSolutionversion = workbenchProcessor.FinishHandshake(carrier);
                workbenchProcessor.Success(_message, workbench, carrier, _state, _status);
            }
            catch (Exception e)
            {
                //Reset (Handshake)
                workbenchProcessor.ResetHandshake(carrier);
                workbenchProcessor.Failure(e.RootMessage(), workbench, carrier);
                return ExecutionResult.Failure;
            }
            return ExecutionResult.Ok;
        }
    }
}
