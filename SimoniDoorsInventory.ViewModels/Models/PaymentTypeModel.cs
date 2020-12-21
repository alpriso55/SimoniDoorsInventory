using System;

using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.Models
{
    public class PaymentTypeModel : ObservableObject
    {
        public int PaymentTypeID { get; set; }
        public string Name { get; set; }
    }
}
