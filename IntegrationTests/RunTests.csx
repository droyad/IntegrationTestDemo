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

foreach (var e in endpoints)
{
	var url = $"http://{e.Host}:7000/20/50";

	while(true)
		try
		{
			if(sw.Elapsed > TimeSpan.FromMinutes(1))
				throw new Exception("Execution timed out");

			Console.WriteLine($"Requesting {url}");

			using(var response = WebRequest.CreateHttp(url).GetResponse())
			using (var s = new StreamReader(response.GetResponseStream()))
				Console.WriteLine("Got " + s.ReadToEnd());
		}
		catch (WebException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
			Thread.Sleep(TimeSpan.FromSeconds(10));
		}
}
