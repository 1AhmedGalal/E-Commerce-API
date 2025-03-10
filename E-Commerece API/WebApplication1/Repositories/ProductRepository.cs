using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class ProductRepository : IDataRepository<Product>
    {
        private AppDbContext _appDbContext = null!;

        public ProductRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Add(Product item)
        {
            _appDbContext.Products.Add(item);
            _appDbContext.SaveChanges();
        }

        public void DeleteById(int id)
        {
            Product? product = _appDbContext.Products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                _appDbContext.Products.Remove(product);
                _appDbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Product wasn't found");
            }
        }

        public List<Product>? GetAll()
        {
            return _appDbContext.Products.ToList();
        }

        public Product? GetById(int id)
        {
            return _appDbContext.Products.FirstOrDefault(p => p.Id == id);
        }

        public void UpdateById(int id, Product item)
        {
            Product? oldProduct = _appDbContext.Products.FirstOrDefault(p => p.Id == id);

            if (oldProduct != null)
            {
                oldProduct.Name = item.Name;
                oldProduct.Description = item.Description;
                oldProduct.Price = item.Price;
                oldProduct.Stock = item.Stock;
                _appDbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Product wasn't found");
            }
        }
    }
}
