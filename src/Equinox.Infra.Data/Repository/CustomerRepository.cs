using System;
using System.Linq;
using Equinox.Domain.Interfaces;
using Equinox.Domain.Models;
using Microsoft.Azure.Cosmos;

namespace Equinox.Infra.Data.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CosmosClient _client;
        private readonly Container _customersContainer;
        private readonly Container _ordersContainer;

        public CustomerRepository(CosmosClient client)
        {
            _client = client;
            _customersContainer = _client.GetContainer("Equinox","Customers");
            _ordersContainer = _client.GetContainer("Equinox", "Orders");
        }

        public Customer GetByEmail(string email)
        {
            return _customersContainer.GetItemLinqQueryable<Customer>(allowSynchronousQueryExecution: true)
                .Where(c => c.Email == email)
                .Take(1)
                .ToList()
                .FirstOrDefault();
        }

        public IQueryable<Order> OrdersFor(Guid id)
        {
            var customerId = id.ToString();

            return _ordersContainer.GetItemLinqQueryable<Order>(allowSynchronousQueryExecution: true)
                .Where(c => c.CustomerId == customerId);
        }

        public int OrdersCount(Guid id)
        {
            var query = new QueryDefinition(@"SELECT value count(1) FROM root WHERE root.CustomerId = @cust")
                .WithParameter("@cust", id.ToString());
            var it = _ordersContainer.GetItemQueryIterator<int>(query);
            return it.ReadNextAsync().Result.FirstOrDefault();
        }

        public void Dispose()
        {
            
        }

        public void Add(Customer obj)
        {
            _customersContainer.CreateItemAsync(obj).Wait();
        }

        public Customer GetById(Guid id)
        {
            try
            {
                return _customersContainer.ReadItemAsync<Customer>(id.ToString(), new PartitionKey(id.ToString())).Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public IQueryable<Customer> GetAll()
        {
            return _customersContainer.GetItemLinqQueryable<Customer>(allowSynchronousQueryExecution: true);
        }

        public void Update(Customer obj)
        {
            _customersContainer.UpsertItemAsync(obj).Wait();
        }

        public void Remove(Guid id)
        {
            _customersContainer.DeleteItemAsync<Customer>(id.ToString(), PartitionKey.None).Wait();
        }

        public int SaveChanges()
        {
            return 1;
        }
    }
}
