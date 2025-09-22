using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FHIRMetricPOC.API.Services;
using FHIRMetricPOC.API.Models;
using System.Collections.Generic;

namespace FHIRMetricPOC.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {
    private readonly IFhirClientService _fhirClientService;
    private readonly ITransformerService _transformerService;

        public PatientController(IFhirClientService fhirClientService, ITransformerService transformerService)
        {
            _fhirClientService = fhirClientService;
            _transformerService = transformerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PatientDto>>> Get([FromQuery] string? id = null, [FromQuery] string? name = null)
        {
            var fhirJson = await _fhirClientService.GetPatientsAsync(id, name);
            var patients = _transformerService.TransformPatients(fhirJson);
            return Ok(patients);
        }
    }
}
