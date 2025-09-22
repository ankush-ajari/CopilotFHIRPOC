
namespace FHIRMetricPOC.API.Utils
{
	   public class FhirSettings
	   {
		   public string? BaseUrl { get; set; }
	   }

	   public class ConfigSettings
	   {
		   public FhirSettings? FhirSettings { get; set; }
	   }
}
