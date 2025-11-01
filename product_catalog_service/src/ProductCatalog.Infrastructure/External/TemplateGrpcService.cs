using System.Threading;
using System.Threading.Tasks;
using CategoryTemplate.Grpc;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Abstractions.External;
using ProductCatalog.Application.Dtos.Template;

namespace ProductCatalog.Infrastructure.External
{
    public class TemplateGrpcService : ITemplateService
    {
        private readonly CategoryTemplateService.CategoryTemplateServiceClient _client;
        private readonly ILogger<TemplateGrpcService> _logger;

        public TemplateGrpcService(CategoryTemplateService.CategoryTemplateServiceClient client, ILogger<TemplateGrpcService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<TemplateDto?> GetTemplateByIdAsync(string templateId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(templateId)) return null;
            if (!long.TryParse(templateId, out var id)) return null;

            try
            {
                var resp = await _client.GetTemplateByIdAsync(new GetTemplateByIdRequest { Id = id }, cancellationToken: cancellationToken).ConfigureAwait(false);
                return Map(resp.Template);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "gRPC error fetching template {TemplateId}", templateId);
                return null;
            }
        }

        private static TemplateDto Map(Template t)
        {
            var dto = new TemplateDto
            {
                Id = (int)t.Id,
                Name = t.Name,
                Description = t.Description,
                Department = t.Department != null ? new DepartmentDto { Id = (int)t.Department.Id, Name = t.Department.Name } : null,
                Category = t.Category != null ? new CategoryDto { Id = (int)t.Category.Id, Name = t.Category.Name } : null,
                Attributes = new System.Collections.Generic.List<TemplateAttributeDto>()
            };
            foreach (var a in t.Attributes)
            {
                dto.Attributes.Add(new TemplateAttributeDto
                {
                    Id = (int)a.Id,
                    Name = a.Name,
                    DataType = a.DataType.ToString()
                });
            }
            return dto;
        }
    }
}
