using System;
using System.Data;

using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.Models
{
    public class OrderModel : ObservableObject
    {
        static public OrderModel CreateEmpty() => new OrderModel
        {
            OrderID = -1,
            CustomerID = -1,
            IsEmpty = true
        };

        public long OrderID { get; set; }
        public long CustomerID { get; set; }
        public CustomerModel Customer { get; set; }
        public string OrderName { get; set; }
        // public long AccountID { get; set; }
        // public AccountModel Account { get; set; }
        public int CrewId { get; set; }
        public CrewModel Crew { get; set; }

        public string AddressLine { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public int Floor { get; set; }

        private DateTimeOffset _orderDate;
        public DateTimeOffset OrderDate 
        { 
            get => _orderDate; 
            set => Set(ref _orderDate, value); 
        }

        private DateTimeOffset? _deliveryDateTime;
        public DateTimeOffset? DeliveryDateTime 
        {
            get => _deliveryDateTime;
            set => Set(ref _deliveryDateTime, value);
        }

        private int _status;
        public int Status 
        { 
            get => _status;
            set { if (Set(ref _status, value)) NotifyPropertyChanged(nameof(StatusDesc)); } 
        }

        public decimal TotalCost { get; set; }
        public string Observations { get; set; }


        public bool IsNew => OrderID <= 0;

        public string StatusDesc => LookupTablesProxy.Instance.GetOrderStatus(Status);

        public string FullAddress
        {
            get
            {
                return $"{AddressLine}, {City} {PostalCode}, {Floor}ος";
            }
        }

        public string FullName => Customer?.FullName;

        public override void Merge(ObservableObject source)
        {
            if (source is OrderModel model)
            {
                Merge(model);
            }
        }

        public void Merge(OrderModel source)
        {
            if (source != null)
            {
                OrderID = source.OrderID;
                CustomerID = source.CustomerID;
                Customer = source.Customer;
                OrderName = source.OrderName;
                // AccountID = source.AccountID;
                // Account = source.Account;
                CrewId = source.CrewId;
                Crew = source.Crew;

                AddressLine = source.AddressLine;
                City = source.City;
                PostalCode = source.PostalCode;
                Floor = source.Floor;
                
                OrderDate = source.OrderDate;
                DeliveryDateTime = source.DeliveryDateTime;
                Status = source.Status;
                TotalCost = source.TotalCost;
                Observations = source.Observations;
            }
        }

        public override string ToString()
        {
            return OrderID.ToString();
        }
    }
}
