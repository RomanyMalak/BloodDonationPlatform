//using BloodDonation.Application.Interfaces;
//using BloodDonation.Infrastructure.Persistence;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace BloodDonation.Infrastructure.Services
//{
//    public sealed class BloodRequestAgentWorker : BackgroundService
//    {
//        private readonly INotificationAgentQueue _queue;
//        private readonly IServiceScopeFactory _scopeFactory;
//        private readonly HttpClient _httpClient;
//        private readonly ILogger<BloodRequestAgentWorker> _logger;

//        private const string LangFlowUrl = "http://localhost:7860/api/v1/run/5788604f-cae4-4ceb-97ff-e626610c1842";
//        public BloodRequestAgentWorker(
//            INotificationAgentQueue queue,
//            IServiceScopeFactory scopeFactory,
//            IHttpClientFactory httpClientFactory,
//            ILogger<BloodRequestAgentWorker> logger)
//        {
//            _queue = queue;
//            _scopeFactory = scopeFactory;
//            _httpClient = httpClientFactory.CreateClient();
//            _logger = logger;
//        }

//        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
//        {
//            await foreach (var bloodRequestId in _queue.ReadAllAsync(cancellationToken))
//            {
//                try
//                {
//                    await ProcessAsync(bloodRequestId, cancellationToken);
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error processing bloodRequestId {Id}", bloodRequestId);
//                }
//            }
//        }

//        private async Task ProcessAsync(Guid bloodRequestId, CancellationToken cancellationToken)
//        {
//            using var scope = _scopeFactory.CreateScope();
//            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//            var bloodRequest = await dbContext.BloodRequests
//                .FirstOrDefaultAsync(x => x.Id == bloodRequestId, cancellationToken);

//            if (bloodRequest is null) return;

//            var requestBody = new
//            {
//                input_value = $"فصيلة الدم المطلوبة: {bloodRequest.RequiredBloodType}",
//                input_type = "chat",
//                output_type = "chat"
//            };

//            var content = new StringContent(
//                JsonSerializer.Serialize(requestBody),
//                Encoding.UTF8,
//                "application/json");

//            var response = await _httpClient.PostAsync(LangFlowUrl, content, cancellationToken);

//            if (response.IsSuccessStatusCode)
//                _logger.LogInformation("Agent processed bloodRequestId {Id}", bloodRequestId);
//            else
//                _logger.LogError("Agent failed for bloodRequestId {Id}", bloodRequestId);
//        }
//    }
//    }
