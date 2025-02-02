using MongoDB.Driver;
using BookStoreApi.Models;
using Microsoft.Extensions.Logging;

namespace BookStoreApi.Context
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDbContext> _logger;
        public MongoDbContext(string ConnectionString, string DatabaseName, ILogger<MongoDbContext> logger)
        {
            _logger = logger;
            // Conexión a MongoDB
            var client = new MongoClient(ConnectionString);
            _database = client.GetDatabase(DatabaseName);

            try
            {
                // Intenta acceder al cluster para verificar la conexión
                client.Cluster.Description.ToString();
                _logger.LogInformation("Conexión a MongoDB establecida correctamente."); // Log de éxito
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al conectar a MongoDB."); // Log de error
                throw new InvalidOperationException("No se pudo conectar a MongoDB.", ex);
            }
        }

        // Colecciones existentes
        public IMongoCollection<Book> Libros =>
            _database.GetCollection<Book>("books");
        public IMongoCollection<User> Usuarios =>
       _database.GetCollection<User>("users");

    }
}
