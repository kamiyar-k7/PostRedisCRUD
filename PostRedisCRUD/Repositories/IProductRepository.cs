using PostRedisCRUD.Entity;

namespace PostRedisCRUD.Repositories;

public interface IProductRepository
{
    Task<Product> GetProductById(int Id);
    Task<List<Product>> GetProducts();
    Task AddNewProduct(Product product);
    Task UpdateProduct(Product product);
    Task DeleteProduct(int Id);
}
