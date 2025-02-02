using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.DTOs.Books
{
    public class CreateBookDto
    {
        [Required(ErrorMessage = "El nombre del libro es obligatorio.")]
        public string BookName { get; set; } = null!;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public string Category { get; set; } = null!;

        [Required(ErrorMessage = "El autor es obligatorio.")]
        public string Author { get; set; } = null!;
    }
}