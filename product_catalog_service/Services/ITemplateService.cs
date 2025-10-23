using System.Threading;
using System.Threading.Tasks;
using product_catalog_service.Dtos.Template;

namespace product_catalog_service.Services
{
    public interface ITemplateService
    {
        Task<TemplateDto?> GetTemplateByIdAsync(string templateId, CancellationToken cancellationToken = default);
    }
}
