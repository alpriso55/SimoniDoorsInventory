using System;
using System.Data;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.Models
{
    public class PaymentModel : ObservableObject
    {
        static public PaymentModel CreateEmpty() => new PaymentModel
        {
            PaymentID = -1,
            IsEmpty = true
        };

        public long PaymentID { get; set; }
        public long CustomerID { get; set; }
        public CustomerModel Customer { get; set; }
        public decimal Amount { get; set; }
        public int? PaymentTypeID { get; set; }
        public PaymentTypeModel PaymentType { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public string Observations { get; set; }

        public bool IsNew => PaymentID <= 0;

        public string PaymentDesc => $"{Customer?.FullName}: +{Amount} €";

        public override void Merge(ObservableObject source)
        {
            if (source is PaymentModel model)
            {
                Merge(model);
            }
        }
        public void Merge(PaymentModel source)
        {
            if (source != null)
            {
                PaymentID = source.PaymentID;
                CustomerID = source.CustomerID;
                Customer = source.Customer;
                Amount = source.Amount;
                PaymentTypeID = source.PaymentTypeID;
                PaymentType = source.PaymentType;
                PaymentDate = source.PaymentDate;
                Observations = source.Observations;
            }
        }

        public override string ToString()
        {
            return PaymentID.ToString();
        }
    }
}
