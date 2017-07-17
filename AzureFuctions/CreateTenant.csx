using System.Net;
using System;
using Octopus.Client;
using Octopus.Client.Model;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, ExecutionContext context)
{
    log.Info("C# HTTP trigger function processed a request.");

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Get the branch name
    string branchName = data["ref"];
    log.Info("Creating tenant " + branchName);

    var endpoint = new OctopusServerEndpoint("https://droyad.gq", Environment.GetEnvironmentVariable("ApiKey"));
    var repository = new OctopusRepository(endpoint);

    var projects = repository.Projects.FindAll();
    var environments = new ReferenceCollection(new [] {
        repository.Environments.FindByName("Integration Test").Id
    });
    var tenant = repository.Tenants.FindByName(branchName);

    // Create the tenant if it doesn't exist
    if (tenant == null)
        tenant = repository.Tenants.Create(new TenantResource()
        {
            Name = branchName,
            ProjectEnvironments = projects.ToDictionary(p => p.Id, p => environments)
        });

    // Update the logo
    using (var fs = File.OpenRead($@"{context.FunctionDirectory}\tennant.jpg"))
	repository.Tenants.SetLogo(tenant, "logo.jpg", fs);


    return req.CreateResponse(HttpStatusCode.OK);
}
