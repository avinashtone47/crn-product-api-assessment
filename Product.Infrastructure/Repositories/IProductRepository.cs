using System.Collections.Generic;
using System.Threading.Tasks;

namespace Product.Infrastructure.Repositories
{
    public interface IProductRepository : IRepository<Product.Domain.Entities.Product>
    {
        Task<Product.Domain.Entities.Product?> GetByIdWithItemsAsync(int id);
        Task<IEnumerable<Product.Domain.Entities.Product>> GetPagedAsync(int pageNumber, int pageSize);
        Task<int> CountAsync();
    }
}