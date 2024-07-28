using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using dgt.solutions.Plugins.Contract;
using dgt.solutions.Plugins.Helper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.Processor
{
    internal class WorkbenchProcessor
    {
        protected readonly Executor Executor;

        protected WorkbenchHistoryLogger _workbenchHistoryLogger;

        public WorkbenchProcessor(Executor executor) : this(executor, default) { }

        public WorkbenchProcessor(Executor executor, WorkbenchHistoryLogger workbenchHistoryLogger)
        {
            Executor = executor;
            _workbenchHistoryLogger = workbenchHistoryLogger;
        }

        public void Success(string message, DgtWorkbench workbench, DgtCarrier carrier, int state, int status, List<ComponentMoverLogEntry> moverLog = null, string constrainLog = null)
        {
            if (message?.Length > 490)
            {
                message = message.Substring(0, 490);
            }
            message = $"Success - {message}";
            var solution = Executor.ElevatedOrganizationService.Retrieve(
                Solution.EntityLogicalName,
                Guid.Parse(workbench.DgtSolutionid), new ColumnSet(Solution.LogicalNames.Version)).ToEntity<Solution>();
            var versions = solution.Version.Split('.');
            //increment patch level
            versions[3] = $"{Convert.ToInt32(versions[3]) + 1}";
            var version = string.Join(".", versions);
            Executor.ElevatedOrganizationService.Update(new Solution(solution.Id)
            {
                Version = version
            });

            var componentMoverLog = moverLog != null ? new SerializerService().JsonSerialize<List<ComponentMoverLogEntry>>(moverLog) : null;
            new StatusReasonHandler(Executor).Update(new DgtWorkbench(workbench.Id)
            {
                Statecode = new OptionSetValue(state),
                Statuscode = new OptionSetValue(status),
                DgtSolutionversion = version,
                DgtCarrierId = null
            }, new DgtWorkbenchHistory
            {
                Id = _workbenchHistoryLogger.WorkbenchHistory.Id,
                DgtEntry = message,
                DgtWorkbenchId = workbench.ToEntityReference(),
                DgtWorkbenchVersion = version,
                DgtCarrierId = carrier.ToEntityReference(),
                DgtCarrierVersion = carrier.DgtSolutionversion,
                DgtComponentMoverLog = componentMoverLog,
                DgtConstraintCheckLog = constrainLog,
                Statuscode = new OptionSetValue(DgtWorkbenchHistory.Options.Statuscode.Success),
                Statecode = new OptionSetValue(DgtWorkbenchHistory.Options.Statecode.Inactive),
            }); ;
        }

        public void Close(string message, DgtWorkbench workbench, int state, int status)
        {
            Executor.LoggingService.Error(message);
            if (message?.Length > 490)
            {
                message = message.Substring(0, 490);
            }
            message = $"Success - {message}";
            var solution = Executor.ElevatedOrganizationService.Retrieve(
                Solution.EntityLogicalName,
                Guid.Parse(workbench.DgtSolutionid), new ColumnSet(Solution.LogicalNames.Version)).ToEntity<Solution>();
            new StatusReasonHandler(Executor).Update(new DgtWorkbench(workbench.Id)
            {
                Statecode = new OptionSetValue(state),
                Statuscode = new OptionSetValue(status),
                DgtSolutionversion = solution.Version,
                DgtCarrierId = null
            }, new DgtWorkbenchHistory
            {
                Id = _workbenchHistoryLogger.WorkbenchHistory.Id,
                DgtEntry = message,
                DgtWorkbenchId = workbench.ToEntityReference(),
                DgtWorkbenchVersion = workbench.DgtSolutionversion,
                DgtCarrierId = null,
                DgtCarrierVersion = null,
                Statuscode = new OptionSetValue(DgtWorkbenchHistory.Options.Statuscode.Success),
                Statecode = new OptionSetValue(DgtWorkbenchHistory.Options.Statecode.Inactive),
            });
        }

        public void Failure(string message, DgtWorkbench workbench, DgtCarrier carrier = null, List<ComponentMoverLogEntry> moverLog = null, string constrainLog = null)
        {
            Executor.LoggingService.Error(message);
            if (message?.Length > 490)
            {
                message = message.Substring(0, 490);
            }
            message = $"Failure - {message}";
            var componentMoverLog = moverLog != null ? new SerializerService().JsonSerialize<List<ComponentMoverLogEntry>>(moverLog) : null;
            new StatusReasonHandler(Executor).Update(new DgtWorkbench(workbench.Id)
            {
                Statuscode = new OptionSetValue(DgtWorkbench.Options.Statuscode.Failure)
            }, new DgtWorkbenchHistory
            {
                Id = _workbenchHistoryLogger.WorkbenchHistory.Id,
                DgtEntry = message,
                DgtWorkbenchId = workbench.ToEntityReference(),
                DgtWorkbenchVersion = workbench.DgtSolutionversion,
                DgtCarrierId = carrier?.ToEntityReference(),
                DgtCarrierVersion = carrier?.DgtSolutionversion,
                DgtComponentMoverLog = componentMoverLog,
                DgtConstraintCheckLog = constrainLog,
                Statuscode = new OptionSetValue(DgtWorkbenchHistory.Options.Statuscode.Failure),
                Statecode = new OptionSetValue(DgtWorkbenchHistory.Options.Statecode.Inactive),
            });;
        }

        public string FinishHandshake(DgtCarrier carrier)
        {
            var solution = Executor.ElevatedOrganizationService.Retrieve(
                Solution.EntityLogicalName,
                Guid.Parse(carrier.DgtSolutionid), new ColumnSet(Solution.LogicalNames.Version)).ToEntity<Solution>();
            var versions = solution.Version.Split('.');
            //increment patch level
            versions[3] = $"{Convert.ToInt32(versions[3]) + 1}";
            var version = string.Join(".", versions);
            Executor.ElevatedOrganizationService.Update(new Solution(solution.Id)
            {
                Version = version
            });
            new StatusReasonHandler(Executor).Update(new DgtCarrier(carrier.Id)
            {
                DgtWorkbenchId = null,
                DgtHandshakeTs = null,
                DgtSolutionversion = version
            });
            return version;
        }

        public void ResetHandshake(DgtCarrier carrier)
        {
            new StatusReasonHandler(Executor).Update(new DgtCarrier(carrier.Id)
            {
                DgtWorkbenchId = null,
                DgtHandshakeTs = null
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workbench"></param>
        /// <param name="carrier">out carrier</param>
        /// <returns></returns>
        public bool Handshake(DgtWorkbench workbench, out DgtCarrier carrier)
        {
            var targetCarrierId = workbench.DgtTargetCarrierId?.Id;
            if (targetCarrierId == null || targetCarrierId == Guid.Empty)
            {
                Failure("No Target Carrier!", workbench);
                carrier = null;
                return false;
            }

            using (var dataContext = Executor.DataContext(Executor.ElevatedOrganizationService))
            {
                carrier = (from rec in dataContext.DgtCarrierSet
                           where rec.Statecode != null
                           where rec.Statecode.Value == DgtCarrier.Options.Statecode.Active
                           where rec.Id == targetCarrierId
                           select rec).SingleOrDefault();

                if (carrier == null)
                {
                    Failure("Target Carrier not found or Target Carrier inactive!", workbench);
                    return false;
                }

                if (carrier.DgtLockedOpt == true)
                {
                    Failure("Target Carrier is locked!", workbench);
                    return false;
                }

                if (carrier.DgtWorkbenchId != null && workbench.Id != carrier.DgtWorkbenchId?.Id)
                {
                    Failure("Target Carrier is blocked by another Workbench", workbench, carrier);
                    return false;
                }

                Executor.ElevatedOrganizationService.Update(new DgtCarrier(carrier.Id)
                {
                    DgtWorkbenchId = workbench.ToEntityReference(),
                    DgtHandshakeTs = DateTime.UtcNow
                });

                carrier = Executor.ElevatedOrganizationService.Retrieve(
                    DgtCarrier.EntityLogicalName,
                    carrier.Id,
                    new ColumnSet(true)).ToEntity<DgtCarrier>();

                if (carrier?.DgtWorkbenchId?.Id != workbench.Id)
                {
                    Failure("Handshake failed; try again later!", workbench, carrier);
                    return false;
                }
                new StatusReasonHandler(Executor).Update(new DgtWorkbench(workbench.Id)
                {
                    DgtCarrierId = carrier.ToEntityReference()
                });
            }
            return true;
        }
    }
}
