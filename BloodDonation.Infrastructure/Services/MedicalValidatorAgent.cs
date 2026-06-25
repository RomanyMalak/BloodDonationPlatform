using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BloodDonation.Infrastructure.Services
{
    public class MedicalValidatorAgent : IMedicalValidatorAgent
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MedicalValidatorAgent(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<string>> GetCompatibleBloodTypesAsync(
            string patientBloodType,
            List<string> availableBloodTypes,
            CancellationToken cancellationToken)
        {
            var input = new
            {
                patient_blood_type = patientBloodType,
                available_blood_types = availableBloodTypes
            };

            var body = new
            {
                output_type = "chat",
                input_type = "chat",
                input_value = JsonSerializer.Serialize(input)
            };

            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add(
                "x-api-key",
                _configuration["LangflowSettings:ApiKey"]);

            var url =
                $"{_configuration["LangflowSettings:BaseUrl"]}/api/v1/run/{_configuration["LangflowSettings:FlowId"]}?stream=false";

            var response = await _httpClient.PostAsJsonAsync(
                url,
                body,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);

                throw new Exception(
                    $"Langflow Error: {(int)response.StatusCode} - {error}");
            }

            var responseContent =
                await response.Content.ReadAsStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(responseContent);

            var aiText = doc
                .RootElement
                .GetProperty("outputs")[0]
                .GetProperty("outputs")[0]
                .GetProperty("results")
                .GetProperty("message")
                .GetProperty("text")
                .GetString();

            var compatibleBloodTypes = JsonSerializer.Deserialize<CompatibleBloodTypesDto>(aiText);



            return compatibleBloodTypes?.CompatibleBloodTypes ?? new List<string>();

        }
    }
    }

