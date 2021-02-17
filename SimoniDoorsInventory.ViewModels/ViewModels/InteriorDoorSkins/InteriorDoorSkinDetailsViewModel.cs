using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;

using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    #region InteriorDoorSkinDetailsArgs
    public class InteriorDoorSkinDetailsArgs
    {
        static public InteriorDoorSkinDetailsArgs CreateDefault() => new InteriorDoorSkinDetailsArgs();

        public string InteriorDoorSkinID { get; set; }

        public bool IsNew => string.IsNullOrWhiteSpace(InteriorDoorSkinID);
    }
    #endregion

    public class InteriorDoorSkinDetailsViewModel : GenericDetailsViewModel<InteriorDoorSkinModel>
    {
        public InteriorDoorSkinDetailsViewModel(IInteriorDoorSkinService interiorDoorSkinService, ICommonServices commonServices) : base(commonServices)
        {
            InteriorDoorSkinService = interiorDoorSkinService;
        }

        public IInteriorDoorSkinService InteriorDoorSkinService { get; }

        public override string Title => (Item?.IsNew ?? true) ? "Νέα Επένδυση Μεσόπορτας" : TitleEdit;
        public string TitleEdit => Item == null ? "Επένδυση" : $"{Item.InteriorDoorSkinDesc}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public InteriorDoorSkinDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(InteriorDoorSkinDetailsArgs args)
        {
            ViewModelArgs = args ?? InteriorDoorSkinDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = new InteriorDoorSkinModel();
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await InteriorDoorSkinService.GetInteriorDoorSkinAsync(ViewModelArgs.InteriorDoorSkinID);
                    Item = item ?? new InteriorDoorSkinModel { InteriorDoorSkinID = ViewModelArgs.InteriorDoorSkinID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    LogException("InteriorDoorSkin", "Load", ex);
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.InteriorDoorSkinID = Item?.InteriorDoorSkinID ?? "";
        }

        public void Subscribe()
        {
            MessageService.Subscribe<InteriorDoorSkinDetailsViewModel, InteriorDoorSkinModel>(this, OnDetailsMessage);
            MessageService.Subscribe<InteriorDoorSkinListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public InteriorDoorSkinDetailsArgs CreateArgs()
        {
            return new InteriorDoorSkinDetailsArgs
            {
                InteriorDoorSkinID = Item?.InteriorDoorSkinID ?? ""
            };
        }

        protected override async Task<bool> SaveItemAsync(InteriorDoorSkinModel model)
        {
            try
            {
                StartStatusMessage("Αποθήκευση επένδυσης μεσόπορτας...");
                await Task.Delay(100);
                await InteriorDoorSkinService.UpdateInteriorDoorSkinAsync(model);
                EndStatusMessage("Επένδυση μεσόπορτας αποθηκεύτηκε");
                LogInformation("InteriorDoorSkin", "Save", "Interior Door Skin saved successfully", $"InteriorDoorSkin {model.InteriorDoorSkinID} was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Σφάλμα στην αποθήκευση της Επένδυσης Μεσσόπορτας: {ex.Message}");
                LogException("InteriorDoorSkin", "Save", ex);
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(InteriorDoorSkinModel model)
        {
            try
            {
                StartStatusMessage("Διαγραφή επένδυσης μεσόπορτας...");
                await Task.Delay(100);
                await InteriorDoorSkinService.DeleteInteriorDoorSkinAsync(model);
                EndStatusMessage("Επένδυση μεσόπορτας διεγράφη");
                LogWarning("InteriorDoorSkin", "Delete", "InteriorDoorSkin deleted", $"InteriorDoorSkin {model.InteriorDoorSkinID} was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Σφάλμα διαγραφής επένδυσης μεσόπορτας: {ex.Message}");
                LogException("InteriorDoorSkin", "Delete", ex);
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await DialogService.ShowAsync("Επιβεβαίωση Διαγραφής", "Σίγουρα θέλετε να διαγράψετε την επιλεγμένη επένδυση μεσόπορτας;", "Ναι", "Ακύρωση");
        }

        protected override IEnumerable<IValidationConstraint<InteriorDoorSkinModel>> GetValidationConstraints(InteriorDoorSkinModel model)
        {
            yield return new RequiredConstraint<InteriorDoorSkinModel>("Interior Door Skin ID", m => m.InteriorDoorSkinID);
            yield return new PositiveConstraint<InteriorDoorSkinModel>("Stock Units", m => m.StockUnits);
            yield return new PositiveConstraint<InteriorDoorSkinModel>("Safety Stock Lavel", m => m.SafetyStockLevel);

        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(InteriorDoorSkinDetailsViewModel sender, string message, InteriorDoorSkinModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.InteriorDoorSkinID == current?.InteriorDoorSkinID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await ContextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await InteriorDoorSkinService.GetInteriorDoorSkinAsync(current.InteriorDoorSkinID);
                                    item = item ?? new InteriorDoorSkinModel { InteriorDoorSkinID = current.InteriorDoorSkinID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    NotifyPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: Η συγκεκριμένη επένδυση έχει επεξεργαστεί εξωτερικά");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException("InteriorDoorSkin", "Handle Changes", ex);
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

        private async void OnListMessage(InteriorDoorSkinListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<InteriorDoorSkinModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.InteriorDoorSkinID == current.InteriorDoorSkinID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await InteriorDoorSkinService.GetInteriorDoorSkinAsync(current.InteriorDoorSkinID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException("InteriorDoorSkin", "Handle Ranges Deleted", ex);
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
                StatusMessage("WARNING: Η συγκεκριμένη επένδυση έχει διαγραφτεί εξωτερικά");
            });
        }

    }
}
