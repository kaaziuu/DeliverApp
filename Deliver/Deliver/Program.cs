using Deliver;
using Mcrio.Configuration.Provider.Docker.Secrets;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configBuilder =>
    {
        configBuilder
            .AddDockerSecrets();
    })
    .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); }).Build().Run();
