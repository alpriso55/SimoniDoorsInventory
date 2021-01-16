using System;

using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.Models
{
    public class InteriorDoorSkinModel : ObservableObject
    {
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

        public string InteriorDoorSkinDesc => $"{InteriorDoorSkinID} ({StockUnits} Τμχ.)";
    }
}
