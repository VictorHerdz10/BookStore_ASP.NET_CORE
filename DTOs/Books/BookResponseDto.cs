namespace BookStoreApi.DTOs.Books
{
    public class BookResponseDto
    {
        public string? Id { get; set; } = null!;
        public string BookName { get; set; } = null!;
        public decimal Price { get; set; }
        public string Category { get; set; } = null!;
        public string Author { get; set; } = null!;
    }
}