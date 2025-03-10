using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class OrderRepository : IDataRepository<Order>
    {
        private AppDbContext _appDbContext = null!;

        public OrderRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Add(Order item)
        {
            _appDbContext.Orders.Add(item);
            _appDbContext.SaveChanges();
        }
        
        public void DeleteById(int id)
        {
            Order? order = _appDbContext.Orders.FirstOrDefault(o => o.Id == id);

            if (order != null)
            {
                _appDbContext.Orders.Remove(order);
                _appDbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Order wasn't found");
            }
        }

        public List<Order>? GetAll()
        {
            return _appDbContext.Orders.ToList();
        }

        public Order? GetById(int id)
        {
            return _appDbContext.Orders.FirstOrDefault(o => o.Id == id);
        }

        public void UpdateById(int id, Order item)
        {
            Order? oldOrder = _appDbContext.Orders.FirstOrDefault(o => o.Id == id);

            if (oldOrder != null)
            {
                oldOrder.ProductID = item.ProductID;
                oldOrder.UserId = item.UserId;
                _appDbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Order wasn't found");
            }
        }
    }
}
