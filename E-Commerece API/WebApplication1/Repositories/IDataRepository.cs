namespace WebApplication1.Repositories
{
    public interface IDataRepository<T> 
    {
        T? GetById(int id);

        List<T>? GetAll();

        void Add(T item);

        void DeleteById(int id);

        void UpdateById(int id, T item);

    }
}
