
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using FHIRMetricPOC.API.Utils;

namespace FHIRMetricPOC.API.Services
{
    public class FhirClientService : IFhirClientService
    {
        private readonly HttpClient _httpClient;
    private readonly string? _fhirBaseUrl;

        public FhirClientService(HttpClient httpClient, IOptions<ConfigSettings> configOptions)
        {
            _httpClient = httpClient;
            _fhirBaseUrl = configOptions.Value.FhirSettings?.BaseUrl;
            if (string.IsNullOrEmpty(_fhirBaseUrl))
                throw new System.ArgumentException("FHIR BaseUrl must be provided in configuration.");
        }

    public async Task<string> GetPatientsAsync(string? id = null, string? name = null)
        {
            if (string.IsNullOrEmpty(_fhirBaseUrl))
                throw new System.InvalidOperationException("FHIR BaseUrl is not configured.");
            var url = $"{_fhirBaseUrl}/Patient";
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(name))
                queryParams.Add($"name={System.Net.WebUtility.UrlEncode(name)}");
            if (!string.IsNullOrEmpty(id))
                queryParams.Add($"_id={System.Net.WebUtility.UrlEncode(id)}");
            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

            /// <summary>
            /// Returns all patients as a JSON string.
            /// </summary>
            public async Task<string> GetAllPatientsAsync()
            {
                // This implementation assumes you have a method to get all patients from the FHIR server.
                // Replace the below with your actual FHIR client logic.
                var url = $"{_fhirBaseUrl}/Patient";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }

    public async Task<string> GetObservationsAsync(string? code = null)
        {
            if (string.IsNullOrEmpty(_fhirBaseUrl))
                throw new System.InvalidOperationException("FHIR BaseUrl is not configured.");
            var url = $"{_fhirBaseUrl}/Observation";
            if (!string.IsNullOrEmpty(code))
            {
                // Support multiple codes separated by comma or pipe, and encode properly
                // Build query as _search?code=code1&code=code2
                var codes = code.Split(new[] { ',', '|' }, System.StringSplitOptions.RemoveEmptyEntries)
                                .Select(c => System.Net.WebUtility.UrlEncode(c.Trim()))
                                .ToList();
                var codeParams = string.Join("&", codes.Select(c => $"code={c}"));
                url += $"/_search?{codeParams}";
            }
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
