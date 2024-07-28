using System;
using D365.Extension.Core;
using D365.Extension.Model;

namespace dgt.solutions.Plugins.Processor
{
    internal class CloseProcessor : IWorkbenchProcessor
    {
        private string _message;
        private int _state;
        private int _status;
        private readonly Executor _executor;

        public CloseProcessor(Executor executor)
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
            var workbenchProcessor = new WorkbenchProcessor(_executor);

            try
            {
                workbenchProcessor.Close(_message, workbench, _state, _status);
            }
            catch (Exception e)
            {
                workbenchProcessor.Failure(e.RootMessage(), workbench);
                return ExecutionResult.Failure;
            }
            return ExecutionResult.Ok;
        }
    }
}
