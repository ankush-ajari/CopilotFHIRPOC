using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Collections.Generic;
using System.Text.Json;
using FHIRMetricPOC.API.Services;
using FHIRMetricPOC.API.Models;

namespace FHIRMetricPOC.API.Functions
{
    public class MeasuresFunctions
    {
        private readonly IFhirClientService _fhirClientService;
        private readonly ITransformerService _transformerService;
        private readonly IMetricsService _metricsService;
        private readonly ILogger _logger;

        public MeasuresFunctions(IFhirClientService fhirClientService, ITransformerService transformerService, IMetricsService metricsService, ILoggerFactory loggerFactory)
        {
            _fhirClientService = fhirClientService;
            _transformerService = transformerService;
            _metricsService = metricsService;
            _logger = loggerFactory.CreateLogger<MeasuresFunctions>();
        }

        [Function("GetAllPatients")]
        public async Task<HttpResponseData> GetAllPatients(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "measures/patients")] HttpRequestData req)
        {
            var patientsJson = await _fhirClientService.GetAllPatientsAsync();
            var patients = _transformerService.TransformPatients(patientsJson);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(patients);
            return response;
        }

        [Function("GetHbA1cMeasure")]
        public async Task<HttpResponseData> GetHbA1cMeasure(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "measures/hba1c")] HttpRequestData req)
        {
            var patientsJson = await _fhirClientService.GetAllPatientsAsync();
            var obsJson = await _fhirClientService.GetObservationsAsync("4548-4"); // HbA1c LOINC code
            var patients = _transformerService.TransformPatients(patientsJson);
            var observations = _transformerService.TransformObservations(obsJson);
            var result = _metricsService.CalculateHbA1cMeasure(observations, patients);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }

        [Function("GetBpMeasure")]
        public async Task<HttpResponseData> GetBpMeasure(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "measures/bp")] HttpRequestData req)
        {
            // Accept 'code' as a query parameter, supporting comma or pipe separated values
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string code = query["code"] ?? string.Empty;
            var patientsJson = await _fhirClientService.GetAllPatientsAsync();
            var obsJson = await _fhirClientService.GetObservationsAsync(code);
            var patients = _transformerService.TransformPatients(patientsJson);
            var observations = _transformerService.TransformObservations(obsJson);
            var result = _metricsService.CalculateBpMeasure(observations, patients);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }

        [Function("GetPatients")]
        public async Task<HttpResponseData> GetPatients(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "patients")] HttpRequestData req)
        {
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string? id = query["id"];
            string? name = query["name"];
            var patientsJson = await _fhirClientService.GetPatientsAsync(id, name);
            var patients = _transformerService.TransformPatients(patientsJson);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(patients);
            return response;
        }

        [Function("GetObservations")]
        public async Task<HttpResponseData> GetObservations(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "observations")] HttpRequestData req)
        {
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string? code = query["code"];
            var obsJson = await _fhirClientService.GetObservationsAsync(code);
            var observations = _transformerService.TransformObservations(obsJson);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(observations);
            return response;
        }
    }
}