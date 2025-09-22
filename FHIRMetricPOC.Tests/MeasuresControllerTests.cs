
// ...existing code up to the first MeasuresControllerTests class...
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
using System.Text.Json;

namespace FHIRMetricPOC.Tests
{
    public class MeasuresControllerTests
    {
        [Test]
        public async Task GetHbA1cMeasure_ReturnsMeasureResult()
        {
            // Arrange: Use real HttpClient and services for integration test
            var httpClient = new HttpClient();
            var config = LoadConfigSettings();
            var fhirClient = new FHIRMetricPOC.API.Services.FhirClientService(httpClient, config);
            var transformer = new FHIRMetricPOC.API.Services.TransformerService();
            var metrics = new FHIRMetricPOC.API.Services.MetricsService();
            var controller = new MeasuresController(fhirClient, transformer, metrics);

            // Act
            var result = await controller.GetHbA1cMeasure();

            // Assert
            var okResult = result.Result as OkObjectResult;
            NUnit.Framework.Assert.That(okResult, NUnit.Framework.Is.InstanceOf<OkObjectResult>());
            var measure = okResult.Value as MeasureResult;
            NUnit.Framework.Assert.That(measure, NUnit.Framework.Is.InstanceOf<MeasureResult>());
            NUnit.Framework.Assert.AreEqual("HbA1c < 6%", measure.Measure);
            // Percentage may not be exactly 100 due to test data, so just check type
        }

        [Test]
        public async Task GetBpMeasure_ReturnsMeasureResult()
        {
            // Arrange: Use real HttpClient and services for integration test
            var httpClient = new HttpClient();
            var config = LoadConfigSettings();
            var fhirClient = new FHIRMetricPOC.API.Services.FhirClientService(httpClient, config);
            var transformer = new FHIRMetricPOC.API.Services.TransformerService();
            var metrics = new FHIRMetricPOC.API.Services.MetricsService();
            var controller = new MeasuresController(fhirClient, transformer, metrics);

            // Act & Assert for single BP code
            var resultSingle = await controller.GetBpMeasure(code: "8480-6");
            var okResultSingle = resultSingle.Result as OkObjectResult;
            NUnit.Framework.Assert.That(okResultSingle, NUnit.Framework.Is.InstanceOf<OkObjectResult>());
            var measureSingle = okResultSingle.Value as MeasureResult;
            NUnit.Framework.Assert.That(measureSingle, NUnit.Framework.Is.InstanceOf<MeasureResult>());
            NUnit.Framework.Assert.AreEqual("BP Controlled", measureSingle.Measure);

            // Act & Assert for multiple BP codes (comma separated)
            // Example BP codes: systolic (8480-6), diastolic (8462-4)
            var resultComma = await controller.GetBpMeasure(code: "8480-6,8462-4");
            var okResultComma = resultComma.Result as OkObjectResult;
            NUnit.Framework.Assert.That(okResultComma, NUnit.Framework.Is.InstanceOf<OkObjectResult>());
            var measureComma = okResultComma.Value as MeasureResult;
            NUnit.Framework.Assert.That(measureComma, NUnit.Framework.Is.InstanceOf<MeasureResult>());
            NUnit.Framework.Assert.AreEqual("BP Controlled", measureComma.Measure);

            // Act & Assert for multiple BP codes (pipe separated)
            var resultPipe = await controller.GetBpMeasure(code: "8480-6|8462-4");
            var okResultPipe = resultPipe.Result as OkObjectResult;
            NUnit.Framework.Assert.That(okResultPipe, NUnit.Framework.Is.InstanceOf<OkObjectResult>());
            var measurePipe = okResultPipe.Value as MeasureResult;
            NUnit.Framework.Assert.That(measurePipe, NUnit.Framework.Is.InstanceOf<MeasureResult>());
            NUnit.Framework.Assert.AreEqual("BP Controlled", measurePipe.Measure);
            // Percentage may not be exactly 100 due to test data, so just check type
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

    // Removed TestHttpMessageHandler: using real HttpClient for integration test
    }
}
