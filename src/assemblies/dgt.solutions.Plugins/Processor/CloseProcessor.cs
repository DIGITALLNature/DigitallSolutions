using System;
using D365.Extension.Core;
using D365.Extension.Model;

namespace dgt.solutions.Plugins.Processor
{
    internal class CloseProcessor : WorkbenchProcessor
    {
        private string _message;
        private int _state;
        private int _status;

        public CloseProcessor(Executor executor) : base(executor)
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
            try
            {
                Close(_message, workbench, _state, _status);
            }
            catch (Exception e)
            {
                Failure(e.RootMessage(), workbench);
                return ExecutionResult.Failure;
            }
            return ExecutionResult.Ok;
        }
    }
}
