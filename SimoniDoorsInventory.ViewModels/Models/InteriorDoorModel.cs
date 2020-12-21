using System;

using SimoniDoorsInventory.Services;
using SimoniDoorsInventory.Data;

namespace SimoniDoorsInventory.Models
{
    public class InteriorDoorModel : ObservableObject
    {
        public long OrderID { get; set; }
        public int InteriorDoorID { get; set; }
        public string InteriorDoorSkinID { get; set; }
        public InteriorDoorSkinModel InteriorDoorSkin { get; set; }
        public string InteriorDoorDesignID { get; set; }
        public InteriorDoorDesignModel InteriorDoorDesign { get; set; }
        public int OpeningSideID { get; set; }
        public OpeningSideModel OpeningSide { get; set; }
        public int OpeningTypeID { get; set; }
        public OpeningTypeModel OpeningType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Lamb { get; set; }
        public int? AccessoryID { get; set; }
        public AccessoryModel Accessory { get; set; }
        public string Observations { get; set; }
        public decimal Price { get; set; }

        public bool IsNew => InteriorDoorID <= 0;

        public string OpeningSideDesc => LookupTablesProxy.Instance.GetOpeningSide(OpeningSideID);
        public string OpeningTypeDesc => LookupTablesProxy.Instance.GetOpeningType(OpeningTypeID);
        public string AccessoryDesc => AccessoryID == null ? "" : LookupTablesProxy.Instance.GetAccessory(AccessoryID.Value);

        public override void Merge(ObservableObject source)
        {
            if (source is InteriorDoorModel model)
            {
                Merge(model);
            }
        }
        public void Merge(InteriorDoorModel source)
        {
            if (source != null)
            {
                OrderID = source.OrderID;
                InteriorDoorID = source.InteriorDoorID;
                InteriorDoorSkinID = source.InteriorDoorSkinID;
                InteriorDoorSkin = source.InteriorDoorSkin;
                InteriorDoorDesignID = source.InteriorDoorDesignID;
                InteriorDoorDesign = source.InteriorDoorDesign;
                OpeningSideID = source.OpeningSideID;
                OpeningSide = source.OpeningSide;
                OpeningTypeID = source.OpeningTypeID;
                OpeningType = source.OpeningType;
                Width = source.Width;
                Height = source.Height;
                Lamb = source.Lamb;
                AccessoryID = source.AccessoryID;
                Accessory = source.Accessory;
                Observations = source.Observations;
                Price = source.Price;
            }
        }

        public override string ToString()
        {
            return OrderID.ToString() + InteriorDoorID.ToString();
        }
    }
}
