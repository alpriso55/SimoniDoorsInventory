using System;

using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.Models
{
    public class InteriorDoorSkinModel : ObservableObject
    {
        static public InteriorDoorSkinModel CreateEmpty() => new InteriorDoorSkinModel
        {
            InteriorDoorSkinID = "",
            IsEmpty = true
        };

        public string InteriorDoorSkinID { get; set; }

        private int _stockUnits;
        public int StockUnits 
        {
            get => _stockUnits;
            set { if (Set(ref _stockUnits, value)) NotifyPropertyChanged(nameof(IsBelowSafetyStockLevel)); }
        }
        public int SafetyStockLevel { get; set; }
        public string Description { get; set; }

        public bool IsBelowSafetyStockLevel => StockUnits <= SafetyStockLevel;

        // --------------------------------------------------------
        public bool IsNew => string.IsNullOrWhiteSpace(InteriorDoorSkinID);
        public string InteriorDoorSkinDesc => $"{InteriorDoorSkinID} ({StockUnits} Τμχ.)";

        public override string ToString()
        {
            return IsEmpty ? "Επένδυση Μεσόπορτας" : $"{InteriorDoorSkinID} ({StockUnits} Τμχ.)";
        }

        public override void Merge(ObservableObject source)
        {
            if (source is InteriorDoorSkinModel model)
            {
                Merge(model);
            }
        }
        public void Merge(InteriorDoorSkinModel source)
        {
            if (source != null)
            {
                InteriorDoorSkinID = source.InteriorDoorSkinID;
                StockUnits = source.StockUnits;
                SafetyStockLevel = source.SafetyStockLevel;
                Description = source.Description;
            }
        }
    }
}
