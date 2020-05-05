using Equinox.Domain.Core.Events;
using Equinox.Domain.Interfaces;
using Equinox.Infra.Data.Repository.EventSourcing;
using Microsoft.Azure.Cosmos;
using System.Text.Json;


namespace Equinox.Infra.Data.EventSourcing
{
    public class CosmosEventStore : IEventStore
    {
        private readonly CosmosClient _client;
        private readonly IUser _user;
        private Container _container;

        public CosmosEventStore(CosmosClient client, IUser user)
        {
            _client = client;
            _user = user;
            _container = _client.GetContainer("Equinox", "Events");
        }

        public void Save<T>(T theEvent) where T : Event
        {
            var serializedData = JsonSerializer.Serialize(theEvent);

            var storedEvent = new StoredEvent(
                theEvent,
                serializedData,
                _user.Name);

            _container.CreateItemAsync(storedEvent).Wait();
        }
    }
}