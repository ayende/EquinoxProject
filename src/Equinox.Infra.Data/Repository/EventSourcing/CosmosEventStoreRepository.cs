using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinox.Domain.Core.Events;
using Microsoft.Azure.Cosmos;

namespace Equinox.Infra.Data.Repository.EventSourcing
{
    public class CosmosEventStoreRepository: IEventStoreRepository
    {
        private readonly CosmosClient _client;
        private Container _container;

        public CosmosEventStoreRepository(CosmosClient client)
        {
            _client = client;
            _container = _client.GetContainer("Equinox", "Events");
        }

        public void Dispose()
        {
        }

        public void Store(StoredEvent theEvent)
        {
            _container.CreateItemAsync(theEvent).Wait();
        }

        public IList<StoredEvent> All(Guid aggregateId)
        {
            return _container.GetItemLinqQueryable<StoredEvent>(allowSynchronousQueryExecution: true)
                .Where(e => e.AggregateId == aggregateId)
                .ToList();
        }
    }
}