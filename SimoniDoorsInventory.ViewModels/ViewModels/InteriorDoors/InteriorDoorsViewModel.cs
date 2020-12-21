using System;
using System.Threading.Tasks;

using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    public class InteriorDoorsViewModel : ViewModelBase
    {
        public InteriorDoorsViewModel(IInteriorDoorService interiorDoorService, 
                                      IOrderService orderService, 
                                      ICommonServices commonServices) : base(commonServices)
        {
            InteriorDoorService = interiorDoorService;

            InteriorDoorList = new InteriorDoorListViewModel(InteriorDoorService, commonServices);
            InteriorDoorDetails = new InteriorDoorDetailsViewModel(InteriorDoorService, commonServices);
        }

        public IInteriorDoorService InteriorDoorService { get; }

        public InteriorDoorListViewModel InteriorDoorList { get; set; }
        public InteriorDoorDetailsViewModel InteriorDoorDetails { get; set; }

        public async Task LoadAsync(InteriorDoorListArgs args)
        {
            await InteriorDoorList.LoadAsync(args);
        }
        public void Unload()
        {
            InteriorDoorDetails.CancelEdit();
            InteriorDoorList.Unload();
        }

        public void Subscribe()
        {
            MessageService.Subscribe<InteriorDoorListViewModel>(this, OnMessage);
            InteriorDoorList.Subscribe();
            InteriorDoorDetails.Subscribe();
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            InteriorDoorList.Unsubscribe();
            InteriorDoorDetails.Unsubscribe();
        }

        private async void OnMessage(InteriorDoorListViewModel viewModel, string message, object args)
        {
            if (viewModel == InteriorDoorList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnItemSelected()
        {
            if (InteriorDoorDetails.IsEditMode)
            {
                StatusReady();
                InteriorDoorDetails.CancelEdit();
            }
            var selected = InteriorDoorList.SelectedItem;
            if (!InteriorDoorList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                }
            }
            InteriorDoorDetails.Item = selected;
        }

        private async Task PopulateDetails(InteriorDoorModel selected)
        {
            try
            {
                var model = await InteriorDoorService.GetInteriorDoorAsync(selected.OrderID, selected.InteriorDoorID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("InteriorDoors", "Load Details", ex);
            }
        }

    }
}
