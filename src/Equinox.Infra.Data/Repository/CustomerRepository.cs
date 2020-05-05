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
        private Container _container;

        public CustomerRepository(CosmosClient client)
        {
            _client = client;
            _container = _client.GetContainer("Equinox","Customers");
        }

        public Customer GetByEmail(string email)
        {
            return _container.GetItemLinqQueryable<Customer>(allowSynchronousQueryExecution: true)
                .Where(c => c.Email == email)
                .Take(1)
                .ToList()
                .FirstOrDefault();
        }

        public IQueryable<Order> OrdersFor(Guid id)
        {
            var customerId = id.ToString();

            return _container.GetItemLinqQueryable<Order>(allowSynchronousQueryExecution: true)
                .Where(c => c.CustomerId == customerId);
        }

        public void Dispose()
        {
            
        }

        public void Add(Customer obj)
        {
            _container.CreateItemAsync(obj).Wait();
        }

        public Customer GetById(Guid id)
        {
            try
            {
                return _container.ReadItemAsync<Customer>(id.ToString(), new PartitionKey(id.ToString())).Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public IQueryable<Customer> GetAll()
        {
            return _container.GetItemLinqQueryable<Customer>(allowSynchronousQueryExecution: true);
        }

        public void Update(Customer obj)
        {
            _container.UpsertItemAsync(obj).Wait();
        }

        public void Remove(Guid id)
        {
            _container.DeleteItemAsync<Customer>(id.ToString(), PartitionKey.None).Wait();
        }

        public int SaveChanges()
        {
            return 1;
        }
    }
}
