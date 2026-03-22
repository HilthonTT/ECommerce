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

var ollama = builder.AddOllama("ollama")
    .WithDataVolume();

var chatCompletion = ollama.AddModel("chatcompletion", "qwen2.5:3b");
var embedding = ollama.AddModel("embedding", "nomic-embed-text");

var keycloak = builder.AddKeycloakContainer("ecommerce-keycloak", port: 8080)
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
    .WithExternalHttpEndpoints()
    .WithSwaggerUI()
    .WithScalar()
    .WithRedoc()
    .WithReference(postgres, "Database")
    .WithReference(redis, "Cache")
    .WithReference(blobStorage)
    .WithReference(chatCompletion)
    .WithReference(embedding)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WaitFor(postgres)
    .WaitFor(redis)
    .WaitFor(storage)
    .WaitFor(chatCompletion)
    .WaitFor(embedding);

builder.AddProject<Projects.ECommerce_WebApp>("ecommerce-webapp")
    .WithReference(api)
    .WaitFor(api);

await builder.Build().RunAsync();
