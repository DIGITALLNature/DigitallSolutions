class WorkbenchRibbon {
  public static CmdOpenSolutionEditor(): void {
    const openUrlOptions = { height: 480, width: 640 };
    const solutionid = Xrm.Page.getAttribute('dgt_solutionid').getValue();

    Xrm.Navigation.openUrl(Xrm.Utility.getGlobalContext().getClientUrl() + '/tools/solution/edit.aspx?id={' + solutionid + '}#', openUrlOptions);
  }

  public static CmdOpenMakeSolution(): void {
    const openUrlOptions = { height: 480, width: 640 };
    const solutionid = Xrm.Page.getAttribute('dgt_solutionid').getValue();

    WorkbenchRibbon.CallWebApiExecute(WorkbenchRibbon.BuildRequest('dgt_lookup_make_environment')).then(
      (result) => {
        result.json().then(function (Response: any) {
          if (Response.MakeEnvironmentId == '00000000-0000-0000-0000-000000000000') {
            const alertStrings = {
              confirmButtonLabel: 'Abort',
              text: 'SecureConfig missing! e.g. { "MakeEnvironmentId": "7a28c553-78bc-4866-bad9-edfdb2538201" }',
              title: 'SecureConfig missing!',
            };
            Xrm.Navigation.openAlertDialog(alertStrings);
          } else {
            Xrm.Navigation.openUrl('https://make.powerapps.com/environments/' + Response.MakeEnvironmentId + '/solutions/' + solutionid, openUrlOptions);
          }
        });
      },
      (error) => {
        const alertStrings = {
          confirmButtonLabel: 'OK',
          text: error,
          title: 'Error occured!',
        };
        Xrm.Navigation.openAlertDialog(alertStrings).then(function () {
          return;
        });
      },
    );
  }

  private static BuildRequest(operationName: string): any {
    const request = {
      getMetadata: function () {
        const metadata = {
          boundParameter: null,
          parameterTypes: {},
          operationName: operationName,
          operationType: 0,
        };
        return metadata;
      },
    };
    return request;
  }

  private static async CallWebApiExecute(request: any): Promise<any> {
    return await Xrm.WebApi.online.execute(request).then(
      function (result) {
        if (!result.ok) {
          console.log('Error: ' + result.status + ': ' + result.statusText);
          console.log('Data: ' + request);
        }
        return result;
      },
      function (error) {
        console.error('Error: ' + JSON.stringify(error));
        console.error('Data: ' + request);
        return error;
      },
    );
  }
}
