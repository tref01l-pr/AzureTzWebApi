using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tzBlobAzureFunctions.Interfaces;
using tzBlobAzureFunctions.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton(x => new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=tzblobstorageaccount;AccountKey=N8O1wiWHpw0x0M7JF2MsOF5uf5XiNFAvW5k2aC+OCpC0Rp9J7ZBhI4jx4m0A6Xr4a9OQelA7wamQ+AStxJGejA==;EndpointSuffix=core.windows.net"));
        services.AddTransient<IEmailSender, EmailSender>();
    })
    .Build();

host.Run();
