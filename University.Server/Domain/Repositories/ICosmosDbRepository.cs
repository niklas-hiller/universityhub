namespace University.Server.Domain.Repositories
{
    public interface ICosmosDbRepository<T1, T2> where T1 : class where T2 : class
    {
        Task<IEnumerable<T1>> GetItemsAsync(string query);

        Task<T1> GetItemAsync(Guid id);

        Task AddItemAsync(T1 item);

        Task UpdateItemAsync(Guid id, T1 item);

        Task DeleteItemAsync(Guid id);
    }
}