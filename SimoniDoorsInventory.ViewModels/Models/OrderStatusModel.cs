using System;

using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.Models
{
    public class OrderStatusModel : ObservableObject
    {
        public int Status { get; set; }
        public string Name { get; set; }
    }
}
