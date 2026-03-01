using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
builder.Configuration.Sources.Add(new JsonConfigurationSource { Path = "appsettings.Local.json", Optional= true });

bool isE2ETest = builder.Configuration["E2E_TEST"] == "true";

var postgres = builder.AddPostgres("ecommerce-postgres")
    .WithDataVolume()
    .WithPgAdmin();

var redis = builder.AddRedis("ecommerce-redis")
    .WithDataVolume();

var chatCompletion = builder.AddOllama("chatcompletion").WithDataVolume();

var keycloak = builder.AddKeycloakContainer("ecommerce-keycloak")
    .WithDataVolume();

var storage = builder.AddAzureStorage("ecommerce-storage");
if (builder.Environment.IsDevelopment())
{
    storage.RunAsEmulator(r =>
    {
        if (!isE2ETest)
        {
            r.WithDataVolume();
        }
    });
}

var blobStorage = storage.AddBlobs("eshopsupport-blobs");

var api = builder.AddProject<Projects.ECommerce_Api>("ecommerce-api")
    .WithSwaggerUI()
    .WithScalar()
    .WithRedoc()
    .WithReference(postgres, "Database")
    .WithReference(redis, "Cache")
    .WithReference(blobStorage)
    .WithOllamaReference(chatCompletion)
    .WithReference(keycloak)
    .WaitFor(postgres)
    .WaitFor(redis)
    .WaitFor(storage);

builder.AddProject<Projects.ECommerce_WebApp>("ecommerce-webapp")
    .WithReference(api)
    .WaitFor(api);

await builder.Build().RunAsync();
