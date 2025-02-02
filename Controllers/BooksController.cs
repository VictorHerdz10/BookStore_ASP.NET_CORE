using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookStoreApi.DTOs.Books; // Asegúrate de que esta línea esté presente

namespace BookStoreApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooksController(IBookService booksService, ILogger<BooksController> logger) : ControllerBase
{
    private readonly IBookService _booksService = booksService;
    private readonly ILogger<BooksController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponseDto>>> Get()
    {
        try
        {
            var books = await _booksService.GetAllAsync();

            // Convertir cada Book a BookResponseDto
            var response = books.Select(book => new BookResponseDto
            {
                Id = book.Id,
                BookName = book.BookName,
                Price = book.Price,
                Category = book.Category,
                Author = book.Author
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener libros");
            return StatusCode(500, new ErrorResponse { Message = "Ha ocurrido un error al obtener los libros." });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookResponseDto>> Get(string id)
    {
        try
        {
            // Verificar si el libro existe
            bool exists = await _booksService.ExistsAsync(id);

            if (!exists)
            {
                return NotFound(new ErrorResponse { Message = $"Este libro no existe" });
            }

            var book = await _booksService.GetByIdAsync(id);
       if(book==null){
            return NotFound(new ErrorResponse { Message = $"Este libro no existe" });
         }
       // Convertir Book a BookResponseDto
            var response = new BookResponseDto
            {
                Id = book.Id,
                BookName = book.BookName,
                Price = book.Price,
                Category = book.Category,
                Author = book.Author
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener libro con id: {id}");
            return StatusCode(500, new ErrorResponse { Message = $"Ha ocurrido un error al obtener el libro." });
        }
    }

    [HttpPost]
    public async Task<ActionResult<BookResponseDto>> Post([FromBody] CreateBookDto createBookDto)
    {
        try
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new ErrorResponse { Message = "Error de validación del modelo", Errors = validationErrors });
            }

            // Convertir el DTO a un objeto Book
            var newBook = new Book
            {
                BookName = createBookDto.BookName,
                Price = createBookDto.Price,
                Category = createBookDto.Category,
                Author = createBookDto.Author
            };

            // Crear el nuevo libro
            await _booksService.CreateAsync(newBook);

            // Convertir Book a BookResponseDto
            var response = new BookResponseDto
            {
                Id = newBook.Id,
                BookName = newBook.BookName,
                Price = newBook.Price,
                Category = newBook.Category,
                Author = newBook.Author
            };

            // Retornar la respuesta de creación exitosa
            return CreatedAtAction(nameof(Get), new { id = newBook.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear un nuevo libro");
            return StatusCode(500, new ErrorResponse { Message = $"Ha ocurrido un error al crear el libro: {ex.Message}" });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookResponseDto>> Put(string id, [FromBody] UpdateBookDto updateBookDto)
    {
        try
        {
            // Verificar si el libro existe
            bool exists = await _booksService.ExistsAsync(id);

            if (!exists)
            {
                return NotFound(new ErrorResponse { Message = $"No se encontró un libro" });
            }

            // Obtener el libro existente
            var book = await _booksService.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound(new ErrorResponse { Message = $"No se encontró un libro" });
            }

            // Actualizar solo los campos proporcionados en el DTO
            if (updateBookDto.BookName != null)
            {
                book.BookName = updateBookDto.BookName;
            }
            if (updateBookDto.Price.HasValue)
            {
                book.Price = updateBookDto.Price.Value;
            }
            if (updateBookDto.Category != null)
            {
                book.Category = updateBookDto.Category;
            }
            if (updateBookDto.Author != null)
            {
                book.Author = updateBookDto.Author;
            }

            // Guardar los cambios
            await _booksService.UpdateAsync(id, book);

            // Convertir Book a BookResponseDto
            var response = new BookResponseDto
            {
                Id = book.Id,
                BookName = book.BookName,
                Price = book.Price,
                Category = book.Category,
                Author = book.Author
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar libro");
            return BadRequest(new ErrorResponse { Message = $"Ha ocurrido un error al actualizar el libro: {ex.Message}" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            // Verificar si el libro existe
            bool exists = await _booksService.ExistsAsync(id);

            if (!exists)
            {
                return NotFound(new ErrorResponse { Message = $"No se encontró un libro" });
            }

            // Eliminar el libro
            await _booksService.RemoveAsync(id);

            return Ok(new SuccessResponse { Message = $"Libro eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar libro");
            return StatusCode(500, new ErrorResponse { Message = $"Ha ocurrido un error al eliminar el libro." });
        }
    }
}

public class ErrorResponse
{
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; } // Para incluir errores de validación
}

public class SuccessResponse
{
    public string? Message { get; set; }
}