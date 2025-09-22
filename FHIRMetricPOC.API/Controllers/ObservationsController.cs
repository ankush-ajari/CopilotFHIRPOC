using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FHIRMetricPOC.API.Services;
using FHIRMetricPOC.API.Models;
using System.Collections.Generic;

namespace FHIRMetricPOC.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ObservationsController : ControllerBase
    {
    private readonly IFhirClientService _fhirClientService;
    private readonly ITransformerService _transformerService;

        public ObservationsController(IFhirClientService fhirClientService, ITransformerService transformerService)
        {
            _fhirClientService = fhirClientService;
            _transformerService = transformerService;
        }

        [HttpGet]
    public async Task<ActionResult<List<ObservationDto>>> Get([FromQuery] string? code = null)
        {
            // 'code' can be a single code or multiple codes separated by comma or pipe (OR logic)
            // Example: ?code=1234-5,6789-0 or ?code=1234-5|6789-0
            var fhirJson = await _fhirClientService.GetObservationsAsync(code);
            var observations = _transformerService.TransformObservations(fhirJson);
            return Ok(observations);
        }
    }
}
