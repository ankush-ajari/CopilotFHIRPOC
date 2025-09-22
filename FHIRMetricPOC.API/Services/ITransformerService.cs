using System.Collections.Generic;
using FHIRMetricPOC.API.Models;

namespace FHIRMetricPOC.API.Services
{
    public interface ITransformerService
    {
        List<PatientDto> TransformPatients(string fhirJson);
        List<ObservationDto> TransformObservations(string fhirJson);
    }
}
