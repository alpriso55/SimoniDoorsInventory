using System;

namespace SimoniDoorsInventory.Models
{
    public class AccessoryModel : ObservableObject
    {
        public int AccessoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
