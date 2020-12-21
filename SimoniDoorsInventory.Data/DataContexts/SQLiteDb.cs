using System;
using Microsoft.EntityFrameworkCore;

namespace SimoniDoorsInventory.Data.Services
{
    public class SQLiteDb : DbContext, IDataSource
    {
        private string _connectionString = null;

        public SQLiteDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* 
            // -----------------------------------------------------------
            // Account
            modelBuilder.Entity<Account>()
                        .Property(p => p.Balance)
                        .HasDefaultValue(0);

            // -----------------------------------------------------------
            // Armored Door
            modelBuilder.Entity<ArmoredDoor>()
                        .Property(p => p.Armor)
                        .HasDefaultValue(ArmorType.ΔΙΠΛΗ);

            modelBuilder.Entity<ArmoredDoor>()
                        .Property(p => p.LockingPoints)
                        .HasDefaultValue(20);

            modelBuilder.Entity<ArmoredDoor>()
                        .Property(p => p.ArmoredDoorLockId)
                        .HasDefaultValue(1);
            modelBuilder.Entity<ArmoredDoor>()
                        .Property(p => p.ArmoredDoorLock)
                        .HasDefaultValue(ArmoredDoorLocks.Find(1));

            modelBuilder.Entity<ArmoredDoor>()
                        .Property(p => p.ArmoredDoorCylinderId)
                        .HasDefaultValue(1);
            modelBuilder.Entity<ArmoredDoor>()
                        .Property(p => p.ArmoredDoorCylinder)
                        .HasDefaultValue(ArmoredDoorCylinders.Find(1));

            modelBuilder.Entity<ArmoredDoor>()
                        .Property(p => p.ArmoredDoorSkinId)
                        .HasDefaultValue(1);
            modelBuilder.Entity<ArmoredDoor>()
                        .Property(p => p.ArmoredDoorSkin)
                        .HasDefaultValue(ArmoredDoorSkins.Find(1));

            modelBuilder.Entity<ArmoredDoor>()
                        .Property(p => p.Opening)
                        .HasDefaultValue(OpeningSide.ΔΕΞΙΑ);

            modelBuilder.Entity<ArmoredDoor>()
                        .Property(p => p.Price)
                        .HasDefaultValue((int)CommonPrice.ArmoredDoor);

            // -----------------------------------------------------------
            // Interior Door
            modelBuilder.Entity<InteriorDoor>()
                        .Property(p => p.OpeningType)
                        .HasDefaultValue(OpeningTypes.ΑΝΟΙΓΟΜΕΝΗ);

            modelBuilder.Entity<InteriorDoor>()
                        .Property(p => p.InteriorDoorSkinId)
                        .HasDefaultValue(1);
            modelBuilder.Entity<InteriorDoor>()
                        .Property(p => p.InteriorDoorSkin)
                        .HasDefaultValue(InteriorDoorSkins.Find(1));

            modelBuilder.Entity<InteriorDoor>()
                        .Property(p => p.Opening)
                        .HasDefaultValue(OpeningSide.ΔΕΞΙΑ);

            modelBuilder.Entity<InteriorDoor>()
                        .Property(p => p.Price)
                        .HasDefaultValue(CommonPrice.InteriorDoor);

            // -----------------------------------------------------------
            // Order
            modelBuilder.Entity<Order>()
                        .Property(p => p.TotalCost)
                        .HasDefaultValue(0);
            */
            modelBuilder.Entity<InteriorDoor>().HasKey(e => new { e.OrderID, e.InteriorDoorID });
        }

        public DbSet<DbVersion> DbVersion { get; set; }

        public DbSet<Accessory> Accessories { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Crew> Crews { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<InteriorDoor> InteriorDoors { get; set; }

        public DbSet<InteriorDoorDesign> InteriorDoorDesigns { get; set; }

        public DbSet<InteriorDoorSkin> InteriorDoorSkins { get; set; }

        public DbSet<OpeningSide> OpeningSides { get; set; }

        public DbSet<OpeningType> OpeningTypes { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderStatus> OrderStatus { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<PaymentType> PaymentTypes { get; set; }
    }
}
