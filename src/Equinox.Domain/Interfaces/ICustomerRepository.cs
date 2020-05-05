using System;
using System.Linq;
using Equinox.Domain.Models;

namespace Equinox.Domain.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Customer GetByEmail(string email);
        IQueryable<Order> OrdersFor(Guid id);

        int OrdersCount(Guid id);
    }
}