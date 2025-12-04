using InterviewApp.API;

public class Program
{
    public static void Main(string[] args)
    {
#warning убедится что так корректно и безопасно
        AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
                    {
                        var environment = webHostBuilderContext.HostingEnvironment;
                        configurationBuilder
                            .AddJsonFile(Path.Combine(environment.ContentRootPath, "connectionString.json"));

                        configurationBuilder.AddEnvironmentVariables();
                    })
                    .CaptureStartupErrors(true)
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>();
            });
    }
}