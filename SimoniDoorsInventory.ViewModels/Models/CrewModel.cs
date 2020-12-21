using System;

namespace SimoniDoorsInventory.Models
{
    public class CrewModel : ObservableObject
    {
        public int CrewID { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Observations { get; set; }

        public string CrewDesc => $"{Name} ({Phone})";
    }
}
