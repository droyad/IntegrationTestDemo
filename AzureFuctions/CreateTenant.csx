using System.Net;
using System;
using Octopus.Client;
using Octopus.Client.Model;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, ExecutionContext context)
{
    log.Info("C# HTTP trigger function processed a request.");

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Extract github comment from request body
    string branchName = data["ref"];
    log.Info("Creating tentant " + branchName);

    var endpoint = new OctopusServerEndpoint("https://droyad.gq", Environment.GetEnvironmentVariable("ApiKey"));
    var repository = new OctopusRepository(endpoint);

    var projects = repository.Projects.FindAll();
    var environments = new ReferenceCollection(new [] {
        repository.Environments.FindByName("Integration Test").Id,
        repository.Environments.FindByName("Demo").Id
    });
    var tenant = repository.Tenants.FindByName(branchName);
    if (tenant == null)
        tenant = repository.Tenants.Create(new TenantResource()
        {
            Name = branchName,
            ProjectEnvironments = projects.ToDictionary(p => p.Id, p => environments)
        });

    using (var fs = File.OpenRead($@"{context.FunctionDirectory}\tennant.jpg"))
	repository.Tenants.SetLogo(tenant, "logo.jpg", fs);


    return req.CreateResponse(HttpStatusCode.OK);
}
