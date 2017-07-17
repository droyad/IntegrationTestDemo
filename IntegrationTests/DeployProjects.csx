#r "C:\\Program Files\\Octopus Deploy\\Octopus\\Octopus.Client.dll"

using Octopus.Client;
using Octopus.Client.Model;
using System.Threading;
using System.Text.RegularExpressions;

var apiKey = Octopus.Parameters["ApiKey"];
var tenantId = Octopus.Parameters["Octopus.Deployment.Tenant.Id"];
var branch = Octopus.Parameters["Octopus.Deployment.Tenant.Name"];
var expectedMachineCount = int.Parse(Octopus.Parameters["ExpectedMachineCount"]);

var endpoint = new OctopusServerEndpoint("https://droyad.gq", apiKey);
var repository = new OctopusRepository(endpoint);

var production = repository.Environments.FindByName("Production").Id;
var integrationTest = repository.Environments.FindByName("Integration Test").Id;
var group = repository.ProjectGroups.FindByName("My Projects");
var projects = repository.ProjectGroups.GetProjects(group);

// Wait for Machines
int MachineCount()
{
	return repository.Machines.FindAll()
		.Count(m => m.HealthStatus == MachineModelHealthStatus.Healthy && m.TenantIds.Contains(tenantId));
}

var sw = Stopwatch.StartNew();

var count = MachineCount();
while (count < expectedMachineCount)
{
	if(sw.Elapsed > TimeSpan.FromMinutes(5))
		throw new Exception("Timed out waiting for machines");

	Console.WriteLine($"{count} of {expectedMachineCount} machines");
	Thread.Sleep(1000);
	count = MachineCount();
}

// Deploy projects

var dashboard = repository.Dashboards.GetDashboard();

ReleaseResource GetReleaseToDeploy(ProjectResource project)
{
	var release = repository.Projects.GetAllReleases(project).FirstOrDefault(r => Regex.IsMatch(r.Version, $"-{branch}[0-9]+$", RegexOptions.IgnoreCase));
	if (release != null)
		return release;

	var item = dashboard.Items.FirstOrDefault(i => i.EnvironmentId == production && i.ProjectId == project.Id);
	if (item != null)
		return repository.Releases.Get(item.ReleaseId);

	return null;
}

void CreateRelease(ProjectResource project, ReleaseResource release)
{
	Console.WriteLine($"Deploying {project.Name} {release.Version}");
	repository.Deployments.Create(new DeploymentResource()
	{
		ProjectId = release.ProjectId,
		ReleaseId = release.Id,
		ChannelId = release.ChannelId,
		TenantId = tenantId,
		EnvironmentId = integrationTest,
	});
}

foreach (var project in projects)
{
	var release = GetReleaseToDeploy(project);
	if (release == null)
		Console.WriteLine("Could not find a suitable release for " + project.Name);
	else
		CreateRelease(project, release);
}

// Wait for projects
string[] IncompleteDeployments()
{
	var dash = repository.Dashboards.GetDynamicDashboard(projects.Select(p => p.Id).ToArray(), new[] { integrationTest });
	return dash.Items
		.Where(i => i.TenantId == tenantId && (i.State == TaskState.Queued || i.State == TaskState.Executing))
		.Select(i => dash.Projects.First(p => p.Id == i.ProjectId).Name)
		.ToArray();
}

var incomplete = IncompleteDeployments();
while (incomplete.Any())
{
	Console.WriteLine($"Waiting for {string.Join(", ", incomplete)}");
	Thread.Sleep(1000);
	incomplete = IncompleteDeployments();
}