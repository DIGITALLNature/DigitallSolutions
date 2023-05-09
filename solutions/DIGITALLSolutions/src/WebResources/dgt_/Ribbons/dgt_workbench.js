"use strict";
// eslint-disable-next-line @typescript-eslint/no-unused-vars
var WorkbenchRibbon;
(function (WorkbenchRibbon) {
    function CmdOpenSolutionEditor() {
        const openUrlOptions = { height: 480, width: 640 };
        const solutionid = Xrm.Page.getAttribute('dgt_solutionid').getValue();
        Xrm.Navigation.openUrl(Xrm.Utility.getGlobalContext().getClientUrl() + '/tools/solution/edit.aspx?id={' + solutionid + '}#', openUrlOptions);
    }
    WorkbenchRibbon.CmdOpenSolutionEditor = CmdOpenSolutionEditor;
    function CmdOpenMakeSolution() {
        const openUrlOptions = { height: 480, width: 640 };
        const solutionid = Xrm.Page.getAttribute('dgt_solutionid').getValue();
        const request = {
            getMetadata: function () {
                return {
                    boundParameter: null,
                    parameterTypes: {},
                    operationType: 1,
                    operationName: 'dgt_lookup_make_environment',
                };
            },
        };
        Xrm.WebApi.online
            .execute(request)
            .then(function success(response) {
            if (response.ok) {
                return response.json();
            }
        })
            .then(function (responseBody) {
            const result = responseBody;
            console.log(result);
            // Return Type: mscrm.dgt_lookup_make_environmentResponse
            // Output Parameters
            const MakeEnvironmentId = result['MakeEnvironmentId']; // Edm.Guid
            if (MakeEnvironmentId == '00000000-0000-0000-0000-000000000000') {
                const alertStrings = {
                    confirmButtonLabel: 'Abort',
                    text: 'SecureConfig missing! e.g. { "MakeEnvironmentId": "7a28c553-78bc-4866-bad9-edfdb2538201" }',
                    title: 'SecureConfig missing!',
                };
                Xrm.Navigation.openAlertDialog(alertStrings);
            }
            else {
                Xrm.Navigation.openUrl('https://make.powerapps.com/environments/' + MakeEnvironmentId + '/solutions/' + solutionid, openUrlOptions);
            }
        })
            .catch(function (error) {
            console.log(error.message);
            const alertStrings = {
                confirmButtonLabel: 'OK',
                text: error.message,
                title: 'Error occured!',
            };
            Xrm.Navigation.openAlertDialog(alertStrings).then(function () {
                return;
            });
        });
    }
    WorkbenchRibbon.CmdOpenMakeSolution = CmdOpenMakeSolution;
})(WorkbenchRibbon || (WorkbenchRibbon = {}));
