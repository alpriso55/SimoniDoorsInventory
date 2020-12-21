using System;

using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.Models
{
    public class AccountModel : ObservableObject
    {
        static public AccountModel CreateEmpty() => new AccountModel
        {
            AccountID = -1,
            CustomerID = -1,
            IsEmpty = true
        };

        public long AccountID { get; set; }

        public long CustomerID { get; set; }
        public CustomerModel Customer { get; set; }

        private decimal _balance;
        public decimal Balance 
        {
            get => _balance;
            set => Set(ref _balance, value); 
        }
        
        public string Observations { get; set; }

        public string AccountDesc => $"{Customer.FullName} Λογαριασμός";

        public bool IsNew => AccountID <= 0;

        public override void Merge(ObservableObject source)
        {
            if (source is AccountModel model)
            {
                Merge(model);
            }
        }
        public void Merge(AccountModel source)
        {
            if (source != null)
            {
                CustomerID = source.CustomerID;
                Customer = source.Customer;
                Balance = source.Balance;
                Observations = source.Observations;
            }
        }

        public override string ToString()
        {
            return $"{Customer.FullName} Λογαριασμός";
        }
    }
}
