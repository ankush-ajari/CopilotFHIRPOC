using System.Collections.Generic;
using FHIRMetricPOC.API.Models;

namespace FHIRMetricPOC.API.Services
{
    public interface IMetricsService
    {
        MeasureResult CalculateHbA1cMeasure(List<ObservationDto> observations, List<PatientDto> patients);
        MeasureResult CalculateBpMeasure(List<ObservationDto> observations, List<PatientDto> patients);
    }
}
