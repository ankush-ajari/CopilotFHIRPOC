# FHIRMetricPOC

A .NET 7 Azure Functions project for interacting with FHIR (Fast Healthcare Interoperability Resources) servers, providing APIs to retrieve patient and observation data for healthcare metrics and analytics.

## Features
- Retrieve patient data from a FHIR server
- Retrieve observation data (with support for filtering by code)
- Extensible service-based architecture
- Configuration-driven (via `appsettings.json`)
- Unit tests included

## Project Structure
```
FHIRMetricPOC.sln
FHIRMetricPOC.API/           # Main Azure Functions API project
  Controllers/              # API controllers (Patient, Observations, Measures)
  Functions/                # Azure Functions endpoints
  Models/                   # DTOs and data models
  Services/                 # Service interfaces and implementations
  Utils/                    # Utility classes (e.g., configuration)
  appsettings.json          # Configuration file
  Program.cs                # Host and DI setup
  ...
FHIRMetricPOC.Tests/        # Unit tests for API controllers and services
publish/                    # Published output (for deployment)
```

## Getting Started

### Prerequisites
- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- Azure Functions Core Tools (for local development)

### Configuration
1. Update `FHIRMetricPOC.API/appsettings.json` with your FHIR server base URL and any other required settings.
2. Example:
   ```json
   {
     "FhirSettings": {
       "BaseUrl": "https://your-fhir-server-url"
     }
   }
   ```

### Build and Run Locally
1. Restore dependencies:
   ```powershell
   dotnet restore
   ```
2. Build the solution:
   ```powershell
   dotnet build
   ```
3. Run the Azure Functions host:
   ```powershell
   func start
   ```

### Running Tests
1. Navigate to the test project directory:
   ```powershell
   cd FHIRMetricPOC.Tests
   ```
2. Run tests:
   ```powershell
   dotnet test
   ```

-## Metrics Calculated

### HbA1c (<6%)
- **Clinical Meaning:** HbA1c (Hemoglobin A1c) is a measure of average blood glucose over the past 2-3 months. An HbA1c value less than 6% is considered well-controlled for many patients with diabetes.
- **How Calculated:** The project identifies patients with at least one observation with LOINC code `4548-4` (HbA1c) and a value (Display) less than 6. The percentage is calculated as:
   > (Number of patients with HbA1c < 6%) / (Total number of patients) × 100

### Blood Pressure (BP Controlled)
- **Clinical Meaning:** Blood pressure control is defined as having a systolic BP < 140 mmHg and diastolic BP < 90 mmHg. This is a common clinical target for hypertension management.
- **How Calculated:** The project groups observations by patient and checks for:
   - At least one systolic BP observation (LOINC `8480-6`) with value < 140
   - At least one diastolic BP observation (LOINC `8462-4`) with value < 90
   - The percentage is:
   > (Number of patients with BP < 140/90) / (Total number of patients) × 100

These calculations are implemented in `Services/MetricsService.cs`.

## Key Files
- `Program.cs` – Configures DI and services
- `Services/FhirClientService.cs` – Handles FHIR API calls
- `Controllers/` – API endpoints for patients, observations, and measures
- `appsettings.json` – Configuration for FHIR server and other settings

## Deployment
1. Publish the project:
   ```powershell
   dotnet publish FHIRMetricPOC.API/FHIRMetricPOC.API.csproj --configuration Release
   ```
2. Deploy the contents of the `publish/` directory to your Azure Function App or preferred hosting environment.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
This project is licensed under the MIT License.
