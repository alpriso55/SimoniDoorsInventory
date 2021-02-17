using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

using SimoniDoorsInventory.Services;
using SimoniDoorsInventory.Data.Services;

namespace SimoniDoorsInventory.ViewModels
{
    public class CreateDatabaseViewModel : ViewModelBase
    {
        public CreateDatabaseViewModel(ISettingsService settingsService, 
                                       ICommonServices commonServices) : base(commonServices)
        {
            SettingsService = settingsService;
            Result = Result.Error("Operation cancelled");
        }

        public ISettingsService SettingsService { get; }

        public Result Result { get; private set; }

        private string _progressStatus = null;
        public string ProgressStatus
        {
            get => _progressStatus;
            set => Set(ref _progressStatus, value);
        }

        private double _progressMaximum = 1;
        public double ProgressMaximum
        {
            get => _progressMaximum;
            set => Set(ref _progressMaximum, value);
        }

        private double _progressValue = 0;
        public double ProgressValue
        {
            get => _progressValue;
            set => Set(ref _progressValue, value);
        }

        private string _message = null;
        public string Message
        {
            get { return _message; }
            set { if (Set(ref _message, value)) NotifyPropertyChanged(nameof(HasMessage)); }
        }

        public bool HasMessage => _message != null;

        private string _primaryButtonText;
        public string PrimaryButtonText
        {
            get => _primaryButtonText;
            set => Set(ref _primaryButtonText, value);
        }

        private string _secondaryButtonText = "Ακύρωση";
        public string SecondaryButtonText
        {
            get => _secondaryButtonText;
            set => Set(ref _secondaryButtonText, value);
        }

        public async Task ExecuteAsync(string connectionString)
        {
            try
            {
                ProgressMaximum = 14;
                ProgressStatus = "Σύνδεση με Βάση Δεδομένων";
                using (var db = new SQLServerDb(connectionString))
                {
                    var dbCreator = db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                    if (!await dbCreator.ExistsAsync())
                    {
                        ProgressValue = 1;
                        ProgressStatus = "Δημιουργία Βάσης Δεδομένων...";
                        await db.Database.EnsureCreatedAsync();
                        ProgressValue = 2;
                        await CopyDataTables(db);
                        ProgressValue = 14;
                        Message = "Βάση Δεδομένων δημιουργήθηκε επιτυχώς.";
                        Result = Result.Ok("Βάση Δεδομένων δημιουργήθηκε επιτυχώς.");
                    }
                    else
                    {
                        ProgressValue = 14;
                        Message = $"Η βάση δεδομένων ήδη υπάρχει. Παρακαλώ, διαγράψτε τη βάση δεδομένων και προσπαθήστε ξανά.";
                        Result = Result.Error("Η βάση δεδομένων ήδη υπάρχει.");
                    }
                }
            }
            catch (Exception ex)
            {
                Result = Result.Error("Σφάλμα κατά τη δημιουργία βάσης δεδομένων. Δες περισσότερες πληροφορίες στο Activity Log");
                Message = $"Σφάλμα κατά τη δημιουργία βάσης δεδομένων: {ex.Message}";
                LogException("Settings", "Create Database", ex);
            }
            PrimaryButtonText = "Ok";
            SecondaryButtonText = null;
        }

        private async Task CopyDataTables(SQLServerDb db)
        {
            using (var sourceDb = new SQLiteDb(SettingsService.PatternConnectionString))
            {
                ProgressStatus = "Δημιουργία πίνακα Accessories...";
                foreach (var item in sourceDb.Accessories.AsNoTracking())
                {
                    await db.Accessories.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 1;

                ProgressStatus = "Δημιουργία πίνακα Categories...";
                foreach (var item in sourceDb.Categories.AsNoTracking())
                {
                    await db.Categories.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 1;

                ProgressStatus = "Δημιουργία πίνακα Crews...";
                foreach (var item in sourceDb.Crews.AsNoTracking())
                {
                    await db.Crews.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 2;

                ProgressStatus = "Δημιουργία πίνακα InteriorDoorDesigns...";
                foreach (var item in sourceDb.InteriorDoorDesigns.AsNoTracking())
                {
                    await db.InteriorDoorDesigns.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 3;

                ProgressStatus = "Δημιουργία πίνακα InteriorDoorSkins...";
                foreach (var item in sourceDb.InteriorDoorSkins.AsNoTracking())
                {
                    await db.InteriorDoorSkins.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 4;

                ProgressStatus = "Δημιουργία πίνακα OpeningSides...";
                foreach (var item in sourceDb.OpeningSides.AsNoTracking())
                {
                    await db.OpeningSides.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 5;

                ProgressStatus = "Δημιουργία πίνακα OpeningTypes...";
                foreach (var item in sourceDb.OpeningTypes.AsNoTracking())
                {
                    await db.OpeningTypes.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 6;

                ProgressStatus = "Δημιουργία πίνακα OrderStatus...";
                foreach (var item in sourceDb.OrderStatus.AsNoTracking())
                {
                    await db.OrderStatus.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 7;

                ProgressStatus = "Δημιουργία πίνακα PaymentTypes...";
                foreach (var item in sourceDb.PaymentTypes.AsNoTracking())
                {
                    await db.PaymentTypes.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 8;

                ProgressStatus = "Δημιουργία πίνακα Customers...";
                foreach (var item in sourceDb.Customers.AsNoTracking())
                {
                    await db.Customers.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 9;

                ProgressStatus = "Δημιουργία πίνακα Orders...";
                foreach (var item in sourceDb.Orders.AsNoTracking())
                {
                    await db.Orders.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 10;

                ProgressStatus = "Δημιουργία πίνακα InteriorDoors...";
                foreach (var item in sourceDb.InteriorDoors.AsNoTracking())
                {
                    await db.InteriorDoors.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 11;

                ProgressStatus = "Δημιουργία πίνακα Payments...";
                foreach (var item in sourceDb.Payments.AsNoTracking())
                {
                    await db.Payments.AddAsync(item);
                }
                await db.SaveChangesAsync();
                ProgressValue = 12;

                ProgressStatus = "Δημιουργία πίνακα Version...";
                await db.DbVersion.AddAsync(await sourceDb.DbVersion.FirstAsync());
                await db.SaveChangesAsync();
                ProgressValue = 14;
            }
        }
    }
}
