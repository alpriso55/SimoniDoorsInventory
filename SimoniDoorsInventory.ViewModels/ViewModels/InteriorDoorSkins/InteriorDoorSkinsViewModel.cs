using System;
using System.Threading.Tasks;

using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    public class InteriorDoorSkinsViewModel : ViewModelBase
    {
        public InteriorDoorSkinsViewModel(IInteriorDoorSkinService interiorDoorSkinService,
                                          ICommonServices commonServices)
            : base(commonServices)
        {
            InteriorDoorSkinService = interiorDoorSkinService;

            InteriorDoorSkinList = new InteriorDoorSkinListViewModel(InteriorDoorSkinService, commonServices);
            InteriorDoorSkinDetails = new InteriorDoorSkinDetailsViewModel(InteriorDoorSkinService, commonServices);
        }

        public IInteriorDoorSkinService InteriorDoorSkinService { get; }

        public InteriorDoorSkinListViewModel InteriorDoorSkinList { get; set; }
        public InteriorDoorSkinDetailsViewModel InteriorDoorSkinDetails { get; set; }
        
        public async Task LoadAsync(InteriorDoorSkinListArgs args)
        {
            await InteriorDoorSkinList.LoadAsync(args);
        }

        public void Unload()
        {
            InteriorDoorSkinDetails.CancelEdit();
            InteriorDoorSkinList.Unload();
        }

        public void Subscribe()
        {
            MessageService.Subscribe<InteriorDoorSkinListViewModel>(this, OnInteriorDoorSkinListMessage);
            InteriorDoorSkinList.Subscribe();
            InteriorDoorSkinDetails.Subscribe();
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            InteriorDoorSkinList.Unsubscribe();
            InteriorDoorSkinDetails.Unsubscribe();
        }

        private async void OnInteriorDoorSkinListMessage(InteriorDoorSkinListViewModel viewModel, string message, object args)
        {
            if (viewModel == InteriorDoorSkinList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        public async void OnItemSelected()
        {
            if (InteriorDoorSkinDetails.IsEditMode)
            {
                StatusReady();
                InteriorDoorSkinDetails.CancelEdit();
            }
            var selected = InteriorDoorSkinList.SelectedItem;
            if (!InteriorDoorSkinList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                }
            }

            InteriorDoorSkinDetails.Item = selected;
        }

        private async Task PopulateDetails(InteriorDoorSkinModel selected)
        {
            try
            {
                var model = await InteriorDoorSkinService.GetInteriorDoorSkinAsync(selected.InteriorDoorSkinID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("InteriorDoorSkins", "Load Details", ex);
            }
        }

    }
}
