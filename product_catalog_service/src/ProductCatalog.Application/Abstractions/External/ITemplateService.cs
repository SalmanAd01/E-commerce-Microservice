using System.Threading;
using System.Threading.Tasks;
using ProductCatalog.Application.Dtos.Template;

namespace ProductCatalog.Application.Abstractions.External
{
    public interface ITemplateService
    {
        Task<TemplateDto?> GetTemplateByIdAsync(string templateId, CancellationToken cancellationToken = default);
    }
}
