#r "C:\\Program Files\\Octopus Deploy\\Octopus\\Octopus.Client.dll"

using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using System.Net;
using System.IO;
using System.Threading;

var apiKey = Octopus.Parameters["ApiKey"];
var tenantId = Octopus.Parameters["Octopus.Deployment.Tenant.Id"];

var endpoint = new OctopusServerEndpoint("https://droyad.gq", apiKey);
var repository = new OctopusRepository(endpoint);

var integrationTest = repository.Environments.FindByName("Integration Test").Id;

var endpoints = from m in repository.Machines.FindAll()
	where m.TenantIds.Contains(tenantId) &&
			m.Roles.Contains("Web")
	select (SshEndpointResource)m.Endpoint;

var sw = Stopwatch.StartNew();

void TestEndpoint(SshEndpointResource ssh)
{
	var url = $"http://{ssh.Host}:7000/20/50";
	while (sw.Elapsed < TimeSpan.FromMinutes(1))
		try
		{
			Console.WriteLine($"Requesting {url}");

			using (var response = WebRequest.CreateHttp(url).GetResponse())
			using (var s = new StreamReader(response.GetResponseStream()))
				Console.WriteLine("Got " + s.ReadToEnd());

			return;
		}
		catch (WebException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
			Thread.Sleep(TimeSpan.FromSeconds(10));
		}
	throw new Exception("Execution timed out");
}

foreach (var e in endpoints)
	TestEndpoint(e);

