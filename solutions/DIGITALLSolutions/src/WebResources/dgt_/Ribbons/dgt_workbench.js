"use strict";
class WorkbenchRibbon {
    static CmdOpenSolutionEditor() {
        var openUrlOptions = { height: 480, width: 640 };
        var solutionid = Xrm.Page.getAttribute("dgt_solutionid").getValue();
        Xrm.Navigation.openUrl(Xrm.Utility.getGlobalContext().getClientUrl() + "/tools/solution/edit.aspx?id={" + solutionid + "}#", openUrlOptions);
    }
    static CmdOpenMakeSolution() {
        var openUrlOptions = { height: 480, width: 640 };
        var solutionid = Xrm.Page.getAttribute("dgt_solutionid").getValue();
        WorkbenchRibbon.CallWebApiExecute(WorkbenchRibbon.BuildRequest("dgt_lookup_make_environment")).then((result) => {
            result.json().then(function (Response) {
                if (Response.MakeEnvironmentId == "00000000-0000-0000-0000-000000000000") {
                    Xrm.Utility.alertDialog("SecureConfig missing! e.g. { \"MakeEnvironmentId\": \"7a28c553-78bc-4866-bad9-edfdb2538201\" }", function () { return; });
                }
                else {
                    Xrm.Navigation.openUrl("https://make.powerapps.com/environments/" + Response.MakeEnvironmentId + "/solutions/" + solutionid, openUrlOptions);
                }
            });
        }, (error) => {
            Xrm.Utility.alertDialog(error.message, function () { return; });
        });
    }
    static BuildRequest(operationName) {
        var request = {
            getMetadata: function () {
                var metadata = {
                    boundParameter: null,
                    parameterTypes: {},
                    operationName: operationName,
                    operationType: 0
                };
                return metadata;
            }
        };
        return request;
    }
    static async CallWebApiExecute(request) {
        return await Xrm.WebApi.online.execute(request).then(function (result) {
            if (!result.ok) {
                console.log("Error: " + result.status + ": " + result.statusText);
                console.log("Data: " + request);
            }
            return result;
        }, function (error) {
            console.error("Error: " + JSON.stringify(error));
            console.error("Data: " + request);
            return error;
        });
    }
}
