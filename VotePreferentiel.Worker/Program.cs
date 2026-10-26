using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VotePreferentiel;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<VoteProvider>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHostedService<VotePreferentiel.Worker.Worker>();

var host = builder.Build();

await host.RunAsync();