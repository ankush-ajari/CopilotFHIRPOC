using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FHIRMetricPOC.API.Services;
using FHIRMetricPOC.API.Utils;

public class Program
{
	public static void Main()
	{
		var host = new HostBuilder()
			.ConfigureFunctionsWorkerDefaults()
			.ConfigureServices((context, services) =>
			{
				// Bind ConfigSettings from appsettings.json
				services.Configure<ConfigSettings>(context.Configuration);

				// Register custom services
				services.AddHttpClient<IFhirClientService, FhirClientService>();
				services.AddScoped<ITransformerService, TransformerService>();
				services.AddScoped<IMetricsService, MetricsService>();
			})
			.Build();

		host.Run();
	}
}

