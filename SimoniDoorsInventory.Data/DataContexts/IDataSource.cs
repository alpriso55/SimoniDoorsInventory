using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SimoniDoorsInventory.Data.Services
{
    public interface IDataSource : IDisposable
    {
        DbSet<DbVersion> DbVersion { get; }
        DbSet<Accessory> Accessories { get; }
        DbSet<Category> Categories { get; }
        DbSet<Crew> Crews { get; }
        DbSet<Customer> Customers { get; }
        DbSet<InteriorDoor> InteriorDoors { get; }
        DbSet<InteriorDoorDesign> InteriorDoorDesigns { get; }
        DbSet<InteriorDoorSkin> InteriorDoorSkins { get; }
        DbSet<OpeningSide> OpeningSides { get; }
        DbSet<OpeningType> OpeningTypes { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderStatus> OrderStatus { get; }
        DbSet<Payment> Payments { get; }
        DbSet<PaymentType> PaymentTypes { get; }

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
