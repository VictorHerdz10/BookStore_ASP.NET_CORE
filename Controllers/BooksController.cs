using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _booksService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService booksService, ILogger<BooksController> logger)
    {
        _booksService = booksService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> Get()
    {
        try
        {
            var books = await _booksService.GetAllAsync();
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener libros");
            return StatusCode(500, new ErrorResponse { Message = "Ha ocurrido un error al obtener los libros." });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> Get(string id)
    {
        try
        {
            // Primero, verificamos si el libro existe en la base de datos
            bool exists = await _booksService.ExistsAsync(id);

            if (!exists)
            {
                return NotFound(new ErrorResponse { Message = $"Este libro no existe" });
            }

            var book = await _booksService.GetByIdAsync(id);

            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener libro con id: {id}");
            return StatusCode(500, new ErrorResponse { Message = $"Ha ocurrido un error al obtener el libro." });
        }
    }
    [HttpPost]
    public async Task<ActionResult<Book>> Post([FromBody] Book newBook)
    {
        try
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine(validationErrors);
                return BadRequest(new ErrorResponse { Message = "Error de validación del modelo" });
            }

            // Crear el nuevo libro
            await _booksService.CreateAsync(newBook);

            // Retornar la respuesta de creación exitosa
            return CreatedAtAction(nameof(Get), new { id = newBook.Id }, new SuccessResponse { Message = $"Libro creado satisfactoriamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear un nuevo libro");
            return StatusCode(500, new ErrorResponse { Message = $"Ha ocurrido un error al crear el libro: {ex.Message}" });
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Book updatedBook)
    {
        try
        {
            // Verificar si el libro existe antes de actualizar
            bool exists = await _booksService.ExistsAsync(id);

            if (!exists)
            {
                return NotFound(new ErrorResponse { Message = $"No se encontró un libro " });
            }

            // Actualizar el libro
            var book = await _booksService.GetByIdAsync(id);
            if (book == null)
            {
                // Esto debería ser imposible si ExistsAsync devolvió true,
                // pero lo dejamos aquí por precaución
                return NotFound(new ErrorResponse { Message = $"No se encontró un libro" });
            }

            updatedBook.Id = book.Id; // Asegurarse de que el Id no cambie
            await _booksService.UpdateAsync(id, updatedBook);

            return Ok(new SuccessResponse { Message = $"Libro  actualizado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar libro ");
            return BadRequest(new ErrorResponse { Message = $"Ha ocurrido un error al actualizar el libro: {ex.Message}" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            // Verificar si el libro existe antes de eliminar
            bool exists = await _booksService.ExistsAsync(id);

            if (!exists)
            {
                return NotFound(new ErrorResponse { Message = $"No se encontró un libro" });
            }

            // Eliminar el libro
            var book = await _booksService.GetByIdAsync(id);
            if (book == null)
            {
                // Esto debería ser imposible si ExistsAsync devolvió true,
                // pero lo dejamos aquí por precaución
                return NotFound(new ErrorResponse { Message = $"No se encontró un libro " });
            }

            await _booksService.RemoveAsync(id);

            return Ok(new SuccessResponse { Message = $"Libro  eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar libro");
            return StatusCode(500, new ErrorResponse { Message = $"Ha ocurrido un error al eliminar ." });
        }
    }
}

public class ErrorResponse
{
    public string? Message { get; set; }
}

public class SuccessResponse
{
    public string? Message { get; set; }
}