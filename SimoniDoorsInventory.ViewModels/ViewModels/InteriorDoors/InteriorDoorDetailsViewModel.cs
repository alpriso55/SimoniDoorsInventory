using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    #region InteriorDoorDetailsArgs
    public class InteriorDoorDetailsArgs
    {
        static public InteriorDoorDetailsArgs CreateDefault() => new InteriorDoorDetailsArgs();

        public long OrderID { get; set; }
        public int InteriorDoorID { get; set; }

        public bool IsNew => InteriorDoorID <= 0;
    }
    #endregion

    public class InteriorDoorDetailsViewModel : GenericDetailsViewModel<InteriorDoorModel>
    {
        public InteriorDoorDetailsViewModel(IInteriorDoorService interiorDoorService, ICommonServices commonServices)
            : base(commonServices)
        {
            InteriorDoorService = interiorDoorService;
        }

        public IInteriorDoorService InteriorDoorService { get; }

        override public string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
        public string TitleNew => $"New Interior Door, Order #{OrderID}";
        public string TitleEdit => $"Interior Door Number {Item?.InteriorDoorID}, #{Item?.OrderID}" ?? string.Empty;

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public InteriorDoorDetailsArgs ViewModelArgs { get; private set; }

        public long OrderID { get; set; }

        public ICommand InteriorDoorSkinSelectedCommand => new RelayCommand<InteriorDoorSkinModel>(InteriorDoorSkinSelected);
        private void InteriorDoorSkinSelected(InteriorDoorSkinModel skin)
        {
            EditableItem.InteriorDoorSkinID = skin.InteriorDoorSkinID;
            EditableItem.InteriorDoorSkin = skin;

            // TODO: It could be a good idea if each component of the door adds up to its price
            // EditableItem.Price += skin.Price;

            EditableItem.NotifyChanges();
        }

        public ICommand InteriorDoorDesignSelectedCommand => new RelayCommand<InteriorDoorDesignModel>(InteriorDoorDesignSelected);
        private void InteriorDoorDesignSelected(InteriorDoorDesignModel design)
        {
            EditableItem.InteriorDoorDesignID = design.InteriorDoorDesignID;
            EditableItem.InteriorDoorDesign = design;

            // TODO: It could be a good idea if each component of the door adds up to its price
            // EditableItem.Price += design.Price;

            EditableItem.NotifyChanges();
        }

        public ICommand OpeningSideSelectedCommand => new RelayCommand<OpeningSideModel>(OpeningSideSelected);
        private void OpeningSideSelected(OpeningSideModel openingSide)
        {
            EditableItem.OpeningSideID = openingSide.OpeningSideID;
            EditableItem.OpeningSide = openingSide;

            // TODO: It could be a good idea if each component of the door adds up to its price
            // EditableItem.Price += openingSide.Price;

            EditableItem.NotifyChanges();
        }

        public ICommand OpeningTypeSelectedCommand => new RelayCommand<OpeningTypeModel>(OpeningTypeSelected);
        private void OpeningTypeSelected(OpeningTypeModel openingType)
        {
            EditableItem.OpeningTypeID = openingType.OpeningTypeID;
            EditableItem.OpeningType = openingType;

            // TODO: It could be a good idea if each component of the door adds up to its price
            // EditableItem.Price += openingType.Price;

            EditableItem.NotifyChanges();
        }

        public ICommand AccessorySelectedCommand => new RelayCommand<AccessoryModel>(AccessorySelected);
        private void AccessorySelected(AccessoryModel accessory)
        {
            EditableItem.AccessoryID = accessory.AccessoryID;
            EditableItem.Accessory = accessory;

            // TODO: It could be a good idea if each component of the door adds up to its price
            // EditableItem.Price += accessory.Price;

            EditableItem.NotifyChanges();
        }

        public async Task LoadAsync(InteriorDoorDetailsArgs args)
        {
            ViewModelArgs = args ?? InteriorDoorDetailsArgs.CreateDefault();
            OrderID = ViewModelArgs.OrderID;

            if (ViewModelArgs.IsNew)
            {
                Item = new InteriorDoorModel { OrderID = OrderID };
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await InteriorDoorService.GetInteriorDoorAsync(OrderID, ViewModelArgs.InteriorDoorID);
                    Item = item ?? new InteriorDoorModel { OrderID = OrderID, InteriorDoorID = ViewModelArgs.InteriorDoorID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    LogException("OrderItem", "Load", ex);
                }
            }
        }

        public void Unload()
        {
            ViewModelArgs.OrderID = Item?.OrderID ?? 0;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<InteriorDoorDetailsViewModel, InteriorDoorModel>(this, OnDetailsMessage);
            MessageService.Subscribe<InteriorDoorListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public InteriorDoorDetailsArgs CreateArgs()
        {
            return new InteriorDoorDetailsArgs
            {
                OrderID = Item?.OrderID ?? 0,
                InteriorDoorID = Item?.InteriorDoorID ?? 0
            };
        }

        protected override async Task<bool> SaveItemAsync(InteriorDoorModel model)
        {
            try
            {
                StartStatusMessage("Saving Interior door...");
                await Task.Delay(100);
                await InteriorDoorService.UpdateInteriorDoorAsync(model);
                EndStatusMessage("Interior door saved");
                LogInformation("OrderItem", "Save", "Interior Door saved successfully", $"Interior Door #{model.OrderID}, {model.InteriorDoorID} was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Interior Door: {ex.Message}");
                LogException("InteriorDoor", "Save", ex);
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(InteriorDoorModel model)
        {
            try
            {
                StartStatusMessage("Deleting Interior Door...");
                await Task.Delay(100);
                await InteriorDoorService.DeleteInteriorDoorAsync(model);
                EndStatusMessage("Interior Door deleted");
                LogWarning("InteriorDoor", "Delete", "Interior Door deleted", $"Interior Door #{model.OrderID}, {model.InteriorDoorID} was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Interior Door: {ex.Message}");
                LogException("Interior Door", "Delete", ex);
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await DialogService.ShowAsync("Επιβεβαίωση Διαγραφής", "Σίγουρα θέλετε να διαγράψετε το επιλεγμένο προϊόν;", "Ναι", "Ακύρωση");
        }

        override protected IEnumerable<IValidationConstraint<InteriorDoorModel>> GetValidationConstraints(InteriorDoorModel model)
        {
            yield return new RequiredConstraint<InteriorDoorModel>("InteriorDoorSkin", m => m.InteriorDoorSkinID);
            yield return new RequiredConstraint<InteriorDoorModel>("OpeningType", m => m.OpeningTypeID);
            yield return new RequiredConstraint<InteriorDoorModel>("OpeningSide", m => m.OpeningSideID);

            yield return new GreaterThanConstraint<InteriorDoorModel>("Width", m => m.Width, 29);
            yield return new GreaterThanConstraint<InteriorDoorModel>("Height", m => m.Height, 29);
            yield return new GreaterThanConstraint<InteriorDoorModel>("Lamb", m => m.Lamb, 5);
            yield return new PositiveConstraint<InteriorDoorModel>("Price", m => m.Price);            
        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(InteriorDoorDetailsViewModel sender, string message, InteriorDoorModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.OrderID == current?.OrderID && changed.InteriorDoorID == current?.InteriorDoorID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await ContextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await InteriorDoorService.GetInteriorDoorAsync(current.OrderID, current.InteriorDoorID);
                                    item = item ?? new InteriorDoorModel { OrderID = OrderID, InteriorDoorID = ViewModelArgs.InteriorDoorID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    NotifyPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: This interiorDoor has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException("InteriorDoor", "Handle Changes", ex);
                                }
                            });
                            break;
                        case "ItemDeleted":
                            await OnItemDeletedExternally();
                            break;
                    }
                }
            }
        }

        private async void OnListMessage(InteriorDoorListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<InteriorDoorModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.OrderID == current.OrderID && r.InteriorDoorID == current.InteriorDoorID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await InteriorDoorService.GetInteriorDoorAsync(current.OrderID, current.InteriorDoorID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException("InteriorDoor", "Handle Ranges Deleted", ex);
                        }
                        break;
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            await ContextService.RunAsync(() =>
            {
                CancelEdit();
                IsEnabled = false;
                StatusMessage("WARNING: This interiorDoor has been deleted externally");
            });
        }

    }
}
