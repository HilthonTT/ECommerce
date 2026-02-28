using Aspire.Hosting.Eventing;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace Aspire.Hosting;

internal static class OllamaResourceExtensions
{
    public static IResourceBuilder<OllamaResource> AddOllama(this IDistributedApplicationBuilder builder, string name, string[]? models = null, string? defaultModel = null, bool enableGpu = true, int? port = null)
    {
        const string configKey = "OllamaModel";
        defaultModel ??= builder.Configuration[configKey];

        if (models is null or { Length: 0 })
        {
            if (string.IsNullOrEmpty(defaultModel))
            {
                throw new InvalidOperationException($"Expected the parameter '{nameof(defaultModel)}' or '{nameof(models)}' to be nonempty, or to find a configuration value '{configKey}', but none were provided.");
            }
            models = [defaultModel];
        }

        var resource = new OllamaResource(name, models, defaultModel ?? models.First(), enableGpu);
        var ollama = builder.AddResource(resource)
            .WithHttpEndpoint(port: port, targetPort: 11434)
            .WithImage("ollama/ollama", tag: "0.6.5");

        if (enableGpu)
        {
            ollama = ollama.WithContainerRuntimeArgs("--gpus=all");
        }

        // Fix 1: Use TryAddEventingSubscriber instead of TryAddLifecycleHook
        builder.Services.TryAddEventingSubscriber<OllamaEnsureModelAvailableSubscriber>();

        // Fix 2: Use IsHidden = true instead of KnownResourceStates.Hidden
        builder.AddResource(new OllamaModelDownloaderResource($"ollama-model-downloader-{name}", resource))
            .WithInitialState(new()
            {
                Properties = [],
                ResourceType = "ollama downloader",
                IsHidden = true
            })
            .ExcludeFromManifest();

        return ollama;
    }

    public static IResourceBuilder<OllamaResource> WithDataVolume(this IResourceBuilder<OllamaResource> builder)
    {
        return builder.WithVolume(CreateVolumeName(builder, builder.Resource.Name), "/root/.ollama");
    }

    public static IResourceBuilder<TDestination> WithOllamaReference<TDestination>(
        this IResourceBuilder<TDestination> builder,
        IResourceBuilder<OllamaResource> ollamaBuilder)
        where TDestination : IResourceWithEnvironment
    {
        return builder
            .WithReference(ollamaBuilder.GetEndpoint("http"))
            .WithEnvironment($"{ollamaBuilder.Resource.Name}:Type", "ollama")
            .WithEnvironment($"{ollamaBuilder.Resource.Name}:LlmModelName", ollamaBuilder.Resource.DefaultModel);
    }

    private static string CreateVolumeName<T>(IResourceBuilder<T> builder, string suffix) where T : IResource
    {
        var appName = builder.ApplicationBuilder.Environment.ApplicationName;
        var volumeName = $"{appName}-{suffix}";
        return new string(volumeName
            .ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) || c == '-' ? c : '-')
            .ToArray());
    }

    private sealed class OllamaEnsureModelAvailableSubscriber(
        ResourceLoggerService loggerService,
        ResourceNotificationService notificationService,
        DistributedApplicationModel appModel) : IDistributedApplicationEventingSubscriber
    {
        public Task SubscribeAsync(
            IDistributedApplicationEventing eventing, 
            DistributedApplicationExecutionContext executionContext, 
            CancellationToken cancellationToken)
        {
            if (executionContext.IsPublishMode)
            {
                return Task.CompletedTask;
            }

            foreach (var downloader in appModel.Resources.OfType<OllamaModelDownloaderResource>())
            {
                eventing.Subscribe<ResourceEndpointsAllocatedEvent>(
                    downloader.ollamaResource,
                    (@event, ct) => HandleEndpointsAllocatedAsync(downloader, ct));
            }

            return Task.CompletedTask;
        }

        private Task HandleEndpointsAllocatedAsync(OllamaModelDownloaderResource downloader, CancellationToken cancellationToken)
        {
            var ollama = downloader.ollamaResource;
            var logger = loggerService.GetLogger(downloader);

            _ = Task.Run(async () =>
            {
                var httpEndpoint = ollama.GetEndpoint("http");
                var client = new HttpClient();

                var ollamaModelsAvailable = await client.GetFromJsonAsync<OllamaGetTagsResponse>(
                    $"{httpEndpoint.Url}/api/tags",
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                if (ollamaModelsAvailable is null)
                {
                    return;
                }

                var availableModelNames = ollamaModelsAvailable.Models?.Select(m => m.Name) ?? [];
                var modelsToDownload = ollama.Models.Except(availableModelNames);

                if (!modelsToDownload.Any())
                {
                    return;
                }

                logger.LogInformation("Downloading models {Models} for ollama {OllamaName}...", string.Join(", ", modelsToDownload), ollama.Name);

                await notificationService.PublishUpdateAsync(downloader, s => s with
                {
                    State = new("Downloading models...", KnownResourceStateStyles.Info)
                });

                await Parallel.ForEachAsync(modelsToDownload, async (modelName, ct) =>
                {
                    await DownloadModelAsync(logger, httpEndpoint, modelName, ct);
                });

                await notificationService.PublishUpdateAsync(downloader, s => s with
                {
                    State = new("Models downloaded", KnownResourceStateStyles.Success)
                });
            },
            cancellationToken);

            return Task.CompletedTask;
        }

        private static async Task DownloadModelAsync(ILogger logger, EndpointReference httpEndpoint, string? modelName, CancellationToken cancellationToken)
        {
            logger.LogInformation("Pulling ollama model {ModelName}...", modelName);

            var httpClient = new HttpClient { Timeout = TimeSpan.FromDays(1) };
            var request = new HttpRequestMessage(HttpMethod.Post, $"{httpEndpoint.Url}/api/pull")
            {
                Content = JsonContent.Create(new { name = modelName })
            };
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var responseContentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var streamReader = new StreamReader(responseContentStream);
            string? line;
            while ((line = await streamReader.ReadLineAsync(cancellationToken)) is not null)
            {
                logger.Log(LogLevel.Information, 0, line, null, (s, ex) => s);
            }

            logger.LogInformation("Finished pulling ollama model {ModelName}", modelName);
        }

        private sealed record OllamaGetTagsResponse(OllamaGetTagsResponseModel[]? Models);
        private sealed record OllamaGetTagsResponseModel(string Name);
    }

    private sealed class OllamaModelDownloaderResource(string name, OllamaResource ollamaResource) : Resource(name)
    {
        public OllamaResource ollamaResource { get; } = ollamaResource;
    }
}