using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FHIRMetricPOC.API.Services;
using FHIRMetricPOC.API.Models;
using System.Collections.Generic;

namespace FHIRMetricPOC.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeasuresController : ControllerBase
    {
    private readonly IFhirClientService _fhirClientService;
    private readonly ITransformerService _transformerService;
    private readonly IMetricsService _metricsService;

        public MeasuresController(IFhirClientService fhirClientService, ITransformerService transformerService, IMetricsService metricsService)
        {
            _fhirClientService = fhirClientService;
            _transformerService = transformerService;
            _metricsService = metricsService;
        }


        [HttpGet("patients")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
        {
            var patientsJson = await _fhirClientService.GetAllPatientsAsync();
            var patients = _transformerService.TransformPatients(patientsJson);
            return Ok(patients);
        }

        [HttpGet("hba1c")]
        public async Task<ActionResult<MeasureResult>> GetHbA1cMeasure()
        {
            var patientsJson = await _fhirClientService.GetAllPatientsAsync();
            var obsJson = await _fhirClientService.GetObservationsAsync("4548-4"); // HbA1c LOINC code
            var patients = _transformerService.TransformPatients(patientsJson);
            var observations = _transformerService.TransformObservations(obsJson);
            var result = _metricsService.CalculateHbA1cMeasure(observations, patients);
            return Ok(result);
        }

        [HttpGet("bp")]
        public async Task<ActionResult<MeasureResult>> GetBpMeasure([FromQuery] string? code = null)
        {
            var patientsJson = await _fhirClientService.GetAllPatientsAsync();
            var obsJson = await _fhirClientService.GetObservationsAsync(code);
            var patients = _transformerService.TransformPatients(patientsJson);
            var observations = _transformerService.TransformObservations(obsJson);
            var result = _metricsService.CalculateBpMeasure(observations, patients);
            return Ok(result);
        }
    }
}
