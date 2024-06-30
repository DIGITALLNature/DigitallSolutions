using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using dgt.solutions.Plugins.Contract;
using dgt.solutions.Plugins.Helper;

namespace dgt.solutions.Plugins.Processor
{
    internal class ComponentMoveProcessor : WorkbenchProcessor
    {
        private string _message;
        private int _state;
        private int _status;

        public ComponentMoveProcessor(Executor executor) : base(executor)
        {
        }

        internal WorkbenchProcessor Init(string message, int state, int status)
        {
            _message = message;
            _state = state;
            _status = status;
            return this;
        }

        internal override ExecutionResult Execute(DgtWorkbench workbench)
        {
            //Handshake
            if (!Handshake(workbench, out var carrier))
            {
                return ExecutionResult.Failure;
            }


            var componentMoverLog = new List<ComponentMoverLogEntry>();
            var constraintCheckLog = string.Empty;

            try
            {
                //Constraints
                var constraintsCheckRequest = new DgtRunCarrierConstraintsCheckRequest
                {
                    Target = workbench.ToEntityReference()
                };
                var constraintsCheckResponse = (DgtRunCarrierConstraintsCheckResponse)Executor.ElevatedOrganizationService.Execute(constraintsCheckRequest);
                constraintCheckLog = constraintsCheckResponse.CarrierConstraintsLog;

                if(!constraintsCheckResponse.CarrierConstraintsSuccessStatus)
                {
                    throw new ConstraintViolationException("Constraint violations occured, please check logs for details.");
                }

                //Move
                componentMoverLog.AddRange(new ComponentMover(Executor).MoveComponents(Guid.Parse(workbench.DgtSolutionid), carrier.DgtSolutionuniquename));
                //Reset (Handshake)
                carrier.DgtSolutionversion = FinishHandshake(carrier);
                Success(_message, workbench, carrier, _state, _status, componentMoverLog, constraintCheckLog);
            }
            catch (Exception e)
            {
                //Reset (Handshake)
                ResetHandshake(carrier);
                Failure(e.RootMessage(), workbench, carrier, componentMoverLog, constraintCheckLog);
                return ExecutionResult.Failure;
            }
            return ExecutionResult.Ok;
        }
    }
}
