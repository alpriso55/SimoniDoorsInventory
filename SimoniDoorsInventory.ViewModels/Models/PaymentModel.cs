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
        public long AccountID { get; set; }
        public AccountModel Account { get; set; }
        public long OrderID { get; set; }
        public OrderModel Order { get; set; }
        public decimal Amount { get; set; }
        public int? PaymentTypeID { get; set; }
        public PaymentTypeModel PaymentType { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public string Observations { get; set; }

        public bool IsNew => PaymentID <= 0;

        public string AccountDesc => LookupTablesProxy.Instance.GetAccount(AccountID);
        public string PaymentDesc => $"{AccountDesc} <=> {Amount} €";

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
                AccountID = source.AccountID;
                Account = source.Account;
                OrderID = source.OrderID;
                Order = source.Order;
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
