<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

var baseDir = @"C:\source\IntegrationTestDemo\AWS";
var data = File.ReadAllText(Path.Combine(baseDir, "UserData.sh"));
var formationJson = File.ReadAllText(Path.Combine(baseDir, "CloudFormation.json"));
var formation = JsonConvert.DeserializeObject<dynamic>(formationJson);
formation.Resources.AppServerInstance.Properties.UserData["Fn::Base64"] = data;
formationJson = JsonConvert.SerializeObject((object)formation, Newtonsoft.Json.Formatting.Indented);
File.WriteAllText(Path.Combine(baseDir, "CloudFormation.json"), formationJson);


