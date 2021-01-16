using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Services
{
    public interface ILookupTables
    {
        Task InitializeAsync();

        IList<AccessoryModel> Accessories { get; }
        IList<CategoryModel> Categories { get; }
        IList<CrewModel> Crews { get; }
        IList<InteriorDoorDesignModel> InteriorDoorDesigns { get; }
        IList<InteriorDoorSkinModel> InteriorDoorSkins { get; }
        IList<OpeningSideModel> OpeningSides { get; }
        IList<OpeningTypeModel> OpeningTypes { get; }
        IList<OrderStatusModel> OrderStatus { get; }
        IList<PaymentTypeModel> PaymentTypes { get; }

        string GetAccessory(int id);
        string GetCategory(int id);
        string GetCrew(int id);
        string GetInteriorDoorDesign(string id);
        string GetInteriorDoorSkin(string id);
        string GetOpeningSide(int id);
        string GetOpeningType(int id);
        string GetOrderStatus(int id);
        string GetPaymentType(int? id);
    }

    public class LookupTablesProxy
    {
        static public ILookupTables Instance { get; set; }
    }
}
