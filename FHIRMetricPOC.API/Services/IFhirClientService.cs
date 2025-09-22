using System.Threading.Tasks;

namespace FHIRMetricPOC.API.Services
{
    public interface IFhirClientService
    {
    Task<string> GetPatientsAsync(string? id = null, string? name = null);
        Task<string> GetObservationsAsync(string? code = null);

    /// <summary>
    /// Returns all patients as a JSON string.
    /// </summary>
    Task<string> GetAllPatientsAsync();
    }
}
