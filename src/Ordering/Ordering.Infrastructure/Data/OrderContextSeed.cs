using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Ordering.Infrastructure.Data
{
    public class OrderContextSeed
    {
         public static async Task SeedAsync(OrderContext orderContext, ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            try
            {
                // INFO: Run this if using a real database. Used to automaticly migrate docker image of sql server db.
                orderContext.Database.Migrate();
                //orderContext.Database.EnsureCreated();

                if (!orderContext.Orders.Any())
                {
                    orderContext.Orders.AddRange(GetPreconfiguredOrders());
                    await orderContext.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                if (retryForAvailability < 5)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<OrderContextSeed>();
                    log.LogError(exception.Message);
                    await SeedAsync(orderContext, loggerFactory, retryForAvailability);
                }
                throw;
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>()
            {
                new Order() { UserName = "bob", FirstName = "Bob", LastName = "Baron",
                 EmailAddress = "bob@mail.com", AddressLine = "ny", TotalPrice = 111 },
                new Order() { UserName = "john", FirstName = "John", LastName = "Doe", 
                EmailAddress ="john@mail.com", AddressLine = "ca", TotalPrice = 222 }
            };
        }
    }
}