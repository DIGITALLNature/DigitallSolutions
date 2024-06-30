using System.Collections.Generic;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Contract;
using dgt.solutions.Plugins.Helper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

[CustomApiRegistration(SdkMessageNames.DgtRunCarrierConstraintsCheck)]
public class RunCarrierConstraintsCheck : Executor
{
    protected override ExecutionResult Execute()
    {
        GetInputParameter("Target", out EntityReference workbenchReference);

        var elevatedOrgService = ElevatedOrganizationService;

        var workbench = elevatedOrgService
            .Retrieve(DgtWorkbench.EntityLogicalName, workbenchReference.Id, new ColumnSet(true))
            .ToEntity<DgtWorkbench>();
        var carrier = elevatedOrgService
            .Retrieve(DgtCarrier.EntityLogicalName, workbench.DgtTargetCarrierId.Id, new ColumnSet(true))
            .ToEntity<DgtCarrier>();

        var constraintCheckLog = new ConstraintCheck(this).Check(carrier, workbench);
        var constraintCheckLogJson = new SerializerService().JsonSerialize<List<ConstraintCheckLogEntry>>(constraintCheckLog);

        SetOutputParameter(DgtRunCarrierConstraintsCheckResponse.OutParameters.CarrierConstraintsSuccessStatus, constraintCheckLog.All(cl => cl.Succeded));
        SetOutputParameter(DgtRunCarrierConstraintsCheckResponse.OutParameters.CarrierConstraintsLog, constraintCheckLogJson);

        return ExecutionResult.Ok;
    }
}