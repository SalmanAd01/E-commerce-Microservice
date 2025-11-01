using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Application.Dtos.Brand
{
    public class UpdateBrandDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Name { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 1)]
        public required string Description { get; set; }

        [Required]
        [Url]
        public required string Logo { get; set; }
    }
}
