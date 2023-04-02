using AutoMapper;
using Microsoft.Azure.Cosmos;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;

namespace University.Server.Domain.Persistence.Repositories
{
    public class CosmosDbRepository<T1, T2> : ICosmosDbRepository<T1, T2> where T1 : Base where T2 : BaseEntity
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CosmosDbRepository<T1, T2>> _logger;
        private readonly Container _container;

        public CosmosDbRepository(IConfiguration configuration, IMapper mapper, ILogger<CosmosDbRepository<T1, T2>> logger)
        {
            _mapper = mapper;
            _logger = logger;

            var connectionString = configuration["ConnectionString"] ?? throw new ArgumentNullException("ConnectionString");
            var options = new CosmosClientOptions
            {
                IdleTcpConnectionTimeout = new TimeSpan(0, 0, 10, 0),
                MaxRequestsPerTcpConnection = 50, // Maximale Anzahl von Anfragen pro Verbindung
                MaxTcpConnectionsPerEndpoint = 16 // Maximale Anzahl von Verbindungen im Pool
            };
            var cosmosClient = new CosmosClient(connectionString, options);
            var databaseName = "UniversityHub";
            var containerName = typeof(T2).Name;

            var database = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName).Result;
            _container = database.Database.CreateContainerIfNotExistsAsync(containerName, "/id").Result;
        }

        public async Task<IEnumerable<T1>> GetItemsAsync(string query)
        {
            var results = new List<T2>();

            using(var iterator = _container.GetItemQueryIterator<T2>(new QueryDefinition(query)))
            {
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response.ToList());
                }
            };

            return results.Select(r => _mapper.Map<T2, T1>(r));
        }

        public async Task<T1> GetItemAsync(Guid id)
        {
            var response = await _container.ReadItemAsync<T2>(id.ToString(), new PartitionKey(id.ToString()));
            return _mapper.Map<T2, T1>(response.Resource);
        }

        public async Task AddItemAsync(T1 item)
        {
            item.Id = Guid.NewGuid();
            _logger.LogInformation($"Attempting to save new object in database: {item}");
            var itemEntity = _mapper.Map<T1, T2>(item);
            _logger.LogInformation($"Mapped object to entity, creating entity on database: {itemEntity}");
            itemEntity.CreatedAt = DateTime.UtcNow;
            itemEntity.UpdatedAt = DateTime.UtcNow;
            await _container.CreateItemAsync(itemEntity, new PartitionKey(itemEntity.Id));
            _logger.LogInformation("Successfully created entity on database!");
        }

        public async Task UpdateItemAsync(Guid id, T1 item)
        {
            var itemEntity = _mapper.Map<T1, T2>(item);
            itemEntity.UpdatedAt = DateTime.UtcNow;
            await _container.UpsertItemAsync(itemEntity, new PartitionKey(id.ToString()));
        }

        public async Task DeleteItemAsync(Guid id)
        {
            await _container.DeleteItemAsync<T2>(id.ToString(), new PartitionKey(id.ToString()));
        }
    }
}