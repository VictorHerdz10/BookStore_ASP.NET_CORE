using BookStoreApi.Context;
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Services;
public interface IBookService
{
    Task<List<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(string id);
    Task CreateAsync(Book product);
    Task UpdateAsync(string id, Book book);
    Task RemoveAsync(string id);
    Task<bool> ExistsAsync(string id);
}
public class BooksService(MongoDbContext context) : IBookService
{
    private readonly IMongoCollection<Book> _booksCollection = context.Libros;

    //Listar todos los libros
    public async Task<List<Book>> GetAllAsync() =>
        await _booksCollection.Find(_ => true).ToListAsync();

    public async Task<Book?> GetByIdAsync(string id) =>
        await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Book newBook) =>
        await _booksCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(string id, Book updatedBook) =>
        await _booksCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _booksCollection.DeleteOneAsync(x => x.Id == id);

    public async Task<bool> ExistsAsync(string id)
    {
        var filter = Builders<Book>.Filter.Eq(b => b.Id, id);
        var result = await _booksCollection.Find(filter).FirstOrDefaultAsync();
        return result != null;
    }
}