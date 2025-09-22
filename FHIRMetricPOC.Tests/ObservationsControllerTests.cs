using System;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FHIRMetricPOC.API.Controllers;
using FHIRMetricPOC.API.Services;
using FHIRMetricPOC.API.Models;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FHIRMetricPOC.Tests
{
    public class ObservationsControllerTests
    {
        [Test]
        public async Task Get_ReturnsObservationsList_SingleCode()
        {
            var httpClient = new HttpClient();
            var config = LoadConfigSettings();
            var fhirClient = new FHIRMetricPOC.API.Services.FhirClientService(httpClient, config);
            var transformer = new FHIRMetricPOC.API.Services.TransformerService();
            var controller = new ObservationsController(fhirClient, transformer);

            // Act
            var result = await controller.Get("4548-4");

            // Assert
            var okResult = result.Result as OkObjectResult;
            NUnit.Framework.Assert.That(okResult, NUnit.Framework.Is.InstanceOf<OkObjectResult>());
            var observations = okResult.Value as List<ObservationDto>;
            NUnit.Framework.Assert.That(observations, NUnit.Framework.Is.InstanceOf<List<ObservationDto>>());
            NUnit.Framework.Assert.IsTrue(observations.Count >= 1);
            NUnit.Framework.Assert.IsTrue(observations.Exists(o => o.Codes != null && o.Codes.Contains("4548-4")));
        }

        [Test]
        public async Task Get_ReturnsObservationsList_MultipleCodes_CommaSeparated()
        {
            var httpClient = new HttpClient();
            var config = LoadConfigSettings();
            var fhirClient = new FHIRMetricPOC.API.Services.FhirClientService(httpClient, config);
            var transformer = new FHIRMetricPOC.API.Services.TransformerService();
            var controller = new ObservationsController(fhirClient, transformer);

            // Act
            var result = await controller.Get("4548-4,1234-5");

            // Assert
            var okResult = result.Result as OkObjectResult;
            NUnit.Framework.Assert.That(okResult, NUnit.Framework.Is.InstanceOf<OkObjectResult>());
            var observations = okResult.Value as List<ObservationDto>;
            NUnit.Framework.Assert.That(observations, NUnit.Framework.Is.InstanceOf<List<ObservationDto>>());
            NUnit.Framework.Assert.IsTrue(observations.Any(o => o.Codes != null && o.Codes.Count > 0));
            NUnit.Framework.Assert.IsTrue(observations.Exists(o => o.Codes != null && o.Codes.Contains("4548-4")) ||
                                         observations.Exists(o => o.Codes != null && o.Codes.Contains("1234-5")));
        }

        [Test]
        public async Task Get_ReturnsObservationsList_MultipleCodes_PipeSeparated()
        {
            var httpClient = new HttpClient();
            var config = LoadConfigSettings();
            var fhirClient = new FHIRMetricPOC.API.Services.FhirClientService(httpClient, config);
            var transformer = new FHIRMetricPOC.API.Services.TransformerService();
            var controller = new ObservationsController(fhirClient, transformer);

            // Act
            var result = await controller.Get("4548-4|1234-5");

            // Assert
            var okResult = result.Result as OkObjectResult;
            NUnit.Framework.Assert.That(okResult, NUnit.Framework.Is.InstanceOf<OkObjectResult>());
            var observations = okResult.Value as List<ObservationDto>;
            NUnit.Framework.Assert.That(observations, NUnit.Framework.Is.InstanceOf<List<ObservationDto>>());
            NUnit.Framework.Assert.IsTrue(observations.Count >= 1);
            NUnit.Framework.Assert.IsTrue(observations.Exists(o => o.Codes != null && o.Codes.Contains("4548-4")) ||
                                         observations.Exists(o => o.Codes != null && o.Codes.Contains("1234-5")));
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

    }
}
