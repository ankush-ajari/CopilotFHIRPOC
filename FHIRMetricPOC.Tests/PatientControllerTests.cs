using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using FHIRMetricPOC.API.Controllers;
using FHIRMetricPOC.API.Services;
using FHIRMetricPOC.API.Models;
using System.Net.Http;
using System.IO;
using System.Text.Json;
using System;

namespace FHIRMetricPOC.Tests
{
    public class PatientControllerTests
    {
        [Test]
        public async Task Get_ReturnsPatientsList()
        {
            // Arrange: Use real FhirClientService and TransformerService
            // Use a real HttpClient for actual FHIR server call
            var httpClient = new HttpClient();
            var config = LoadConfigSettings();
            var fhirClient = new FHIRMetricPOC.API.Services.FhirClientService(httpClient, config);
            var transformer = new FHIRMetricPOC.API.Services.TransformerService();
            var controller = new PatientController(fhirClient, transformer);

            // Act
            // Example test values
            string testId = "46741809";
            string testName = "Luis";
            var result = await controller.Get(testId, testName);

            // Assert
            var okResult = result.Result as OkObjectResult;
            NUnit.Framework.Assert.That(okResult, NUnit.Framework.Is.InstanceOf<OkObjectResult>());
            var patients = okResult.Value as List<PatientDto>;
            NUnit.Framework.Assert.That(patients, NUnit.Framework.Is.InstanceOf<List<PatientDto>>());
            NUnit.Framework.Assert.That(patients.Count, NUnit.Framework.Is.GreaterThan(0));
        }

        private static Microsoft.Extensions.Options.IOptions<FHIRMetricPOC.API.Utils.ConfigSettings> LoadConfigSettings()
        {
            // Looks for appsettings.json in the test output directory (copied from API project)
            var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(configPath))
                throw new FileNotFoundException($"Could not find appsettings.json at {configPath}");
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<FHIRMetricPOC.API.Utils.ConfigSettings>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Microsoft.Extensions.Options.Options.Create(config);
        }

        // Helper handler to mock HTTP responses
    // Removed TestHttpMessageHandler: using real HttpClient for integration test
    }
}
