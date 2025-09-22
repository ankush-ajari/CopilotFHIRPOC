using System.Collections.Generic;
using System.Linq;
using FHIRMetricPOC.API.Services;
using FHIRMetricPOC.API.Models;

namespace FHIRMetricPOC.API.Services
{
    public class MetricsService : IMetricsService
    {
        public MeasureResult CalculateHbA1cMeasure(List<ObservationDto> observations, List<PatientDto> patients)
        {
            // HbA1c code: 4548-4, use Display as value
            var hba1cObs = observations.Where(o => o.Codes != null && o.Codes.Contains("4548-4") && o.Displays != null && o.Displays.Any(d => double.TryParse(d, out var v) && v < 6)).Select(o => o.PatientId).Distinct();
            var percent = patients.Count == 0 ? 0 : (hba1cObs.Count() * 100.0) / patients.Count;
            return new MeasureResult
            {
                Measure = "HbA1c < 6%",
                Percentage = percent,
                Description = "Percentage of patients with HbA1c < 6%"
            };
        }

        public MeasureResult CalculateBpMeasure(List<ObservationDto> observations, List<PatientDto> patients)
        {
            // Systolic: 8480-6, Diastolic: 8462-4, use Display as value
            var bpGroups = observations.Where(o => o.Codes != null && (o.Codes.Contains("8480-6") || o.Codes.Contains("8462-4")))
                .GroupBy(o => o.PatientId)
                .Where(g => g.Any(o => o.Codes != null && o.Codes.Contains("8480-6") && o.Displays != null && o.Displays.Any(d => double.TryParse(d, out var sys) && sys < 140))
                    && g.Any(o => o.Codes != null && o.Codes.Contains("8462-4") && o.Displays != null && o.Displays.Any(d => double.TryParse(d, out var dia) && dia < 90)));
            var percent = patients.Count == 0 ? 0 : (bpGroups.Count() * 100.0) / patients.Count;
            return new MeasureResult
            {
                Measure = "BP Controlled",
                Percentage = percent,
                Description = "Percentage of patients with BP < 140/90"
            };
        }
    }
}
