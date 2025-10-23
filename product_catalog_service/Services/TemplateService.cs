using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using product_catalog_service.Dtos.Template;

namespace product_catalog_service.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly HttpClient _client;
        private readonly ILogger<TemplateService> _logger;

        public TemplateService(HttpClient client, ILogger<TemplateService> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TemplateDto?> GetTemplateByIdAsync(string templateId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(templateId)) return null;

            try
            {
                using var resp = await _client.GetAsync($"api/v1/templates/{templateId}", cancellationToken).ConfigureAwait(false);
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Template service returned {Status} for id {TemplateId}", resp.StatusCode, templateId);
                    return null;
                }

                var json = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var template = JsonSerializer.Deserialize<TemplateDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching template {TemplateId}", templateId);
                return null;
            }
        }
    }
}
