// See https://aka.ms/new-console-template for more information

using Amazon.CDK;
using WetransfersClone.Infra;

var app = new App();
new InfraStack(app, "WetransferCloneInfraStack");
app.Synth();
