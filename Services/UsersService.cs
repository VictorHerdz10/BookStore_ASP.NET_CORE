using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using BCrypt.Net;

namespace BookStoreApi.Services;

public class UsersService
{
    private readonly IMongoCollection<User> _usersCollection;

    public UsersService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            bookStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookStoreDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<User>("Users");
    }

    // Crear índice único para email
    public async Task CreateIndexes()
    {
        var indexKeysDefinition = Builders<User>.IndexKeys.Ascending(user => user.Email);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var model = new CreateIndexModel<User>(indexKeysDefinition, indexOptions);
        await _usersCollection.Indexes.CreateOneAsync(model);
    }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

    public async Task<User?> GetByIdAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<User> CreateAsync(User newUser)
    {
        // Cambiar BCrypt.HashPassword a BCrypt.Net.BCrypt.HashPassword
        newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
        await _usersCollection.InsertOneAsync(newUser);
        return newUser;
    }

    public async Task UpdateAsync(string id, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    public async Task RemoveAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.Id == id);

    public bool ValidatePassword(User user, string password) =>
        BCrypt.Net.BCrypt.Verify(password, user.Password);
} 