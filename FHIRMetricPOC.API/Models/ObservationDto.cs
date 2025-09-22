namespace FHIRMetricPOC.API.Models
{
    public class ObservationDto
    {
    public string? Id { get; set; }
    public string? PatientId { get; set; }
    public List<string>? Codes { get; set; } // All codes from coding array
    public List<string>? Displays { get; set; } // All displays from coding array
    public string? Unit { get; set; }
    public string? EffectiveDateTime { get; set; }
    }
}
