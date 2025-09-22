using System.Collections.Generic;
using System.Text.Json;
using FHIRMetricPOC.API.Models;

namespace FHIRMetricPOC.API.Services
{
    public class TransformerService : ITransformerService
    {
        public List<PatientDto> TransformPatients(string fhirJson)
        {
            // Example: parse FHIR Bundle and map to PatientDto
            var result = new List<PatientDto>();
            using var doc = JsonDocument.Parse(fhirJson);
            if (doc.RootElement.TryGetProperty("entry", out var entries))
            {
                foreach (var entry in entries.EnumerateArray())
                {
                    var resource = entry.GetProperty("resource");
                    string? id = resource.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String ? idProp.GetString() ?? string.Empty : string.Empty;
                    string? name = string.Empty;
                    if (resource.TryGetProperty("name", out var names) && names.ValueKind == JsonValueKind.Array && names.GetArrayLength() > 0)
                    {
                        var firstName = names[0];
                        if (firstName.ValueKind == JsonValueKind.Object && firstName.TryGetProperty("text", out var text) && text.ValueKind == JsonValueKind.String)
                        {
                            name = text.GetString() ?? string.Empty;
                        }
                    }
                    string? gender = resource.TryGetProperty("gender", out var genderProp) && genderProp.ValueKind == JsonValueKind.String ? genderProp.GetString() : null;
                    string? birthDate = resource.TryGetProperty("birthDate", out var birthDateProp) && birthDateProp.ValueKind == JsonValueKind.String ? birthDateProp.GetString() : null;
                    var patient = new PatientDto
                    {
                        Id = id,
                        Name = name,
                        Gender = gender,
                        BirthDate = birthDate
                    };
                    result.Add(patient);
                }
            }
            return result;
        }

        public List<ObservationDto> TransformObservations(string fhirJson)
        {
            var result = new List<ObservationDto>();
            using var doc = JsonDocument.Parse(fhirJson);
            if (doc.RootElement.TryGetProperty("entry", out var entries))
            {
                foreach (var entry in entries.EnumerateArray())
                {
                    var resource = entry.GetProperty("resource");
                    string? id = resource.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String ? idProp.GetString() : null;
                    string? patientId = null;
                    if (resource.TryGetProperty("subject", out var subject) && subject.ValueKind == JsonValueKind.Object && subject.TryGetProperty("reference", out var reference) && reference.ValueKind == JsonValueKind.String)
                    {
                        patientId = reference.GetString()?.Replace("Patient/", "");
                    }
                    List<string> codes = new List<string>();
                    List<string> displays = new List<string>();
                    if (resource.TryGetProperty("code", out var codeProp) && codeProp.ValueKind == JsonValueKind.Object && codeProp.TryGetProperty("coding", out var codingArr) && codingArr.ValueKind == JsonValueKind.Array && codingArr.GetArrayLength() > 0)
                    {
                        foreach (var coding in codingArr.EnumerateArray())
                        {
                            if (coding.ValueKind == JsonValueKind.Object)
                            {
                                if (coding.TryGetProperty("code", out var codeVal) && codeVal.ValueKind == JsonValueKind.String)
                                {
                                    var codeStr = codeVal.GetString();
                                    if (!string.IsNullOrEmpty(codeStr)) codes.Add(codeStr);
                                }
                                if (coding.TryGetProperty("display", out var displayVal) && displayVal.ValueKind == JsonValueKind.String)
                                {
                                    var displayStr = displayVal.GetString();
                                    if (!string.IsNullOrEmpty(displayStr)) displays.Add(displayStr);
                                }
                            }
                        }
                    }
                    string? unit = null;
                    if (resource.TryGetProperty("valueQuantity", out var valueQuantity) && valueQuantity.ValueKind == JsonValueKind.Object)
                    {
                        if (valueQuantity.TryGetProperty("unit", out var unitProp) && unitProp.ValueKind == JsonValueKind.String)
                            unit = unitProp.GetString();
                    }
                    string? effectiveDateTime = resource.TryGetProperty("effectiveDateTime", out var eff) && eff.ValueKind == JsonValueKind.String ? eff.GetString() : null;
                    var obs = new ObservationDto
                    {
                        Id = id,
                        PatientId = patientId,
                        Codes = codes,
                        Displays = displays,
                        Unit = unit,
                        EffectiveDateTime = effectiveDateTime
                    };
                    result.Add(obs);
                }
            }
            return result;
        }
    }
}
