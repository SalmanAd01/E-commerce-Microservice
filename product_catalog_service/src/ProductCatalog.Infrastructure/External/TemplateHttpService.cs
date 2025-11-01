using System.Text.Json;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Abstractions.External;
using ProductCatalog.Application.Dtos.Template;

namespace ProductCatalog.Infrastructure.External
{
    public class TemplateHttpService : ITemplateService
    {
        private readonly HttpClient _client;
        private readonly ILogger<TemplateHttpService> _logger;

        public TemplateHttpService(HttpClient client, ILogger<TemplateHttpService> logger)
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
