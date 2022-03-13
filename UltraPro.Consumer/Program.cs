using UltraPro.Consumer;
using UltraPro.MassTransit.Configuration;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.ConfigureReceiver();
    })
    .Build();

await host.RunAsync();
