using Dapper;
using Npgsql;
using PostRedisCRUD.Entity;
using StackExchange.Redis;
using System.Text.Json;

namespace PostRedisCRUD.Repositories;

public class ProductRepository : IProductRepository
{

    #region Ctor

    private readonly ILogger<ProductRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDatabase _redis;
    public ProductRepository(ILogger<ProductRepository> logger, IConfiguration configuration, IConnectionMultiplexer connectionMultiplexer)
    {
        _logger = logger;
        _configuration = configuration;
        _redis = connectionMultiplexer.GetDatabase();
    }

    #endregion

    #region Create
    public async Task AddNewProduct(Product product)
    {
        using var _context = new NpgsqlConnection(_configuration.GetConnectionString("PostgresqlConnectionString"));

        await _context.ExecuteAsync("INSERT INTO Product(Name, Description, Price) VALUES (@Name, @Description, @Price)", new { Name = product.Name, Description = product.Description, Price = product.Price });

    }
    #endregion

    #region Get Product by id

    public async Task<Product> GetProductById(int Id)
    {

        #region Get Redis Cache

        var cache = await _redis.StringGetAsync($"product:{Id}");
        if (!string.IsNullOrEmpty(cache))
        {
            var serialized = JsonSerializer.Deserialize<Product>(cache)!;
            Console.WriteLine("This is from Redis cache : " + JsonSerializer.Serialize(serialized) );
            return serialized;

        }
        #endregion

        #region Get From postgre
        using var _context = new NpgsqlConnection(_configuration.GetConnectionString("PostgresqlConnectionString"));

        var product = await _context.QueryFirstOrDefaultAsync<Product>("SELECT * FROM Product WHERE id = @Id", new { id = Id });

        #endregion

        #region Set Redis Cache
        if (product != null)
        {
            await _redis.StringSetAsync($"product:{Id}", System.Text.Json.JsonSerializer.Serialize(product), TimeSpan.FromSeconds(30));

        }
        #endregion


        return product ?? new Product
        {
            Id = 0,
            Name = "Not Found",
            Description = "Not Found",
            Price = 0
        };

    }
    #endregion

    #region Get List Of Products

    public async Task<List<Product>> GetProducts()
    {

        #region get Redis cache

        var cache = await _redis.StringGetAsync("products");
        if (!string.IsNullOrEmpty(cache))
        {
            var serialized = JsonSerializer.Deserialize<List<Product>>(cache)!;
            _logger.LogInformation($"This is from Redis cache : {JsonSerializer.Serialize(serialized) }");
            _logger.LogInformation("test");
            return serialized;
        }
        #endregion

        #region Get From postgre 
        using var _context = new NpgsqlConnection(_configuration.GetConnectionString("PostgresqlConnectionString"));

        var products = await _context.QueryAsync<Product>("SELECT * FROM Product");

        #endregion

        #region Set to redis
        if (products.Any())
        {
            await _redis.StringSetAsync("products", JsonSerializer.Serialize(products) , TimeSpan.FromSeconds(30));
        }
        #endregion


        return products.ToList();
    }
    #endregion

    #region Update
    public async Task UpdateProduct(Product product)
    {
        using var _context = new NpgsqlConnection(_configuration.GetConnectionString("PostgresqlConnectionString"));

        await _context.ExecuteAsync("UPDATE Product SET Name = @Name , Description = @Description , Price = @Price WHERE id = @Id",
            new { Name = product.Name, Description = product.Description, Price = product.Price, id = product.Id });

        // set redis
        await _redis.StringSetAsync($"product:{product.Id}", JsonSerializer.Serialize(product) , TimeSpan.FromSeconds(30));
    }
    #endregion

    #region Delete
    public async Task DeleteProduct(int Id)
    {
        using var _context = new NpgsqlConnection(_configuration.GetConnectionString("PostgresqlConnectionString"));

        await _context.ExecuteAsync("DELETE FROM Product WHERE id = @Id", new { id = Id });
                                     
        await _redis.KeyDeleteAsync($"product:{Id}");


    }

    #endregion


}