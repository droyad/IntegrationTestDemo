#r "C:\\Program Files\\Octopus Deploy\\Octopus\\Octopus.Client.dll"

using Octopus.Client;
using Octopus.Client.Model;

var apiKey = Octopus.Parameters["ApiKey"];
var tenantId = Octopus.Parameters["Octopus.Deployment.Tenant.Id"];

var endpoint = new OctopusServerEndpoint("https://droyad.gq", apiKey);
var repository = new OctopusRepository(endpoint);

var machines = from m in repository.Machines.FindAll()
	where m.TenantIds.Contains(tenantId)
	select m;

foreach (var machine in machines)
	repository.Machines.Delete(machine);
