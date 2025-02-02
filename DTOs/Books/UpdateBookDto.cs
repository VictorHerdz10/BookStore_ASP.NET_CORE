using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.DTOs.Books
{
    public class UpdateBookDto
    {
        [StringLength(100, ErrorMessage = "El nombre del libro no puede exceder los 100 caracteres.")]
        public string? BookName { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0.")]
        public decimal? Price { get; set; }

        [StringLength(50, ErrorMessage = "La categoría no puede exceder los 50 caracteres.")]
        public string? Category { get; set; }

        [StringLength(100, ErrorMessage = "El autor no puede exceder los 100 caracteres.")]
        public string? Author { get; set; }
    }
}