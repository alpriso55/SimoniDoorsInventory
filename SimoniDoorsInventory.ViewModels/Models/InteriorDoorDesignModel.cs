using System;

namespace SimoniDoorsInventory.Models
{
    public class InteriorDoorDesignModel : ObservableObject
    {
        public string InteriorDoorDesignID { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public object PictureSource { get; set; }
    }
}
